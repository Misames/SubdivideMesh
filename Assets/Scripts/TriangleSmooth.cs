////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////// This script is The only working script for the project.//////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////


//- Le script suivant réalise la subdivision de maillage en utilisant l'algorithme de subdivision de loop.
//- La première partie du script subdivise le maillage en triangles plus petits.
//- La deuxième partie du script lisse le maillage en déplaçant les sommets vers le centre de leurs voisins.
//- Le script est attaché à un objet dans la scène et utilise un MeshFilter pour obtenir le maillage d'origine 
//et mettre à jour le maillage de l'objet avec le maillage subdivisé et lissé.
//- Il faut jouer avec les paramètres subdivisionIterations, smoothingIterations et smoothingAmount pour obtenir
//le résultat souhaité.

using UnityEngine;
using System.Collections.Generic;

public class TriangleSmooth : MonoBehaviour
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

        // Subdivision
        for (int i = 0; i < subdivisionIterations; i++)
        {
            SubdivideMesh(originalMesh);
            originalMesh = meshFilter.mesh;
            //ici on applique le lissage autant de fois que l'on subdivise
            SmoothMesh();
        }

        // Smoothing if we want to control the amount of smoothing
        //for (int i = 0; i < smoothingIterations; i++)
        //{
        //     SmoothMesh();
        //}

    }

    /// <summary>
    /// subdivise le maillage en triangles plus petits.
    /// </summary>
    private void SubdivideMesh(Mesh mesh)
    {
        // Récupérer les sommets et les triangles du maillage original
        Vector3[] originalVertices = mesh.vertices;
        int[] originalTriangles = mesh.triangles;

        // Les listes sont utilisées pour ajouter de nouveaux sommets et triangles au fur et à mesure
        List<Vector3> subdividedVertices = new List<Vector3>(originalVertices);
        List<int> subdividedTriangles = new List<int>(originalTriangles);

        // Récupérer le nombre de triangles et l'index du premier sommet du triangle courant
        int triangleCount = originalTriangles.Length / 3;
        int vertexIndex = subdividedVertices.Count;

    
        for (int i = 0; i < triangleCount; i++)
        {
            // Récupérer les sommets du triangle courant sur lequel on itere
            int vertexA = originalTriangles[i * 3];
            int vertexB = originalTriangles[i * 3 + 1];
            int vertexC = originalTriangles[i * 3 + 2];

            // Recuperer le point milieu de chaque arrete du triangle courant sur lequel on itere
            Vector3 midPointAB = (originalVertices[vertexA] + originalVertices[vertexB]) * 0.5f;
            Vector3 midPointBC = (originalVertices[vertexB] + originalVertices[vertexC]) * 0.5f;
            Vector3 midPointCA = (originalVertices[vertexC] + originalVertices[vertexA]) * 0.5f;

            // Ajouter les nouveaux sommets à la liste des sommets subdivisés et récupérer leurs indices
            int midPointABIndex = vertexIndex++;
            int midPointBCIndex = vertexIndex++;
            int midPointCAIndex = vertexIndex++;

            // Ajouter les nouveaux triangles à la liste des triangles subdivisés en utilisant les indices des nouveaux sommets
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

        // Créer un nouveau maillage et appliquer les sommets et les triangles subdivisés
        Mesh subdividedMesh = new Mesh();
        subdividedMesh.vertices = subdividedVertices.ToArray();
        subdividedMesh.triangles = subdividedTriangles.ToArray();
        subdividedMesh.RecalculateNormals();
        subdividedMesh.RecalculateBounds();
        subdividedMesh.Optimize();

        // Application du maillage subdivisé au maillage de l'objet
        meshFilter.mesh = subdividedMesh;
    }

    /// <summary>
    /// lissage du mesh en déplaçant chaque sommet vers la position moyenne de ses sommets voisins.
    /// </summary>
    private void SmoothMesh()
    {
        // Récupérer le maillage et ses sommets et créer un nouveau tableau pour les sommets lissés
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] smoothedVertices = new Vector3[vertices.Length];

        // Boucle sur les sommets du maillage et récupère les sommets voisins et les déplace vers la position moyenne de leurs voisins
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 averagePosition = Vector3.zero;
            int neighborCount = 0;

 
            // boucle sur les sommets voisins 
            for (int j = 0; j < mesh.vertexCount; j++)
            {
                if (i == j)
                    continue;

                // Verifier la distance entre les sommets voisins et le sommet courant et si elle est inférieure à la valeur smoothingAmount
                //smoothing amount détermine a quel point on lisse le mesh
                float distance = Vector3.Distance(vertices[i], vertices[j]);
                if (distance <= smoothingAmount)
                {
                    averagePosition += vertices[j];
                    neighborCount++;
                }
            }

            // Recuperer la position moyenne des sommets voisins et déplacer le sommet courant vers cette position
            averagePosition /= neighborCount;
            smoothedVertices[i] = averagePosition;
        }

        // Définir les sommets lissés sur le maillage et recalculer ses normales
        mesh.vertices = smoothedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}