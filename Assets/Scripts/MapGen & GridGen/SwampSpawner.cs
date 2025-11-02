using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // Optional: Nur zur Laufzeit entfernen, wenn du es nicht im Editor brauchst
public class SwampSpawner : MonoBehaviour
{
    [Header("Einstellungen")]
    public GameObject terrainMesh;                // Dein Terrain-Mesh GameObject
    public GameObject[] SwampPrefabs;              // Liste von ca. 4 Baum-Prefabs
    public float noiseScale = 0.1f;
    public float swampThreshold = 0.4f;            // Ab diesem Noise-Wert wird ein Baum platziert
    public float swampLine = 1000f;
    public float spacing = 4f;                    // Rasterabstand in Welt-Einheiten
    public float minDistanceBetweenRocks = 2f;    // Mindestabstand zwischen Bäumen

    [Header("Zufälligkeit")]
    public float minScale = 2.9f;
    public float maxScale = 3.2f;

    [Header("Optionen")]
    public int seed = 42;
    public bool clearExisting = true;
    public int maxRocks = 100; // << NEU: Maximale Baumanzahl

    [Header("Noise Map")]
    public int mapSize = 100; // Muss ggf. auf Terrain-Größe angepasst werden
    [SerializeField] public noiseConfiguration[] nConfig = new noiseConfiguration[1];

    private MeshCollider terrainCollider;
    private List<Vector3> placedPositions = new List<Vector3>();

    public void GenerateRocks()
    {


        if (clearExisting)
        {
            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);
        }

        if (terrainMesh == null || SwampPrefabs.Length == 0)
        {
            Debug.LogWarning("Terrain oder Fels-Prefabs fehlen.");
            return;
        }

        terrainCollider = terrainMesh.GetComponent<MeshCollider>();
        if (terrainCollider == null)
        {
            terrainCollider = terrainMesh.AddComponent<MeshCollider>();
        }

        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, nConfig);


        placedPositions.Clear();
        Random.InitState(seed);

        Bounds bounds = terrainCollider.bounds;
        for (float x = bounds.min.x; x < bounds.max.x; x += spacing)
        {
            for (float z = bounds.min.z; z < bounds.max.z; z += spacing)
            {
                int sampleX = Mathf.FloorToInt(Mathf.InverseLerp(bounds.min.x, bounds.max.x, x) * (mapSize - 1));
                int sampleZ = Mathf.FloorToInt(Mathf.InverseLerp(bounds.min.z, bounds.max.z, z) * (mapSize - 1));

                float noiseValue = noiseMap[sampleZ, sampleX];

                if (noiseValue < swampThreshold) continue;

                Vector3 rayOrigin = new Vector3(x, bounds.max.y + 10f, z);
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 5000f))
                {
                    if (hit.collider.gameObject != terrainMesh) continue;



                    Vector3 spawnPosition = hit.point;

                    // Prüfe Mindestabstand
                    bool tooClose = false;
                    foreach (Vector3 pos in placedPositions)
                    {
                        if ((spawnPosition - pos).sqrMagnitude < minDistanceBetweenRocks * minDistanceBetweenRocks)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (tooClose) continue;
                    if (spawnPosition.y < swampLine) continue;

                    GameObject prefab = SwampPrefabs[Random.Range(0, SwampPrefabs.Length)];
                    Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    float scale = Random.Range(minScale, maxScale);

                    GameObject tree = Instantiate(prefab, spawnPosition, rot, transform);
                    tree.transform.localScale *= scale;

                    placedPositions.Add(spawnPosition);

                    if (placedPositions.Count >= maxRocks)
                    {
                        Debug.Log($"Maximale Anzahl von {maxRocks} Felsen erreicht.");
                        return;
                    }
                }
            }
        }

        Debug.Log($"Wald generiert mit {placedPositions.Count} Felsen.");
    }
}
