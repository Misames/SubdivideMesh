using System.Collections.Generic;
using UnityEngine;

public class LoopTest : MonoBehaviour
{
    public int subdivisionLevel = 1;

    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        SubdivideMesh();
    }

    private void SubdivideMesh()
    {
        Mesh originalMesh = meshFilter.mesh;

        for (int i = 0; i < subdivisionLevel; i++)
        {
            originalMesh = SubdivideOnce(originalMesh);
        }

        meshFilter.mesh = originalMesh;
    }

    private Mesh SubdivideOnce(Mesh mesh)
    {
        Mesh newMesh = new Mesh();
        newMesh.vertices = mesh.vertices;
        newMesh.triangles = mesh.triangles;
        newMesh.uv = mesh.uv;
        newMesh.tangents = mesh.tangents;


        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
        List<int> triangles = new List<int>(mesh.triangles);
        List<Vector2> uvs = new List<Vector2>(mesh.uv);
        List<Vector4> tangents = new List<Vector4>(mesh.tangents);


        // Make sure that the uvs list has the same number of elements as the vertices list
        if (uvs.Count < vertices.Count)
        {
            for (int i = uvs.Count; i < vertices.Count; i++)
            {
                uvs.Add(Vector2.zero);
            }
        }

        int originalVertexCount = vertices.Count;
        int originalTriangleCount = triangles.Count / 3;

        List<int> newTriangles = new List<int>();
        List<Vector2> newUVs = new List<Vector2>();

        int triangleCount = triangles.Count;
        for (int i = 0; i < triangleCount; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            int v4 = GetOrCreateEdgePoint(v1, v2, vertices, uvs, newUVs);
            int v5 = GetOrCreateEdgePoint(v2, v3, vertices, uvs, newUVs);
            int v6 = GetOrCreateEdgePoint(v3, v1, vertices, uvs, newUVs);

            newTriangles.Add(v1);
            newTriangles.Add(v4);
            newTriangles.Add(v6);

            newTriangles.Add(v4);
            newTriangles.Add(v2);
            newTriangles.Add(v5);

            newTriangles.Add(v6);
            newTriangles.Add(v5);
            newTriangles.Add(v3);

            newTriangles.Add(v4);
            newTriangles.Add(v5);
            newTriangles.Add(v6);
        }

        for (int i = 0; i < newTriangles.Count; i++)
        {
            int newIndex = newTriangles[i];
            if (newIndex >= originalVertexCount)
            {
                newTriangles[i] = newIndex - originalVertexCount;
            }
        }

        newMesh.SetVertices(vertices);
        newMesh.SetTriangles(newTriangles, 0);
        newMesh.SetUVs(0, newUVs);
        newMesh.RecalculateNormals();

        newMesh.bounds = mesh.bounds;
        newMesh.colors = mesh.colors;

        return newMesh;
    }

    private int GetOrCreateEdgePoint(int v1, int v2, List<Vector3> vertices, List<Vector2> uvs, List<Vector2> newUVs)
    {
        int edgePointIndex = FindEdgePoint(v1, v2, vertices);
        if (edgePointIndex != -1)
        {
            return edgePointIndex;
        }

        Vector3 edgePoint = (vertices[v1] + vertices[v2]) / 2f;
        int newPointIndex = vertices.Count;
        vertices.Add(edgePoint);

        Vector2 uv = (uvs[v1] + uvs[v2]) / 2f;
        newUVs.Add(uv);

        return newPointIndex;
    }

    private int FindEdgePoint(int v1, int v2, List<Vector3> vertices)
    {
        for (int i = vertices.Count - 1; i >= 0; i--)
        {
            if (vertices[i] == (vertices[v1] + vertices[v2]) / 2f)
            {
                return i;
            }
        }

        return -1;
    }
}
