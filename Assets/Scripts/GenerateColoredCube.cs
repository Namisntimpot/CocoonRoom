using UnityEngine;
using Valve.VR.InteractionSystem;


public class GenerateColoredCube : MonoBehaviour
{
    public Color cubeColor = Color.red; // ����Ĭ����ɫΪ��ɫ

    void Start()
    {
        GenerateCube(cubeColor);
    }

    void GenerateCube(Color color)
    {
        // ��ȡ�ű������ӵ��������λ��
        Vector3 spawnPosition = transform.position;

        // ���ƫ��
        float random = Random.Range(-0.4f, 0.4f);
        spawnPosition += new Vector3(random, 0.3f, random);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = spawnPosition; // �����ɵ�����λ������Ϊ�ű������ӵ��������λ��
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cube.GetComponent<Renderer>().material.color = color;
        cube.AddComponent<Rigidbody>().useGravity = false;
        cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        cube.AddComponent<CubeMovement>();
        cube.AddComponent<Interactable>();
        cube.AddComponent<Throwable>();
        cube.AddComponent<InteractableHoverEvents>();
    }
}
