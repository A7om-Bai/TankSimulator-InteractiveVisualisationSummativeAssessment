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

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.useWorldSpace = true;
        lineRenderer.alignment = LineAlignment.View;

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

        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!hasDestination) return;

        if (fadingOut)
        {
            FadeOutLine();
        }
        else
        {
            UpdatePathLine();
        }

        if (lineRenderer.enabled && lineRenderer.material != null && lineLength > 0.01f)
        {
            Vector2 offset = lineRenderer.material.mainTextureOffset;
            offset.x -= Time.deltaTime * dashScrollSpeed;
            lineRenderer.material.mainTextureOffset = offset;

            lineRenderer.material.mainTextureScale = new Vector2(lineLength * dashRepeatFactor, 1f);
        }
    }

    public void SetDestination(Vector3 target)
    {
        destination = target;
        hasDestination = true;
        fadingOut = false;
        currentAlpha = 1f;
        lineRenderer.enabled = true;
        UpdatePathLine();
    }

    private void UpdatePathLine()
    {
        if (agent == null || !hasDestination) return;

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(destination, path) && path.corners.Length > 1)
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);

            lineLength = 0f;
            for (int i = 1; i < path.corners.Length; i++)
                lineLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);

            Color c = lineColor;
            c.a = currentAlpha;
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
        }

        if (!agent.pathPending && agent.remainingDistance > 0f && agent.remainingDistance < 0.5f)
            StartFadeOut();
    }

    private void StartFadeOut()
    {
        fadingOut = true;
        hasDestination = false;
    }

    private void FadeOutLine()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, fadeOutSpeed * Time.deltaTime);

        Color c = lineColor;
        c.a = currentAlpha;
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;

        if (currentAlpha <= 0.01f)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
            fadingOut = false;
        }
    }

    public void SetDestinationWithNavMesh(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            SetDestination(hit.position);
        }
    }
}
