using UnityEngine;
using System.Collections.Generic;

public class TriangleSmooth : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int subdivisionIterations = 1;
    public int smoothingIterations = 1;
    public float smoothingAmount = 0.5f;

    private void Start()
    {
        SubdivideAndSmoothMesh();
    }

    private void SubdivideAndSmoothMesh()
    {
        Mesh originalMesh = meshFilter.mesh;

        // Subdivision
        for (int i = 0; i < subdivisionIterations; i++)
        {
            SubdivideMesh(originalMesh);
            originalMesh = meshFilter.mesh;
        }

        // Smoothing
        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMesh();
        }
    }

    /// <summary>
    /// Subdivides the mesh using Loop subdivision algorithm.
    /// </summary>
    private void SubdivideMesh(Mesh mesh)
    {
        // Get original vertices and triangles
        Vector3[] originalVertices = mesh.vertices;
        int[] originalTriangles = mesh.triangles;

        // Create new lists for subdivided vertices and triangles
        List<Vector3> subdividedVertices = new List<Vector3>(originalVertices);
        List<int> subdividedTriangles = new List<int>(originalTriangles);

        // Loop through each triangle and subdivide it into 4 new triangles
        int triangleCount = originalTriangles.Length / 3;
        int vertexIndex = subdividedVertices.Count;

    
        for (int i = 0; i < triangleCount; i++)
        {
            // Get the vertices of the current triangle we are iterating on
            int vertexA = originalTriangles[i * 3];
            int vertexB = originalTriangles[i * 3 + 1];
            int vertexC = originalTriangles[i * 3 + 2];

            // Get the midpoint of each edge in the current triangle we are iterating on
            Vector3 midPointAB = (originalVertices[vertexA] + originalVertices[vertexB]) * 0.5f;
            Vector3 midPointBC = (originalVertices[vertexB] + originalVertices[vertexC]) * 0.5f;
            Vector3 midPointCA = (originalVertices[vertexC] + originalVertices[vertexA]) * 0.5f;

            // Add the new vertices to the list of subdivided vertices and get their indices
            int midPointABIndex = vertexIndex++;
            int midPointBCIndex = vertexIndex++;
            int midPointCAIndex = vertexIndex++;

            // Add the new triangles to the list of subdivided triangles using the indices of the new vertices
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

        // Create a new mesh and apply the subdivided vertices and triangles to it
        Mesh subdividedMesh = new Mesh();
        subdividedMesh.vertices = subdividedVertices.ToArray();
        subdividedMesh.triangles = subdividedTriangles.ToArray();
        subdividedMesh.RecalculateNormals();
        subdividedMesh.RecalculateBounds();

        // Set the new mesh to the mesh filter
        meshFilter.mesh = subdividedMesh;
    }

    /// <summary>
    /// Smooths the mesh by moving each vertex to the average position of its neighboring vertices.
    /// </summary>
    private void SmoothMesh()
    {
        // Get the mesh and its vertices and create a new array for the smoothed vertices
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] smoothedVertices = new Vector3[vertices.Length];

        // Loop through each vertex and get its neighboring vertices and move it to the average position of its neighbors
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 averagePosition = Vector3.zero;
            int neighborCount = 0;

            // Loop through each neighboring vertex
            for (int j = 0; j < mesh.vertexCount; j++)
            {
                if (i == j)
                    continue;

                float distance = Vector3.Distance(vertices[i], vertices[j]);
                if (distance <= smoothingAmount)
                {
                    averagePosition += vertices[j];
                    neighborCount++;
                }
            }

            // Get the average position of the neighboring vertices and move the current vertex to that position
            averagePosition /= neighborCount;
            smoothedVertices[i] = averagePosition;
        }

        // Set the smoothed vertices to the mesh and recalculate its normals
        mesh.vertices = smoothedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}