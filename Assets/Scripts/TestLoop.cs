using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoop : MonoBehaviour
{
    public int subdivisionCount = 1;
    public float smoothingAmount = 0.1f;

    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        SubdivideMesh();
        SmoothMesh();
    }

    private void SubdivideMesh()
    {
        Mesh originalMesh = meshFilter.mesh;

        for (int i = 0; i < subdivisionCount; i++)
        {
            originalMesh = LoopSubdivision(originalMesh);
        }

        meshFilter.mesh = originalMesh;
    }

    private Mesh LoopSubdivision(Mesh mesh)
    {
        Mesh subdividedMesh = new Mesh();

        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        int triangleCount = triangles.Length / 3;
        int vertexCount = vertices.Length;

        Vector3[] newVertices = new Vector3[triangleCount * 3 + vertexCount];
        Vector3[] newNormals = new Vector3[triangleCount * 3 + vertexCount];
        int[] newTriangles = new int[triangleCount * 3 * 4];

        // Copy existing vertices and normals
        System.Array.Copy(vertices, newVertices, vertexCount);
        System.Array.Copy(normals, newNormals, vertexCount);

        // Subdivide triangles
        int newIndex = vertexCount;
        int newTriangleIndex = 0;
        for (int i = 0; i < triangleCount; i++)
        {
            int indexA = triangles[i * 3];
            int indexB = triangles[i * 3 + 1];
            int indexC = triangles[i * 3 + 2];

            Vector3 vertexA = vertices[indexA];
            Vector3 vertexB = vertices[indexB];
            Vector3 vertexC = vertices[indexC];

            Vector3 midPointAB = (vertexA + vertexB) * 0.5f;
            Vector3 midPointBC = (vertexB + vertexC) * 0.5f;
            Vector3 midPointCA = (vertexC + vertexA) * 0.5f;

            newVertices[newIndex] = midPointAB;
            newVertices[newIndex + 1] = midPointBC;
            newVertices[newIndex + 2] = midPointCA;

            // Calculate new normals
            Vector3 normalA = normals[indexA];
            Vector3 normalB = normals[indexB];
            Vector3 normalC = normals[indexC];

            Vector3 newNormalAB = (normalA + normalB) * 0.5f;
            Vector3 newNormalBC = (normalB + normalC) * 0.5f;
            Vector3 newNormalCA = (normalC + normalA) * 0.5f;

            newNormals[newIndex] = newNormalAB.normalized;
            newNormals[newIndex + 1] = newNormalBC.normalized;
            newNormals[newIndex + 2] = newNormalCA.normalized;

            newTriangles[newTriangleIndex] = indexA;
            newTriangles[newTriangleIndex + 1] = newIndex;
            newTriangles[newTriangleIndex + 2] = newIndex + 2;

            newTriangles[newTriangleIndex + 3] = newIndex;
            newTriangles[newTriangleIndex + 4] = indexB;
            newTriangles[newTriangleIndex + 5] = newIndex + 1;

            newTriangles[newTriangleIndex + 6] = newIndex + 2;
            newTriangles[newTriangleIndex + 7] = newIndex + 1;
            newTriangles[newTriangleIndex + 8] = indexC;

            newTriangles[newTriangleIndex + 9] = newIndex;
            newTriangles[newTriangleIndex + 10] = newIndex + 1;
            newTriangles[newTriangleIndex + 11] = newIndex + 2;

            newIndex += 3;
            newTriangleIndex += 12;
        }

        subdividedMesh.vertices = newVertices;
        subdividedMesh.normals = newNormals;
        subdividedMesh.triangles = newTriangles;

        return subdividedMesh;
    }
    private void SmoothMesh()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] smoothedVertices = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            List<Vector3> neighborVertices = GetNeighborVertices(mesh, i);

            // Calculate the average position of the neighboring vertices
            Vector3 averagePosition = Vector3.zero;
            foreach (Vector3 neighborVertex in neighborVertices)
            {
                averagePosition += neighborVertex;
            }
            averagePosition /= neighborVertices.Count;

            // Calculate the displacement vector and add it to the original vertex position
            Vector3 displacement = (averagePosition - vertex) * smoothingAmount;
            smoothedVertices[i] = vertex + displacement;
        }

        // Replace the original mesh vertices with the smoothed vertices
        mesh.vertices = smoothedVertices;
        mesh.RecalculateNormals();
    }

    private List<Vector3> GetNeighborVertices(Mesh mesh, int vertexIndex)
    {
        List<Vector3> neighborVertices = new List<Vector3>();

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int indexA = mesh.triangles[i];
            int indexB = mesh.triangles[i + 1];
            int indexC = mesh.triangles[i + 2];

            if (indexA == vertexIndex)
            {
                neighborVertices.Add(mesh.vertices[indexB]);
                neighborVertices.Add(mesh.vertices[indexC]);
            }
            else if (indexB == vertexIndex)
            {
                neighborVertices.Add(mesh.vertices[indexA]);
                neighborVertices.Add(mesh.vertices[indexC]);
            }
            else if (indexC == vertexIndex)
            {
                neighborVertices.Add(mesh.vertices[indexA]);
                neighborVertices.Add(mesh.vertices[indexB]);
            }
        }

        return neighborVertices;
    }


}