using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OpenDoor : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 originPosition;
    bool opened = false;
    void Start()
    {
        originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenOrCloseDoor(Hand unuse)
    {
        Debug.Log("trigger");
        if (opened)
        {
            transform.position = originPosition;
        }
        else
        {
            Vector3 off = new Vector3(1f, 0f, -0.2f);
            transform.position += off;
        }
    }
}
