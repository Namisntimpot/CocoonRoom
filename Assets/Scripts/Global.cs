using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    //全局使用的一些值
    public Transform[] Controllers = new Transform[2];   // 0-left hand, 1 - right hand.
    static public Color[] cubeColors = new Color[]
    {
        Color.red, Color.green, Color.blue, Color.white,
    };
}
