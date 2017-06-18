using System.Collections.Generic;
using UnityEngine;

public class LockAndMove : ICellSeparator, IComparer<Cell>
{
    #region implement ICellSeparator
    public bool SeparateCells(List<Cell> cells, int maxPass)
    {
        bool overlapped = true;
        int pass = 0;
        for (; pass < maxPass && overlapped; ++pass)
            overlapped = SeparateCellsOnePass(cells);
        return overlapped;
    }

    public bool SeparateCellsOnePass(List<Cell> cells)
    {
        bool overlapped = false;
        for (int i = 0; i < cells.Count; ++i)
        {
            cells.Sort(i, cells.Count - i, this);
            Cell cell = cells[i];
            for (int j = i + 1; j < cells.Count; ++j)
            {
                Cell other = cells[j];
                overlapped |= Separate(cell, other);
            }
        }
        return overlapped;
    }
    #endregion implement ICellSeparator

    #region algorithm
    int IComparer<Cell>.Compare(Cell x, Cell y)
    {
        float r = x.transform.position.magnitude - y.transform.position.magnitude;
        if (r > 0)
            return 1;
        else if (r < 0)
            return -1;
        else
            return 0;
    }

    public static bool Separate(Cell b, Cell a)
    {
        Vector3 d = CalcOverlappedDisplacement(b, a);
        bool overlapped = Overlapped(d);
        if (overlapped)
            a.transform.position += SeperateByXZ(d);
        return overlapped;
    }

    public static Vector3 CalcOverlappedDisplacement(Cell b, Cell a)
    {
        Vector3 amin = a.bounds.min;
        Vector3 amax = a.bounds.max;
        Vector3 bmin = b.bounds.min;
        Vector3 bmax = b.bounds.max;

        return new Vector3(
            OverlappedDisplacement(bmax.x, bmin.x, amax.x, amin.x, a.adjustDir.x),
            OverlappedDisplacement(bmax.y, bmin.y, amax.y, amin.y, a.adjustDir.y),
            OverlappedDisplacement(bmax.z, bmin.z, amax.z, amin.z, a.adjustDir.z));
    }

    public static bool Overlapped(Vector3 d)
    {
        return Mathf.Abs(d.x) > 1E-6f &&
               Mathf.Abs(d.y) > 1E-6f &&
               Mathf.Abs(d.z) > 1E-6f;
    }

    static float OverlappedDisplacement(float bmax, float bmin, float amax, float amin, float dir)
    {
        float d1 = bmin - amax;
        float d2 = bmax - amin;
        if (d1 >= 0f || d2 <= 0f)
            return 0f;

        return Mathf.Sign(d1) == dir ? d1 : d2;
    }

    static Vector3 SeperateByXZ(Vector3 displacement)
    {
        float x = Step(displacement.x);
        float z = Step(displacement.z);
        return UnityEngine.Random.value < 0.5f 
            ? new Vector3(x, 0f, 0f) 
            : new Vector3(0f, 0f, z);
    }

    static float Step(float v)
    {
        return v;
        // return Mathf.Min(Mathf.Abs(v), 1f) * Mathf.Sign(v);
    }
    #endregion algorithm
}
