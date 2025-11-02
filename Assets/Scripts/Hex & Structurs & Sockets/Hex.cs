using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ───────────────────────────── FELDER & INSPECTOR ─────────────────────────────
public class Hex : MonoBehaviour
{
    [Header("Hexfeld-Status & Resourcen")]
    public Biom biom;
    public OccupType occupantType;

    [Tooltip("Maximal verfügbare Resourcen pro Hexfeld")]
    public List<Res> resourcesList_MEPR = new();


    [Tooltip("Maximal abbaubare Resourcen pro Runde")]
    public List<Res> resourcesList_EPR = new();

    [Tooltip("Resourcenfelder um eine Siedlung")]
    public List<GameObject> resTilesOnSettlement = new();

    public ResList minValues;
    public ResList maxValues;
    public ResList actValues;

    [Header("Würfelanzeige")]
    public SpriteRenderer visibleSideRenderer;
    public int diceNumber = -1;
    public string diceText;

    [Header("Referenzen & Visuals")]
    public GameObject structure;
    public GameObject uiSourceReference;
    public GameObject socketHolder;
    public List<GameObject> directionalObjects = new();

    [Header("Mesh / Material / Shader")]
    private Mesh terrainMesh;
    private MeshCollider terrainCollider;
    private Transform terrainTransform;
    private Mesh tileMesh;
    private Material tileMaterial;

    [SerializeField] private HexOutlineConfiguration[] hexConfig = new HexOutlineConfiguration[1];

    [Header("Hex-in-Hex Einstellungen")]
    public GameObject hexPrefab;
    public float heightOffset = 0.2f;
    public float rotationYOffset = 0f;
    public float positionXOffset = 0f;
    public float positionZOffset = 0f;

    public float basePointerLength;
    public float secondPointerMultiplier = 1.8f;
    public float thirdPointerMultiplier = 2f;
    public float secondPointerAngleOffset = 30f;

    private int socketCounter = 0;

// ───────────────────────────── UNITY LIFECYCLE ─────────────────────────────
    private void Start()
    {
        var meshObject = GameObject.Find("Mesh");
        if (meshObject == null)
        {
            Debug.LogError("Mesh-Objekt nicht gefunden!");
            return;
        }
        terrainMesh = meshObject.GetComponent<MeshFilter>()?.sharedMesh;
        terrainCollider = meshObject.GetComponent<MeshCollider>();
        terrainTransform = meshObject.transform;
    }

// ───────────────────────────── RESSOURCEN-MANAGEMENT ─────────────────────────────
    public void ChangeListMEPR(List<CardProperties.Resources> resourceEarningList)
    {
        if (resourceEarningList == null) return;
        foreach (var resource in resourceEarningList)
        {
            int index = resourcesList_MEPR.FindIndex(item => item.resourceName == resource.resourceName);
            if (index != -1)
            {
                resourcesList_MEPR[index].resourceAmount += resource.resourceAmount;
            }
        }
    }

    public void ChangeListeEPR()
    {
        var cardPropertiesArray = GetComponentsInChildren<CardProperties>();
        if (cardPropertiesArray.Length == 0) return;

        // EPR Liste zurücksetzen
        foreach (var res in resourcesList_EPR)
            res.resourceAmount = 0;

        foreach (var cardProperties in cardPropertiesArray)
        {
            foreach (var resources in cardProperties.resourceEarningList)
            {
                int index = resourcesList_EPR.FindIndex(item => item.resourceName == resources.resourceName);
                if (index != -1)
                {
                    resourcesList_EPR[index].resourceAmount += resources.resourceAmount;
                }
            }
        }
    }

    public void GenerateAllResourceAmounts(bool rerollResources)
    {
        foreach (var resource in resourcesList_MEPR)
        {
            if ((!resource.isIdentified && !rerollResources) || (resource.isIdentified && rerollResources))
                SetAmount(resource);
        }
    }

    public void GenerateResourceAmount(string resourceName, bool rerollResources)
    {
        foreach (var resource in resourcesList_MEPR)
        {
            if (resource.resourceName.ToString() == resourceName &&
                ((!resource.isIdentified && !rerollResources) || rerollResources))
            {
                SetAmount(resource);
            }
        }
    }

    private void SetAmount(Res resource)
    {
        if (resource == null) return;
        var generator = new BasicResourcesGenerator();
        resource.isIdentified = true;
        resource.resourceAmount = generator.GeneratResourcesAmountOnHexfield(resource.resourceName, biom);
    }

