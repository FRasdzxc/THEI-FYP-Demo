using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;
    public enum Panels { info}
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void TogglePanel(Panels panel, bool toggle)
    {
        switch(panel)
        {
            case Panels.info:
                infoPanel.SetActive(toggle);
                break;
        }
    }
}
