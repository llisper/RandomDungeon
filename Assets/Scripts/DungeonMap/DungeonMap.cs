using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DungeonMap : MonoBehaviour
{
    #region inspector
    public int tilesPerUnit = 1;
    public int seed = -1;
    public GenerateCells.Config generateCells;
    public SeparateCells.Config separateCells;
    public MainCells.Config mainCells;
    #endregion inspector

    static DungeonMap sInstance;
    public static DungeonMap instance
    {
        get
        {
            if (!Application.isPlaying)
            {
                if (null == sInstance)
                {
                    sInstance = GameObject.FindObjectOfType<DungeonMap>();
                    sInstance.Awake();
                }
            }
            return sInstance;
        }
    }

    [NonSerialized]
    public List<Cell> cells = new List<Cell>();
    [NonSerialized]
    public List<IGenPipeline> pipelines = new List<IGenPipeline>();
    [NonSerialized]
    public bool generating;

    public void ExecutePipelines()
    {
        UnityEngine.Random.InitState(seed < 0 ? (int)Time.time : seed);
        StartCoroutine(ExecutingPipeline());
    }

    IEnumerator ExecutingPipeline()
    {
        generating = true;
        for (int i = 0; i < pipelines.Count; ++i)
            yield return StartCoroutine(pipelines[i].Execute(this));
        generating = false;
    }

    void Awake()
    {
        sInstance = this;
        pipelines.Add(new GenerateCells());
        pipelines.Add(new SeparateCells());
        pipelines.Add(new MainCells());
    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < pipelines.Count; ++i)
            pipelines[i].OnDrawGizmosSelected(this);
    }
}
