using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GuardAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    private int currentPoint = 0;
    private NavMeshAgent agent;

    [Header("Statue Start Settings")]
    public bool startAsStatue = true;
    public float wakeDelay = 3f;
    private bool isStatue = true;

    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 2f;
    public float chaseStopDistance = 8f;

    [Header("Detection Meter Settings")]
    public float detectionRaiseRate = 0.5f;
    public float detectionDecayRate = 0.2f;
    public float chaseDetectionRate = 0.8f;

    private bool isChasing = false;
    private DetectionMeter detectionMeter;
    private float detectionGracePeriod = 2f;
    private float graceTimer = 0f;

    [Header("Vision Cone Settings")]
    public float viewDistance = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Ghost Hover Animation")]
    public float hoverHeight = 0.5f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.3f;
    private float startY;

    [Header("Safe Zone Settings")]
    public string safeZoneTag = "SafeZone";

    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 7f;

    private ChaseManager chaseManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
            agent.destination = patrolPoints[currentPoint].position;

        startY = transform.position.y;

        detectionMeter = FindObjectOfType<DetectionMeter>(true);
        chaseManager = FindObjectOfType<ChaseManager>();

        if (startAsStatue)
        {
            isStatue = true;
            agent.isStopped = true;
        }
        else
        {
            isStatue = false;
        }
    }

    void Update()
    {
        if (isStatue) return;

        if (!isChasing)
        {
            Patrol();
            DetectPlayer();
        }
        else
        {
            ChasePlayer();
        }

        UpdateDetectionMeter();

        // hover animation
        Vector3 pos = transform.position;
        pos.y = startY + hoverHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = pos;
    }

    // Triggered by WakeTrigger
    public void WakeGhost()
    {
        if (!isStatue) return;

        // Start flickering lights immediately
        if (chaseManager != null)
            chaseManager.StartChase();

        StartCoroutine(WakeDelayRoutine());
    }

    IEnumerator WakeDelayRoutine()
    {
        yield return new WaitForSeconds(wakeDelay);

        isStatue = false;
        agent.isStopped = false;

        // Start chasing after delay
        StartChase();
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPoint].position;
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        if (IsPlayerInSafeZone())
        {
            StopChase();
            return;
        }

        if (CanSeePlayer())
        {
            StartChase();
            return;
        }

        Vector3 guardPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPos = new Vector3(player.position.x, 0, player.position.z);

        if (Vector3.Distance(guardPos, playerPos) < detectionRange)
            StartChase();
    }

    void ChasePlayer()
    {
        if (player == null) return;

        if (IsPlayerInSafeZone())
        {
            StopChase();
            agent.destination = patrolPoints[currentPoint].position;
            return;
        }

        // chase speed
        agent.speed = chaseSpeed;

        // always follow player until safe zone
        agent.destination = player.position;
    }

    void StartChase()
    {
        if (isChasing) return;

        isChasing = true;
        agent.speed = chaseSpeed;

        if (chaseManager != null)
            chaseManager.StartChase();
    }

    void StopChase()
    {
        if (!isChasing) return;

        isChasing = false;
        agent.speed = patrolSpeed;

        if (chaseManager != null)
            chaseManager.StopChase();
    }

    void UpdateDetectionMeter()
    {
        if (detectionMeter == null || player == null) return;

        graceTimer += Time.deltaTime;

        if (graceTimer < detectionGracePeriod)
        {
            detectionMeter.SetActiveDetection(false);
            detectionMeter.ResetDetection();
            return;
        }

        if (IsPlayerInSafeZone())
        {
            detectionMeter.SetActiveDetection(false);
            detectionMeter.DecreaseDetection(detectionDecayRate * Time.deltaTime);
            return;
        }

        float dist = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(player.position.x, 0, player.position.z));

        bool detecting = dist < detectionRange;
        detectionMeter.SetActiveDetection(detecting);

        if (detecting)
        {
            float proximityFactor = 1f - Mathf.Clamp01(dist / detectionRange);

            float rate = isChasing
                ? chaseDetectionRate
                : Mathf.Lerp(detectionRaiseRate * 0.3f, detectionRaiseRate, proximityFactor);

            detectionMeter.IncreaseDetection(rate * Time.deltaTime);
        }
        else
        {
            detectionMeter.DecreaseDetection(detectionDecayRate * Time.deltaTime);
        }
    }

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

    bool IsPlayerInSafeZone()
    {
        if (player == null) return false;

        Collider[] hitColliders = Physics.OverlapSphere(player.position, 0.1f);

        foreach (var col in hitColliders)
        {
            if (col.CompareTag(safeZoneTag))
                return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = transform.forward * viewDistance;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}