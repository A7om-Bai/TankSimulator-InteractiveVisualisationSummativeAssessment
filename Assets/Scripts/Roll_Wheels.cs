using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll_Wheels : MonoBehaviour
{
    public UnitMovements unitMovements;
    public float speedMultiplier = 10f;

    private Quaternion lastRotation;

    void Start()
    {
        if (unitMovements == null)
        {
            Debug.LogError("UnitMovements component is not assigned!");
            enabled = false;
            return;
        }

        lastRotation = unitMovements.transform.rotation; // Initialize the last rotation
    }

    void Update()
    {
        if (unitMovements == null)
        {
            return;
        }

        // Get the linear speed of the tank
        float unitSpeed = unitMovements.GetCurrentsSpeed();

        // Calculate the rotational speed of the tank
        Quaternion currentRotation = unitMovements.transform.rotation;
        float rotationDelta = Quaternion.Angle(lastRotation, currentRotation);
        lastRotation = currentRotation;

        // Combine linear and rotational speeds
        float combinedSpeed = unitSpeed + (rotationDelta * Mathf.Deg2Rad);

        // Rotate the wheels based on the combined speed
        for (int i = 0; i < transform.childCount; i++)
        {
            float wheelSize = transform.GetChild(i).GetComponent<MeshFilter>().mesh.bounds.size.y;
            float rotationAngle = (combinedSpeed * speedMultiplier / wheelSize) * Time.deltaTime * Mathf.Rad2Deg;
            transform.GetChild(i).Rotate(rotationAngle, 0, 0, Space.Self);
        }
    }
}
