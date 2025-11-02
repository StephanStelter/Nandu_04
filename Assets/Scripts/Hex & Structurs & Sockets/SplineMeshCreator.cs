using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshCreator : MonoBehaviour
{
    public float offsetRange = 50f;
    public float width = 30f;
    public int segments = 30;

    public Material material;


    public static SplineMeshCreator Instance { get; private set; }
    private void Awake()
    {
        // Sicher stellen, dass nur eine Instanz existiert
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Eine Instanz existiert bereits, diese sollte zerstört werden
            DontDestroyOnLoad(gameObject); // optional: behält die Instanz über Szenenwechsel hinweg
        }
    }


    public void CreateSplineMesh(Vector3 start, Vector3 end, Transform hex)
    {
        Vector3[] controlPoints = GenerateControlPoints(start, end, offsetRange);
        Mesh mesh = GenerateMeshAlongCurve(controlPoints, segments, width);

        GameObject meshObj = new GameObject("SplineMesh");
        meshObj.transform.SetParent(hex);
        MeshFilter mf = meshObj.AddComponent<MeshFilter>();
        MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
        MeshCollider mc = meshObj.AddComponent<MeshCollider>();

        mf.mesh = mesh;
        mr.material = material;
        mc.enabled = true;
        mc.convex = true;
    }

    Vector3 RandomOffset(Vector3 side, Vector3 up, float range)
    {
        return side * Random.Range(-range, range) + up * 1;// Random.Range(-range, range);
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }
    Mesh GenerateMeshAlongCurve(Vector3[] points, int segmentCount, float width)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int numSegments = points.Length - 3; // 6 Punkte = 3 Segmente (0-3, 1-4, 2-5)
        int vertsPerSegment = segmentCount / numSegments;

        for (int seg = 0; seg < numSegments; seg++)
        {
            Vector3 p0 = points[seg];
            Vector3 p1 = points[seg + 1];
            Vector3 p2 = points[seg + 2];
            Vector3 p3 = points[seg + 3];

            for (int i = 0; i <= vertsPerSegment; i++)
            {
                float t = i / (float)vertsPerSegment;
                Vector3 pos = CatmullRom(p0, p1, p2, p3, t);

                Vector3 forward = Vector3.zero;
                if (i < vertsPerSegment)
                {
                    float tNext = (i + 1) / (float)vertsPerSegment;
                    forward = CatmullRom(p0, p1, p2, p3, tNext) - pos;
                }
                else if (i > 0)
                {
                    float tPrev = (i - 1) / (float)vertsPerSegment;
                    forward = pos - CatmullRom(p0, p1, p2, p3, tPrev);
                }

                Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
                vertices.Add(pos + right * width * 0.5f);
                vertices.Add(pos - right * width * 0.5f);

                if (seg > 0 || i > 0)
                {
                    int index = vertices.Count - 4;
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);

                    triangles.Add(index + 1);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    Vector3[] GenerateControlPoints(Vector3 start, Vector3 end, float offsetRange)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        Vector3 up = Vector3.up;
        Vector3 side = Vector3.Cross(direction, up).normalized;

        // 4 echte Punkte
        Vector3 p1 = start;
        Vector3 p2 = start + direction * (distance * 0.33f) + RandomOffset(side, up, offsetRange);
        Vector3 p3 = start + direction * (distance * 0.66f) + RandomOffset(side, up, offsetRange);
        Vector3 p4 = end;

        // Fake p0 und p5 für glatten Übergang am Anfang/Ende
        Vector3 p0 = p1 - (p2 - p1);
        Vector3 p5 = p4 + (p4 - p3);

        // Rückgabe: 6 Punkte für spätere Segmente
        return new Vector3[] { p0, p1, p2, p3, p4, p5 };
    }

}

