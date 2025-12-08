using UnityEngine;

public class InMoving_TurretOffset : MonoBehaviour
{
    [SerializeField]
    private Transform tankBody;

    [SerializeField]
    public float maxOffsetAngle = 30f;

    [SerializeField]
    private float offsetSpeed = 4f;

    [SerializeField]
    private float idleSwayAmplitude = 2f;

    [SerializeField]
    private float idleSwayFrequency = 1f;

    private float lastTankYAngle;
    private Vector3 lastTankPosition;

    public float CurrentOffsetAngle { get; private set; } = 0f;

    private bool offsetDirection = true;
    private float idleSwayTimer = 0f;

    private TankCombatSystem combat;

    void Start()
    {
        combat = GetComponentInParent<TankCombatSystem>();

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

        bool isInCombat = combat != null && combat.IsInCombat;
        float tankSpeed = (tankBody.position - lastTankPosition).magnitude / Time.deltaTime;

        float targetOffsetAngle = CurrentOffsetAngle;

        // ================== MOVING ==================
        if (tankSpeed > 0.1f)
        {
            float currentYAngle = tankBody.eulerAngles.y;
            float angleDelta = Mathf.DeltaAngle(lastTankYAngle, currentYAngle);

            targetOffsetAngle -= angleDelta;

            float swayDelta = offsetSpeed * Time.deltaTime * (offsetDirection ? 1f : -1f);
            targetOffsetAngle += swayDelta;

            if (targetOffsetAngle >= maxOffsetAngle) offsetDirection = false;
            if (targetOffsetAngle <= -maxOffsetAngle) offsetDirection = true;

            idleSwayTimer = 0f;
        }
        // ================== COMBAT (NO IDLE) ==================
        else if (isInCombat)
        {
            float currentYAngle = tankBody.eulerAngles.y;
            float angleDelta = Mathf.DeltaAngle(lastTankYAngle, currentYAngle);

            targetOffsetAngle -= angleDelta;
            idleSwayTimer = 0f;
        }
        // ================== TRUE IDLE ==================
        else
        {
            if (Mathf.Abs(CurrentOffsetAngle) > 1.0f)
            {
                targetOffsetAngle = Mathf.Lerp(
                    CurrentOffsetAngle,
                    0f,
                    Time.deltaTime * offsetSpeed
                );

                idleSwayTimer = 0f;
            }
            else
            {
                idleSwayTimer += Time.deltaTime * idleSwayFrequency;

                float idleSway = Mathf.Sin(idleSwayTimer) * idleSwayAmplitude;

                targetOffsetAngle = Mathf.Lerp(
                    CurrentOffsetAngle,
                    idleSway,
                    Time.deltaTime * (offsetSpeed * 0.5f)
                );
            }
        }


        // Clamp & Smooth
        targetOffsetAngle = Mathf.Clamp(targetOffsetAngle, -maxOffsetAngle, maxOffsetAngle);
        CurrentOffsetAngle = Mathf.Lerp(
            CurrentOffsetAngle,
            targetOffsetAngle,
            Time.deltaTime * offsetSpeed
        );

        // Cache state
        lastTankYAngle = tankBody.eulerAngles.y;
        lastTankPosition = tankBody.position;
    }
}
