using UnityEngine;
using System.Collections.Generic;

public class Coons : MonoBehaviour
{
    private List<Vector3> controlPoints = new List<Vector3>();
    private List<int> indices = new List<int>();

    private void Start()
    {
        CreateControlPoints();
        GetComponent<MeshFilter>().mesh = CreateMesh();
        Chaikin();
        UpdateMesh();
    }

    private void Chaikin(uint iteration = 5)
    {
        for (int i = 0; i < iteration; i++)
            controlPoints = ChaikinIteration(controlPoints);
    }

    private List<Vector3> ChaikinIteration(List<Vector3> points)
    {
        int numPoints = points.Count;
        List<Vector3> updatedPoints = new List<Vector3>(numPoints * 2 - 1);

        for (int i = 0; i < numPoints - 1; i++)
        {
            // Calculer les points de contrôle intermédiaires sur les segments
            Vector3 pointA = points[i];
            Vector3 pointB = points[i + 1];

            Vector3 updatedPointA = pointA + (pointB - pointA) * 0.25f;
            Vector3 updatedPointB = pointA + (pointB - pointA) * 0.75f;

            updatedPointA.z = pointA.z;
            updatedPointB.z = pointB.z;

            updatedPoints.Add(updatedPointA);
            updatedPoints.Add(updatedPointB);
        }

        // Ajouter le dernier point de contrôle d'origine
        updatedPoints.Add(points[numPoints - 1]);

        return updatedPoints;
    }

    private void UpdateMesh()
    {
        indices.Clear();
        int cuttingPoint = GetCuttingPoint();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            if (i == cuttingPoint - 1) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        // Relier les points de contrôle de C1 à C2 avec des segments
        for (int i = 0; i < cuttingPoint; i++)
        {
            indices.Add(i);
            indices.Add(i + cuttingPoint);
        }

        Mesh newMesh = new Mesh();
        newMesh.SetVertices(controlPoints);
        newMesh.SetIndices(indices, MeshTopology.Lines, 0);
        GetComponent<MeshFilter>().mesh = newMesh;
    }

    private void CreateControlPoints()
    {
        // Coordonnées pour la première courbe (C1)
        controlPoints.Add(new Vector3(0, 0, 0));
        controlPoints.Add(new Vector3(1, 1, 0));
        controlPoints.Add(new Vector3(2, 1, 0));
        controlPoints.Add(new Vector3(3, 0, 0));

        // Coordonnées pour la deuxième courbe (C2)
        controlPoints.Add(new Vector3(0, 0, 5));
        controlPoints.Add(new Vector3(1, 1, 5));
        controlPoints.Add(new Vector3(2, 1, 5));
        controlPoints.Add(new Vector3(3, 0, 5));
    }

    private Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        int cuttingPoint = GetCuttingPoint();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            if (i == cuttingPoint - 1) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        // Relier les points de contrôle de C1 à C2 avec des segments
        for (int i = 0; i < cuttingPoint; i++)
        {
            indices.Add(i);
            indices.Add(i + cuttingPoint);
        }

        mesh.SetVertices(controlPoints);
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        return mesh;
    }

    private int GetCuttingPoint()
    {
        int j = 0;
        while (controlPoints[j].z == 0) j++;
        return j;
    }
}
