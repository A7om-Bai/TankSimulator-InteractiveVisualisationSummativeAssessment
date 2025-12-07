using UnityEngine;

public class UI_Delay_Disappear : MonoBehaviour
{
    public GameObject TextUI;
    public float delayTime = 3f; //default time of delay
    float timer;
    void Start()
    {
        timer = delayTime;  
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer - Time.deltaTime; // timer countdown

        if( timer < 0 )
        {
            gameObject.SetActive(false);  //hide Text
        }
    }
}
