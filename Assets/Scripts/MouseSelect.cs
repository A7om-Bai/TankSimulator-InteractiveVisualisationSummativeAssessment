using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
                    return;

                selectedObject = hit.collider.gameObject;

                Outline outline = selectedObject.GetComponent<Outline>();
                if (outline != null)
                    outline.enabled = true;

                TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
                if (drawer != null)
                    drawer.ShowLastPath();
            }
            else
            {
                Outline outline = selectedObject.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;

                TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
                if (drawer != null) drawer.HidePath();

                selectedObject = null;
            }
        }


        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, 2f, NavMesh.AllAreas))
                {
                    Vector3 finalPos = navHit.position;

                    UnitMovements unitMovements = selectedObject.GetComponent<UnitMovements>();
                    if (unitMovements != null)
                        unitMovements.MoveToPoint(finalPos);

                    TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
                    if (drawer != null)
                    {
                        drawer.isPathVisible = true;
                        drawer.SetDestination(finalPos);
                    }

                    return;
                }
            }

            TankPathDrawer hideDrawer = selectedObject.GetComponent<TankPathDrawer>();
            if (hideDrawer != null)
                hideDrawer.HidePath();
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
