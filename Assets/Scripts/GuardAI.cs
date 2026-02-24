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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = patrolPoints[currentPoint].position;
    }

    void Update()
    {
        Patrol();
        DetectPlayer();
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

        // Flatten Y positions so vertical difference is ignored
        Vector3 guardPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPos = new Vector3(player.position.x, 0, player.position.z);

        float distance = Vector3.Distance(guardPos, playerPos);

        Debug.Log("Distance to player: " + distance); // For debugging

        if (distance < detectionRange)
        {
            Debug.Log("Caught!");
            // Add your caught logic here
        }
    }
}