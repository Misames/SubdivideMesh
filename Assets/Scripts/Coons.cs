using UnityEngine;
using System.Collections.Generic;

public class Coons : MonoBehaviour
{
    private List<Vector3> controlPoints;
    private Vector3[] updatedControlPoints;

    private void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateCurve();
    }

    private void Chaikin(uint nbIteration = 5)
    {
        for (int i = 0; i < nbIteration; i++)
        {
        }
    }

    private Mesh CreateCurve()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0); // Premier point
        vertices[1] = new Vector3(1, 1, 0); // Deuxième point
        vertices[2] = new Vector3(2, 1, 0); // Troisième point
        vertices[3] = new Vector3(3, 0, 0); // Quatrième point

        // Ajout des points
        foreach (var point in vertices)
            controlPoints.Add(point);

        int[] indices = new int[6];
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 1;
        indices[3] = 2;
        indices[4] = 2;
        indices[5] = 3;

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        return mesh;
    }
}
