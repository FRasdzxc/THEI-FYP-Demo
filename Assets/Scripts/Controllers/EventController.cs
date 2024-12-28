using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    private InformationRecorder ir;
    // Start is called before the first frame update
    void Start()
    {
        ir = GameObject.FindWithTag("EventSystem").GetComponent<InformationRecorder>();
    }
}
