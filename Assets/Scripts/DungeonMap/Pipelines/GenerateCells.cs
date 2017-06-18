using UnityEngine;
using System;
using System.Collections;

public class GenerateCells : IGenPipeline
{
    #region inspector
    [Serializable]
    public class Config
    {
        public float radius = 10f;
        public int numCells = 10;
        public int minArea = 9;
        public int maxArea = 100;
        public bool useGaussianDistribution = true;
        public float mean = 0f;
        public float deviation = 1f;
    }
    #endregion inspector

    public IEnumerator Execute(DungeonMap map)
    {
        Config config = map.generateCells;
        Clear(map);
        int[] areaCount = new int[config.maxArea - config.minArea + 1];
        int wgtl = 0;
        int lgtw = 0;
        for (int i = 0; i < config.numCells; ++i)
        {
            Vector2 size = RandomSize(config, areaCount);
            Cell newCell = Cell.New(
                (int)size.x,
                (int)size.y,
                map.transform,
                RandomPosition(config));
            newCell.name = string.Format("{0}:{1}", i, newCell.name);
            map.cells.Add(newCell);
            if (newCell.width > newCell.length)
                ++wgtl;
            else if (newCell.width < newCell.length)
                ++lgtw;
        }
        GameObject.FindObjectOfType<ShowDistribution>().Show(areaCount);
        Debug.LogFormat("width greater than length: {0:p}", (float)wgtl / map.cells.Count);
        Debug.LogFormat("length greater than width: {0:p}", (float)lgtw / map.cells.Count);
        yield break;
    }

    public void Clear(DungeonMap map)
    {
        for (int i = map.transform.childCount - 1; i >= 0; --i)
        {
            if (Application.isPlaying)
                GameObject.Destroy(map.transform.GetChild(i).gameObject);
            else
                GameObject.DestroyImmediate(map.transform.GetChild(i).gameObject);
        }
        map.cells.Clear();
    }

    Vector2 RandomSize(Config config, int[] areaCount)
    {
        int area;
        if (config.useGaussianDistribution)
        {
            float t;
            do
            {
                t = GaussianDistribution.Next(config.mean, config.deviation);
            } while (t < 0f || t > 1f);
            t = 2f * Mathf.Abs(t - 0.5f);
            area = (int)Mathf.Lerp(config.minArea, config.maxArea, t);
        }
        else
        {
            area = UnityEngine.Random.Range(config.minArea, config.maxArea + 1);
        }
        ++areaCount[area - config.minArea];

        float asqrt = Mathf.Sqrt(area);
        int l = Mathf.Max(1, Mathf.FloorToInt(asqrt));
        int r = Mathf.CeilToInt(asqrt) + 1;
        float width = UnityEngine.Random.Range(l, r);
        float length = area / width;
        return new Vector2(width, length);                
    }

    Vector3 RandomPosition(Config config)
    {
        Vector2 p = UnityEngine.Random.insideUnitCircle;
        return new Vector3(Mathf.RoundToInt(p.x * config.radius), 0f, Mathf.RoundToInt(p.y * config.radius));
    }

    void IGenPipeline.OnDrawGizmosSelected(DungeonMap map)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(map.transform.position, map.generateCells.radius);
        for (int i = 0, max = map.transform.childCount; i < max; ++i)
        {
            Transform c = map.transform.GetChild(i);
            Gizmos.DrawSphere(c.position, 0.25f);
        }
    }
}
