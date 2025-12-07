using UnityEngine;
using System.Collections.Generic;

public class GarageUIManager : MonoBehaviour
{
    public List<GameObject> panels; //make a list of ui panles
    void Start()
    {
        HideUI(); //Hide all UI panels at the start of the game
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HideUI()
    {
        for(int i = 0; i < panels.Count; i= i+1) // look through every panels in the list
        {
            GameObject panel = panels[i];
            if ( panel!= null)  // if the panel not exist
            {
                panel.SetActive(false); // hide ui
            }
        }
    }
    public void ShowUI(int index)
    {
        HideUI(); // hide all panels at first

        if (index >0 && index < panels.Count)  // make sure the index of panel is always smaller than the length of list
        {
            GameObject panel = panels[index];            //get the panels
            
            panel.SetActive(true);   // show panels
        }
    }
}
