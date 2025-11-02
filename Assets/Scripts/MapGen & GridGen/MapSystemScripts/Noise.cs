using UnityEngine;
using UnityEngine.UIElements;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapSize, noiseConfiguration[] nConfig )
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        System.Random prng = new System.Random(nConfig[0].seed);
        Vector2[] octaveOffsets = new Vector2[nConfig[0].octaves];
        for (int i = 0; i < nConfig[0].octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + nConfig[0].offset.x;
            float offsetY = prng.Next(-100000, 100000) + nConfig[0].offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (nConfig[0].noiseScale <= 0)
        {
            nConfig[0].noiseScale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapSize / 2f;
        float halfHeight = mapSize / 2f;



        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < nConfig[0].octaves; i++)
                {
                    float sampleX = (x - halfWidth) / nConfig[0].noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / nConfig[0].noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= nConfig[0].persistance;
                    frequency *= nConfig[0].lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[y, x] = noiseHeight;
            }
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
