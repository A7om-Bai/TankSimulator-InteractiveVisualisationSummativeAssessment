using UnityEngine;

public class InMoving_TurretOffset : MonoBehaviour
{
    [SerializeField]
    private Transform tankBody;

    [SerializeField]
    private float maxOffsetAngle = 30f;

    [SerializeField]
    private float offsetSpeed = 3f;

    private float lastTankYAngle;
    private Vector3 lastTankPosition;
    private float currentOffsetAngle = 0f;
    private bool offsetDirection = true;

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

        float currentYAngle = tankBody.eulerAngles.y;
        float angleDelta = Mathf.DeltaAngle(lastTankYAngle, currentYAngle);

        currentOffsetAngle += -angleDelta;

        currentOffsetAngle = Mathf.Clamp(currentOffsetAngle, -maxOffsetAngle, maxOffsetAngle);

        float tankSpeed = (tankBody.position - lastTankPosition).magnitude / Time.deltaTime;
        if (tankSpeed > 0.1f)
        {
            float swayDelta = offsetSpeed * Time.deltaTime * (offsetDirection ? 1f : -1f);
            currentOffsetAngle += swayDelta;
            if (currentOffsetAngle >= maxOffsetAngle) offsetDirection = false;
            if (currentOffsetAngle <= -maxOffsetAngle) offsetDirection = true;
        }
        else
        {
            currentOffsetAngle = Mathf.Lerp(currentOffsetAngle, 0f, offsetSpeed * Time.deltaTime);
        }

        transform.localRotation = Quaternion.Euler(0f, currentOffsetAngle, 0f);

        lastTankYAngle = currentYAngle;
        lastTankPosition = tankBody.position;
    }
}
