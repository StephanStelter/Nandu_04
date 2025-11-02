using UnityEngine;

public static class TextureGenerator
{

    public static Texture2D TextureFromColourMap(Color[] colourMap, int mapSize)
    {
        Texture2D texture = new Texture2D(mapSize, mapSize);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Color[] MapColorsFromHeightMap(float[,] noiseMap, int mapSize, TerrainColorType[] regions)
    {
        Color[] colourMap = new Color[mapSize * mapSize];
        for (int y = 0; y < mapSize; y++)
            for (int x = 0; x < mapSize; x++)
                for (int i = 0; i < regions.Length; i++)
                    if (noiseMap[x, y] <= regions[i].height) { colourMap[y * mapSize + x] = regions[i].colour; break; }

        return colourMap;
    }
}
