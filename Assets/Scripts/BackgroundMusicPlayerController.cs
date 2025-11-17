using UnityEngine;

public class BackgroundMusicPlayerController : MonoBehaviour
{
    private void Awake()
    {
        if(FindObjectsOfType<BackgroundMusicPlayerController>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
