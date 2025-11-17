using UnityEngine;

public class Scroll_Track_MaterialOffset : MonoBehaviour
{
    [SerializeField]
    private float scrollFactor = 0.1f;

    private Renderer trackRenderer;
    private Vector3 lastPosition;
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
    }

    void Update()
    {
        if (trackRenderer != null)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            offset = (offset + distanceMoved * scrollFactor) % 1f;

            trackRenderer.material.SetTextureOffset("_BaseMap", new Vector2(offset, 0f));

            lastPosition = transform.position;
        }
    }
}