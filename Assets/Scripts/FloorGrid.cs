using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour
{
    public enum MoveDirection
    {
        RowAdd, RowMinus, ColAdd, ColMinus, CantMove,
    };

    public int gridLength = 11;  // 11*11 ��grid
    public float x_min = -5.5f, x_max = 5.5f, z_min = -5.5f, z_max = 5.5f;
    public GameObject gridFloor, visualizingCube, generatedCube, pointlight;
    float range = 5;
    public Material[] floorMaterialsByColorId;
     
    ArrayList placedIds = new ArrayList();  // ������±ꡣ����*gridLength+����
    int[,] grid;

    Transform roomTransform;   // 自己所处的room

    public int redCubeCnt = 0, greenCubeCnt = 0, blueCubeCnt = 0, whiteCubeCnt = 0;
    float redLandCnt = 0.2f, greenLandCnt = 0.2f, blueLandCnt = 0.2f, whiteLandCnt=4;
    // Start is called before the first frame update
    void Start()
    {
        //挂载在"/room/grid"上
        roomTransform = transform.parent;
        grid = new int[gridLength, gridLength];
        grid[10, 4] = grid[10, 5] = grid[10, 6] = grid[9, 5] = 4;
        placedIds.Add(new Vector2(-1, 5));
        placedIds.Add(new Vector2(0, 5));
        placedIds.Add(new Vector2(1, 5));
        placedIds.Add(new Vector2(0, 4));
        InvokeRepeating("GenerateCubes", 5, 5);
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

    public void pickUpCube(Color color)
    {
        // ����һ�������ɫ�Ŀ�.
        if (color == Color.red)
            redCubeCnt--;
        else if (color == Color.blue)
            blueCubeCnt--;
        else if (color == Color.green)
            greenCubeCnt--;
        else
            whiteCubeCnt--;
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
        int row = (int)(gridCoord.z - z_min);
        int col = (int)(gridCoord.x - x_min);

        if (grid[row, col] != 0)
            return false;  // �ص���.
        int colorId = GetColorId(color);
        if (!checkValidRowCol(row, col, colorId))  // 不能放在这.
            return false;
        // ���÷���
        // "/room/grid/..."
        GameObject instance = Instantiate(gridFloor, this.transform, false);
        instance.transform.localPosition = gridCoord;
        instance.GetComponent<MeshRenderer>().material = floorMaterialsByColorId[colorId];
        Debug.Log("place cube in " + gridCoord);
        // ͳ��
        placedIds.Add(new Vector2(gridCoord.x, gridCoord.z));
        // ��ɫ���д������
        
        grid[row, col] = colorId;
        // ����ͳ�Ƶ�ש����
        if (color == Color.red)
            redLandCnt += 1;
        else if (color == Color.green)
            greenLandCnt += 1;
        else if (color == Color.blue)
            blueLandCnt += 1;
        else
            whiteLandCnt += 1;

        // 如果新块的位置比现有的range更远，就改成距离+0.5
        float newdis = Vector3.Distance(instance.transform.position, pointlight.transform.position);
        if(newdis > range)
        {
            range = newdis + 0.7f;
            pointlight.GetComponent<Light>().range = range;
        }
        return true;

        // 不再试图移动
        //if(checkValidRowCol(row, col, colorId))
        //{
        //    grid[row, col] = colorId;
        //    return true;
        //}
        //else
        //{
        //    // Ѱ����һ�����ܵ�λ�� ���finded����������ȥ�����û���ҵ������ƶ�һС��֮����ʧ.
        //    bool finded = findNextPlace(row, col, colorId, out MoveDirection move);
        //    instance.GetComponent<MoveLandCube>().MoveTo(move, finded);
        //    if (finded)
        //    {
        //        switch (move)
        //        {
        //            case MoveDirection.ColAdd:
        //                grid[row, col + 1] = colorId;
        //                break;
        //            case MoveDirection.ColMinus:
        //                grid[row, col - 1] = colorId;
        //                break;
        //            case MoveDirection.RowAdd:
        //                grid[row + 1, col] = colorId;
        //                break;
        //            case MoveDirection.RowMinus:
        //                grid[row - 1, col] = colorId;
        //                break;
        //        }
        //    }
        //    return finded;
        //}
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

    public void GenerateCubes(Vector3 position, Color color)
    {
        Transform parent = transform.parent.Find("generatedBlocks");
        GameObject instance = Instantiate(generatedCube, parent, false);
        instance.transform.localPosition = position;
        instance.GetComponent<GeneratedBlocks>().SetColor(color);
    }

    public void GenerateCubes(Vector3[] positions, Color[] colors)
    {
        for(int i=0; i<positions.Length; ++i)
        {
            GenerateCubes(positions[i], colors[i]);
        }
    }

    int GetColorId(Color color)
    {
        return Global.IndexOfColor(color) + 1;
    }

    /// <summary>
    /// �ж����λ���Ƿ�Ϸ�(��Χ����λ���Ƿ��в�ͬ��ɫ��)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    bool checkValidRowCol(int row, int col, int colorId)
    {
        int whiteId = GetColorId(Color.white);
        if (colorId == whiteId)
            return true;
        foreach(int i in new int[] {-1, 1 })
        {
            if (row + i >= 0 && row + i < gridLength && grid[row + i, col] == whiteId)
                return true;
            
        }
        foreach (int j in new int[] { -1, 1 })
        {
            if (col+j >= 0 && col+j < gridLength && grid[row, col + j] == whiteId)
                return true;
        }

        for (int i = -1; i <= 1; ++i)
        {
            if (row + i < 0 || row + i >= gridLength)
                continue;
            for(int j=-1; j<=1; ++j)
            {
                if (col + j < 0 || col + j >= gridLength)
                    continue;
                if (grid[row + i, col + j] != 0 && grid[row + i, col + j] != colorId && grid[row+i, col+j] != whiteId)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Ѱ����һ��λ�ÿ��ܵ�λ��. ���û������û�к��ʵķ��򣬾����һ��û����Ȼ����ʧ.
    /// </summary>
    bool findNextPlace(int row, int col, int colorId, out MoveDirection move)
    {
        move = MoveDirection.CantMove;
        if(col!=0 && grid[row, col - 1] == 0)
        {
            move = MoveDirection.ColMinus;
            if (checkValidRowCol(row, col - 1, colorId))
                return true;
        }
        if(col!=gridLength-1 && grid[row, col + 1] == 0)
        {
            move = MoveDirection.ColAdd;
            if (checkValidRowCol(row, col + 1, colorId))
                return true;
        }
        if(row!=0 && grid[row-1, col] == 0)
        {
            move = MoveDirection.RowMinus;
            if (checkValidRowCol(row - 1, col, colorId))
                return true;
        }
        if(row!=gridLength-1 && grid[row+1, col] == 0)
        {
            move = MoveDirection.RowAdd;
            if (checkValidRowCol(row + 1, col, colorId))
                return true;
        }
        return false;
    }


    public float generateRate = 1;    //整体生成效率
    public float gameDifficulty = 0.15f;  //白色生成概率最大值
    public float eps = 1e-6f;
    public void GenerateCubes()
    {
        float existColorLandSum = redLandCnt + blueLandCnt + greenLandCnt;
        float whitePossibility = (1 - (Mathf.Pow(existColorLandSum / 3 - redLandCnt, 2) + Mathf.Pow(existColorLandSum / 3 - greenLandCnt, 2) + Mathf.Pow(existColorLandSum / 3 - blueLandCnt, 2)) / (Mathf.Pow(existColorLandSum * (2f/3f), 2))) * gameDifficulty;
        if(whitePossibility < 0.05f) whitePossibility = 0.05f;
        if (existColorLandSum == 0.6f) whitePossibility = 0;
        float redPossibility = (redLandCnt / (existColorLandSum)) * (1 - whitePossibility);
        float greenPossibility = (greenLandCnt / (existColorLandSum)) * (1 - whitePossibility);
        float bluePossibility = (blueLandCnt / (existColorLandSum)) * (1 - whitePossibility);
        Debug.Log(redPossibility + ", " + greenPossibility + ", " + bluePossibility + ", " + whitePossibility);
        
        int targetSum = Mathf.RoundToInt((3 + (existColorLandSum + whiteLandCnt) / 3) * generateRate);
        int needAddCount = targetSum - (redCubeCnt + greenCubeCnt + blueCubeCnt + whiteCubeCnt);
        Vector3[] positions = new Vector3[needAddCount];
        Color[] colors = new Color[needAddCount];
        Debug.Log("generate " + needAddCount);
        
            for (int i = 0; i < needAddCount; i++) {
            float seedColor = Random.Range(0f, 1f);
            if(seedColor < whitePossibility) {
                colors[i] = Color.white;
                whiteCubeCnt++;
            }
            else if(seedColor < whitePossibility + redPossibility) {
                colors[i] = Color.red;
                redCubeCnt++;
            }
            else if(seedColor < whitePossibility + redPossibility + greenPossibility) {
                colors[i] = Color.green;
                greenCubeCnt++;
            }
            else {
                colors[i] = Color.blue;
                blueCubeCnt++;
            }

            int seedPosition = Mathf.RoundToInt(Random.Range(0f, placedIds.Count - 1f));
            positions[i] = new Vector3(((Vector2)placedIds[seedPosition]).x + Random.Range(-1f, 0f), Random.Range(0.7f, 1f), ((Vector2)placedIds[seedPosition]).y + Random.Range(0f, 1f));
        }
        if(needAddCount > 0)
            GenerateCubes(positions, colors);
    }
}
