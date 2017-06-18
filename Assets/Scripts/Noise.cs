using System;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public GameObject cube;
    public int columns = 10;
    public int mapsize = 10;
    public int randsize = 360;
    public int freqsize = 30;
    public float lift = 0f;
    [Range(-3f, 3f)]
    public float ampexp = 0f;

    List<GameObject> mCubes = new List<GameObject>();

    [ContextMenu("Execute")]
    void Execute()
    {
        foreach (var go in mCubes)
            GameObject.DestroyImmediate(go);
        mCubes.Clear();

        for (int i = 0; i < columns; ++i)
        {
            UnityEngine.Random.InitState(i);
            List<List<float>> noises = new List<List<float>>();
            List<int> frequencies = Frequencies(freqsize);
            for (int k = 0; k < frequencies.Count; ++k)
                noises.Add(CalcNoise(frequencies[k]));
            List<float> amplitudes = Amplitudes(frequencies, ampexp);
            List<float> list = WeightedSum(amplitudes, noises);

            for (int j = 0; j < mapsize; ++j)
            {
                var go = (GameObject)Instantiate(cube);
                go.transform.parent = transform;
                go.transform.position = new Vector3(j, 0f, i);
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = new Vector3(1f, list[j] + lift, 1f);
                mCubes.Add(go);
            }
        }
    }

    List<int> Frequencies(int count)
    {
        List<int> freq = new List<int>();
        for (int i = 1; i <= count; ++i)
            freq.Add(i);
        return freq;
    }

    List<float> Amplitudes(List<int> freq, float exp)
    {
        return Amplitudes(freq, f => Mathf.Pow(f, exp));
    }

    List<float> Amplitudes(List<int> freq, Func<int, float> ampFunc)
    {
        List<float> amps = new List<float>();
        for (int i = 0; i < freq.Count; ++i)
            amps.Add(ampFunc(freq[i]));
        return amps;
    }

    string Join(List<float> list)
    {
        return string.Join(",", list.ConvertAll(v => v.ToString()).ToArray());
    }

    List<float> CalcNoise(int freq)
    {
        List<float> list = new List<float>();
        float phase = UnityEngine.Random.Range(0f, randsize * Mathf.Deg2Rad);
        for (int i = 0; i < mapsize; ++i)
        {
            float value = Mathf.Sin(2 * Mathf.PI * freq * (float)i / mapsize + phase);
            list.Add(value);
        }
        return list;
    }

    List<float> WeightedSum(List<float> amps, List<List<float>> noises)
    {
        List<float> output = new List<float>();
        for (int i = 0; i < mapsize; ++i)
        {
            float value = 0f;
            for (int k = 0; k < noises.Count; ++k)
                value += amps[k] * noises[k][i];
            output.Add(value);
        }
        return output;
    }
}
