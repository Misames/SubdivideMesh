using UnityEngine;
using System.Collections.Generic;

public class SmoothingLoop : MonoBehaviour
{
    public int subdivisions = 1; // Nombre de subdivisions à appliquer

    private void Start()
    {
        // Obtenir le composant MeshFilter attaché à l'objet
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("Le MeshFilter n'a pas été trouvé.");
            return;
        }

        // Obtenir le maillage d'origine
        Mesh originalMesh = meshFilter.mesh;
        if (originalMesh == null)
        {
            Debug.LogError("Le maillage d'origine n'a pas été trouvé.");
            return;
        }

        // Appliquer la subdivision
        Mesh subdividedMesh = SubdivideMesh(originalMesh, subdivisions);

        // Mettre à jour le maillage de l'objet avec le maillage subdivisé
        meshFilter.mesh = subdividedMesh;
    }

    private Mesh SubdivideMesh(Mesh mesh, int subdivisions)
    {
        // Effectuer les subdivisions en boucle
        for (int i = 0; i < subdivisions; i++)
        {
            mesh = SubdivideOnce(mesh);
        }

        return mesh;
    }

    private Mesh SubdivideOnce(Mesh mesh)
    {
        // Copier les données du maillage d'origine
        Vector3[] originalVertices = mesh.vertices;
        int[] originalTriangles = mesh.triangles;

        // Créer des listes pour stocker les nouveaux sommets et les nouveaux triangles
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        // Créer un dictionnaire pour stocker les voisins de chaque sommet
        Dictionary<int, List<int>> vertexNeighbors = new Dictionary<int, List<int>>();

        // Parcourir les triangles d'origine pour construire le dictionnaire des voisins
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            int v1 = originalTriangles[i];
            int v2 = originalTriangles[i + 1];
            int v3 = originalTriangles[i + 2];

            AddNeighbor(vertexNeighbors, v1, v2);
            AddNeighbor(vertexNeighbors, v1, v3);
            AddNeighbor(vertexNeighbors, v2, v3);
        }

        // Parcourir les triangles d'origine et effectuer la subdivision
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            int v1 = originalTriangles[i];
            int v2 = originalTriangles[i + 1];
            int v3 = originalTriangles[i + 2];

            Vector3 e1 = CalculateNewEdgePoint(originalVertices[v1], originalVertices[v2], originalVertices[v3]);
            Vector3 e2 = CalculateNewEdgePoint(originalVertices[v2], originalVertices[v3], originalVertices[v1]);
            Vector3 e3 = CalculateNewEdgePoint(originalVertices[v3], originalVertices[v1], originalVertices[v2]);

            if (!vertexNeighbors.ContainsKey(v1) || !vertexNeighbors.ContainsKey(v2) || !vertexNeighbors.ContainsKey(v3))
            {
                // Ignorer les triangles dont les sommets n'ont pas de voisins
                continue;
            }

            Vector3 v1Prime = CalculateNewVertexPoint(originalVertices[v1], e2, e3, vertexNeighbors[v1], originalVertices);
            Vector3 v2Prime = CalculateNewVertexPoint(originalVertices[v2], e3, e1, vertexNeighbors[v2], originalVertices);
            Vector3 v3Prime = CalculateNewVertexPoint(originalVertices[v3], e1, e2, vertexNeighbors[v3], originalVertices);

            int newIndex = newVertices.Count;
            newVertices.AddRange(new Vector3[] { v1Prime, v2Prime, v3Prime, originalVertices[v1], originalVertices[v2], originalVertices[v3], e1, e2, e3 });

            newTriangles.AddRange(new int[] {
                newIndex, newIndex + 3, newIndex + 5,
                newIndex + 1, newIndex + 4, newIndex + 3,
                newIndex + 2, newIndex + 5, newIndex + 4,
                newIndex + 3, newIndex + 4, newIndex + 5
            });
        }

        // Normaliser les nouveaux sommets
        for (int i = 0; i < newVertices.Count; i++)
        {
            newVertices[i] = newVertices[i].normalized;
        }

        // Créer un nouveau maillage avec les sommets et les triangles subdivisés
        Mesh subdividedMesh = new Mesh();
        subdividedMesh.SetVertices(newVertices);
        subdividedMesh.SetTriangles(newTriangles, 0);
        subdividedMesh.RecalculateNormals();

        return subdividedMesh;
    }

    private Vector3 CalculateNewEdgePoint(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return (v1 + v2 + v3) / 3f + ((3f / 8f) * ((v1 + v2) / 2f));
    }

    private Vector3 CalculateNewVertexPoint(Vector3 v, Vector3 e1, Vector3 e2, List<int> neighbors, Vector3[] originalVertices)
    {
        int n = neighbors.Count;
        Vector3 sum = Vector3.zero;

        for (int i = 0; i < n; i++)
        {
            sum += originalVertices[neighbors[i]];
        }

        float alpha = (3f / 8f) + (1f / 4f * Mathf.Cos(2f * Mathf.PI / n));
        float beta = 1f / n * (1f - alpha);

        Vector3 vPrime = v * alpha + sum * beta;

        return vPrime;
    }

    private void AddNeighbor(Dictionary<int, List<int>> neighbors, int v1, int v2)
    {
        if (!neighbors.ContainsKey(v1))
        {
            neighbors[v1] = new List<int>();
        }
        if (!neighbors[v1].Contains(v2))
        {
            neighbors[v1].Add(v2);
        }
    }
}
