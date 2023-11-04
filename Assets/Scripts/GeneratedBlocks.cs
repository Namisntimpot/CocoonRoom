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

    //public SteamVR_Action_Pose pose = SteamVR_Input.GetAction<SteamVR_Action_Pose>("default", "pose");
    public SteamVR_Action_Boolean grabpinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "grabpinch");  // 用来判断是哪只手按下
    SteamVR_Input_Sources hand = SteamVR_Input_Sources.Any;  // 左手或是右手

    Vector2 coordToPlace;   // 用来放方块的 room空间坐标.
    Transform roomTransform;     // 其所属的房间的世界坐标. 注意它会变..
    Transform[] controllerTransforms;

    FloorGrid floorGrid; 

    static public void GenerateSmallCubes(Transform[] positions, Color[] colors)
    {

    }

    static public void GenerateASmallCube(Transform position, Color color)
    {

    }

    void Start()
    {
        // "/room/smallblocks/smallblock"
        roomTransform = this.transform.parent.parent;
        controllerTransforms = roomTransform.GetComponent<Global>().Controllers;
        floorGrid = this.transform.parent.parent.GetComponentInChildren<FloorGrid>();
    }

    private void Awake()
    {
        Throwable throwable = GetComponent<Throwable>();
        throwable.onPickUp.AddListener(onPickup);
        throwable.onDetachFromHand.AddListener(onDetach);
    }
    private void OnDestroy()
    {
        Throwable throwable = GetComponent<Throwable>();
        throwable.onPickUp.RemoveListener(onPickup);
        throwable.onDetachFromHand.RemoveListener(onDetach);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPicked)
            return;
        //var pos = pose[hand].localPosition;  // 相对于universe origin
        //var rot = pose[hand].localRotation;
        int index = hand == SteamVR_Input_Sources.LeftHand ? 0 : 1;
        var pos = controllerTransforms[index].position;  // 世界坐标
        var forward = controllerTransforms[index].forward;   // 前向
        //var forward = (rot * Vector3.forward).normalized;
        computeCoordToPlace(pos, forward);
        floorGrid.VisualizeCoord(coordToPlace);
    }

    public void onPickup()
    {
        isPicked = true;
        this.GetComponent<Rigidbody>().useGravity = true;
        if (grabpinch[SteamVR_Input_Sources.LeftHand].state)
            hand = SteamVR_Input_Sources.LeftHand;
        else
            hand = SteamVR_Input_Sources.RightHand;
        Debug.Log("Picked with hand " + hand);
    }

    public void onDetach()
    {
        Debug.Log("Detached");
        isPicked = false;
        isPlaced = true;
        if(floorGrid.placeCube(coordToPlace, color))  // 成功放了一个块
        {
            DestroyGameObject();
        }
        else
        {
            Invoke("DestroyGameObject", 1);
        }
    }

    /// <summary>
    ///  用来计算手柄所指向的用来放块的位置
    /// </summary>
    /// <param name="controllerWS"></param> 手柄的世界坐标
    /// <param name="forwardWS"></param>  手柄朝向的世界坐标
    void computeCoordToPlace(Vector3 controllerWS, Vector3 forwardWS)
    {
        // 计算相对于本房间的坐标, roomSpace
        Vector3 posRS = controllerWS - roomTransform.position;
        // 从posRS原点，打出方向forwardWS, 求交. y 是 0.
        float t = -posRS.y / forwardWS.y;
        float x = posRS.x + forwardWS.x * t;
        float z = posRS.z + forwardWS.z * t;
        coordToPlace = new Vector2(x, z);
        Debug.Log(coordToPlace + ", " + controllerWS + ", " + forwardWS);
    }

    void DestroyGameObject()
    {
        Destroy(this.gameObject);
    }
}
