using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin : MonoBehaviour
{
    public static float[,,] Noise3D(
        Vector3Int _dimension,
        Vector3 startPos,
        float frequency,
        float lacunarity, 
        float gain,
        int numLayer)
    {
        float[,,] noises = new float[_dimension.x,_dimension.y,_dimension.z];

        float amplitude;
        float amplitudeMax;
        Vector3 point;
        float noise;

        for (int x = 0; x < _dimension.x; x++)
        {
            for (int y = 0; y < _dimension.y; y++)
            {
                for (int z = 0; z < _dimension.z; z++)
                {
                    amplitudeMax = 0.0f;
                    noise = 0.0f;

                    amplitude = 1.0f;
                    point = new Vector3(x, y, z) * frequency;

                    for (int k = 0; k < numLayer; k++)
                    {
                        noise += Perlin3D(point.x + startPos.x, point.y + startPos.y, point.z + startPos.z) * amplitude;

                        point *= lacunarity;
                        amplitude *= gain;

                        amplitudeMax += amplitude;
                    }
                    noise /= amplitudeMax;
                    // noises[x, y, z] = noise;
                    // range.x = Mathf.Min(noise, range.x);
                    // range.y = Mathf.Max(noise, range.y);
                    // range.z = Mathf.Max(noise, range.z);

                    noises[x, y, z] = Mathf.Clamp(noise, 0, 1);
                }
            }
        }

        return noises;
    }


    public static float Perlin3D(float x, float y, float z)
    {
        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);

        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        float ABC = AB + BC + AC + BA + CB + CA;
        return ABC / 6f;
    }



    public static Texture2D FractalNoise(
            int width, int height,
            float frequency,
            float lacunarity, float gain,
            int numLayer)
    {

        Texture2D texture = new Texture2D(width, height);

        float amplitude;
        float amplitudeMax;
        Vector2 point;
        float noise;

        float[,] noises = new float[width, height];

        Vector2 range = new Vector2(1000.0f, 0.0f);

        Vector2 startPos = new Vector2(Random.value, Random.value);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                amplitudeMax = 0.0f;
                noise = 0.0f;

                amplitude = 1.0f;
                point = new Vector2(i, j) * frequency;

                for (int k = 0; k < numLayer; k++)
                {
                    noise += (Mathf.PerlinNoise(point.x + startPos.x, point.y + startPos.y) / 2.0f) * amplitude;

                    point *= lacunarity;
                    amplitude *= gain;

                    amplitudeMax += amplitude;
                }
                noise /= amplitudeMax;
                noises[i, j] = noise;

                range.x = Mathf.Min(noise, range.x);
                range.y = Mathf.Max(noise, range.y);

                noise = Mathf.Clamp(noise, 0, 1);
                texture.SetPixel(i, j, new Color(noise, noise, noise));
            }
        }

        texture.Apply();

        return texture;
    }
}
