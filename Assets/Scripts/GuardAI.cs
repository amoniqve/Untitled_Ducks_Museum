using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    private int currentPoint = 0;
    private NavMeshAgent agent;

    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 2f;       
    public float chaseStopDistance = 8f; 
    public float caughtDistance = 1.5f; 

    private bool isChasing = false;

    [Header("Vision Cone Settings")]
    public float viewDistance = 5f;         // how far the guard can see
    [Range(0, 360)]
    public float viewAngle = 90f;           // guard's field of view
    public LayerMask obstacleMask;          // walls/objects that block vision

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = patrolPoints[currentPoint].position;
    }

    void Update()
    {
        if (!isChasing)
        {
            Patrol();
            DetectPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    
    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPoint].position;
        }
    }

    
    void DetectPlayer()
    {
        if (player == null) return;

        
        if (CanSeePlayer())
        {
            isChasing = true;
            Debug.Log("Guard detected player via vision cone! Chasing!");
            return;
        }

        
        Vector3 guardPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPos = new Vector3(player.position.x, 0, player.position.z);
        float distance = Vector3.Distance(guardPos, playerPos);

        if (distance < detectionRange)
        {
            isChasing = true;
            Debug.Log("Guard detected player by proximity! Chasing!");
        }
    }

    // chasiiiing
    void ChasePlayer()
    {
        if (player == null) return;

        agent.destination = player.position;

        Vector3 guardPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPos = new Vector3(player.position.x, 0, player.position.z);
        float distance = Vector3.Distance(guardPos, playerPos);

        // Stop chasing 
        if (distance > chaseStopDistance)
        {
            isChasing = false;
            agent.destination = patrolPoints[currentPoint].position;
            Debug.Log("Player escaped, resuming patrol.");
        }

        // Player caught
        if (distance < caughtDistance)
        {
            Debug.Log("Caught by guard!");
            
        }
    }

    // vision
    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; 

        
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > viewDistance) return false;

        
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2) return false;

        
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance, obstacleMask))
        {
            if (hit.transform != player) return false; 
        }

        return true; 
    }
    void OnDrawGizmosSelected()
{
    // Draw vision distance
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, viewDistance);

    // Draw vision cone lines
    Vector3 forward = transform.forward * viewDistance;
    Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
    Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
    Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
}
}