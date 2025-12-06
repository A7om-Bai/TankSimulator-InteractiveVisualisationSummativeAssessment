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
            RaycastHit hit = CastRay();

            if (hit.collider == null)
            {
                Deselect();
                return;
            }

            if (!hit.collider.CompareTag(selectableTag))
            {
                Deselect();
                return;
            }

            TankHealth hitHealth = hit.collider.GetComponentInParent<TankHealth>();
            if (hitHealth != null && hitHealth.isDead)
            {
                Deselect();
                return;
            }

            GameObject hitObj = hit.collider.gameObject;

            if (selectedObject == null)
            {
                Select(hitObj);
            }
            else
            {
                Deselect();
                Select(hitObj);
            }
        }


        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            TankHealth selHealth = selectedObject.GetComponent<TankHealth>();
            if (selHealth != null && selHealth.isDead)
            {
                Deselect();
                return;
            }

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

    private void Select(GameObject obj)
    {
        selectedObject = obj;

        Outline outline = selectedObject.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = true;

        TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
        if (drawer != null)
            drawer.ShowLastPath();
    }

    private void Deselect()
    {
        if (selectedObject == null) return;

        Outline outline = selectedObject.GetComponent<Outline>();
        if (outline != null) outline.enabled = false;

        TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
        if (drawer != null) drawer.HidePath();

        selectedObject = null;
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
