using UnityEngine;

public static class GaussianDistribution
{
    /// <summary>
    /// normal distribution in N(0, 1)
    /// </summary>
    public static float value
    {
        get
        {
            float v1, v2, s;
            do
            {
                v1 = 2.0f * UnityEngine.Random.value - 1.0f;
                v2 = 2.0f * UnityEngine.Random.value - 1.0f;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1.0f || s == 0f);

            s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

            return v1 * s;
        }
    }

    /// <summary>
    /// normal distribution in N(mean, deviation)
    /// </summary>
    public static float Next(float mean, float standard_deviation)
    {
        return mean + value * standard_deviation;
    }
}