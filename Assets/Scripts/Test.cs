using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int subdivisionIterations = 1;
    //public int smoothingIterations = 1;
    public float smoothingAmount = 0.5f;

    private void Start()
    {
        SubdivideAndSmoothMesh();
    }

    private void SubdivideAndSmoothMesh()
    {
        Mesh originalMesh = meshFilter.mesh;

        for (int i = 0; i < subdivisionIterations; i++)
        {
            originalMesh = SubdivideMesh(originalMesh);
            originalMesh = SmoothMesh(originalMesh);
        }

        meshFilter.mesh = originalMesh;
    }

    private Mesh SubdivideMesh(Mesh mesh)
    {
        Vector3[] originalVertices = mesh.vertices;
        int[] originalTriangles = mesh.triangles;

        List<Vector3> subdividedVertices = new List<Vector3>(originalVertices);
        List<int> subdividedTriangles = new List<int>();

        int triangleCount = originalTriangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int vertexA = originalTriangles[i * 3];
            int vertexB = originalTriangles[i * 3 + 1];
            int vertexC = originalTriangles[i * 3 + 2];

            Vector3 midPointAB = (originalVertices[vertexA] + originalVertices[vertexB]) * 0.5f;
            Vector3 midPointBC = (originalVertices[vertexB] + originalVertices[vertexC]) * 0.5f;
            Vector3 midPointCA = (originalVertices[vertexC] + originalVertices[vertexA]) * 0.5f;

            int midPointABIndex = subdividedVertices.Count;
            int midPointBCIndex = midPointABIndex + 1;
            int midPointCAIndex = midPointABIndex + 2;

            subdividedVertices.Add(midPointAB);
            subdividedVertices.Add(midPointBC);
            subdividedVertices.Add(midPointCA);

            subdividedTriangles.Add(vertexA);
            subdividedTriangles.Add(midPointABIndex);
            subdividedTriangles.Add(midPointCAIndex);

            subdividedTriangles.Add(midPointABIndex);
            subdividedTriangles.Add(vertexB);
            subdividedTriangles.Add(midPointBCIndex);

            subdividedTriangles.Add(midPointBCIndex);
            subdividedTriangles.Add(vertexC);
            subdividedTriangles.Add(midPointCAIndex);

            subdividedTriangles.Add(midPointABIndex);
            subdividedTriangles.Add(midPointBCIndex);
            subdividedTriangles.Add(midPointCAIndex);
        }

        Mesh subdividedMesh = new Mesh();
        subdividedMesh.vertices = subdividedVertices.ToArray();
        subdividedMesh.triangles = subdividedTriangles.ToArray();
        subdividedMesh.RecalculateNormals();
        subdividedMesh.RecalculateBounds();
        subdividedMesh.Optimize();

        return subdividedMesh;
    }

    private Mesh SmoothMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] smoothedVertices = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            List<Vector3> neighborPositions = new List<Vector3>();

            for (int j = 0; j < mesh.vertexCount; j++)
            {
                if (i == j)
                    continue;

                float distance = Vector3.Distance(vertices[i], vertices[j]);
                if (distance <= smoothingAmount)
                {
                    neighborPositions.Add(vertices[j]);
                }
            }

            if (neighborPositions.Count > 0)
            {
                // Ajout du vertex courant à la liste des voisins pour prendre en compte le barycentre
                neighborPositions.Add(vertices[i]);

                // Calcul du barycentre
                Vector3 averagePosition = GetAveragePosition(neighborPositions);

                smoothedVertices[i] = averagePosition;
            }
            else
            {
                smoothedVertices[i] = vertices[i];
            }
        }

        mesh.vertices = smoothedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    private Vector3 GetAveragePosition(List<Vector3> positions)
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (Vector3 position in positions)
        {
            averagePosition += position;
        }

        averagePosition /= positions.Count;

        return averagePosition;
    }

}
