using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseSelect : MonoBehaviour
{
    private GameObject selectedObject;
    [SerializeField] private string selectableTag = "Selectable";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                RaycastHit hit = CastRay();
                if (hit.collider == null || !hit.collider.CompareTag(selectableTag))
                {
                    return;
                }
                selectedObject = hit.collider.gameObject;
                Outline outline = selectedObject.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = true;
                }
                else
                {
                    Debug.LogWarning("Selected object does not have an Outline component.");
                }
            }
            else
            {
                selectedObject.GetComponent<Outline>().enabled = false;
                selectedObject = null;
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            RaycastHit hit = CastRay();
            if (hit.collider != null)
            {
                UnitMovements unitMovements = selectedObject.GetComponent<UnitMovements>();
                if (unitMovements != null)
                {
                    unitMovements.MoveToPoint(hit.point);

                    TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
                    if (drawer != null)
                        drawer.SetDestination(hit.point);
                }
            }
        }
    }

    private RaycastHit CastRay()
    {
        Vector3 screenFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 far = Camera.main.ScreenToWorldPoint(screenFar);
        Vector3 near = Camera.main.ScreenToWorldPoint(screenNear);
        RaycastHit hit;
        Physics.Raycast(near, far - near, out hit);
        return hit;
    }
}
