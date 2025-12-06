using UnityEngine;
using System.Collections;

/* 
 * This is an improved orbit script based on the MouseOrbitImproved script found
 * on the unity community wiki. It should run smoother then the original version
 * 
 * */
[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class DragOrbitImproved : MonoBehaviour
{
	public Transform target;
	public float distance = 30.0f;
	public float xSpeed = 0.2f;
	public float ySpeed = 3.0f;
	
	public float yMinLimit = 4f;
	public float yMaxLimit = 45f;
	
	public float distanceMin = 5f;
	public float distanceMax = 30f;
	
	public float smoothTime = 5f;

	public float zoomSpeed = 15.0f;
	
	float rotationYAxis = 0.0f;
	float rotationXAxis = 0.0f;
	
	float velocityX = 0.0f;
	float velocityY = 0.0f;

	public bool detectColliders = false;

    // Use this for initialization

    void Start()
    {
        if (target)
        {
            Vector3 angles = transform.eulerAngles;
            rotationYAxis = angles.y;
            rotationXAxis = angles.x;

            distance = Vector3.Distance(transform.position, target.position);
        }

        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }


    // Late update happens after the normal update, and is used to make sure other 
    // update events have happened first
    void LateUpdate()
	{
        if (!Input.GetMouseButton(0) && Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) < 0.001f)
            return;

        if (target)
		{
			if (Input.GetMouseButton(0))
			{
				velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
				velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
			}
			
			rotationYAxis += velocityX;
			rotationXAxis -= velocityY;
			
			rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
			
			//Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
			Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
			Quaternion rotation = toRotation;
			
			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, distanceMin, distanceMax);


			if (detectColliders) {
				RaycastHit hit;
				if (Physics.Linecast(target.position, transform.position, out hit))
				{
					distanceMin = hit.distance;
				}
			}
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;
			
			velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
		}
		
	}

	// Clamp angle make sure the orbit does not flip the object upside down
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}