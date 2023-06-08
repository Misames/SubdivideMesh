using UnityEngine;
using System.Collections.Generic;

public class Coons : MonoBehaviour
{
    private List<Vector3> controlPoints = new List<Vector3>();
    private List<int> indices = new List<int>();
    private List<Vector3> inPoints = new List<Vector3>();

    private void Start()
    {
        CreateControlPoints();
        GetComponent<MeshFilter>().mesh = CreateMesh();
        Chaikin();
        ShapeMesh();
    }

    private void Chaikin(uint iteration = 3)
    {
        for (int i = 0; i < iteration; i++)
            controlPoints = ChaikinIteration(controlPoints);
    }

    private List<Vector3> SubdivideLine(List<Vector3> interlignes)
    {
        int length = GetCuttingPoint();

        for (int i = 0; i <= length; i++)
        {
            for (int j = 1; j <= length; j++)
            {
                float step = (float)j / (length + 1);

                Vector3 startPoint = controlPoints[i];
                Vector3 endPoint = controlPoints[i + length + 1];

                float x = Mathf.Lerp(startPoint.x, endPoint.x, step);
                float y = Mathf.Lerp(startPoint.y, endPoint.y, step);
                float z = Mathf.Lerp(startPoint.z, endPoint.z, step);

                Vector3 newPoint = new Vector3(x, y, z);
                interlignes.Add(newPoint);
            }
        }

        return interlignes;
    }

    private List<Vector3> ChaikinIteration(List<Vector3> points)
    {
        int numPoints = points.Count;
        List<Vector3> updatedPoints = new List<Vector3>();

        for (int i = 0; i < numPoints - 1; i++)
        {
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


    private void ShapeObject()
    {
        foreach (var ligne in inPoints)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale /= 70;
            go.transform.position = ligne + this.transform.position;
            Instantiate<GameObject>(go);
        }
    }

    private void ShapeMesh()
    {
        indices.Clear();

        // Relie chaque point pour former C1 et C2
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            if (i == controlPoints.Count - 1) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        indices.Add(controlPoints.Count - 1);
        indices.Add(0);

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

        // C3
        controlPoints.Add(new Vector3(3, 1, 1));
        controlPoints.Add(new Vector3(3, 1, 2));

        // Coordonnées pour la deuxième courbe (C2)
        controlPoints.Add(new Vector3(3, 0, 3));
        controlPoints.Add(new Vector3(2, 1, 3));
        controlPoints.Add(new Vector3(1, 1, 3));
        controlPoints.Add(new Vector3(0, 0, 3));

        // C4
        controlPoints.Add(new Vector3(0, 1, 2));
        controlPoints.Add(new Vector3(0, 1, 1));
    }

    private Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        int cuttingPoint = GetCuttingPoint();

        for (int i = 0; i <= controlPoints.Count - 1; i++)
        {
            if (i == controlPoints.Count - 1) continue;
            indices.Add(i);
            indices.Add(i + 1);
        }

        indices.Add(controlPoints.Count - 1);
        indices.Add(0);

        mesh.SetVertices(controlPoints);
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        return mesh;
    }

    // Dernier point du segment avant de passer à l'autre
    private int GetCuttingPoint()
    {
        int j = 0;
        while (controlPoints[j].z == 0) j++;
        return j - 1;
    }
}
