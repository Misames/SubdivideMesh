using UnityEngine;
using System.Collections.Generic;

public class Coons : MonoBehaviour
{
    private List<Vector3> controlPoints = new List<Vector3>();
    private List<int> indices = new List<int>();

    private void Start()
    {
        // Courbe de base
        GetComponent<MeshFilter>().mesh = CreateCurve();
        // Ajout des subdivision
        Chainkin();
        // Affichage
        UpdateMesh();
    }

    private void Chainkin(uint iteration = 5)
    {
        for (int i = 0; i < 5; i++)
            controlPoints = ChaikinIteration();
    }

    private List<Vector3> ChaikinIteration()
    {
        int numPoints = controlPoints.Count;
        List<Vector3> updatedPoints = new List<Vector3>(numPoints * 2 - 1);

        for (int i = 0; i < numPoints - 1; i++)
        {
            // Calculer les controlPoints intermédiaires sur les segments
            Vector3 pointA = controlPoints[i];
            Vector3 pointB = controlPoints[i + 1];

            Vector3 updatedPointA = pointA + (pointB - pointA) * 0.25f;
            Vector3 updatedPointB = pointA + (pointB - pointA) * 0.75f;

            updatedPoints.Add(updatedPointA);
            updatedPoints.Add(updatedPointB);
        }

        // Ajouter le dernier point de contrôle d'origine
        updatedPoints.Add(controlPoints[numPoints - 1]);

        return updatedPoints;
    }

    private void UpdateMesh()
    {
        indices.Clear();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            indices.Add(i);
            indices.Add(i + 1);
        }

        Mesh newMesh = new Mesh();
        newMesh.SetVertices(controlPoints);
        newMesh.SetIndices(indices, MeshTopology.Lines, 0);
        GetComponent<MeshFilter>().mesh = newMesh;
    }

    private Mesh CreateCurve(float z = 0)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, z); // Premier point
        vertices[1] = new Vector3(1, 1, z); // Deuxième point
        vertices[2] = new Vector3(2, 1, z); // Troisième point
        vertices[3] = new Vector3(3, 0, z); // Quatrième point

        foreach (Vector3 point in vertices)
            controlPoints.Add(point);

        for (int i = 0; i < vertices.Length - 1; i++)
        {
            indices.Add(i);
            indices.Add(i + 1);
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        return mesh;
    }
}
