using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class CubeMovement : MonoBehaviour
{
    private Hand currentHand;
    private Vector3 initialPosition;
    float rotationSpeed = 0.1f;
    bool onhover = false;


    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnHoverBegin(Hand hand)
    {
        Debug.Log("手控制器开始悬停在物体上");

        // 在这里可以添加你希望在悬停开始时执行的操作
    }

    void OnDestroy()
    {
    }

    private void Update()
    {
        //this.GetComponent<InteractableHoverEvents>().onHandHoverBegin.Invoke();
        //if (onhover)
        //{
        //    // 如果没有手控制器持有物体，执行上下跳动逻辑
        //    float jumpHeight = 0.2f;
        //    float jumpSpeed = 1.0f;
        //    transform.position = initialPosition + Vector3.up * Mathf.PingPong(Time.time * jumpSpeed, jumpHeight);
        //}
        //else
        //{
        //    initialPosition = transform.position;
        //}
    }
}