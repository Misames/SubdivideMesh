using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Loop subdivision algorithm for meshes in Unity.
/// </summary>

public class LoopAlgo : MonoBehaviour
{
    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh originalMesh = meshFilter.mesh;
        Mesh newMesh = new Mesh();
        Mesh subMesh = new Mesh();

        //créer un nouveau point pour chaque face
        for (int i=0; i<originalMesh.triangles.Length; i+=3)
        {
            Vector3 v1 = originalMesh.vertices[originalMesh.triangles[i]];
            Vector3 v2 = originalMesh.vertices[originalMesh.triangles[i+1]];
            Vector3 v3 = originalMesh.vertices[originalMesh.triangles[i+2]];
            Vector3 facePoint = (v1 + v2 + v3) / 3f;
            subMesh.vertices[i] = facePoint;
        }

        //créer un nouveau point pour chaque arête
        for (int i = 0; i < originalMesh.vertexCount; i++)
        {
            Vector3 sum=Vector3.zero;
            int count = 0;
            for (int j = 0; j < originalMesh.triangles.Length; j+=3)
            {
                if (originalMesh.triangles[j] == i || originalMesh.triangles[j+1] == i || originalMesh.triangles[j+2] == i)
                {
                    Vector3 v1 = originalMesh.vertices[originalMesh.triangles[j]];
                    Vector3 v2 = originalMesh.vertices[originalMesh.triangles[j + 1]];
                    Vector3 v3 = originalMesh.vertices[originalMesh.triangles[j + 2]];
                    sum += (v1 + v2 + v3) / 3f;
                    count++;
                }
            }
            subMesh.vertices[originalMesh.triangles.Length/3 + i] = sum / count;
            
        }

        //calculer les positions pour chaque sommet
        for (int i=0; i<originalMesh.vertexCount; i++)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            for (int j = 0; j < originalMesh.triangles.Length; j += 3)
            {
                if (originalMesh.triangles[j] == i || originalMesh.triangles[j + 1] == i || originalMesh.triangles[j + 2] == i)
                {
                    Vector3 v1 = originalMesh.vertices[originalMesh.triangles[j]];
                    Vector3 v2 = originalMesh.vertices[originalMesh.triangles[j + 1]];
                    Vector3 v3 = originalMesh.vertices[originalMesh.triangles[j + 2]];
                    sum += (v1 + v2 + v3) / 3f;
                    count++;
                }
            }
            Vector3 edgeSum = Vector3.zero;
            for (int j = 0; j< originalMesh.vertexCount; j++)
            {
                if (i==j) continue;
                if (originalMesh.GetEdge(i , j) != -1)
                {
                    edgeSum += subMesh.vertices[originalMesh.triangles.Length / 3 + j];
                }
            }
            Vector3 newVertex = (subMesh.vertices[originalMesh.triangles.Length / 3 + i] * 2 + edgeSum / count * (count - 2)) / count;
            subMesh.vertices[originalMesh.triangles.Length / 3 * 2 + i] = newVertex;
        }

        //creer de nouvelles faces à partir des nouveaux points et des anciens sommets
        for (int i = 0 ; i<originalMesh.triangles.Length; i +=3)
        {
            int v1 = originalMesh.triangles[i];
            int v2 = originalMesh.triangles[i + 1];
            int v3 = originalMesh.triangles[i + 2];
            int v4 = originalMesh.triangles.Length / 3 + v1;
            int v5 = originalMesh.triangles.Length / 3 + v2;
            int v6 = originalMesh.triangles.Length / 3 + v3;
            int v7 = originalMesh.triangles.Length / 3 * 2 + v1;
            int v8 = originalMesh.triangles.Length / 3 * 2 + v2;
            int v9 = originalMesh.triangles.Length / 3 * 2 + v3;
            subMesh.triangles[i * 4] = v1;
            subMesh.triangles[i * 4 + 1] = v4;
            subMesh.triangles[i * 4 + 2] = v7;
            subMesh.triangles[i * 4 + 3] = v5;
            subMesh.triangles[i * 4 + 4] = v2;
            subMesh.triangles[i * 4 + 5] = v5;
            subMesh.triangles[i * 4 + 6] = v8;
            subMesh.triangles[i * 4 + 7] = v6;
            subMesh.triangles[i * 4 + 8] = v3;
            subMesh.triangles[i * 4 + 9] = v6;
            subMesh.triangles[i * 4 + 10] = v9;
            subMesh.triangles[i * 4 + 11] = v4;
            subMesh.triangles[i * 4 + 12] = v4;
            subMesh.triangles[i * 4 + 13] = v5;
            subMesh.triangles[i * 4 + 14] = v6;
            subMesh.triangles[i * 4 + 15] = v4;
            subMesh.triangles[i * 4 + 16] = v6;
            subMesh.triangles[i * 4 + 17] = v7;
            subMesh.triangles[i * 4 + 18] = v6;
            subMesh.triangles[i * 4 + 19] = v8;
            subMesh.triangles[i * 4 + 20] = v9;
        }

        //ajouter les nouveaux points et les nouvelles faces au maillage subdivisé
        subMesh.vertices = subMesh.vertices.Concat(new Vector3[originalMesh.vertexCount*2]).ToArray();
        subMesh.triangles = subMesh.triangles.Concat(new int[originalMesh.triangles.Length*4]).ToArray();

        //assigner le maillage subdivisé au maillage du meshFilter
        meshFilter.mesh = subMesh;
    }

   

    //Methode getEdge
    
}