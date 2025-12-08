using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class TankPathDrawer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Color lineColor = Color.green;

    [SerializeField] private float lineWidth = 0.2f;

    [SerializeField] private Material dashedMaterial;

    [SerializeField] private float dashScrollSpeed = 1.5f;

    [SerializeField] private float dashRepeatFactor = 1.5f;

    [SerializeField] private float fadeOutSpeed = 2f;

    private LineRenderer lineRenderer;
    private Vector3 destination;
    private bool hasDestination = false;
    private bool fadingOut = false;
    private float currentAlpha = 1f;
    private float lineLength = 0f;
    public bool isPathVisible = true;
    private NavMeshPath lastPath;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    void Awake()
    {
        // Initialize the LineRenderer and NavMeshAgent components
        lineRenderer = GetComponent<LineRenderer>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        // Configure the LineRenderer properties
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.useWorldSpace = true;
        lineRenderer.alignment = LineAlignment.View;

        // Set up the dashed material for the line, or use a default material
        if (dashedMaterial != null)
        {
            lineRenderer.material = new Material(dashedMaterial);
        }
        else
        {
            Shader shader = Shader.Find("Sprites/Default");
            lineRenderer.material = new Material(shader);
            Texture2D tex = new Texture2D(2, 1);
            tex.SetPixels(new[] { Color.white, Color.clear });
            tex.Apply();
            lineRenderer.material.mainTexture = tex;
        }

        // Disable the line renderer initially
        lineRenderer.enabled = false;
        lastPath = new NavMeshPath();
    }

    void Update()
    {
        // If no destination is set, skip updates
        if (!hasDestination) return;

        // Handle fading out the line if required
        if (fadingOut)
        {
            FadeOutLine();
        }
        else
        {
            // Update the path line to reflect the current path
            UpdatePathLine();
        }

        // Scroll the dashed line texture to create a moving effect
        if (lineRenderer.enabled && lineRenderer.material != null && lineLength > 0.01f)
        {
            Vector2 offset = lineRenderer.material.mainTextureOffset;
            offset.x -= Time.deltaTime * dashScrollSpeed;
            lineRenderer.material.mainTextureOffset = offset;

            lineRenderer.material.mainTextureScale = new Vector2(lineLength * dashRepeatFactor, 1f);
        }
    }

    /// <summary>
    /// Sets the destination for the NavMeshAgent and starts drawing the path.
    /// </summary>
    /// <param name="target">The target position to navigate to.</param>
    public void SetDestination(Vector3 target)
    {
        destination = target;
        hasDestination = true;
        fadingOut = false;
        currentAlpha = 1f;
        isPathVisible = true;
        lineRenderer.enabled = true;

        // Update the path line to reflect the new destination
        UpdatePathLine();
    }

    /// <summary>
    /// Updates the path line based on the NavMeshAgent's calculated path.
    /// </summary>
    private void UpdatePathLine()
    {
        if (agent == null || !hasDestination) return;

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(destination, path) && path.corners.Length > 1)
        {
            lastPath = path;

            // Update the line renderer to display the path
            if (isPathVisible)
            {
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
            }

            // Calculate the total length of the path
            lineLength = 0f;
            for (int i = 1; i < path.corners.Length; i++)
                lineLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);

            // Update the line color with the current alpha value
            Color c = lineColor;
            c.a = currentAlpha;
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
        }

        // Start fading out the line if the agent is close to the destination
        if (!agent.pathPending && agent.remainingDistance > 0f && agent.remainingDistance < 0.5f)
            StartFadeOut();
    }

    /// <summary>
    /// Starts fading out the path line.
    /// </summary>
    private void StartFadeOut()
    {
        fadingOut = true;
        hasDestination = false;
    }

    /// <summary>
    /// Gradually fades out the path line by reducing its alpha value.
    /// </summary>
    private void FadeOutLine()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, fadeOutSpeed * Time.deltaTime);

        // Update the line color with the new alpha value
        Color c = lineColor;
        c.a = currentAlpha;
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;

        // Disable the line renderer once the line is fully faded out
        if (currentAlpha <= 0.01f)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
            fadingOut = false;
        }
    }

    /// <summary>
    /// Sets the destination using NavMesh sampling to ensure the target position is valid.
    /// </summary>
    /// <param name="targetPosition">The target position to navigate to.</param>
    public void SetDestinationWithNavMesh(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            SetDestination(hit.position);
        }
    }

    /// <summary>
    /// Hides the path line immediately.
    /// </summary>
    public void HidePath()
    {
        isPathVisible = false;
        fadingOut = false;
        currentAlpha = 0f;

        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// Restores and displays the last calculated path.
    /// </summary>
    public void ShowLastPath()
    {
        if (lastPath == null || lastPath.corners.Length == 0)
            return;

        isPathVisible = true;
        fadingOut = false;
        currentAlpha = 1f;
        lineRenderer.enabled = true;

        // Restore the path line using the last calculated path
        lineRenderer.positionCount = lastPath.corners.Length;
        lineRenderer.SetPositions(lastPath.corners);
    }
}
