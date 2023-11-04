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
    public SteamVR_Action_Boolean grabpinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "grabpinch");  // 用来判断是哪只手按下
    SteamVR_Input_Sources hand = SteamVR_Input_Sources.Any;  // 左手或是右手

    Vector2 coordToPlace;   // 用来放方块的 room空间坐标.
    Vector3 roomOrigin;     // 其所属的房间的世界坐标.

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
        var pos = pose[hand].localPosition;  // 相对于universe origin
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
    ///  用来计算手柄所指向的用来放块的位置
    /// </summary>
    /// <param name="controllerWS"></param> 手柄的世界坐标
    /// <param name="forwardWS"></param>  手柄朝向的世界坐标
    void computeCoordToPlace(Vector3 controllerWS, Vector3 forwardWS)
    {
        // 计算相对于本房间的坐标, roomSpace
        Vector3 posRS = controllerWS - roomOrigin;
        // 从posRS原点，打出方向forwardWS, 求交. y 是 0.
        float t = -controllerWS.y / forwardWS.y;
        float x = controllerWS.x + forwardWS.x * t;
        float z = controllerWS.z + forwardWS.z * t;
        coordToPlace = new Vector2(x, z);
        Debug.Log(coordToPlace + ", " + controllerWS + ", " + forwardWS);
    }
}
