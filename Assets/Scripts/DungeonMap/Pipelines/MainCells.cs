using UnityEngine;
using System;
using System.Collections;

public class MainCells : IGenPipeline
{
    #region inspector
    [Serializable]
    public class Config
    {
        public float areaThreshold = 0.75f;
    }
    #endregion inspector

    public IEnumerator Execute(DungeonMap map)
    {
        Clear(map);
        Config config = map.mainCells;
        int max = map.generateCells.maxArea;
        int min = map.generateCells.minArea;
        int threshold = (int)(min * (1f - config.areaThreshold) + max * config.areaThreshold);
        foreach (Cell c in map.cells)
        {
            if (c.width * c.length >= threshold)
                c.GetComponent<Renderer>().material.color = Color.red;
        }
        yield break;
    }

    public void Clear(DungeonMap map)
    {
        foreach (Cell c in map.cells)
            c.GetComponent<Renderer>().material.color = Color.white;
    }

    void IGenPipeline.OnDrawGizmosSelected(DungeonMap map) { }
}
