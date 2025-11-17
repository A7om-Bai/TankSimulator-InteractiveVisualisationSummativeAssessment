using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayClipAudio : MonoBehaviour
{
    public AudioSource[] audioSources;

    private void OnMouseDown()
    {
        if (audioSources.Length > 0)
        {
            int randomIndex = Random.Range(0, audioSources.Length);
            audioSources[randomIndex].Play();
        }
        else
        {
            Debug.Log("No audio sources assigned!");
        }
    }
}
