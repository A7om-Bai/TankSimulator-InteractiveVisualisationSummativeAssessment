using UnityEngine;

public class InMoving_TurretOffset : MonoBehaviour
{
    [SerializeField]
    private Transform tankBody;

    [SerializeField]
    private float maxOffsetAngle = 20f;

    [SerializeField]
    private float offsetSpeed = 4f;

    [SerializeField]
    private float idleSwayAmplitude = 2f; // Amplitude of the sway when idle

    [SerializeField]
    private float idleSwayFrequency = 1f; // Frequency of the sway when idle

    private float lastTankYAngle;
    private Vector3 lastTankPosition;
    public float CurrentOffsetAngle { get; private set; } = 0f;
    private bool offsetDirection = true;
    private float idleSwayTimer = 0f; // Timer for idle sway

    void Start()
    {
        if (tankBody == null)
        {
            Debug.LogError("Tank body is not assigned!");
            enabled = false;
            return;
        }

        lastTankYAngle = tankBody.eulerAngles.y;
        lastTankPosition = tankBody.position;
    }

    void Update()
    {
        if (tankBody == null) return;

        // Get the current rotation and position of the tank body
        float currentYAngle = tankBody.eulerAngles.y;
        float angleDelta = Mathf.DeltaAngle(lastTankYAngle, currentYAngle);

        // Calculate the target offset angle based on the tank's rotation
        float targetOffsetAngle = CurrentOffsetAngle - angleDelta;
        targetOffsetAngle = Mathf.Clamp(targetOffsetAngle, -maxOffsetAngle, maxOffsetAngle);

        // Calculate the tank's movement speed
        float tankSpeed = (tankBody.position - lastTankPosition).magnitude / Time.deltaTime;

        if (tankSpeed > 0.1f)
        {
            // Moving state: sway based on direction
            float swayDelta = offsetSpeed * Time.deltaTime * (offsetDirection ? 1f : -1f);
            targetOffsetAngle += swayDelta;

            // Reverse sway direction if the offset angle exceeds the maximum
            if (targetOffsetAngle >= maxOffsetAngle) offsetDirection = false;
            if (targetOffsetAngle <= -maxOffsetAngle) offsetDirection = true;

            // Reset idle sway timer when moving
            idleSwayTimer = 0f;
        }
        else
        {
            // Idle state: apply small sway using sine wave
            idleSwayTimer += Time.deltaTime * idleSwayFrequency;
            float idleSway = Mathf.Sin(idleSwayTimer) * idleSwayAmplitude;

            // Gradually reduce the target offset angle towards the idle sway
            targetOffsetAngle = Mathf.Lerp(CurrentOffsetAngle, idleSway, Time.deltaTime * (offsetSpeed * 0.5f));
        }

        // Smoothly transition to the target offset angle
        CurrentOffsetAngle = Mathf.Lerp(CurrentOffsetAngle, targetOffsetAngle, Time.deltaTime * offsetSpeed);

        // Apply the rotation to the turret
        transform.localRotation = Quaternion.Euler(0f, CurrentOffsetAngle, 0f);

        // Update the last known tank state
        lastTankYAngle = currentYAngle;
        lastTankPosition = tankBody.position;
    }
}
