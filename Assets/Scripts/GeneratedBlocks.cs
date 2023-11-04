using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class GeneratedBlocks : MonoBehaviour
{
    // Start is called before the first frame update
    bool isPicked = false, isPlaced = false;
    public Color color;

    public SteamVR_Action_Pose pose = SteamVR_Input.GetAction<SteamVR_Action_Pose>("default", "pose");
    public SteamVR_Action_Boolean grabpinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "grabpinch");  // �����ж�����ֻ�ְ���
    SteamVR_Input_Sources hand = SteamVR_Input_Sources.Any;  // ���ֻ�������

    Vector2 coordToPlace;   // �����ŷ���� room�ռ�����.
    Vector3 roomOrigin;     // �������ķ������������.

    FloorGrid floorGrid; 

    void Start()
    {
        // "/room/smallblocks/smallblock"
        roomOrigin = this.transform.parent.parent.position;
        floorGrid = this.transform.parent.parent.GetComponentInChildren<FloorGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPicked)
            return;
        var pos = pose[hand].localPosition;  // �����universe origin
        var rot = pose[hand].localRotation;
        var forward = (rot * Vector3.forward).normalized;
        computeCoordToPlace(pos, forward);
    }

    public void onPickup()
    {
        isPicked = true;
        this.GetComponent<Animation>().Stop();
        this.GetComponent<Rigidbody>().useGravity = true;
        if (grabpinch[SteamVR_Input_Sources.LeftHand].state)
            hand = SteamVR_Input_Sources.LeftHand;
        else
            hand = SteamVR_Input_Sources.RightHand;
        Debug.Log(hand);
    }

    public void onDetach()
    {
        Debug.Log("Detach");
        isPicked = false;
        isPlaced = true;
        floorGrid.placeCube(coordToPlace, color);
    }

    /// <summary>
    ///  ���������ֱ���ָ��������ſ��λ��
    /// </summary>
    /// <param name="controllerWS"></param> �ֱ�����������
    /// <param name="forwardWS"></param>  �ֱ��������������
    void computeCoordToPlace(Vector3 controllerWS, Vector3 forwardWS)
    {
        // ��������ڱ����������, roomSpace
        Vector3 posRS = controllerWS - roomOrigin;
        // ��posRSԭ�㣬�������forwardWS, ��. y �� 0.
        float t = -controllerWS.y / forwardWS.y;
        float x = controllerWS.x + forwardWS.x * t;
        float z = controllerWS.z + forwardWS.z * t;
        coordToPlace = new Vector2(x, z);
        Debug.Log(coordToPlace + ", " + controllerWS + ", " + forwardWS);
    }
}
