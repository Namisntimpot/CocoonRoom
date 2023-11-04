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
    public SteamVR_Action_Boolean grabpinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "grabpinch");  // �����ж�����ֻ�ְ���
    SteamVR_Input_Sources hand = SteamVR_Input_Sources.Any;  // ���ֻ�������

    Vector2 coordToPlace;   // �����ŷ���� room�ռ�����.
    Transform roomTransform;     // �������ķ������������. ע�������..
    Transform[] controllerTransforms;

    FloorGrid floorGrid; 

    void Start()
    {
        // "/room/generatedblocks/generatedblock"
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
        //var pos = pose[hand].localPosition;  // �����universe origin
        //var rot = pose[hand].localRotation;
        int index = hand == SteamVR_Input_Sources.LeftHand ? 0 : 1;
        var pos = controllerTransforms[index].position;  // ��������
        var forward = controllerTransforms[index].forward;   // ǰ��
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
        floorGrid.pickUpCube(color);
        Debug.Log("Picked with hand " + hand);
    }

    public void onDetach()
    {
        Debug.Log("Detached");
        isPicked = false;
        isPlaced = true;
        if(floorGrid.placeCube(coordToPlace, color))  // �ɹ�����һ����
        {
            DestroyGameObject();
        }
        else
        {
            Invoke("DestroyGameObject", 1);
        }
    }

    /// <summary>
    ///  ���������ֱ���ָ��������ſ��λ��
    /// </summary>
    /// <param name="controllerWS"></param> �ֱ�����������
    /// <param name="forwardWS"></param>  �ֱ��������������
    void computeCoordToPlace(Vector3 controllerWS, Vector3 forwardWS)
    {
        // ��������ڱ����������, roomSpace
        Vector3 posRS = controllerWS - roomTransform.position;
        // ��posRSԭ�㣬�������forwardWS, ��. y �� 0.
        float t = -posRS.y / forwardWS.y;
        float x = posRS.x + forwardWS.x * t;
        float z = posRS.z + forwardWS.z * t;
        coordToPlace = new Vector2(x, z);
    }

    public void SetColor(Color color)
    {
        this.color = color;
        transform.GetComponentInChildren<MeshRenderer>().material.color = color;
    }

    void DestroyGameObject()
    {
        Destroy(this.gameObject);
    }
}
