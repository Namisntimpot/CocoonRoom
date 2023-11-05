using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OpenDoor : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 originPosition;
    bool opened = false;
    GameObject doorLight;
    void Start()
    {
        originPosition = transform.position;
        doorLight = transform.Find("doorLight").gameObject;
    }

    public void OpenOrCloseDoor(Hand unuse)
    {
        if (opened)
        {
            // πÿ√≈
            transform.position = originPosition;
            opened = false;
            doorLight.SetActive(false);
        }
        else
        {
            Vector3 off = new Vector3(1f, 0f, -0.2f);
            transform.position += off;
            opened = true;
            doorLight.SetActive(true);
        }
    }
}
