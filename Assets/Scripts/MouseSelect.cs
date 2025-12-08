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
        // Handle left mouse button click for selecting objects
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = CastRay();

            // If no object is hit, deselect the current object
            if (hit.collider == null)
            {
                Deselect();
                return;
            }

            // If the hit object does not have the correct tag, deselect
            if (!hit.collider.CompareTag(selectableTag))
            {
                Deselect();
                return;
            }

            // If the hit object is a tank and it is dead, deselect
            TankHealth hitHealth = hit.collider.GetComponentInParent<TankHealth>();
            if (hitHealth != null && hitHealth.isDead)
            {
                Deselect();
                return;
            }

            GameObject hitObj = hit.collider.gameObject;

            // Select the new object, or switch selection if another object is already selected
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

        // Handle right mouse button click for moving the selected object
        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            TankHealth selHealth = selectedObject.GetComponent<TankHealth>();
            // If the selected object is dead, deselect it
            if (selHealth != null && selHealth.isDead)
            {
                Deselect();
                return;
            }

            // Cast a ray to determine the target position for movement
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                NavMeshHit navHit;
                // Check if the hit point is valid on the NavMesh
                if (NavMesh.SamplePosition(hit.point, out navHit, 2f, NavMesh.AllAreas))
                {
                    Vector3 finalPos = navHit.position;

                    // Command the selected object to move to the target position
                    UnitMovements unitMovements = selectedObject.GetComponent<UnitMovements>();
                    if (unitMovements != null)
                        unitMovements.MoveToPoint(finalPos);

                    // Update the path drawer to show the movement path
                    TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
                    if (drawer != null)
                    {
                        drawer.isPathVisible = true;
                        drawer.SetDestination(finalPos);
                    }

                    return;
                }
            }

            // If the target position is invalid, hide the path
            TankPathDrawer hideDrawer = selectedObject.GetComponent<TankPathDrawer>();
            if (hideDrawer != null)
                hideDrawer.HidePath();
        }
    }

    /// <summary>
    /// Selects the specified object and highlights it.
    /// </summary>
    /// <param name="obj">The object to select.</param>
    private void Select(GameObject obj)
    {
        selectedObject = obj;

        // Enable the outline component to highlight the object
        Outline outline = selectedObject.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = true;

        // Show the last movement path of the object (if available)
        TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
        if (drawer != null)
            drawer.ShowLastPath();
    }

    /// <summary>
    /// Deselects the currently selected object and removes its highlight.
    /// </summary>
    private void Deselect()
    {
        if (selectedObject == null) return;

        // Disable the outline component to remove the highlight
        Outline outline = selectedObject.GetComponent<Outline>();
        if (outline != null) outline.enabled = false;

        // Hide the movement path of the object (if available)
        TankPathDrawer drawer = selectedObject.GetComponent<TankPathDrawer>();
        if (drawer != null) drawer.HidePath();

        selectedObject = null;
    }

    /// <summary>
    /// Casts a ray from the mouse position into the scene and returns the hit information.
    /// </summary>
    /// <returns>The RaycastHit containing information about the hit object.</returns>
    private RaycastHit CastRay()
    {
        // Calculate the near and far points of the ray based on the mouse position
        Vector3 screenFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

        Vector3 far = Camera.main.ScreenToWorldPoint(screenFar);
        Vector3 near = Camera.main.ScreenToWorldPoint(screenNear);

        // Perform the raycast and return the hit information
        RaycastHit hit;
        Physics.Raycast(near, far - near, out hit);
        return hit;
    }
}
