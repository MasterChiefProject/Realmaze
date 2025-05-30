using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))] // gives them a physical body
public class WayPointScript : MonoBehaviour
{
    [Header("Way-points")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float speed = 3f;
    public float staySeconds = 1f;
    public float rotationSpeed = 180f; // deg/s
    public float heightOffset = 0.1f;
    public LayerMask groundLayers = ~0;

    [Header("Moan (Audio)")]
    public AudioClip[] moanClips;
    [Range(1f, 60f)] public float minMoanDelay = 1f;
    [Range(1f, 60f)] public float maxMoanDelay = 60f;

    [Header("Collision-avoidance")]
    public float avoidRadius = 1.0f;      // how far we “feel” neighbours
    public float avoidWeight = 1.5f;      // steering strength
    public LayerMask zombieLayer;         // put zombies in their own layer

    /* ────────────────────────────────────────── */
    int currentWp;
    bool waiting;
    float waitTimer;

    Animator anim;
    static readonly int walkHash = Animator.StringToHash("isWalking");

    AudioSource audioSrc;
    float moanTimer;

    CapsuleCollider col;

    /* ────────────────────────────────────────── */
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        col = GetComponent<CapsuleCollider>(); // auto-added by RequireComponent

        anim.SetBool(walkHash, true);

        ResetMoanTimer();
    }

    /* ────────────────────────────────────────── */
    void Update()
    {
        HandleMoan();

        if (waiting)
        {
            RotateTowardNextWaypoint();
            if ((waitTimer -= Time.deltaTime) <= 0f)
            {
                waiting = false;
                anim.SetBool(walkHash, true);
            }
            return;
        }

        MoveAlongTerrain();

        // arrival test (XZ only)
        Vector3 flat = waypoints[currentWp].position - transform.position;
        flat.y = 0;
        if (flat.sqrMagnitude < 0.16f)
        {
            currentWp = (currentWp + 1) % waypoints.Length;
            waiting = true;
            waitTimer = staySeconds;
            anim.SetBool(walkHash, false);
        }
    }

    /* ───────────────── helpers ───────────────── */

    void MoveAlongTerrain()
    {
        // desired direction
        Vector3 target = waypoints[currentWp].position;
        Vector3 dir = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z).normalized;

        // add local-avoidance steering
        dir = ApplyAvoidance(dir);

        // step
        Vector3 step = dir * speed * Time.deltaTime;
        Vector3 nextXZ = transform.position + step;

        // stick to ground
        float groundY = GetGroundHeight(nextXZ);
        nextXZ.y = groundY + heightOffset;
        transform.position = nextXZ;

        // face where we’re going
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }
    }

    Vector3 ApplyAvoidance(Vector3 desiredDir)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, avoidRadius, zombieLayer);
        Vector3 push = Vector3.zero;

        foreach (var h in hits)
        {
            if (h == col) continue;                       // ignore self

            Vector3 away = transform.position - h.transform.position;
            away.y = 0;
            float dist = away.magnitude;
            if (dist > 0.001f)
                push += away.normalized / dist;          // inverse-distance weighting
        }

        if (push == Vector3.zero) return desiredDir;

        Vector3 steered = (desiredDir + push * avoidWeight).normalized;
        return steered;
    }

    void RotateTowardNextWaypoint()
    {
        Vector3 toNext = waypoints[currentWp].position - transform.position;
        toNext.y = 0;
        if (toNext.sqrMagnitude < 0.001f) return;

        Quaternion tgt = Quaternion.LookRotation(toNext);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tgt, rotationSpeed * Time.deltaTime);
    }

    float GetGroundHeight(Vector3 worldXZ)
    {
        if (Terrain.activeTerrain)
            return Terrain.activeTerrain.SampleHeight(worldXZ) + Terrain.activeTerrain.transform.position.y;

        if (Physics.Raycast(worldXZ + Vector3.up * 100f, Vector3.down, out var hit, 200f, groundLayers))
            return hit.point.y;

        return worldXZ.y;
    }

    /* ───────────── zombie moan logic ─────────── */
    void HandleMoan()
    {
        if (moanClips.Length == 0) return;

        if ((moanTimer -= Time.deltaTime) <= 0f)
        {
            AudioClip clip = moanClips[Random.Range(0, moanClips.Length)];
            audioSrc.PlayOneShot(clip);
            ResetMoanTimer();
        }
    }

    void ResetMoanTimer() =>
        moanTimer = Random.Range(minMoanDelay, maxMoanDelay);
}
