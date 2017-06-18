using UnityEngine;
using System;
using System.Collections.Generic;

public class PushAway : ICellSeparator
{
    #region impelment ICellSeparator
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
        mhasMoved.Clear();
        bool overlapped = false;
        int tpu = DungeonMap.instance.tilesPerUnit;
        float tpu2 = tpu * 0.5f;
        int total = 0;
        int favorX = 0;
        int favorZ = 0;
        for (int i = 0; i < cells.Count; ++i)
        {
            for (int j = i + 1; j < cells.Count; ++j)
            {
                Cell refe = cells[i];
                Cell cell = cells[j];
                Vector3 overlappedDisplacement = CalcOverlappedDisplacement(cell, refe);
                if (Overlapped(overlappedDisplacement))
                {
                    overlapped = true;
                    Vector3 offset = SeperateByXZ(overlappedDisplacement, xzDist);
                    float mag = offset.magnitude;
                    offset.Normalize();

                    ++total;
                    Vector3 cellPos = cell.transform.position;
                    Vector3 refePos = refe.transform.position;
                    if (offset.x != 0f)
                    {
                        float val = SelectMagnitude(offset.x, cellPos.x, mag);
                        cellPos.x += val;
                        val = -(mag - val);
                        refePos.x += val;
                        ++favorX;
                    }
                    else
                    {
                        float val = SelectMagnitude(offset.z, cellPos.z, mag);
                        cellPos.z += val;
                        val = -(mag - val);
                        refePos.z += val;
                        ++favorZ;
                    }
                    cell.transform.position = cellPos;
                    mhasMoved.Add(cell);
                    refe.transform.position = refePos;
                    mhasMoved.Add(refe);
                }
            }
        }

        // Debug.LogFormat("favor x: {0:p}", (float)favorX / total);
        // Debug.LogFormat("favor z: {0:p}", (float)favorZ / total);
        return overlapped;
    }
    #endregion impelment ICellSeparator

    public float xzDist = 0.95f;
    HashSet<Cell> mhasMoved = new HashSet<Cell>();

    #region algorithm
    /// <summary>
    /// calculate displacement for cell to move away from reference
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    public static Vector3 CalcOverlappedDisplacement(Cell cell, Cell reference)
    {
        Vector3 amin = cell.bounds.min;
        Vector3 amax = cell.bounds.max;
        Vector3 bmin = reference.bounds.min;
        Vector3 bmax = reference.bounds.max;

        return new Vector3(
            OverlappedDisplacement(bmax.x, bmin.x, amax.x, amin.x),
            OverlappedDisplacement(bmax.y, bmin.y, amax.y, amin.y),
            OverlappedDisplacement(bmax.z, bmin.z, amax.z, amin.z));
    }

    public static bool Overlapped(Vector3 d)
    {
        return Mathf.Abs(d.x) > 1E-6f &&
               Mathf.Abs(d.y) > 1E-6f &&
               Mathf.Abs(d.z) > 1E-6f;
    }

    static float OverlappedDisplacement(float bmax, float bmin, float amax, float amin)
    {
        float d1 = bmin - amax;
        float d2 = bmax - amin;
        if (d1 >= 0f || d2 <= 0f)
            return 0f;

        return Mathf.Abs(d1) < Mathf.Abs(d2) ? d1 : d2;
    }

    static Vector3 SeperateByXZ(Vector3 displacement, float xzDist)
    {
        float x = displacement.x;
        float z = displacement.z;
        return xzDist * Mathf.Abs(x) <= Mathf.Abs(z)
            ? new Vector3(x, 0f, 0f) 
            : new Vector3(0f, 0f, z);
    }

    static float SelectMagnitude(float dir, float val, float mag)
    {
        float tpu2 = DungeonMap.instance.tilesPerUnit / 2f;
        float mag1 = Mathf.CeilToInt(mag * 0.5f / tpu2) * tpu2;
        float mag2 = mag - mag1;
        if (Mathf.Sign(dir) == Mathf.Sign(val))
            return Mathf.Max(mag1, mag2);
        else
            return Mathf.Min(mag1, mag2);
    }
    #endregion algorithm
}
