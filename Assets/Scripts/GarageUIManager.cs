using UnityEngine;
using System.Collections.Generic;

public class GarageUIManager : MonoBehaviour
{
    public List<GameObject> panels;
    void Start()
    {
        HideUI(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HideUI()
    {
        for(int i = 0; i < panels.Count; i= i+1)
        {
            GameObject panel = panels[i];
            if ( panel!= null)
            {
                panel.SetActive(false);
            }
        }
    }
    public void ShowUI(int index)
    {
        HideUI();

        if (index >0 && index < panels.Count)
        {
            GameObject panel = panels[index];            
            
            panel.SetActive(true);
        }
    }
}
