using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("02MainMap");
    }

    public void Garage()
    {
        SceneManager.LoadScene("03Garage");
    }
}
