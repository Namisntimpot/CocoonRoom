using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                dir.z = 0.5f;
                break;
            case FloorGrid.MoveDirection.ColMinus:
                dir.z = -0.5f;
                break;
            case FloorGrid.MoveDirection.RowAdd:
                dir.x = 0.5f;
                break;
            case FloorGrid.MoveDirection.RowMinus:
                dir.x = -0.5f;
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
        // 直接四舍五入即可.
        Vector3 pos = transform.localPosition;
        pos.x = (int)(pos.x + 0.5f);
        pos.z = (int)(pos.z + 0.5f);
        transform.localPosition = pos;
    }
}
