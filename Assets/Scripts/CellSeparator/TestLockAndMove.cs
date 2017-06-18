using UnityEngine;
using System;

public class TestLockAndMove : MonoBehaviour
{
    [NonSerialized]
    public Cell a;
    [NonSerialized]
    public Cell b;

    public Vector2 sizea;
    public Vector3 posa;
    public Vector2 sizeb;
    public Vector3 posb;
    public bool overlapped;
    public Vector3 overlappedDisplacement;

    void Awake()
    {
        a = Cell.New((int)sizea.x, (int)sizea.y, transform, posa);
        a.name = "a:" + a.name;
        b = Cell.New((int)sizeb.x, (int)sizeb.y, transform, posb);
        b.name = "b:" + b.name;
    }

    void OnDestroy()
    {
        if (null != a)
            GameObject.Destroy(a.gameObject);
        if (null != b)
            GameObject.Destroy(b.gameObject);
    }

    void Update()
    {
        Vector3 d = LockAndMove.CalcOverlappedDisplacement(b, a);
        overlapped = LockAndMove.Overlapped(d);
        overlappedDisplacement = d;
    }

    #if UNITY_EDITOR
    [ContextMenu("Seperate")]
    void Seperate()
    {
        LockAndMove.Separate(b, a);
    }
    #endif // UNITY_EDITOR
}