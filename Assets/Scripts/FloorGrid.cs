using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour
{
    public int gridLength = 11;  // 11*11 ��grid
    public float x_min = -5.5f, x_max = 5.5f, z_min = -10.5f, z_max = 0.5f;
    public GameObject gridFloor;
     
    ArrayList placedIds = new ArrayList();  // ������±ꡣ����*gridLength+����

    Transform roomTransform;   // �Լ�������room

    int reds=0, greens=0, blues=0, whites=0;
    // Start is called before the first frame update
    void Start()
    {
        //"/room/grid"
        roomTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ������λ������Ӧ��grid�Ϸ���һ����
    /// </summary>
    /// <param name="posRS">ָ��ذ�ĵ�������Լ��������������</param>
    public void placeCube(Vector2 posRS, Color color)
    {
        Debug.Log("Try to place a brick");
        Vector3 gridCoord = new Vector2();
        if (!transformToGridCoord(posRS, ref gridCoord))
        {
            return;   // ʲôҲ����
        }
        // ���÷���
        // "/room/grid/..."
        GameObject instance = Instantiate(gridFloor, this.transform, false);
        instance.transform.localPosition = gridCoord;
        instance.GetComponent<MeshRenderer>().material.color = color;
        Debug.Log("place cube in " + gridCoord);
        // ͳ��
        int row = (int)gridCoord.x + gridLength / 2;
        int col = (int)gridCoord.z;
        placedIds.Add(row * gridLength + col);
        if (color == Color.red)
            reds += 1;
        else if (color == Color.green)
            greens += 1;
        else if (color == Color.blue)
            blues += 1;
        else
            whites += 1;
    }

    bool transformToGridCoord(Vector2 posRS, ref Vector3 gridCoord)
    {
        int x = (int)(posRS.x + 0.5f), z = (int)(posRS.y + 0.5f);  // ��������
        if (x < x_min || x > x_max || z < z_min || z > z_max)
            return false;  // ����
        gridCoord.x = x;
        gridCoord.y = 0f;
        gridCoord.z = z;
        return true;
    }
}
