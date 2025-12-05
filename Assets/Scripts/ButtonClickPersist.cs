using UnityEngine;

public class ButtonClickPersist : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClick()
    {
        audioSource.Play();
        Destroy(gameObject, 0.2f);
    }
}
