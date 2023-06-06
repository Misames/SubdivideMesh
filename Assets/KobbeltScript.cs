using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KobbeltScript : MonoBehaviour
{

    private MeshFilter meshFilter;

    public int iteration = 1;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        for(int i = 0; i < iteration; i++)
        {
            TrySubdivide(meshFilter.mesh);
        }

    }

    private void TrySubdivide(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int originalTriangleCount = triangles.Length / 3;

        Mesh myNewMesh = new Mesh();

        for (int i = 0; i < originalTriangleCount; i++)
        {
            int a = triangles[i * 3];
            int b = triangles[i * 3 + 1];
            int c = triangles[i * 3 + 2];

            Vector3 verticesA = vertices[a];
            Vector3 verticesB = vertices[b];
            Vector3 verticesC = vertices[c];

            var center = (verticesA + verticesB + verticesC) / 3;

            // Perturbation
            float perturbationAmount = 0.5f; // Ajustez la valeur selon vos besoins

            Vector3 perturbedVerticesA = -perturbationAmount * verticesA + perturbationAmount * (verticesB + verticesC) / 2;
            Vector3 perturbedVerticesB = -perturbationAmount * verticesB + perturbationAmount * (verticesC + verticesA) / 2;
            Vector3 perturbedVerticesC = -perturbationAmount * verticesC + perturbationAmount * (verticesA + verticesB) / 2;

            // Ajout des sommets perturbés et du centre dans les vertices du mesh
            Vector3[] newVertices = mesh.vertices;
            Array.Resize(ref newVertices, newVertices.Length + 1);
            newVertices[a] = perturbedVerticesA;
            newVertices[b] = perturbedVerticesB;
            newVertices[c] = perturbedVerticesC;
            newVertices[mesh.vertices.Length] = center;
            mesh.vertices = newVertices;

            // Ajout du triangle
            int[] newTriangles = mesh.triangles;
            Array.Resize(ref newTriangles, newTriangles.Length + 9);

            newTriangles[newTriangles.Length - 9] = mesh.vertices.Length - 1;
            newTriangles[newTriangles.Length - 8] = a; // Indice du premier sommet existant
            newTriangles[newTriangles.Length - 7] = c; // Indice du deuxième sommet existant

            newTriangles[newTriangles.Length - 6] = mesh.vertices.Length - 1;
            newTriangles[newTriangles.Length - 5] = a; // Indice du premier sommet existant
            newTriangles[newTriangles.Length - 4] = b; // Indice du deuxième sommet existant

            newTriangles[newTriangles.Length - 3] = mesh.vertices.Length - 1;
            newTriangles[newTriangles.Length - 2] = b; // Indice du premier sommet existant
            newTriangles[newTriangles.Length - 1] = c; // Indice du deuxième sommet existant

            mesh.triangles = newTriangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

        }


    }


}
