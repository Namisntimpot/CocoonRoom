using UnityEngine;
using Valve.VR.InteractionSystem;


public class GenerateColoredCube : MonoBehaviour
{
    public Color cubeColor = Color.red; // 设置默认颜色为红色

    void Start()
    {
        GenerateCube(cubeColor);
    }

    void GenerateCube(Color color)
    {
        // 获取脚本所附加到的物体的位置
        Vector3 spawnPosition = transform.position;

        // 随机偏移
        float random = Random.Range(-0.4f, 0.4f);
        spawnPosition += new Vector3(random, 0.3f, random);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = spawnPosition; // 将生成的物块的位置设置为脚本所附加到的物体的位置
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
