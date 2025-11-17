using UnityEngine;
public class RTSCameraController : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public Vector2 moveLimits = new Vector2(50f, 50f);

    public float zoomSpeed = 10.0f;
    public float zoomMin = 5f;
    public float zoomMax = 50f;

    public float rotateSpeed = 5.0f;
    public float yMinLimit = 10f;
    public float yMaxLimit = 80f;

    private float rotationYAxis = 0.0f;
    private float rotationXAxis = 45.0f;
    private float currentZoom = 20.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        currentZoom = Mathf.Clamp(currentZoom, zoomMin, zoomMax);
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
        HandleVerticalDrag();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        Vector3 worldMove = transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + new Vector3(worldMove.x, 0, worldMove.z);
        newPosition.x = Mathf.Clamp(newPosition.x, -moveLimits.x, moveLimits.x);
        newPosition.z = Mathf.Clamp(newPosition.z, -moveLimits.y, moveLimits.y);

        transform.position = newPosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        currentZoom = Mathf.Clamp(currentZoom - scroll * zoomSpeed, zoomMin, zoomMax);

        Vector3 position = transform.position;
        position.y = currentZoom;
        transform.position = position;
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotationYAxis += mouseX * rotateSpeed;
            rotationXAxis -= mouseY * rotateSpeed;

            rotationXAxis = Mathf.Clamp(rotationXAxis, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            transform.rotation = rotation;
        }
    }

    private void HandleVerticalDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            float mouseY = Input.GetAxisRaw("Mouse Y");
            Vector3 position = transform.position;
            position.y += mouseY * moveSpeed * Time.deltaTime;
            position.y = Mathf.Clamp(position.y, zoomMin, zoomMax);
            
            transform.position = position;
        }
    }
}