    // ───────────────────────────── WÜRFEL & PRODUKTION ─────────────────────────────
    public void GenerateDiceNumber()
    {
        diceNumber = Dice.Instance.DiceRoll();
        diceText = Dice.Instance.GetDiceName(diceNumber);
        if (visibleSideRenderer != null)
            visibleSideRenderer.sprite = Dice.Instance.GetSpriteOfDice(diceNumber);
    }

    public int GetDiceNumber() => diceNumber;

// ───────────────────────────── MESH & TERRAIN ─────────────────────────────
    public void GenerateTileMesh()
    {
        if (hexConfig.Length == 0 || hexConfig[0].hexOutlineMesh == null) return;
        var meshFilter = hexConfig[0].hexOutlineMesh.GetComponent<MeshFilter>();
        var meshRenderer = hexConfig[0].hexOutlineMesh.GetComponent<MeshRenderer>();
        tileMesh = new Mesh();
        meshFilter.mesh = tileMesh;

        tileMaterial = new Material(hexConfig[0].tileShader);
        tileMaterial.SetColor("_LineColor", hexConfig[0].lineColor);
        tileMaterial.SetFloat("_LineThickness", hexConfig[0].lineThickness);
        tileMaterial.SetVector("_CenterPosition", transform.position);
        tileMaterial.SetFloat("_HexSize", hexConfig[0].hexSize);
        meshRenderer.material = tileMaterial;

        CreateMesh();
    }

