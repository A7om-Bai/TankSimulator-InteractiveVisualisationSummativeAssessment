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
            FollowPath(); // Follow the calculated path to the destination
            ApplySoftCollisionPush(); // Apply soft collision push to avoid overlapping with other objects
        }
    }

    public void MoveToPoint(Vector3 point)
    {
        // Calculate a path to the specified point
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(point, path) && path.corners.Length > 1)
        {
            pathCorners = path.corners; // Store the calculated path corners
            currentCornerIndex = 1; // Start at the first waypoint
            hasDestination = true; // Set the destination flag
            currentSpeed = 0f; // Reset the current speed
            isAvoiding = false; // Reset the avoidance state
        }
    }

    private void FollowPath()
    {
        // Stop movement if there are no more waypoints
        if (pathCorners == null || currentCornerIndex >= pathCorners.Length)
        {
            StopMovement();
            return;
        }

        Vector3 targetPos = pathCorners[currentCornerIndex]; // Get the current waypoint
        Vector3 toTarget = targetPos - transform.position; // Calculate the vector to the waypoint
        toTarget.y = 0f; // Ignore the y-axis for movement

        // Check if the waypoint is reached
        if (toTarget.magnitude < waypointReachDistance)
        {
            currentCornerIndex++; // Move to the next waypoint
            if (currentCornerIndex >= pathCorners.Length)
            {
                StopMovement(); // Stop movement if all waypoints are reached
                return;
            }
            targetPos = pathCorners[currentCornerIndex];
            toTarget = targetPos - transform.position;
            toTarget.y = 0f;
        }

        Vector3 desiredDir = toTarget.normalized; // Normalize the direction vector
        desiredDir = ApplyAvoidance(desiredDir); // Apply obstacle avoidance logic

        Quaternion targetRot = Quaternion.LookRotation(desiredDir); // Calculate the target rotation
        float angle = Quaternion.Angle(transform.rotation, targetRot); // Calculate the angle to the target rotation

        // Determine if the target is far or near
        bool isFarTarget = toTarget.magnitude > avoidanceCheckDistance;

        if (angle > turnThreshold && !isFarTarget)
        {
            // For near targets, rotate the tank before moving
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            currentSpeed = 0f; // Stop movement until rotation is complete
        }
        else
        {
            // For far targets or normal cases, rotate and move simultaneously
            if (angle > 0.1f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

            float targetSpeed = maxSpeed; // Set the target speed
            if (isAvoiding)
                targetSpeed *= avoidanceSlowdown; // Slow down if avoiding obstacles

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime); // Adjust speed
            transform.position += transform.forward * currentSpeed * Time.deltaTime; // Move forward
            agent.nextPosition = transform.position; // Update the NavMeshAgent position
        }
    }

    private Vector3 ApplyAvoidance(Vector3 desiredDir)
    {
        // Check for obstacles in front, left, and right directions
        Vector3 origin = transform.position + Vector3.up * 0.6f;
        bool hitFront = Physics.Raycast(origin, transform.forward, out RaycastHit hitFrontInfo, avoidanceCheckDistance);
        bool hitLeft = Physics.Raycast(origin, Quaternion.Euler(0, -20, 0) * transform.forward, avoidanceCheckDistance * 0.8f);
        bool hitRight = Physics.Raycast(origin, Quaternion.Euler(0, 20, 0) * transform.forward, avoidanceCheckDistance * 0.8f);

        if (hitFront && hitFrontInfo.transform.CompareTag("Selectable") && hitFrontInfo.transform != transform)
        {
            // Determine avoidance direction based on obstacle positions
            if (!hitLeft && hitRight) avoidanceDirection = -1f;
            else if (!hitRight && hitLeft) avoidanceDirection = 1f;
            else avoidanceDirection = Random.value > 0.5f ? 1f : -1f;

            isAvoiding = true; // Set avoidance state
            avoidanceTimer = avoidanceMemoryTime; // Reset avoidance timer
        }

        if (isAvoiding)
        {
            // Apply avoidance rotation
            Quaternion offsetRot = Quaternion.Euler(0f, avoidanceTurnAngle * avoidanceDirection, 0f);
            desiredDir = offsetRot * desiredDir;

            avoidanceTimer -= Time.deltaTime; // Decrease avoidance timer
            if (avoidanceTimer <= 0f)
            {
                isAvoiding = false; // Reset avoidance state
                avoidanceDirection = 0f;
            }
        }

        return desiredDir.normalized; // Return the adjusted direction
    }

    private void ApplySoftCollisionPush()
    {
        // Check for nearby objects within the push radius
        Collider[] hits = Physics.OverlapSphere(transform.position, pushRadius);
        Vector3 pushVector = Vector3.zero;
        int pushCount = 0;

        foreach (Collider col in hits)
        {
            if (col.transform == transform) continue; // Skip self
            if (!col.CompareTag("Selectable")) continue; // Skip non-selectable objects

            Vector3 diff = transform.position - col.transform.position; // Calculate the vector to the object
            diff.y = 0f; // Ignore the y-axis
            float dist = diff.magnitude; // Calculate the distance to the object

            if (dist < minSeparation && dist > 0.001f)
            {
                // Calculate the push strength based on the distance
                float strength = (minSeparation - dist) / minSeparation;
                pushVector += diff.normalized * strength;
                pushCount++;
            }
        }

        if (pushCount > 0)
        {
            // Apply the average push vector
            pushVector /= pushCount;
            transform.position += pushVector * (pushForce * Time.deltaTime);
        }
    }

    private void StopMovement()
    {
        // Reset movement-related variables
        hasDestination = false;
        pathCorners = null;
        currentCornerIndex = 0;
        currentSpeed = 0f;
        isAvoiding = false;
        agent.ResetPath(); // Clear the NavMeshAgent path
    }

    public void SetSelectedSelf(bool isSelected)
    {
        selectedObject.SetActive(isSelected); // Show or hide the "SelectedIndicator"
    }

    public float GetCurrentsSpeed()
    {
        return currentSpeed; // Return the current movement speed
    }
}
