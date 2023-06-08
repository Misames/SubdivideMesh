using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshExtension 
{
    public static int GetEdge(this Mesh mesh, int vertexA, int vertexB)
    {
        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int indexA = triangles[i * 3];
            int indexB = triangles[i * 3 + 1];
            int indexC = triangles[i * 3 + 2];

            if ((indexA == vertexA && indexB == vertexB) || (indexA == vertexB && indexB == vertexA))
                return indexC;
            else if ((indexB == vertexA && indexC == vertexB) || (indexB == vertexB && indexC == vertexA))
                return indexA;
            else if ((indexC == vertexA && indexA == vertexB) || (indexC == vertexB && indexA == vertexA))
                return indexB;
        }

        return -1; // If the edge does not exist
    }

    public static int Concat( this Mesh mesh, Mesh otherMesh)
    {
        int originalVertexCount = mesh.vertexCount;
        int[] originalTriangles = mesh.triangles;

        int otherVertexCount = otherMesh.vertexCount;
        int[] otherTriangles = otherMesh.triangles;

        Vector3[] combinedVertices = mesh.vertices.Concat(otherMesh.vertices).ToArray();
        int[] combinedTriangles = originalTriangles.Concat(otherTriangles.Select(t => t + originalVertexCount)).ToArray();

        mesh.Clear();
        mesh.vertices = combinedVertices;
        mesh.triangles = combinedTriangles;
        mesh.RecalculateNormals();

        return originalVertexCount;
    }
}