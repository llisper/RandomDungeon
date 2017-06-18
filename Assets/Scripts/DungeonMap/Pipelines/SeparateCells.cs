using UnityEngine;
using System;
using System.Collections;

public class SeparateCells : IGenPipeline
{
    #region inspector
    [Serializable]
    public class Config
    {
        [Range(1, 1000)]
        public int maxPasses = 500;
        public bool runInUpdate = true;
        public bool doShuffle;
    }
    #endregion inspector
    public ICellSeparator cellSeparator = new PushAway();

    public IEnumerator Execute(DungeonMap map)
    {
        var cells = map.cells;
        Config config = map.separateCells;
        int pass = 0;
        bool overlapped = true;
        for (; overlapped && pass < config.maxPasses; ++pass)
        {
            if (config.doShuffle)
            {
                for (int i = 0; i < cells.Count; ++i)
                {
                    int a = UnityEngine.Random.Range(0, cells.Count);
                    int b = UnityEngine.Random.Range(0, cells.Count);
                    Cell tmp = cells[a];
                    cells[a] = cells[b];
                    cells[b] = tmp;
                }
            }
            overlapped = cellSeparator.SeparateCellsOnePass(cells);
            if (config.runInUpdate)
                yield return null;
        }
        Debug.LogFormat("Finish! Run {0} pass(es)! Overlapped {1}!", pass, overlapped);
    }

    void IGenPipeline.OnDrawGizmosSelected(DungeonMap map) { }
}
