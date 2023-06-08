////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
///////// This script is deprecated and is not used in the project./////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
public class MeshSubdivision : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int subdivisions = 1;

    private void Start()
    {
        if (meshFilter != null)
        {
            SubdivideMesh();
        }
    }

    private void SubdivideMesh()
    {
        Mesh originalMesh = meshFilter.mesh;

        for (int i = 0; i < subdivisions; i++)
        {
            originalMesh = Subdivide(originalMesh);
        }

        meshFilter.mesh = originalMesh;
    }

    private Mesh Subdivide(Mesh mesh)
    {
        Mesh subdividedMesh = new Mesh();

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int numTriangles = triangles.Length / 3;
        int numVertices = vertices.Length;

        Vector3[] newVertices = new Vector3[numVertices + numTriangles * 3];
        int[] newTriangles = new int[numTriangles * 4 * 3];

        // Copy original vertices
        for (int i = 0; i < numVertices; i++)
        {
            newVertices[i] = vertices[i];
        }

        int vertexIndex = numVertices;
        int triangleIndex = 0;

        for (int i = 0; i < numTriangles; i++)
        {
            int i1 = triangles[i * 3];
            int i2 = triangles[i * 3 + 1];
            int i3 = triangles[i * 3 + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            // Compute new edge points
            Vector3 e1 = (v1 + v2) / 2f;
            Vector3 e2 = (v2 + v3) / 2f;
            Vector3 e3 = (v3 + v1) / 2f;

            // Compute new vertex points
            Vector3 newV1 = (v1 * 3f + v2 * 3f + v3 * 2f) / 8f;
            Vector3 newV2 = (v2 * 3f + v3 * 3f + v1 * 2f) / 8f;
            Vector3 newV3 = (v3 * 3f + v1 * 3f + v2 * 2f) / 8f;

            // Add new vertices
            newVertices[vertexIndex++] = e1;
            newVertices[vertexIndex++] = e2;
            newVertices[vertexIndex++] = e3;
            newVertices[vertexIndex++] = newV1;
            newVertices[vertexIndex++] = newV2;
            newVertices[vertexIndex++] = newV3;

            // Create new triangles
            newTriangles[triangleIndex++] = i1;
            newTriangles[triangleIndex++] = i2;
            newTriangles[triangleIndex++] = i3;
            newTriangles[triangleIndex++] = vertexIndex - 3;
            newTriangles[triangleIndex++] = vertexIndex - 6;
            newTriangles[triangleIndex++] = vertexIndex - 5;
            newTriangles[triangleIndex++] = vertexIndex - 6;
            newTriangles[triangleIndex++] = vertexIndex - 4;
            newTriangles[triangleIndex++] = vertexIndex - 5;
            newTriangles[triangleIndex++] = vertexIndex - 4;
            newTriangles[triangleIndex++] = i1;
            newTriangles[triangleIndex++] = vertexIndex - 5;
            newTriangles[triangleIndex++] = vertexIndex - 3;
        }

        subdividedMesh.vertices = newVertices;
        subdividedMesh.triangles = newTriangles;

        return subdividedMesh;
    }
}
