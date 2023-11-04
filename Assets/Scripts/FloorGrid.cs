using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour
{
    public int gridLength = 11;  // 11*11 的grid
    public float x_min = -5.5f, x_max = 5.5f, z_min = -5.5f, z_max = 5.5f;
    public GameObject gridFloor, visualizingCube;
     
    ArrayList placedIds = new ArrayList();  // 上面的下标。行数*gridLength+列数

    Transform roomTransform;   // 自己所处的room

    public int redCubeCnt = 0, greenCubeCnt = 0, blueCubeCnt = 0, whiteCubeCnt = 0;
    int redLandCnt=0, greenLandCnt=0, blueLandCnt=0, whiteLandCnt=0;
    // Start is called before the first frame update
    void Start()
    {
        //挂载在"/room/grid"上
        roomTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VisualizeCoord(Vector2 posRS)
    {
        Vector3 gridCoord = new Vector3();
        if(!transformToGridCoord(posRS, ref gridCoord))
        {
            visualizingCube.SetActive(false);
            return;
        }
        visualizingCube.SetActive(true);
        visualizingCube.transform.localPosition = gridCoord;
    }

    /// <summary>
    /// 在输入位置所对应的grid上放置一个块, 返回false如果超出范围.
    /// </summary>
    /// <param name="posRS">指向地板的点相对于自己所处房间的坐标</param>
    public bool placeCube(Vector2 posRS, Color color)
    {
        Debug.Log("Try to place a brick");
        visualizingCube.SetActive(false);   // 关掉位置指示.
        Vector3 gridCoord = new Vector3();
        if (!transformToGridCoord(posRS, ref gridCoord))
        {
            return false;   // 什么也不做
        }
        // 放置方块
        // "/room/grid/..."
        GameObject instance = Instantiate(gridFloor, this.transform, false);
        instance.transform.localPosition = gridCoord;
        instance.GetComponent<MeshRenderer>().material.color = color;
        Debug.Log("place cube in " + gridCoord);
        // 统计
        int row = (int)gridCoord.x + gridLength / 2;
        int col = (int)gridCoord.z;
        placedIds.Add(new Vector2(row, col));
        if (color == Color.red)
            redLandCnt += 1;
        else if (color == Color.green)
            greenLandCnt += 1;
        else if (color == Color.blue)
            blueLandCnt += 1;
        else
            whiteLandCnt += 1;
        
        return true;
    }

    bool transformToGridCoord(Vector2 posRS, ref Vector3 gridCoord)
    {
        int x = (int)(posRS.x + 0.5f), z = (int)(posRS.y + 0.5f);  // 四舍五入
        if (x < x_min || x > x_max || z < z_min || z > z_max)
            return false;  // 超界
        gridCoord.x = x;
        gridCoord.y = 0f;
        gridCoord.z = z;
        return true;
    }



    public int generateRate = 1;    //整体生成效率
    public float gameDifficulty = 0.25f;   //白色生成概率最大值
    public void GenerateCubes(Vector3[] positions, Color[] colors){

        int existColorLandSum = redLandCnt + blueLandCnt + greenLandCnt;
        float whitePossibility = (1 - (Mathf.Pow(existColorLandSum - redLandCnt, 2) + Mathf.Pow(existColorLandSum - greenLandCnt, 2) + Mathf.Pow(existColorLandSum - blueLandCnt, 2)) / Mathf.Pow(existColorLandSum * (2/3), 2)) * gameDifficulty;
        if(whitePossibility < 0.05f) whitePossibility = 0.05f;
        float redPossibility = (redLandCnt / existColorLandSum) * (1 - whitePossibility);
        float greenPossibility = (greenLandCnt / existColorLandSum) * (1 - whitePossibility);
        float bluePossibility = (blueLandCnt / existColorLandSum) * (1 - whitePossibility);

        int targetSum = (3 + (existColorLandSum + whiteLandCnt) / 3) * generateRate;
        int needAddCount = targetSum - (redCubeCnt + greenCubeCnt + blueCubeCnt + whiteCubeCnt);
        for(int i = 0; i < needAddCount; i++) {
            float seedColor = Random.Range(0f, 1f);
            if(seedColor < whitePossibility) {
                colors[i] = Color.white;
            }
            else if(seedColor < whitePossibility + redPossibility) {
                colors[i] = Color.red;
            }
            else if(seedColor < whitePossibility + redPossibility + greenPossibility) {
                colors[i] = Color.green;
            }
            else {
                colors[i] = Color.blue;
            }

            int seedPosition = Mathf.RoundToInt(Random.Range(0f, placedIds.Count - 1f));
            positions[i] = new Vector3(((Vector2)placedIds[seedPosition]).x + Random.Range(-0.5f, 0.5f), Random.Range(0.3f, 0.4f), ((Vector2)placedIds[seedPosition]).y + Random.Range(-0.5f, 0.5f));
        }

    }
}
