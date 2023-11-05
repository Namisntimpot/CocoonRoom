using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ԭ����Ŀ���ǣ�����ŵ���һ�����Ϸ���λ�ã����Զ���һ���Ա߿�����ȷ��λ�á�����debug������.
/// </summary>
public class MoveLandCube : MonoBehaviour
{
    public void MoveTo(FloorGrid.MoveDirection move, bool dontDisappear)
    {
        if (move == FloorGrid.MoveDirection.CantMove)
            Invoke("DestroyGameObject", 2);
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 dir = Vector3.zero;
        switch (move)
        {
            case FloorGrid.MoveDirection.ColAdd:
                dir.x = 0.5f;
                break;
            case FloorGrid.MoveDirection.ColMinus:
                dir.x = -0.5f;
                break;
            case FloorGrid.MoveDirection.RowAdd:
                dir.z = 0.5f;
                break;
            case FloorGrid.MoveDirection.RowMinus:
                dir.z = -0.5f;
                break;
        }
        rigidbody.AddForce(dir, ForceMode.VelocityChange);
        if (!dontDisappear)
            Invoke("DestroyGameObject", 2);
        else
            Invoke("AttachToGrid", 2);
    }

    void AttachToGrid()
    {
        // ֱ���������뼴��.
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Vector3 pos = transform.localPosition;
        pos.x = (int)(pos.x + 0.5f);
        pos.z = (int)(pos.z + 0.5f);
        transform.localPosition = pos;
    }
}
