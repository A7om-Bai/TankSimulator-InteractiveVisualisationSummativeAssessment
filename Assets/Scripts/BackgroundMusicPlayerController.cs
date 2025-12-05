using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicPlayerController : MonoBehaviour
{
    private void Awake()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "03Garage")
        {
            var all = FindObjectsOfType<BackgroundMusicPlayerController>();
            foreach (var bgm in all)
            {
                if (bgm != this && bgm.gameObject.scene.name == "DontDestroyOnLoad")
                {
                    Destroy(bgm.gameObject);
                }
            }
            return;
        }

        var existing = FindObjectsOfType<BackgroundMusicPlayerController>();

        if (existing.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
