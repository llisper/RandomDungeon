using UnityEngine;
using System.Collections.Generic;

public class ShowDistribution : MonoBehaviour
{
    public int columns = 100;
    public int samples = 1000;
    public float scale = 1f;

    public float mean = 0f;
    public float deviation = 1f;

    public void Show(IEnumerable<int> values)
    {
        Show(new List<int>(values));
    }

    public void Show(List<int> values)
    {
        List<Transform> list = CreateColumns(values.Count);
        for (int i = 0; i < values.Count; ++i)
        {
            Transform t = list[i];
            Vector3 s = t.localScale;
            s.y = values[i] * scale;
            t.localScale = s;
            Vector3 p = t.localPosition;
            p.y = s.y * 0.5f;
            t.localPosition = p;
        }
    }

    [ContextMenu("Uniform Distribution")]
    void UniformDistribution()
    {
        int[] list = new int[columns];
        for (int i = 0; i < samples; ++i)
        {
            int c = (int)(UnityEngine.Random.value * columns);
            ++list[c];
        }
        Show(list);
    }

    [ContextMenu("Normal Distribution")]
    void NormalDistribution()
    {
        int[] list = new int[columns];
        for (int i = 0; i < samples; ++i)
        {
            float v = GaussianDistribution.Next(mean, deviation);
            if (v < 0f || v > 1f)
                continue;

            int c = (int)(v * columns);
            ++list[c];
        }
        Show(list);
    }

    [ContextMenu("Normal Distribution Fixed")]
    void NormalDistributionFixed()
    {
        int[] list = new int[columns];
        for (int i = 0; i < samples; ++i)
        {
            float v = (GaussianDistribution.value + 5f) / 10f;
            v = Mathf.Clamp01(v);
            int c = (int)(v * columns);
            ++list[c];
        }
        Show(list);
    }

    List<Transform> CreateColumns(int count)
    {
        Clear();
        List<Transform> list = new List<Transform>(count);
        for (int i = 0; i < count; ++i)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(i, 0f, 0f);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = new Vector3(1f, 0f, 1f);
            list.Add(go.transform);
        }
        return list;
    }

    [ContextMenu("Clear")]
    void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            if (Application.isPlaying)
                GameObject.Destroy(transform.GetChild(i).gameObject);
            else
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}