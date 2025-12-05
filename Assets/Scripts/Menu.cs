using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour
{
    public AudioSource clickSound;

    public void PlayGame()
    {
        SceneManager.LoadScene("02MainMap");
    }

    public void Garage()
    {
        StartCoroutine(LoadGarage());
    }

    IEnumerator LoadGarage()
    {
        if (clickSound != null)
            clickSound.Play();

        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene("03Garage");
    }
}
