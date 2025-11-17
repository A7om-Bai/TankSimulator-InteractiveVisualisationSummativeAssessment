using UnityEngine;
using UnityEngine.UIElements;

public class ExitManager : MonoBehaviour
{
    public GameObject exitPoint;
    public bool visible = false;

    public void Quit()
    {
        Debug.Log("The Game is Quitting...");
        Application.Quit();
    }

    public void ShowWindow()
    {
        if (exitPoint != null)
        {
            exitPoint.SetActive(true);
        }
    }

    public void HideWindow()
    {
        if (exitPoint != null)
        {
            exitPoint.SetActive(false);
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            exitPoint.SetActive(true);
        }
    }
}
