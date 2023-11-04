using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour
{
    public enum MoveDirection
    {
        RowAdd, RowMinus, ColAdd, ColMinus, CantMove,
    };

    public int gridLength = 11;  // 11*11 的grid
    public float x_min = -5.5f, x_max = 5.5f, z_min = -5.5f, z_max = 5.5f;
    public GameObject gridFloor, visualizingCube, generatedCube;
     
    ArrayList placedIds = new ArrayList();  // 上面的下标。行数*gridLength+列数
    int[,] grid;

    Transform roomTransform;   // 自己所处的room

    public int redCubeCnt = 0, greenCubeCnt = 0, blueCubeCnt = 0, whiteCubeCnt = 0;
    int redLandCnt=0, greenLandCnt=0, blueLandCnt=0, whiteLandCnt=0;
    // Start is called before the first frame update
    void Start()
    {
        //挂载在"/room/grid"上
        roomTransform = transform.parent;
        grid = new int[gridLength, gridLength];
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
        // 捡起一个这个颜色的块.
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
        int row = (int)(gridCoord.x - x_min);
        int col = (int)(gridCoord.z - z_min);
        if (grid[row, col] != 0)
            return false;  // 重叠了.
        // 放置方块
        // "/room/grid/..."
        GameObject instance = Instantiate(gridFloor, this.transform, false);
        instance.transform.localPosition = gridCoord;
        instance.GetComponent<MeshRenderer>().material.color = color;
        Debug.Log("place cube in " + gridCoord);
        // 统计
        placedIds.Add(new Vector2(gridCoord.x, gridCoord.z));
        // 颜色编号写入数组
        int colorId = GetColorId(ref color);
        grid[row, col] = colorId;
        // 分类统计地砖数量
        if (color == Color.red)
            redLandCnt += 1;
        else if (color == Color.green)
            greenLandCnt += 1;
        else if (color == Color.blue)
            blueLandCnt += 1;
        else
            whiteLandCnt += 1;

        // 判断周围是否有效.
        if(checkValidRowCol(row, col, colorId))
        {
            grid[row, col] = colorId;
            return true;
        }
        else
        {
            // 寻找下一个可能的位置 如果finded，就吸附过去；如果没有找到，就移动一小段之后消失.
            bool finded = findNextPlace(row, col, colorId, out MoveDirection move);
            instance.GetComponent<MoveLandCube>().MoveTo(move, finded);
            if (finded)
            {
                switch (move)
                {
                    case MoveDirection.ColAdd:
                        grid[row, col + 1] = colorId;
                        break;
                    case MoveDirection.ColMinus:
                        grid[row, col - 1] = colorId;
                        break;
                    case MoveDirection.RowAdd:
                        grid[row + 1, col] = colorId;
                        break;
                    case MoveDirection.RowMinus:
                        grid[row - 1, col] = colorId;
                        break;
                }
            }
            return finded;
        }
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

    int GetColorId(ref Color color)
    {
        return Global.IndexOfColor(color) + 1;
    }

    /// <summary>
    /// 判断这个位置是否合法(周围相邻位置是否有不同颜色的)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    bool checkValidRowCol(int row, int col, int colorId)
    {
        int whiteId = Global.IndexOfColor(Color.white);
        if (row != 0 && grid[row-1,col]!=0 && grid[row - 1, col] != colorId && grid[row - 1, col] != whiteId)
            return false;
        if (row != gridLength - 1 && grid[row+1,col]!=0 && grid[row + 1, col] != colorId && grid[row + 1, col] != whiteId)
            return false;
        if (col != 0 && grid[row,col-1]!=0 && grid[row, col - 1] != colorId && grid[row, col - 1] != whiteId)
            return false;
        if (col != gridLength - 1 && grid[row,col+1]!=0 && grid[row, col+1] != colorId && grid[row, col + 1] != whiteId)
            return false;
        return true;
    }

    /// <summary>
    /// 寻找下一个位置可能的位置. 如果没满但是没有合适的方向，就随机一个没满的然后消失.
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
}
