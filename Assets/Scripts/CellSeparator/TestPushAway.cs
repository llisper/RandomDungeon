using UnityEngine;
using System.Collections.Generic;

public class TestPushAway : MonoBehaviour
{
    public Vector2 size;
    public Vector3 pos;

    PushAway mAlgo = new PushAway();
    List<Cell> mCells = new List<Cell>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            mAlgo.SeparateCellsOnePass(mCells);
    }

    [ContextMenu("Add Cell")]
    void AddCell()
    {
        Cell cell = Cell.New((int)size.x, (int)size.y, transform, pos);
        cell.name = string.Format("#{0}:{1}", mCells.Count, cell.name);
        mCells.Add(cell);
    }
}
