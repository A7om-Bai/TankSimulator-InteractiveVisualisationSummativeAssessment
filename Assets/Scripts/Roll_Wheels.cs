using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll_Wheels : MonoBehaviour {

	public UnitMovements unitMovements;
	public float speedMultiplier = 5f;
	
	void Update () 
	{
		if(unitMovements == null)
		{
			return;
        }
		float unitSpeed = unitMovements.GetCurrentsSpeed();
		for(int i= 0; i < transform.childCount; i++)
		{
			float wheelSize = transform.GetChild(i).GetComponent<MeshFilter>().mesh.bounds.size.y;
			float rotationAngle = (unitSpeed * speedMultiplier / wheelSize) * Time.deltaTime * Mathf.Rad2Deg;
			transform.GetChild(i).Rotate(rotationAngle, 0, 0, Space.Self);
        }
	}
}
