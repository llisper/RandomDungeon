using UnityEngine;
using System;

public class Cell : MonoBehaviour
{
    [NonSerialized]
    public int width;
    [NonSerialized]
    public int length;
    [NonSerialized]
    public Vector3 adjustDir;

    public int tpu { get { return DungeonMap.instance.tilesPerUnit; } }
    public int widthTiles { get { return tpu * width; } }
    public int lengthTiles { get { return tpu * length; } }

    public Bounds bounds
    {
        get
        {
            return new Bounds(transform.position, new Vector3(widthTiles, 1f, lengthTiles));
        }
    }

    public static Cell New(int width, int length, Transform parent, Vector3 position)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = string.Format("{0}x{1}", width, length);
        go.transform.parent = parent;
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.identity;
        Cell cell = go.AddComponent<Cell>();
        cell.SetSize(width, length);
        cell.adjustDir = InitAdjustDir(position);
        return cell;
    }

    static Vector3 InitAdjustDir(Vector3 pos)
    {
        return new Vector3(
            Mathf.Sign(pos.x != 0 ? pos.x : UnityEngine.Random.value - 0.5f),
            Mathf.Sign(pos.y != 0 ? pos.y : UnityEngine.Random.value - 0.5f),
            Mathf.Sign(pos.z != 0 ? pos.z : UnityEngine.Random.value - 0.5f));
    }

    void SetSize(int width, int length)
    {
        this.width = width;
        this.length = length;
        transform.localScale = new Vector3(width * tpu, 1f, length * tpu);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int i = 0, imax = tpu * length; i < imax; ++i)
        {
            for (int j = 0, jmax = tpu * width; j < jmax; ++j)
            {
                Vector3 localPosition = new Vector3(
                    (j - (jmax - 1f)* 0.5f) / jmax,
                    0f,
                    (i - (imax - 1f)* 0.5f) / imax);
                Gizmos.DrawWireCube(
                    transform.TransformPoint(localPosition),
                    Vector3.one);
            }
        }
    }
}