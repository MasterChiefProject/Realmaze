using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]
public class WayPointScript : MonoBehaviour
{
    [Header("Way-points")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float speed = 3f;
    public float staySeconds = 1f;
    public float rotationSpeed = 180f;
    public float heightOffset = 0.1f;
    public LayerMask groundLayers = ~0;

    [Header("Moan (Audio)")]
    public AudioClip[] moanClips;
    [Range(1f, 60f)] public float minMoanDelay = 1f;
    [Range(1f, 60f)] public float maxMoanDelay = 60f;

    [Header("Collision-avoidance")]
    public float avoidRadius = 1.0f;
    public float avoidWeight = 1.5f;
    public LayerMask zombieLayer;

    private const int AvoidanceBufferSize = 24;

#if UNITY_WEBGL && !UNITY_EDITOR
    private const float WebAvoidanceSampleInterval = 0.12f;
#endif

    private int currentWp;
    private bool waiting;
    private float waitTimer;

    private Animator anim;
    private static readonly int walkHash =
        Animator.StringToHash("isWalking");

    private AudioSource audioSrc;
    private float moanTimer;

    private CapsuleCollider col;
    private Terrain activeTerrain;

    // Reused physics buffer removes the array allocation caused by
    // Physics.OverlapSphere on every zombie every frame.
    private readonly Collider[] avoidanceBuffer =
        new Collider[AvoidanceBufferSize];

    private Vector3 cachedAvoidancePush;

#if UNITY_WEBGL && !UNITY_EDITOR
    private float nextAvoidanceSampleTime;
#endif

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        col = GetComponent<CapsuleCollider>();
        activeTerrain = Terrain.activeTerrain;

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning(
                $"[{nameof(WayPointScript)}] '{name}' has no waypoints. " +
                "Patrol movement was disabled.",
                this);

            enabled = false;
            return;
        }

        if (anim)
        {
            anim.SetBool(walkHash, true);
        }

        ResetMoanTimer();

#if UNITY_WEBGL && !UNITY_EDITOR
        // Spread expensive avoidance samples across different frames.
        nextAvoidanceSampleTime =
            Time.time + Random.value * WebAvoidanceSampleInterval;
#endif
    }

    private void Update()
    {
        HandleMoan();

        if (waiting)
        {
            RotateTowardNextWaypoint();

            if ((waitTimer -= Time.deltaTime) <= 0f)
            {
                waiting = false;

                if (anim)
                {
                    anim.SetBool(walkHash, true);
                }
            }

            return;
        }

        MoveAlongTerrain();

        Vector3 flat =
            waypoints[currentWp].position - transform.position;

        flat.y = 0f;

        if (flat.sqrMagnitude < 0.16f)
        {
            currentWp =
                (currentWp + 1) % waypoints.Length;

            waiting = true;
            waitTimer = staySeconds;

            if (anim)
            {
                anim.SetBool(walkHash, false);
            }
        }
    }

    private void MoveAlongTerrain()
    {
        Vector3 target =
            waypoints[currentWp].position;

        Vector3 dir = new Vector3(
            target.x - transform.position.x,
            0f,
            target.z - transform.position.z).normalized;

        dir = ApplyAvoidance(dir);

        Vector3 step =
            dir * speed * Time.deltaTime;

        Vector3 nextXZ =
            transform.position + step;

        float groundY =
            GetGroundHeight(nextXZ);

        nextXZ.y =
            groundY + heightOffset;

        transform.position =
            nextXZ;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look =
                Quaternion.LookRotation(dir);

            transform.rotation =
                Quaternion.RotateTowards(
                    transform.rotation,
                    look,
                    rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 ApplyAvoidance(Vector3 desiredDir)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Time.time >= nextAvoidanceSampleTime)
        {
            cachedAvoidancePush =
                CalculateAvoidancePush();

            nextAvoidanceSampleTime =
                Time.time + WebAvoidanceSampleInterval;
        }
#else
        cachedAvoidancePush =
            CalculateAvoidancePush();
#endif

        if (cachedAvoidancePush.sqrMagnitude < 0.000001f)
        {
            return desiredDir;
        }

        return (
            desiredDir +
            cachedAvoidancePush * avoidWeight
        ).normalized;
    }

    private Vector3 CalculateAvoidancePush()
    {
        int hitCount =
            Physics.OverlapSphereNonAlloc(
                transform.position,
                avoidRadius,
                avoidanceBuffer,
                zombieLayer);

        Vector3 push =
            Vector3.zero;

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit =
                avoidanceBuffer[i];

            if (!hit || hit == col)
            {
                continue;
            }

            Vector3 away =
                transform.position -
                hit.transform.position;

            away.y = 0f;

            float distance =
                away.magnitude;

            if (distance > 0.001f)
            {
                push +=
                    away.normalized / distance;
            }
        }

        return push;
    }

    private void RotateTowardNextWaypoint()
    {
        Vector3 toNext =
            waypoints[currentWp].position -
            transform.position;

        toNext.y = 0f;

        if (toNext.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(toNext);

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
    }

    private float GetGroundHeight(Vector3 worldXZ)
    {
        if (activeTerrain)
        {
            return
                activeTerrain.SampleHeight(worldXZ) +
                activeTerrain.transform.position.y;
        }

        if (Physics.Raycast(
                worldXZ + Vector3.up * 100f,
                Vector3.down,
                out RaycastHit hit,
                200f,
                groundLayers))
        {
            return hit.point.y;
        }

        return worldXZ.y;
    }

    private void HandleMoan()
    {
        if (moanClips == null ||
            moanClips.Length == 0 ||
            !audioSrc)
        {
            return;
        }

        if ((moanTimer -= Time.deltaTime) <= 0f)
        {
            AudioClip clip =
                moanClips[
                    Random.Range(
                        0,
                        moanClips.Length)];

            if (clip)
            {
                audioSrc.PlayOneShot(clip);
            }

            ResetMoanTimer();
        }
    }

    private void ResetMoanTimer()
    {
        moanTimer =
            Random.Range(
                minMoanDelay,
                maxMoanDelay);
    }
}