    private void CreateMesh()
    {
        int hexMeshBorder = hexConfig[0].hexSize / 2;
        int numVertices = (hexMeshBorder + 1) * (hexMeshBorder + 1);
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[hexMeshBorder * hexMeshBorder * 6];

        for (int y = 0; y <= hexMeshBorder; y++)
        {
            for (int x = 0; x <= hexMeshBorder; x++)
            {
                float xPos = (x - hexMeshBorder / 2f) * hexConfig[0].tileSize;
                float zPos = (y - hexMeshBorder / 2f) * hexConfig[0].tileSize;
                Vector3 worldPos = transform.position + new Vector3(xPos, 0, zPos);
                float height = GetHeightFromTerrain(worldPos) + hexConfig[0].heightOffset;
                vertices[y * (hexMeshBorder + 1) + x] = new Vector3(xPos, height - transform.position.y, zPos);
            }
        }

        int triangleIndex = 0;
        for (int y = 0; y < hexMeshBorder; y++)
        {
            for (int x = 0; x < hexMeshBorder; x++)
            {
                int bottomLeft = y * (hexMeshBorder + 1) + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + (hexMeshBorder + 1);
                int topRight = topLeft + 1;

                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        tileMesh.Clear();
        tileMesh.vertices = vertices;
        tileMesh.triangles = triangles;
        tileMesh.RecalculateNormals();
    }

    private float GetHeightFromTerrain(Vector3 position)
    {
        int terrainLayerMask = LayerMask.GetMask("map");
        if (Physics.Raycast(position + Vector3.up * 5000, Vector3.down, out RaycastHit hit, 10000f, terrainLayerMask))
            return hit.point.y;
        return position.y;
    }

    private float SampleTerrainHeight(Vector3 worldPos)
    {
        if (terrainCollider == null)
        {
            Debug.LogWarning("TerrainCollider nicht gesetzt!");
            return worldPos.y;
        }
        Ray ray = new Ray(worldPos + Vector3.up * 1000f, Vector3.down);
        if (terrainCollider.Raycast(ray, out RaycastHit hit, 2000f))
            return hit.point.y;
        Debug.LogWarning("Raycast auf Terrain fehlgeschlagen!");
        return worldPos.y;
    }

// ───────────────────────────── SHADER & VISUALS ─────────────────────────────
    public void SetLineColor(Color color)
    {
        hexConfig[0].lineColor = color;
        tileMaterial?.SetColor("_LineColor", color);
    }

    public void SetLineThickness(float thickness)
    {
        hexConfig[0].lineThickness = thickness;
        tileMaterial?.SetFloat("_LineThickness", thickness);
    }

    public void SetHighlight(Color color)
    {
        SetLineColor(color);
        SetLineThickness(10);
    }

    public void ResetHighlighting()
    {
        switch (occupantType)
        {
            case OccupType.Settlement:
                SetHighlight(Color.red);
                break;
            case OccupType.Resource:
                SetHighlight(Color.yellow);
                break;
            default:
                SetLineColor(hexConfig[0].DefaultlineColor);
                SetLineThickness(hexConfig[0].DefaultlineThickness);
                break;
        }
    }

    public void SetHexSize(float size)
    {
        hexConfig[0].hexSize = (int)size;
    }

// ───────────────────────────── SOCKET-MANAGEMENT ─────────────────────────────
    public void ShowAllSockets()
    {
        Transform socketHolderTransform = transform.Find("SocketHolder");
        if (socketHolderTransform == null) return;
        foreach (Transform t in socketHolderTransform)
        {
            var socket = t.GetComponent<Socket>();
            if (socket != null && socket.socketType != Rarity.None)
                t.GetComponent<Renderer>().enabled = true;
        }
    }

    public void HideAllSockets()
    {
        Transform socketHolderTransform = transform.Find("SocketHolder");
        if (socketHolderTransform == null) return;
        foreach (Transform t in socketHolderTransform)
        {
            var renderer = t.GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
        }
    }

    public void ResetAllSocketHighlighting()
    {
        Transform socketHolderTransform = transform.Find("SocketHolder");
        if (socketHolderTransform == null) return;
        foreach (Transform t in socketHolderTransform)
        {
            var socket = t.GetComponent<Socket>();
            if (socket != null) socket.ResetHighlight();
        }
    }

    public void SetSocketFromStructur()
    {
        if (structure == null) return;
        var settlement = structure.GetComponent<Settlement>();
        if (settlement == null) return;
        List<Rarity> socketKey = settlement.socketKey;
        Transform socketHolderTransform = transform.Find("SocketHolder");
        if (socketHolderTransform == null) return;

        for (int i = 0; i <= 18; i++)
        {
            foreach (Transform t in socketHolderTransform)
            {
                if (t.name == "Socket_" + i)
                {
                    var socket = t.GetComponent<Socket>();
                    if (socket != null)
                    {
                        socket.SetBaseMaterial(socketKey[i]);
                        if (socketKey[i] != Rarity.None)
                            socket.socketType = socketKey[i];
                    }
                }
            }
        }
    }

// ───────────────────────────── HEX-IN-HEX (PLATZIERUNG) ─────────────────────────────
    public void PlaceHexAtPosition(Vector3 worldCenterPosition)
    {
        float centerY = SampleTerrainHeight(worldCenterPosition) + heightOffset;
        Vector3 correctedCenter = new Vector3(
            worldCenterPosition.x + positionXOffset,
            centerY,
            worldCenterPosition.z + positionZOffset
        );

        Quaternion rotation = Quaternion.Euler(0f, rotationYOffset, 0f);
        Transform socketHolderTransform = transform.Find("SocketHolder");
        if (socketHolderTransform == null)
        {
            Debug.LogWarning("SocketHolder nicht gefunden!");
            return;
        }
        GameObject hexInstance = Instantiate(hexPrefab, correctedCenter, rotation, socketHolderTransform);
        hexInstance.name = $"Socket_{socketCounter++}";
        var renderer = hexInstance.GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;

        MeshFilter mf = hexInstance.GetComponent<MeshFilter>();
        if (mf != null)
        {
            Mesh deformableMesh = Instantiate(mf.sharedMesh);
            mf.mesh = deformableMesh;
            DeformHexToTerrain(hexInstance.transform, deformableMesh);
        }
    }

    public void PlaceHexPattern(Vector3 center)
    {
        float[] multipliers = { 1f, secondPointerMultiplier, thirdPointerMultiplier };
        float[] angleOffsets = { secondPointerAngleOffset, 0f, secondPointerAngleOffset };

        PlaceHexAtPosition(center);

        for (int ring = 0; ring < multipliers.Length; ring++)
        {
            float length = basePointerLength * multipliers[ring];
            float startAngle = angleOffsets[ring];

            for (int i = 0; i < 6; i++)
            {
                float angle = startAngle + i * 60f;
                Vector3 dir = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    0f,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );
                Vector3 worldPos = center + dir * length;
                PlaceHexAtPosition(worldPos);
            }
        }
    }

    private void DeformHexToTerrain(Transform hexTransform, Mesh hexMesh)
    {
        Vector3[] vertices = hexMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = hexTransform.TransformPoint(vertices[i]);
            float terrainY = SampleTerrainHeight(worldPos);
            worldPos.y = terrainY + heightOffset;
            vertices[i] = hexTransform.InverseTransformPoint(worldPos);
        }
        hexMesh.vertices = vertices;
        hexMesh.RecalculateNormals();
        hexMesh.RecalculateBounds();
    }
}

// ───────────────────────────── HILFSKLASSEN ─────────────────────────────
[System.Serializable]
public class HexOutlineConfiguration
{
    public Shader tileShader;
    public GameObject hexOutlineMesh;
    public int hexSize;
    public float tileSize = 4.5f;
    public float heightOffset = 4f;
    public Color lineColor = Color.gray;
    public float lineThickness = 0.9f;
    public Color DefaultlineColor = Color.gray;
    public float DefaultlineThickness = 0.9f;
}
