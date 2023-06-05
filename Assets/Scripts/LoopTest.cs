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
        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
        List<int> triangles = new List<int>(mesh.triangles);

        int originalVertexCount = vertices.Count;
        int originalTriangleCount = triangles.Count / 3;

        // Créer un nouveau point pour chaque face
        for (int i = 0; i < originalTriangleCount; i++)
        {
            int v1 = triangles[i * 3];
            int v2 = triangles[i * 3 + 1];
            int v3 = triangles[i * 3 + 2];

            Vector3 facePoint = (vertices[v1] + vertices[v2] + vertices[v3]) / 3f;
            vertices.Add(facePoint);
        }

        // Créer un nouveau point pour chaque arête
        Dictionary<Edge, int> edgePoints = new Dictionary<Edge, int>();
        for (int i = 0; i < originalTriangleCount; i++)
        {
            int v1 = triangles[i * 3];
            int v2 = triangles[i * 3 + 1];
            int v3 = triangles[i * 3 + 2];

            int v4 = GetEdgePoint(v1, v2, vertices, edgePoints);
            int v5 = GetEdgePoint(v2, v3, vertices, edgePoints);
            int v6 = GetEdgePoint(v3, v1, vertices, edgePoints);

            triangles[i * 3] = v1;
            triangles[i * 3 + 1] = v4;
            triangles[i * 3 + 2] = v6;

            triangles.Add(v4);
            triangles.Add(v2);
            triangles.Add(v5);

            triangles.Add(v6);
            triangles.Add(v5);
            triangles.Add(v3);

            triangles.Add(v4);
            triangles.Add(v5);
            triangles.Add(v6);
        }

        // Réorganiser les triangles
        for (int i = originalTriangleCount - 1; i >= 0; i--)
        {
            int offset = originalVertexCount + i * 3;

            triangles.RemoveAt(offset);
            triangles.RemoveAt(offset + 1);
            triangles.RemoveAt(offset + 2);
        }

        newMesh.vertices = vertices.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.RecalculateNormals();

        return newMesh;
    }

    private int GetEdgePoint(int v1, int v2, List<Vector3> vertices, Dictionary<Edge, int> edgePoints)
    {
        Edge edge = new Edge(v1, v2);

        if (edgePoints.ContainsKey(edge))
        {
            return edgePoints[edge];
        }
        else
        {
            Vector3 edgePoint = (vertices[v1] + vertices[v2]) / 2f;
            int newPointIndex = vertices.Count;
            vertices.Add(edgePoint);

            edgePoints.Add(edge, newPointIndex);
            return newPointIndex;
        }
    }

    private struct Edge
    {
        public int vertexIndex1;
        public int vertexIndex2;

        public Edge(int v1, int v2)
        {
            vertexIndex1 = Mathf.Min(v1, v2);
            vertexIndex2 = Mathf.Max(v1, v2);
        }
    }
}
