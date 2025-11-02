using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
   

    public void GenerateMesh(float[,] heightMap, mapConfiguration[] mConfig)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        MeshData meshData = GenerateTerrainMesh(heightMap, mConfig);
        Mesh mesh = CreateMesh(meshData);

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

    }

    private MeshData GenerateTerrainMesh(float[,] heightMap, mapConfiguration[] mConfig)
    {
        int meshSize = heightMap.GetLength(0);
        float topLeft = (meshSize-1) / -2f;
        int simpInc = (mConfig[0].levelOfDetail == 0) ? 1 : mConfig[0].levelOfDetail * 2;

        int vertPerLine = Mathf.FloorToInt((meshSize - 1) / (float)simpInc) + 1;

        MeshData meshData = new MeshData(meshSize);

        int vertIndex = 0;

        for (int y = 0; y < meshSize; y += simpInc)
        {
            for (int x = 0; x < meshSize; x += simpInc)
            {
                Vector3 vertPos = new Vector3(
                    topLeft + x,
                    mConfig[0].meshHeightCurve.Evaluate(heightMap[x, y]) * mConfig[0].meshHeightMultiplier,
                    topLeft + y
                );

                meshData.vertices[vertIndex] = vertPos;
                meshData.uvs[vertIndex] = new Vector2( (x / (float)meshSize),  (y / (float)meshSize));

                if (x < meshSize - 1 && y < meshSize - 1)
                {
                    AddTriangles(meshData, vertIndex, vertPerLine);
                }

                vertIndex++;
            }
        }

        return meshData;
    }

    private void AddTriangles(MeshData meshData, int vertIndex, int vertPerLine)
    {
        AddTriangleIndices(meshData, vertIndex, vertIndex + vertPerLine, vertIndex + 1);
        AddTriangleIndices(meshData, vertIndex + 1, vertIndex + vertPerLine, vertIndex + vertPerLine + 1);
    }

    private void AddTriangleIndices(MeshData meshData, int a, int b, int c)
    {
        meshData.triangles[meshData.triangleIndex] = a;
        meshData.triangles[meshData.triangleIndex + 1] = b;
        meshData.triangles[meshData.triangleIndex + 2] = c;
        meshData.triangleIndex += 3;
    }

    private Mesh CreateMesh(MeshData meshData)
    {
        Mesh mesh = new Mesh
        {
            vertices = meshData.vertices,
            triangles = meshData.triangles,
            uv = meshData.uvs,
            colors = meshData.colors
        };

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        return mesh;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Color[] colors;
    public int Width { get; }
    public int Height { get; }
    public int triangleIndex;

    public MeshData(int meshSize)
    {
        Width = meshSize;
        Height = meshSize;
        vertices = new Vector3[meshSize * meshSize];
        uvs = new Vector2[meshSize * meshSize];
        triangles = new int[(meshSize - 1) * (meshSize - 1) * 6];
        colors = new Color[meshSize * meshSize]; // Initialize colors array
    }
}