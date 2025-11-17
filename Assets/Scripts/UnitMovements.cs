using UnityEngine;
using UnityEngine.AI;

public class UnitMovements : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject selectedObject;

    [Header("Tank Movement Settings")]
    public float maxSpeed = 4f;
    public float acceleration = 2f;
    public float turnSpeed = 120f;
    public float turnThreshold = 8f;
    public float waypointReachDistance = 0.5f;

    [Header("Avoidance Settings")]
    public float avoidanceCheckDistance = 6f;
    public float avoidanceTurnAngle = 45f;
    [Range(0f, 1f)] public float avoidanceSlowdown = 0.6f;
    public float avoidanceMemoryTime = 1.5f;

    [Header("Collision Push Settings")]
    [Tooltip("Checking radius")]
    public float pushRadius = 2.2f;
    [Tooltip("Push Force")]
    public float pushForce = 1.5f;
    [Tooltip("Lowest Distance")]
    public float minSeparation = 1.8f;

    private bool hasDestination = false;
    private int currentCornerIndex = 0;
    private Vector3[] pathCorners;
    private float currentSpeed = 0f;
    private float avoidanceDirection = 0f;
    private float avoidanceTimer = 0f;
    private bool isAvoiding = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        selectedObject = transform.Find("SelectedIndicator").gameObject;
        SetSelectedSelf(false);
        agent.updateRotation = false;
        agent.updatePosition = false;
    }

    void Update()
    {
        if (hasDestination)
        {
            FollowPath();
            ApplySoftCollisionPush();
        }
    }

    public void MoveToPoint(Vector3 point)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(point, path) && path.corners.Length > 1)
        {
            pathCorners = path.corners;
            currentCornerIndex = 1;
            hasDestination = true;
            currentSpeed = 0f;
            isAvoiding = false;
        }
    }

    private void FollowPath()
    {
        if (pathCorners == null || currentCornerIndex >= pathCorners.Length)
        {
            StopMovement();
            return;
        }

        Vector3 targetPos = pathCorners[currentCornerIndex];
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude < waypointReachDistance)
        {
            currentCornerIndex++;
            if (currentCornerIndex >= pathCorners.Length)
            {
                StopMovement();
                return;
            }
            targetPos = pathCorners[currentCornerIndex];
            toTarget = targetPos - transform.position;
            toTarget.y = 0f;
        }

        Vector3 desiredDir = toTarget.normalized;
        desiredDir = ApplyAvoidance(desiredDir);

        Quaternion targetRot = Quaternion.LookRotation(desiredDir);
        float angle = Quaternion.Angle(transform.rotation, targetRot);

        if (angle > 0.1f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        float targetSpeed = maxSpeed;
        if (isAvoiding)
            targetSpeed *= avoidanceSlowdown;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
        agent.nextPosition = transform.position;
    }

    private Vector3 ApplyAvoidance(Vector3 desiredDir)
    {
        Vector3 origin = transform.position + Vector3.up * 0.6f;
        bool hitFront = Physics.Raycast(origin, transform.forward, out RaycastHit hitFrontInfo, avoidanceCheckDistance);
        bool hitLeft = Physics.Raycast(origin, Quaternion.Euler(0, -20, 0) * transform.forward, avoidanceCheckDistance * 0.8f);
        bool hitRight = Physics.Raycast(origin, Quaternion.Euler(0, 20, 0) * transform.forward, avoidanceCheckDistance * 0.8f);

        if (hitFront && hitFrontInfo.transform.CompareTag("Selectable") && hitFrontInfo.transform != transform)
        {
            if (!hitLeft && hitRight) avoidanceDirection = -1f;
            else if (!hitRight && hitLeft) avoidanceDirection = 1f;
            else avoidanceDirection = Random.value > 0.5f ? 1f : -1f;

            isAvoiding = true;
            avoidanceTimer = avoidanceMemoryTime;
        }

        if (isAvoiding)
        {
            Quaternion offsetRot = Quaternion.Euler(0f, avoidanceTurnAngle * avoidanceDirection, 0f);
            desiredDir = offsetRot * desiredDir;

            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0f)
            {
                isAvoiding = false;
                avoidanceDirection = 0f;
            }
        }

        return desiredDir.normalized;
    }

    private void ApplySoftCollisionPush()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pushRadius);
        Vector3 pushVector = Vector3.zero;
        int pushCount = 0;

        foreach (Collider col in hits)
        {
            if (col.transform == transform) continue;
            if (!col.CompareTag("Selectable")) continue;

            Vector3 diff = transform.position - col.transform.position;
            diff.y = 0f;
            float dist = diff.magnitude;

            if (dist < minSeparation && dist > 0.001f)
            {
                float strength = (minSeparation - dist) / minSeparation;
                pushVector += diff.normalized * strength;
                pushCount++;
            }
        }

        if (pushCount > 0)
        {
            pushVector /= pushCount;
            transform.position += pushVector * (pushForce * Time.deltaTime);
        }
    }

    private void StopMovement()
    {
        hasDestination = false;
        pathCorners = null;
        currentCornerIndex = 0;
        currentSpeed = 0f;
        isAvoiding = false;
        agent.ResetPath();
    }

    public void SetSelectedSelf(bool isSelected)
    {
        selectedObject.SetActive(isSelected);
    }

    public float GetCurrentsSpeed()
    {
        return currentSpeed;
    }
}
