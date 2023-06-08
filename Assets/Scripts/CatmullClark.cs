using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CatmullClark : MonoBehaviour
{

    [SerializeField] 
    public GameObject obj;

    private Mesh objMesh;

    private Vector3[] facePoints;
    private Vector3[] edgePoints;
    private Vector3[] vertPoints;

    private List<Vector3> guizmoPoints = new List<Vector3>();

    public void CalcFacePoints()
    {
        int[] triangles = objMesh.triangles;
        Vector3[] vertices = objMesh.vertices;

        facePoints = new Vector3[objMesh.triangles.Length/3];

        for (int i = 0; i < triangles.Length / 3; i++)
        {
            facePoints[i] = Centroid(new[]
            {
                vertices[triangles[i*3]],
                vertices[triangles[i*3+1]],
                vertices[triangles[i*3+2]]
            });
        }
        Debug.Log("Face Points done");
    }
    
    public void CalcEdgePoints()
    {
        int[] triangles = objMesh.triangles;
        Vector3[] vertices = objMesh.vertices;

        edgePoints = new Vector3[objMesh.triangles.Length];

        List<Vector3> pointsForEdgePoint = new List<Vector3>();

        for (int i = 0; i < triangles.Length / 3; i++)
        {
            List<Vector3[]> faceEdge = new List<Vector3[]>();
            faceEdge.Add(new []{vertices[triangles[i*3]], vertices[triangles[i*3+1]]});
            faceEdge.Add(new []{vertices[triangles[i*3+1]], vertices[triangles[i*3+2]]});
            faceEdge.Add(new []{vertices[triangles[i*3+2]], vertices[triangles[i*3]]});

            for(int j=0;j<3;j++)
            {
                pointsForEdgePoint.Clear();
                pointsForEdgePoint.Add(faceEdge[j][0]);
                pointsForEdgePoint.Add(faceEdge[j][1]);
                for (int k = 0; k < triangles.Length / 3; k++)
                {
                    Vector3[] faceVert = new[]
                    {
                        vertices[triangles[k*3]],
                        vertices[triangles[k*3+1]],
                        vertices[triangles[k*3+2]]
                    };
                    if (Array.Exists(faceVert, v => v == faceEdge[j][0]) && Array.Exists(faceVert, v => v == faceEdge[j][1]))
                    {
                        pointsForEdgePoint.Add(facePoints[k]);
                    }
                }

                edgePoints[i*3+j] = Centroid(pointsForEdgePoint.ToArray());
            }
        }
        Debug.Log("Edge Points done");

    }
    
    public void CalcVertexPoint()
    {
        int[] triangles = objMesh.triangles;
        Vector3[] vertices = objMesh.vertices;

        vertPoints = new Vector3[objMesh.vertices.Length];
        List<int> connectedFaceIndexes = new List<int>();
        List<Vector3[]> incidentEdges = new List<Vector3[]>();

        for(int i=0;i<vertices.Length;i++)
        {
            connectedFaceIndexes.Clear();
            incidentEdges.Clear();
            for (int j = 0; j < triangles.Length / 3; j++)
            {
                Vector3[] faceVert = new[]
                {
                    vertices[triangles[j*3]],
                    vertices[triangles[j*3+1]],
                    vertices[triangles[j*3+2]]
                };
                
                if (Array.Exists(faceVert, v => v == vertices[i]))
                {
                    connectedFaceIndexes.Add(j);
                    List<Vector3[]> faceEdge = new List<Vector3[]>();
                    faceEdge.Add(new []{vertices[triangles[j*3]], vertices[triangles[j*3 + 1]]});
                    faceEdge.Add(new []{vertices[triangles[j*3+1]], vertices[triangles[j*3 + 2]]});
                    faceEdge.Add(new []{vertices[triangles[j*3+2]], vertices[triangles[j*3]]});

                    foreach(var edge in faceEdge)
                    {
                        if (Array.Exists(edge, v => v == vertices[i]) && 
                            !incidentEdges.Exists(e => (e[0]==edge[0] || e[0]==edge[1]) && (e[1]==edge[0] || e[1]==edge[1])))
                        {
                            incidentEdges.Add(edge);
                        }
                    }
                }
            }
            
            int n = incidentEdges.Count;
            Vector3 Q = new Vector3();
            connectedFaceIndexes.ForEach(fId => Q += facePoints[fId]);
            Q /= connectedFaceIndexes.Count;

            Vector3 R = new Vector3();
            incidentEdges.ForEach(e => R += ((e[0]+e[1])*0.5f));
            R /= n;

            vertPoints[i] = (1 * Q/ n)  + (2 * R / n)  + ((n - 3) * vertices[i]/ n) ;
            Debug.Log("Q: "+Q+" R: "+R+" v:"+vertices[i]+" v': "+vertPoints[i]);
        }
        Debug.Log("Vertex Points done");
    }

    public Mesh CreateSubdivideMesh()
    {
        Mesh result = new Mesh();
        int[] triangles = objMesh.triangles;
        List<Vector3> newVert = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        for (int i = 0; i < triangles.Length / 3; i++)
        {
            // Get Vertex value
            Vector3 v0 = facePoints[i];
            Vector3 v1 = edgePoints[i*3+2];
            Vector3 v2 = vertPoints[triangles[i*3]];
            Vector3 v3 = edgePoints[i*3];
            Vector3 v4 = vertPoints[triangles[i*3+1]];
            Vector3 v5 = edgePoints[i*3+1];
            Vector3 v6 = vertPoints[triangles[i*3+2]];

            Vector3[] faceVert = new[] {v0, v1, v2, v3, v4, v5, v6};

            // Get Vertex index
            int[] vId = new int[7];
            vId[0] = newVert.Count;
            newVert.Add(v0);

            for (int j = 1; j < 7; j++)
            {
                vId[j] = newVert.IndexOf(faceVert[j]);
                if (vId[j] == -1)
                {
                    vId[j] = newVert.Count;
                    newVert.Add(faceVert[j]);
                }
            }
            
            //triangle 1 {v0,v1,v2}
            newTriangles.AddRange(new []{vId[0],vId[1],vId[2]});
            //triangle 2 {v0,v2,v3}
            newTriangles.AddRange(new []{vId[0],vId[2],vId[3]});
            //triangle 3 {v0,v3,v4}
            newTriangles.AddRange(new []{vId[0],vId[3],vId[4]});
            //triangle 4 {v0,v4,v5}
            newTriangles.AddRange(new []{vId[0],vId[4],vId[5]});
            //triangle 5 {v0,v5,v6}
            newTriangles.AddRange(new []{vId[0],vId[5],vId[6]});
            //triangle 6 {v0,v6,v1}
            newTriangles.AddRange(new []{vId[0],vId[6],vId[1]});
        }

        result.vertices = newVert.ToArray();
        result.triangles = newTriangles.ToArray();

        Debug.Log("Subdivide done");

        return result;
    }
    
    public static Vector3 Centroid(Vector3[] _vert)
    {
        Vector3 centroid = new Vector3();
        foreach (var v in _vert)
        {
            centroid += v;
        }

        centroid /= _vert.Length;
        return centroid;
    }

    public Mesh CatmullClarkAlgorithm(Mesh _mesh)
    {
        CalcFacePoints();
        CalcEdgePoints();
        CalcVertexPoint();
        return CreateSubdivideMesh();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        objMesh = obj.GetComponent<MeshFilter>().mesh;
        guizmoPoints = objMesh.vertices.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        objMesh = obj.GetComponent<MeshFilter>().mesh;
        if (Input.GetKeyDown("space"))
        {
            guizmoPoints.Clear();
            Mesh newMesh = CatmullClarkAlgorithm(objMesh);
            guizmoPoints = newMesh.vertices.ToList();
            objMesh.vertices = newMesh.vertices;
            objMesh.triangles = newMesh.triangles;
            objMesh.RecalculateBounds();
            objMesh.RecalculateNormals();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (guizmoPoints.Count > 0)
        {
            foreach (var v in guizmoPoints)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(v, 0.03f);    
            }
        }
    }
        
        
}
