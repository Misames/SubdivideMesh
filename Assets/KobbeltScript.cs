using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class KobbeltScript : MonoBehaviour
{

    private MeshFilter meshFilter;

    public int iteration = 1;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        var mesh = meshFilter.mesh;

        for (int i = 0; i < iteration; i++)
        {
            Perturbate(mesh);
            mesh = KobbeltSubdivision(mesh);
        }

    }

    private Mesh KobbeltSubdivision(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        int nbTriangles = triangles.Length / 3;

        Vector3[] subVertices = new Vector3[vertices.Length + nbTriangles];
        List<int> subTriangles = new List<int>();
        Array.Copy(vertices, subVertices, vertices.Length);

        for (int i = 0; i < nbTriangles; i++)
        {
            int indexTriangle = i * 3;
            int vertexIndex1 = triangles[indexTriangle];
            int vertexIndex2 = triangles[indexTriangle + 1];
            int vertexIndex3 = triangles[indexTriangle + 2];

            Vector3 vertex1 = vertices[vertexIndex1];
            Vector3 vertex2 = vertices[vertexIndex2];
            Vector3 vertex3 = vertices[vertexIndex3];

            Vector3 center = (vertex1 + vertex2 + vertex3) / 3;
            subVertices[vertices.Length + i] = center;

        }

        for (int i = 0; i < nbTriangles; i++)
        {

            int indexTriangle = i * 3;
            int vertexIndex1 = triangles[indexTriangle];
            int vertexIndex2 = triangles[indexTriangle + 1];
            int vertexIndex3 = triangles[indexTriangle + 2];

            int centerIndex = vertices.Length + i;

            for (int j = 0; j < nbTriangles; j++)
            {
                if (j == i) continue;

                int nextIndexTriangle = j * 3;
                int nextVertexIndex1 = triangles[nextIndexTriangle];
                int nextVertexIndex2 = triangles[nextIndexTriangle + 1];
                int nextVertexIndex3 = triangles[nextIndexTriangle + 2];

                int nextCenterIndex = vertices.Length + j;

                List<int> triangle1 = new List<int>() { vertexIndex1, vertexIndex2, vertexIndex3 };
                List<int> triangle2 = new List<int>() { nextVertexIndex1, nextVertexIndex2, nextVertexIndex3 };

                var MatchingVertices = FindMatchingVertices(triangle1, triangle2, vertices);

                if (MatchingVertices.Count > 1)
                {
                    //nouveaux triangles

                    var matchingVertice1Triangle1 = triangle1[MatchingVertices[0].Item1];
                    var matchingVertice1Triangle2 = triangle2[MatchingVertices[0].Item2];
                    
                    var matchingVertice2Triangle1 = triangle1[MatchingVertices[1].Item1];
                    var matchingVertice2Triangle2 = triangle2[MatchingVertices[1].Item2];

                    subTriangles.Add(matchingVertice1Triangle1); // premier sommet
                    subTriangles.Add(centerIndex); // Deuxieme sommet
                    subTriangles.Add(nextCenterIndex); // 3e sommet

                    subTriangles.Add(matchingVertice2Triangle1); // premier sommet
                    subTriangles.Add(centerIndex); // Deuxieme sommet
                    subTriangles.Add(nextCenterIndex); // 3e sommet

                    subTriangles.Add(matchingVertice1Triangle2); // premier sommet
                    subTriangles.Add(centerIndex); // Deuxieme sommet
                    subTriangles.Add(nextCenterIndex); // 3e sommet

                    subTriangles.Add(matchingVertice2Triangle2); // premier sommet
                    subTriangles.Add(centerIndex); // Deuxieme sommet
                    subTriangles.Add(nextCenterIndex); // 3e sommet

                }

            }

        }

        mesh.Clear();
        mesh.vertices = subVertices;
        mesh.triangles = subTriangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private void Perturbate(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] perturbedVertices = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            HashSet<Vector3> neighbors = GetNeighbors(mesh, vertices[i]);
            float n = neighbors.Count;
            float alpha = ((4 - 2 * Mathf.Cos((2 * Mathf.PI) / n))) / 9;
            Vector3 sum = Vector3.zero;

            foreach (var neighbor in neighbors)
            {
                sum += neighbor;
            }

            perturbedVertices[i] = ((1 - alpha) * vertices[i]) + ((alpha / n) * sum);
        }

        mesh.vertices = perturbedVertices;
        mesh.RecalculateBounds();
    }

    private HashSet<Vector3> GetNeighbors(Mesh mesh, Vector3 vertex)
    {
        HashSet<Vector3> neighborVertices = new HashSet<Vector3>();

        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int triangleIndex = i * 3;

            if (mesh.vertices[triangles[triangleIndex]] == vertex ||
                mesh.vertices[triangles[triangleIndex + 1]] == vertex ||
                mesh.vertices[triangles[triangleIndex + 2]] == vertex)
            {
                if (mesh.vertices[triangles[triangleIndex]] == vertex)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
                }
                else if (mesh.vertices[triangles[triangleIndex + 1]] == vertex)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
                }
                else if (mesh.vertices[triangles[triangleIndex + 2]] == vertex)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
                }
            }
        }

        return neighborVertices;
    }

    List<(int, int)> FindMatchingVertices(List<int> list1, List<int> list2, Vector3[] vertices)
    {
        List<(int, int)> matchingVertices = new List<(int, int)>();

        for (int i = 0; i < list1.Count; i++)
        {
            Vector3 vertex1 = vertices[list1[i]];

            for (int j = 0; j < list2.Count; j++)
            {
                Vector3 vertex2 = vertices[list2[j]];

                if (vertex1 == vertex2)
                {
                    matchingVertices.Add((i, j));
                    break;
                }
            }
        }

        return matchingVertices;
    }


}
