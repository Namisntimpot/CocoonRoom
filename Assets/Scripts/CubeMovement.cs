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
        Debug.Log("�ֿ�������ʼ��ͣ��������");

        // ��������������ϣ������ͣ��ʼʱִ�еĲ���
    }

    void OnDestroy()
    {
    }

    private void Update()
    {
        //this.GetComponent<InteractableHoverEvents>().onHandHoverBegin.Invoke();
        //if (onhover)
        //{
        //    // ���û���ֿ������������壬ִ�����������߼�
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