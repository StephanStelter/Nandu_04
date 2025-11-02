using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TreeSpawner : MonoBehaviour
{
    [Header("Einstellungen")]
    public GameObject terrainMesh;
    public GameObject[] treePrefabs;
    public float noiseScale = 0.1f;
    public float treeThreshold = 0.4f;
    public float treeLine = 1000f;
    public float spacing = 4f;
    public float minDistanceBetweenTrees = 2f;

    [Header("Zufälligkeit")]
    public float minScale = 2.9f;
    public float maxScale = 3.2f;

    [Header("Optionen")]
    public int seed = 42;
    public bool clearExisting = true;
    public int maxTrees = 100;

    [Header("Noise Map")]
    public int mapSize = 100;
    [SerializeField] public noiseConfiguration[] nConfig = new noiseConfiguration[1];

    private MeshCollider terrainCollider;
    private readonly List<Vector3> placedPositions = new();

    // ───────────────────────────── FOREST GENERATION ─────────────────────────────
    public void GenerateForest()
    {
        if (clearExisting)
            ClearExistingTrees();

        if (!ValidateSetup())
            return;

        terrainCollider = GetOrAddCollider(terrainMesh);
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, nConfig);

        placedPositions.Clear();
        Random.InitState(seed);

        Bounds bounds = terrainCollider.bounds;
        int treeCount = 0;

        for (float x = bounds.min.x; x < bounds.max.x; x += spacing)
        {
            for (float z = bounds.min.z; z < bounds.max.z; z += spacing)
            {
                if (treeCount >= maxTrees)
                {
                    Debug.Log($"Maximale Anzahl von {maxTrees} Bäumen erreicht.");
                    return;
                }

                int sampleX = Mathf.FloorToInt(Mathf.InverseLerp(bounds.min.x, bounds.max.x, x) * (mapSize - 1));
                int sampleZ = Mathf.FloorToInt(Mathf.InverseLerp(bounds.min.z, bounds.max.z, z) * (mapSize - 1));
                float noiseValue = noiseMap[sampleZ, sampleX];

                if (noiseValue < treeThreshold)
                    continue;

                Vector3 rayOrigin = new Vector3(x, bounds.max.y + 10f, z);
                if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 5000f))
                    continue;
                if (hit.collider.gameObject != terrainMesh)
                    continue;

                Vector3 spawnPosition = hit.point;
                if (spawnPosition.y < treeLine)
                    continue;
                if (!IsFarEnoughFromOtherTrees(spawnPosition))
                    continue;

                SpawnTree(spawnPosition);
                treeCount++;
            }
        }
        //Debug.Log($"Wald generiert mit {treeCount} Bäumen.");
    }

    // ───────────────────────────── HILFSMETHODEN ─────────────────────────────
    private void ClearExistingTrees()
    {
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);
    }

    private bool ValidateSetup()
    {
        if (terrainMesh == null)
        {
            Debug.LogWarning("Terrain fehlt.");
            return false;
        }
        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogWarning("Baum-Prefabs fehlen.");
            return false;
        }
        return true;
    }

    private MeshCollider GetOrAddCollider(GameObject obj)
    {
        var collider = obj.GetComponent<MeshCollider>();
        if (collider == null)
            collider = obj.AddComponent<MeshCollider>();
        return collider;
    }

    private bool IsFarEnoughFromOtherTrees(Vector3 position)
    {
        float minDistSqr = minDistanceBetweenTrees * minDistanceBetweenTrees;
        foreach (Vector3 pos in placedPositions)
        {
            if ((position - pos).sqrMagnitude < minDistSqr)
                return false;
        }
        return true;
    }

    private void SpawnTree(Vector3 position)
    {
        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        float scale = Random.Range(minScale, maxScale);

        GameObject tree = Instantiate(prefab, position, rot, transform);
        tree.transform.localScale *= scale;
        placedPositions.Add(position);
    }
}
