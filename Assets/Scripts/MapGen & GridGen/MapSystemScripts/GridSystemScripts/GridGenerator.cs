using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class GridGenerator : MonoBehaviour
{
    public Transform tileHolder;
    public Transform hexTile;
    public Material hexMaterial;
    private float tileXOffset = 1.8f;
    private float tileZOffset = 1.565f;
    private float hexSizeCorrection = 1.04f;
    public Vector2 LevelCorrection = new Vector2(20f, 20f); // WaterLevel und Peak Level f�r bedingung ob HexTile vorhanden
    public MeshCollider terrainCollider; // MeshCollider des Landschafts-Mesh

    private Mesh terrainMesh;

    private Dictionary<Biom, ResList> minValues = new();
    private Dictionary<Biom, ResList> maxValues = new();

    public void Start()
    {
        GameObject meshObject = GameObject.Find("Mesh");
        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        terrainMesh = meshFilter.sharedMesh;
        terrainCollider = meshObject.GetComponent<MeshCollider>();
    }


    public void GenerateGrid(gridConfiguration[] gConfig,mapConfiguration[] mConfig, float waterLevel, float peakLevel,TreeSpawner treeSpawner, RockSpawner rockSpawner, SwampSpawner swampSpawner)
    {
        ClearChildren(tileHolder);
        InitTerrainResources();

        Transform mapTransform = transform.root;
        Vector3 mapScale = mapTransform.localScale;
        float mapYPosition = mapTransform.position.y;

        float mapXMin = -gConfig[0].gridSize / 2;
        float mapXMax = gConfig[0].gridSize / 2;

        float mapZMin = -gConfig[0].gridSize / 2;
        float mapZMax = gConfig[0].gridSize / 2;

        float scaledTileXOffset = tileXOffset * (mConfig[0].mapSize * mapScale.x * (1 - 1 / (gConfig[0].gridSize + 2))) / gConfig[0].gridSize;
        float scaledTileZOffset = tileZOffset * (mConfig[0].mapSize * mapScale.x * (1 - 1 / (gConfig[0].gridSize + 2))) / gConfig[0].gridSize;

        float hexSize = ((mConfig[0].mapSize * mapScale.x * (1 - 1 / (gConfig[0].gridSize + 2))) / gConfig[0].gridSize)* hexSizeCorrection;


        for (float x = mapXMin; x < mapXMax; x++)
        {
            for (float z = mapZMin; z < mapZMax; z++)
            {
                Vector3 pos;

                if (z % 2 == 0)
                {
                    pos = new Vector3(x * scaledTileXOffset, 0, z * scaledTileZOffset);
                }
                else
                {
                    pos = new Vector3(x * scaledTileXOffset + scaledTileXOffset / 2, 0, z * scaledTileZOffset);
                }

                // Raycast nach unten, um die H�he des Meshes an der Position zu ermitteln
                Bounds terrainBounds = terrainCollider.bounds;

                Ray ray = new Ray(new Vector3(pos.x, terrainBounds.max.y + 1000, pos.z), Vector3.down);
                RaycastHit hit;


                if (terrainCollider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    pos.y = hit.point.y; // Die H�he des Gel�ndes an dieser Position
                }

                //Korrektur der gernzwerte da h�he mehrfach angepasst wurde beim erstellen der Karte
                float fixedWaterLevel = waterLevel * mapScale.y * mConfig[0].meshHeightCurve.Evaluate(waterLevel) * mConfig[0].meshHeightMultiplier + mapYPosition + LevelCorrection.x;
                float fixedPeakLevel = peakLevel * mapScale.y * mConfig[0].meshHeightCurve.Evaluate(peakLevel) * mConfig[0].meshHeightMultiplier + mapYPosition - LevelCorrection.y;

                if (pos.y > fixedWaterLevel && pos.y < fixedPeakLevel)
                {
                    Transform TempGO = Instantiate(hexTile);
                    TempGO.transform.parent = tileHolder;
                    TempGO.name = x.ToString() + ", " + z.ToString();
                    TempGO.transform.position = pos;

                    Hex tileScript = TempGO.GetComponent<Hex>();

                    tileScript.SetHexSize(hexSize); // �bergibt den berechneten hexSize-Wert
                    tileScript.GenerateTileMesh();
                    SetUpHex(TempGO.gameObject, treeSpawner, rockSpawner, swampSpawner);
                }
                else if (pos.y <= fixedWaterLevel)
                {
                    Transform TempGO = Instantiate(hexTile);
                    TempGO.transform.parent = tileHolder;
                    TempGO.name = x.ToString() + ", " + z.ToString();
                    TempGO.transform.position = pos;

                    Hex tileScript = TempGO.GetComponent<Hex>();
                    if (tileScript != null)
                    {
                        tileScript.SetHexSize(hexSize); // �bergibt den berechneten hexSize-Wert
                        Transform cubeTransform = TempGO.transform.Find("Cube");
                        cubeTransform.gameObject.SetActive(false);
                        SetUpHex(TempGO.gameObject, treeSpawner, rockSpawner, swampSpawner);
                    }

                }
            }
        }
    }

    private static void ClearChildren(Transform container)
    {
        while (container.childCount > 0)
        {
            Transform child = container.GetChild(0);
            GameObject.DestroyImmediate(child.gameObject);
        }
    }


    private void SetUpHex(GameObject hex, TreeSpawner treeSpawner, RockSpawner rockSpawner, SwampSpawner swampSpawner)
    {
        InitializeTerrainMap();
        //InitTerrainResources();
        SpawnDirectionalObjects(200f, hex); // wert sollte am betsen direkt aus MapCOnfiguration kommen
        SetTerrainType(hex, treeSpawner, rockSpawner, swampSpawner);
        SetResourceValues(hex);

    }


    // ALLE Zum Terraintyp realisieren

    public void SpawnDirectionalObjects(float length, GameObject hex)
    {
        List<GameObject> objects = new List<GameObject>();
        string[] directions = { "N", "NO", "SO", "S", "SW", "NW" };

        for (int i = 0; i < directions.Length; i++)
        {
            float angleDegrees = i * 60f;
            float angleRadians = angleDegrees * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRadians), 0, Mathf.Sin(angleRadians)) * length;
            Vector3 horizontalPosition = hex.transform.position + offset;

            Vector3 rayStart = horizontalPosition + Vector3.up * 1000f;
            Ray ray = new Ray(rayStart, Vector3.down);
            RaycastHit hit;

            if (terrainCollider.Raycast(ray, out hit, 2000f))
            {
                Vector3 spawnPosition = hit.point;
                GameObject obj = new GameObject(directions[i]);
                obj.transform.position = spawnPosition;
                obj.transform.parent = hex.transform;
                objects.Add(obj);
            }
        }
        //return objects;
        hex.GetComponent<Hex>().directionalObjects = objects;
    }
    public void SetTerrainType(GameObject hex, TreeSpawner treeSpawner, RockSpawner rockSpawner, SwampSpawner swampSpawner)
    {
        List<GameObject> directionalObjects = hex.GetComponent<Hex>().directionalObjects;

        float averageHeight = CalculateAverageHeight(directionalObjects);
        string heightBin = GetHeightBinary(averageHeight);

        Bounds bounds = terrainCollider.bounds;
        Vector3 hexPos = hex.transform.position;

        float[,] treeNoise = GenerateNoiseMap(treeSpawner.GetComponent<TreeSpawner>().nConfig);
        string forestBin = EvaluateBiome(directionalObjects, treeNoise, bounds, hexPos, treeSpawner.GetComponent<TreeSpawner>().treeThreshold);

        float[,] swampNoise = GenerateNoiseMap(swampSpawner.GetComponent<SwampSpawner>().nConfig);
        string swampBin = EvaluateBiome(directionalObjects,swampNoise, bounds, hexPos, swampSpawner.GetComponent<SwampSpawner>().swampThreshold);

        string key = heightBin + forestBin + swampBin;

        if (terrainKeyMap.TryGetValue(key, out Biom result))
        {
            hex.GetComponent<Hex>().biom = result;
            //Debug.Log($"TerrainType: {result}" + "  "  + key);
        }
        else
        {
            Debug.LogWarning($"Unbekannter TerrainKey: {key}");
            //currentTerrainType = HexMap.Resources.TerrainType.Sea; // Default fallback
        }
    }
    private float CalculateAverageHeight(List<GameObject> directionalObjects)
    {
        float totalHeight = transform.position.y;
        foreach (GameObject obj in directionalObjects)
        {
            totalHeight += obj.transform.position.y;
        }
        return totalHeight / (directionalObjects.Count + 1);
    }
    private string GetHeightBinary(float height)
    {
        return height switch
        {
            < -2100 => "000", // Sea Level
            < -1900 => "001", // Low Land
            < -1500 => "010", // Mid Land
            < -1300 => "011", // High Land
            _ => "100", // Super High Land
        };
    }
    private float[,] GenerateNoiseMap(noiseConfiguration[] config)
    {
        int mapSize = 256;
        return Noise.GenerateNoiseMap(mapSize, config);
    }
    private string EvaluateBiome(List<GameObject> directionalObjects, float[,] noiseMap, Bounds bounds, Vector3 center, float threshold)
    {
        int mapSize = noiseMap.GetLength(0);
        int count = 0;

        foreach (GameObject obj in directionalObjects)
        {
            if (GetNoiseValueAtPosition(noiseMap, bounds, obj.transform.position) > threshold)
                count++;
        }

        // Auch das zentrale Hexfeld pr�fen
        if (GetNoiseValueAtPosition(noiseMap, bounds, center) > threshold)
            count++;

        return count switch
        {
            >= 6 => "11",
            >= 3 => "10",
            _ => "00",
        };
    }
    private float GetNoiseValueAtPosition(float[,] noiseMap, Bounds bounds, Vector3 position)
    {
        int mapSize = noiseMap.GetLength(0);

        float percX = Mathf.InverseLerp(bounds.min.x, bounds.max.x, position.x);
        float percZ = Mathf.InverseLerp(bounds.min.z, bounds.max.z, position.z);

        int x = Mathf.Clamp(Mathf.FloorToInt(percX * (mapSize - 1)), 0, mapSize - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt(percZ * (mapSize - 1)), 0, mapSize - 1);

        return noiseMap[z, x];
    }
    private Dictionary<string, Biom> terrainKeyMap;
    private void InitializeTerrainMap()
    {
        terrainKeyMap = new Dictionary<string, Biom>
        {
            // Beispielhafte Zuordnungen:
            { "1001100", Biom.Berg },       // sehr hoch, mittlerer Wald, wenig Sumpf
            { "0111000", Biom.Hügelig },    // mittel hoch, viel Wald, kein Sumpf
            { "0011000", Biom.Feld },       // tief, viel Wald, kein Sumpf
            { "0010000", Biom.Wiese },      // tief, kein Wald, kein Sumpf
            { "0100010", Biom.Wald },       // mittel, wenig Wald, wenig Sumpf
            { "0010011", Biom.Moor },       // tief, wenig Wald, viel Sumpf
            { "0000000", Biom.Sea },      // sehr tief, kein Wald, kein Sumpf
            // Weitere Kombinationen nach Bedarf...
        };
    }

    private void SetResourceValues(GameObject hex)
    {
        Biom biom = hex.GetComponent<Hex>().biom;

        ResList min = minValues[biom];
        ResList max = maxValues[biom];
        ResList randomized = min & max;

        Hex tile = hex.GetComponent<Hex>();
        tile.minValues = min;
        tile.maxValues = max;
        tile.actValues = randomized;
    }

    private void InitTerrainResources()
    {
        minValues[Biom.Berg] = new ResList(0, 1, 0, 2, 2, 0, 0, 0);
        maxValues[Biom.Berg] = new ResList(1, 3, 1, 4, 4, 1, 0, 0);
        minValues[Biom.Feld] = new ResList(2, 0, 0, 0, 0, 1, 0, 0);
        maxValues[Biom.Feld] = new ResList(4, 1, 1, 1, 1, 2, 0, 0);
        minValues[Biom.Hügelig] = new ResList(1, 1, 1, 1, 1, 1, 0, 0);
        maxValues[Biom.Hügelig] = new ResList(3, 2, 2, 2, 2, 2, 0, 0);
        minValues[Biom.Moor] = new ResList(0, 0, 1, 0, 0, 1, 0, 0);
        maxValues[Biom.Moor] = new ResList(2, 1, 3, 1, 1, 3, 0, 0);
        minValues[Biom.Wald] = new ResList(1, 0, 2, 0, 1, 2, 0, 0);
        maxValues[Biom.Wald] = new ResList(2, 1, 4, 1, 2, 4, 0, 0);
        minValues[Biom.Wiese] = new ResList(2, 0, 1, 0, 1, 2, 0, 0);
        maxValues[Biom.Wiese] = new ResList(3, 1, 2, 1, 2, 4, 0, 0);
        minValues[Biom.Wüste] = new ResList(0, 1, 0, 0, 0, 0, 0, 0);
        maxValues[Biom.Wüste] = new ResList(1, 4, 1, 1, 1, 1, 0, 0);
        minValues[Biom.Sea] = new ResList(0, 0, 1, 0, 0, 2, 0, 0);
        maxValues[Biom.Sea] = new ResList(1, 1, 2, 1, 1, 4, 0, 0);
    }

}
