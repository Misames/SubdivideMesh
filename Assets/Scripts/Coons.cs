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
        List<Vector3> updatedPoints = new List<Vector3>();

        for (int i = 0; i < numPoints - 1; i++)
        {
            if (GetCuttingPoint() == i) continue;

            // Calculer les points de contrôle intermédiaires sur les segments
            Vector3 pointA = points[i];
            Vector3 pointB = points[i + 1];

            Vector3 updatedPointA = pointA + (pointB - pointA) * 0.25f;
            Vector3 updatedPointB = pointA + (pointB - pointA) * 0.75f;

            updatedPoints.Add(updatedPointA);
            updatedPoints.Add(updatedPointB);
        }

        return updatedPoints;
    }

    private void UpdateMesh()
    {
        indices.Clear();
        int cuttingPoint = GetCuttingPoint();

        // Relie chaque point pour former C1 et C2
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            if (i == cuttingPoint) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        // Relier les points de contrôle de C1 à C2 avec des segments
        for (int i = 0; i <= cuttingPoint; i++)
        {
            indices.Add(i);
            indices.Add(i + cuttingPoint + 1);
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
        controlPoints.Add(new Vector3(0 - 1, 0, 5));
        controlPoints.Add(new Vector3(1 - 1, 1, 5));
        controlPoints.Add(new Vector3(2 - 1, 1, 5));
        controlPoints.Add(new Vector3(3 - 1, 0, 5));
    }

    private Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        int cuttingPoint = GetCuttingPoint();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            if (i == cuttingPoint) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        mesh.SetVertices(controlPoints);
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        return mesh;
    }

    private int GetCuttingPoint()
    {
        int j = 0;
        while (controlPoints[j].z == 0) j++;
        return j - 1;
    }
}
