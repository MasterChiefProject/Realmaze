using System.Collections;
using UnityEngine;

public class WayPointScript : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public float staySeconds = 1f;
    public float rotationSpeed = 180f; // degrees per second


    int currentWaypoint = 0;
    bool waiting = false;
    float waitTimer;


    Animator animator;
    const string isWalkingAnimationParam = "isWalking";

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(isWalkingAnimationParam, true);
    }

    void Update()
    {
        // patrol is idle in a waypoint for X seconds
        if(waiting)
        {
            // rotate patrol to next position
            Vector3 nextPosition = waypoints[currentWaypoint].position;
            Vector3 toNext = nextPosition - transform.position;
            toNext.y = 0;

            if(toNext.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toNext);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // make the patrol wait in place
            waitTimer -= Time.deltaTime;
            if(waitTimer <= 0f)
            {
                waiting = false;
                animator.SetBool(isWalkingAnimationParam, true);
            }
            return;
        }

        // patrol moving
        Vector3 targetPosition = waypoints[currentWaypoint].position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, targetPosition) < 0.1f)
        { 
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            waiting = true;
            waitTimer = staySeconds;
            animator.SetBool(isWalkingAnimationParam, false);
        }

    }
}
