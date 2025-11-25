using UnityEngine;

public class Scroll_Track_MaterialOffset : MonoBehaviour
{
    [SerializeField]
    private float scrollFactor = 0.1f; // Factor for movement-based scrolling

    [SerializeField]
    private float rotationScrollFactor = 0.02f; // Factor for rotation-based scrolling

    [SerializeField]
    private float movementThreshold = 0.001f; // Minimum movement distance to trigger scrolling (lowered)

    [SerializeField]
    private float rotationThreshold = 0.01f; // Minimum rotation angle to trigger scrolling (lowered)

    private Renderer trackRenderer;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float offset = 0.0f;

    void Start()
    {
        trackRenderer = GetComponent<Renderer>();
        if (trackRenderer == null)
        {
            Debug.LogError("Renderer component not found on this GameObject!");
            return;
        }
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        if (trackRenderer != null)
        {
            // Calculate movement-based offset
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            // Calculate rotation-based offset
            float rotationDelta = Quaternion.Angle(lastRotation, transform.rotation);

            // Debug logs to check movement and rotation values
            Debug.Log($"Distance Moved: {distanceMoved}, Rotation Delta: {rotationDelta}");

            // Ignore small movements and rotations below the thresholds
            if (distanceMoved > movementThreshold || rotationDelta > rotationThreshold)
            {
                // Combine movement and rotation offsets
                offset = (offset + (distanceMoved * scrollFactor) + (rotationDelta * rotationScrollFactor)) % 1f;

                // Apply the offset to the material
                trackRenderer.material.SetTextureOffset("_BaseMap", new Vector2(offset, 0f));

                Debug.Log($"Offset Applied: {offset}");
            }

            // Update the last position and rotation
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }
}