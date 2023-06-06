using UnityEngine;
using System.Collections.Generic;

public class Coons : MonoBehaviour
{
    private Vector3[] cornerPoints = new Vector3[4];
    private List<Vector3> controlPoints = new List<Vector3>();
    private List<int> indices = new List<int>();

    private void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateCurve();
        Chaikin();
        UpdateMesh();
    }

    private void Chaikin(uint iteration = 5)
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

            updatedPointA.z = pointA.z;
            updatedPointB.z = pointB.z;

            updatedPoints.Add(updatedPointA);
            updatedPoints.Add(updatedPointB);
        }

        // Ajouter le dernier point de contrôle d'origine
        updatedPoints.Add(controlPoints[numPoints - 1]);

        return updatedPoints;
    }

    private void UpdateMesh()
    {
        int cuttingPoint = GetCuttingPoint();

        indices.Clear();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            if (i == cuttingPoint - 1) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        for (int j = 0; j < cuttingPoint; j++)
        {
            if (j <= cuttingPoint)
            {
                indices.Add(j);
                indices.Add(j + cuttingPoint);
            }
        }

        Mesh newMesh = new Mesh();
        newMesh.SetVertices(controlPoints);
        newMesh.SetIndices(indices, MeshTopology.Lines, 0);
        GetComponent<MeshFilter>().mesh = newMesh;
    }

    private Mesh CreateCurve(float z = 5)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 1, 0);
        vertices[2] = new Vector3(2, 1, 0);
        vertices[3] = new Vector3(3, 0, 0);
        vertices[4] = new Vector3(0, 0, z);
        vertices[5] = new Vector3(1, 1, z);
        vertices[6] = new Vector3(2, 1, z);
        vertices[7] = new Vector3(3, 0, z);

        cornerPoints[0] = vertices[0];
        cornerPoints[1] = vertices[3];
        cornerPoints[2] = vertices[4];
        cornerPoints[3] = vertices[7];

        foreach (Vector3 point in vertices)
            controlPoints.Add(point);

        for (int i = 0; i < vertices.Length - 1; i++)
        {
            if (i == 3) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        mesh.SetVertices(vertices);
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
