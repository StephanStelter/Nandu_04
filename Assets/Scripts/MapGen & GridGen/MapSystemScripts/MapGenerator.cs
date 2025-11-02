using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private mapConfiguration[] mConfig = new mapConfiguration[1];
    [SerializeField] private noiseConfiguration[] nConfig = new noiseConfiguration[1];
    [SerializeField] private gridConfiguration[] gConfig = new gridConfiguration[1];
    [SerializeField] private TerrainColorType[] regions;
    [SerializeField] private TerrainTypenew[] regionsnew;

    [SerializeField] private GameObject mesh; // Das GameObject, das das MeshGenerator-Skript enthält
    [SerializeField] private GridGenerator gridGenerator;
    public bool AutoUpdate;

    private MeshGenerator meshGenerator;
    public TreeSpawner treeSpawner;
    public RockSpawner rockSpawner;
    public SwampSpawner swampSpawner;


    private void Start()
    {
        GenerateMap();
    }


    public void GenerateMap()
    {
        meshGenerator = mesh.GetComponent<MeshGenerator>();

        float[,] noiseMap = Noise.GenerateNoiseMap(mConfig[0].mapSize, nConfig);

        Color[] colourMap = TextureGenerator.MapColorsFromHeightMap(noiseMap, mConfig[0].mapSize, regions);

        Texture2D texture = TextureGenerator.TextureFromColourMap(colourMap, mConfig[0].mapSize);

        meshGenerator.GenerateMesh(noiseMap, mConfig); // Übergebe die Daten an MeshGenerator

        MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();

        meshRenderer.sharedMaterial.mainTexture = texture;

        treeSpawner.GenerateForest();

        rockSpawner.GenerateRocks();

        gridGenerator.GenerateGrid(gConfig, mConfig, regions[0].height, regions[5].height, treeSpawner, rockSpawner, swampSpawner);


    }
}



[System.Serializable]
public struct TerrainColorType
{

    public string name;
    public float height;
    public Color colour;
}

[System.Serializable]
public struct TerrainTypenew
{

    public string name;
    public float height;
    public Material material;
}

[System.Serializable]
public class noiseConfiguration
{
    public int seed = 22;
    [Range(0.1f, 50)] public float noiseScale = 1.0f;
    [Range(0f, 1f)] public float persistance = 0.5f;
    [Min(0f)] public float lacunarity = 2.0f;
    public int octaves = 4;
    public Vector2 offset = new Vector2(0, 0);
}

[System.Serializable]
public class mapConfiguration
{
    [Range(2, 1000)] public int mapSize = 100;
    [Range(0, 0)] public int levelOfDetail = 0; // WQird aktuell nicht gebraucht
    [Range(0.1f, 50)] public float meshHeightMultiplier = 5.0f;
    public AnimationCurve meshHeightCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)); // Standard-AnimationCurve
}

[System.Serializable]
public class gridConfiguration
{
    [Min(0f)] public int gridSize = 22;
}