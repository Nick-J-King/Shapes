﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ShapeDodecParameters
{
    public float masterScale;   // Scale of main shape.
    public float nodeScale;     // Scale of each vertex node.
}


public class ShapeDodec : MonoBehaviour
{
    public ShapeDodecParameters parameters;

    // Internal cache for building meshes.
    private int[] myNumVerts;
    private int[] myNumTriangles;
    private List<Vector3>[] myVerts;
    private List<int>[] myTriangles;

    // Mesh gameobjects.
    public GameObject meshMain;

    private MeshFilter[] meshes;  // Point to the "sub meshes"

    public MeshFilter mesh0;
    public MeshFilter mesh1;
    public MeshFilter mesh2;
    public MeshFilter mesh3;
    public MeshFilter mesh4;


    private Vector3[] fullDodecVerts;   // Vertices of the dodecahedra at each of the 20 vertices of the "master" dodecahedron.

    private int[,] fullDodecVertFaces;
    private int[,] dodecCubeConnectors;

    private int connectors = 15;


    // Called before any Start.
    // Initialise so this object can support ComputeGeometry in Start()

    private void Awake()
    {
        // Create the array of meshes.
        meshes = new MeshFilter[5];

        meshes[0] = mesh0;
        meshes[1] = mesh1;
        meshes[2] = mesh2;
        meshes[3] = mesh3;
        meshes[4] = mesh4;

        // Create the builder info for each of the meshes.
        myNumVerts = new int[5];
        myNumTriangles = new int[5];
        myVerts = new List<Vector3>[5];
        myTriangles = new List<int>[5];

        // Initialise intrinsic data.
        ComputeFaces();
    }


    // Update is called once per frame

    void Update ()
    {
		
	}


    // Compute the geometry according to the parameters.

    public void ComputeGeometry()
    {
        ComputeVertices();
        ConstructMesh();
    }


    // Compute the vertices.
    // These are the vertices of the dodecaheda moved to each vertex of the "master" dodecahedon.

    private void ComputeVertices()
    {
        float p;
        float p2;

        Vector3[] dodecVerts;
            // Vertices of a "unit" dodecahedron.

    
        p = 1.618033988749894848204586834f; // Phi
        p2 = 2.61803398875f;                // Phi squared.

        dodecVerts = new Vector3[20];

        dodecVerts[0] = new Vector3(p, -p, p);
        dodecVerts[1] = new Vector3(p, p, p);
        dodecVerts[2] = new Vector3(-p, p, p);
        dodecVerts[3] = new Vector3(-p, -p, p);
        dodecVerts[4] = new Vector3(p, -p, -p);
        dodecVerts[5] = new Vector3(p, p, -p);
        dodecVerts[6] = new Vector3(-p, p, -p);
        dodecVerts[7] = new Vector3(-p, -p, -p);
        dodecVerts[8] = new Vector3(0.0f, -1.0f, p2);
        dodecVerts[9] = new Vector3(0.0f, 1.0f, p2);
        dodecVerts[10] = new Vector3(0.0f, -1.0f, -p2);
        dodecVerts[11] = new Vector3(0.0f, 1.0f, -p2);
        dodecVerts[12] = new Vector3(p2, 0.0f, 1.0f);
        dodecVerts[13] = new Vector3(p2, 0.0f, -1.0f);
        dodecVerts[14] = new Vector3(-p2, 0.0f, 1.0f);
        dodecVerts[15] = new Vector3(-p2, 0.0f, -1.0f);
        dodecVerts[16] = new Vector3(1.0f, -p2, 0.0f);
        dodecVerts[17] = new Vector3(-1.0f, -p2, 0.0f);
        dodecVerts[18] = new Vector3(1.0f, p2, 0.0f);
        dodecVerts[19] = new Vector3(-1, p2, 0.0f);


        fullDodecVerts = new Vector3[20 * 20];
            // The vertices of the 20 dodecahedra (one for each vertex of the "master" dodecahron.

        float dodecScale = parameters.masterScale;
            // Scale of the "master" dodecahedron.

        float vertScale = parameters.nodeScale;
            // Scale of the dodecahedron at each point of the "master" dodecahedron.

        int index;
        Vector3 pos;

        for (int nDodec = 0; nDodec < 20; nDodec++)
        {
            index = nDodec * 20;    // Where to place the 20 vertices.

            pos = dodecVerts[nDodec] * dodecScale;  // Position of "master" dodecahedron vertices.

            // Place a copy of the unit dodecahedon, scaled and translated.

            for (int nVert = 0; nVert < 20; nVert++)
            {
                fullDodecVerts[nVert + index] = dodecVerts[nVert] * vertScale + pos;
            }
        }
    }


    // Compute the connectivity of the faces.
    // Only needs to be done once.

    private void ComputeFaces()
    {
        fullDodecVertFaces = new int[20 * 12, 5];
            // Faces of the dodecahedra at each vertex.

        for (int i = 0; i < 20; i++)
        {
            AddFullDodecVertFaces(i * 12, i * 20);
        }

        dodecCubeConnectors = new int[connectors * 8, 4];
            // Faces of the square prisms connecting each dodecahedron.

        SetDodecCubeConnectors();
    }


    // Add the pentagonal faces of a "node" dodecahedron.

    void AddFullDodecVertFaces(int faceIndex, int vertexOffset)
    {
        AddFullDodecVertFace(faceIndex, new int[] { 9, 1, 18, 19, 2 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 1, new int[] { 11, 6, 19, 18, 5 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 2, new int[] { 12, 13, 5, 18, 1 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 3, new int[] { 15, 14, 2, 19, 6 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 4, new int[] { 12, 1, 9, 8, 0 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 5, new int[] { 14, 3, 8, 9, 2 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 6, new int[] { 10, 11, 5, 13, 4 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 7, new int[] { 11, 10, 7, 15, 6 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 8, new int[] { 13, 12, 0, 16, 4 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 9, new int[] { 14, 15, 7, 17, 3 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 10, new int[] { 8, 3, 17, 16, 0 }, vertexOffset);
        AddFullDodecVertFace(faceIndex + 11, new int[] { 10, 4, 16, 17, 7 }, vertexOffset);
    }


    // Add all the pentagonal faces...

    void AddFullDodecVertFace(int faceIndex, int[] vertexIndices, int vertexOffset)
    {
        for (int i = 0; i < 5; i++)
        {
            fullDodecVertFaces[faceIndex, i] = vertexIndices[i] + vertexOffset;
        }
    }


    void SetDodecCubeConnectors()
    {
        SetDodecCubeConnectorPair(0, new int[] { 0, 1, 2, 3 }, new int[] { 4, 5, 6, 7 }, 260, 240, 300, 280);
        SetDodecCubeConnectorPair(1, new int[] { 0, 3, 7, 4 }, new int[] { 1, 2, 6, 5 }, 180, 160, 220, 200);
        SetDodecCubeConnectorPair(2, new int[] { 4, 5, 1, 0 }, new int[] { 7, 6, 2, 3 }, 380, 360, 340, 320);

        SetDodecCubeConnectorPair(3, new int[] { 12, 5, 19, 9 }, new int[] { 16, 10, 15, 3 }, 80, 260, 280, 40);
        SetDodecCubeConnectorPair(4, new int[] { 3, 16, 12, 9 }, new int[] { 15, 10, 5, 19 }, 360, 20, 140, 340);
        SetDodecCubeConnectorPair(5, new int[] { 16, 10, 5, 12 }, new int[] { 3, 15, 19, 9 }, 160, 0, 120, 220);

        SetDodecCubeConnectorPair(6, new int[] { 8, 12, 18, 2 }, new int[] { 17, 4, 11, 15 }, 120, 380, 320, 0);
        SetDodecCubeConnectorPair(7, new int[] { 17, 4, 12, 8 }, new int[] { 15, 11, 18, 2 }, 100, 260, 280, 60);
        SetDodecCubeConnectorPair(8, new int[] { 2, 15, 17, 8 }, new int[] { 18, 11, 4, 12 }, 20, 180, 200, 140);

        SetDodecCubeConnectorPair(9, new int[] { 14, 8, 1, 19 }, new int[] { 7, 16, 13, 11 }, 100, 360, 340, 60);
        SetDodecCubeConnectorPair(10, new int[] { 7, 16, 8, 14 }, new int[] { 11, 13, 1, 19 }, 240, 0, 120, 300);
        SetDodecCubeConnectorPair(11, new int[] { 19, 11, 7, 14 }, new int[] { 1, 13, 16, 8 }, 180, 40, 80, 200);

        SetDodecCubeConnectorPair(12, new int[] { 14, 9, 18, 6 }, new int[] { 17, 0, 13, 10 }, 240, 20, 140, 300);
        SetDodecCubeConnectorPair(13, new int[] { 17, 0, 9, 14 }, new int[] { 10, 13, 18, 6 }, 80, 320, 380, 40);
        SetDodecCubeConnectorPair(14, new int[] { 14, 6, 10, 17 }, new int[] { 9, 18, 13, 0 }, 100, 220, 160, 60);
    }


    void SetDodecCubeConnectorPair(int faceIndex, int[] sideA, int[] sideB, int vertexSetA1, int vertexSetB1, int vertexSetA2, int vertexSetB2)
    {
        SetDodecCubeConnector(faceIndex * 8 + 0, new int[] { sideA[0] + vertexSetA1, sideA[1] + vertexSetA1, sideB[1] + vertexSetB1, sideB[0] + vertexSetB1 });
        SetDodecCubeConnector(faceIndex * 8 + 1, new int[] { sideA[1] + vertexSetA1, sideA[2] + vertexSetA1, sideB[2] + vertexSetB1, sideB[1] + vertexSetB1 });
        SetDodecCubeConnector(faceIndex * 8 + 2, new int[] { sideA[2] + vertexSetA1, sideA[3] + vertexSetA1, sideB[3] + vertexSetB1, sideB[2] + vertexSetB1 });
        SetDodecCubeConnector(faceIndex * 8 + 3, new int[] { sideA[3] + vertexSetA1, sideA[0] + vertexSetA1, sideB[0] + vertexSetB1, sideB[3] + vertexSetB1 });

        SetDodecCubeConnector(faceIndex * 8 + 4, new int[] { sideA[0] + vertexSetA2, sideA[1] + vertexSetA2, sideB[1] + vertexSetB2, sideB[0] + vertexSetB2 });
        SetDodecCubeConnector(faceIndex * 8 + 5, new int[] { sideA[1] + vertexSetA2, sideA[2] + vertexSetA2, sideB[2] + vertexSetB2, sideB[1] + vertexSetB2 });
        SetDodecCubeConnector(faceIndex * 8 + 6, new int[] { sideA[2] + vertexSetA2, sideA[3] + vertexSetA2, sideB[3] + vertexSetB2, sideB[2] + vertexSetB2 });
        SetDodecCubeConnector(faceIndex * 8 + 7, new int[] { sideA[3] + vertexSetA2, sideA[0] + vertexSetA2, sideB[0] + vertexSetB2, sideB[3] + vertexSetB2 });
    }


    void SetDodecCubeConnector(int faceIndex, int[] vertexIndices)
    {
        for (int i = 0; i < 4; i++)
        {
            dodecCubeConnectors[faceIndex, i] = vertexIndices[i];
        }
    }


    //
    // Mesh utils.
    //

    public void ResetMesh(int mesh)
    {
        myNumVerts[mesh] = 0;
        myNumTriangles[mesh] = 0;
        myVerts[mesh] = new List<Vector3>();
        myTriangles[mesh] = new List<int>();
        meshes[mesh].mesh.Clear();
    }


    public void ProcessMesh(int mesh)
    {
        meshes[mesh].mesh.vertices = myVerts[mesh].ToArray();
        meshes[mesh].mesh.triangles = myTriangles[mesh].ToArray();
        meshes[mesh].mesh.RecalculateBounds();
        meshes[mesh].mesh.RecalculateNormals();
    }


    // We have the internal parameters set.
    // Now, compute the geometry of the figure.
    private void ConstructMesh()
    {
        for (int i = 0; i <= 4; i++)
        {
            ResetMesh(i);
        }

        // Construct the whole figure...
        for (int i = 0; i < 12 * 20; i++)
        {
            AddPentBoth(fullDodecVerts[fullDodecVertFaces[i, 0]],
                        fullDodecVerts[fullDodecVertFaces[i, 1]],
                        fullDodecVerts[fullDodecVertFaces[i, 2]],
                        fullDodecVerts[fullDodecVertFaces[i, 3]],
                        fullDodecVerts[fullDodecVertFaces[i, 4]], 1, 2);    // 1 Bright purple outside, 2 dim inside.
        }

        for (int i = 0; i < connectors * 8; i++)
        {
            AddQuadBoth(fullDodecVerts[dodecCubeConnectors[i, 0]],
                        fullDodecVerts[dodecCubeConnectors[i, 1]],
                        fullDodecVerts[dodecCubeConnectors[i, 2]],
                        fullDodecVerts[dodecCubeConnectors[i, 3]], 3, 4);   // 3 Bright yellow outside, 4 dim inside.
        }

        // Now put the list of triangles in each mesh.
        for (int i = 0; i <= 4; i++)
        {
            ProcessMesh(i);
        }
    }


    //
    // Utils for meshes
    //


    // Vertices are clockwise (from "outside").
    public void AddPentBoth(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int meshOutside, int meshInside)
    {
        //if (myNumVerts[mesh] > MAXTVERTS) return;

        myVerts[meshOutside].Add(v0);
        myVerts[meshOutside].Add(v1);
        myVerts[meshOutside].Add(v2);
        myVerts[meshOutside].Add(v3);
        myVerts[meshOutside].Add(v4);

        myVerts[meshInside].Add(v4);
        myVerts[meshInside].Add(v3);
        myVerts[meshInside].Add(v2);
        myVerts[meshInside].Add(v1);
        myVerts[meshInside].Add(v0);

        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 0);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 1);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 2);

        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 0);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 2);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 3);

        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 0);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 3);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 4);

        // Other side;
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 0);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 1);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 2);

        myTriangles[meshInside].Add(myNumVerts[meshInside] + 0);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 2);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 3);

        myTriangles[meshInside].Add(myNumVerts[meshInside] + 0);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 3);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 4);

        myNumVerts[meshOutside] += 5;
        myNumVerts[meshInside] += 5;
    }


    // Vertices are clockwise (from "outside").
    public void AddQuadBoth(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int meshOutside, int meshInside)
    {
        //if (myNumVerts[mesh] > MAXTVERTS) return;

        myVerts[meshOutside].Add(v0);
        myVerts[meshOutside].Add(v1);
        myVerts[meshOutside].Add(v2);
        myVerts[meshOutside].Add(v3);

        myVerts[meshInside].Add(v3);
        myVerts[meshInside].Add(v2);
        myVerts[meshInside].Add(v1);
        myVerts[meshInside].Add(v0);

        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 0);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 1);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 2);

        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 0);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 2);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 3);

        // Other side;
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 0);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 1);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 2);

        myTriangles[meshInside].Add(myNumVerts[meshInside] + 0);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 2);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 3);

        myNumVerts[meshOutside] += 4;
        myNumVerts[meshInside] += 4;
    }


    // Vertices are clockwise (from "outside").

    public void AddTriangleBoth(Vector3 v0, Vector3 v1, Vector3 v2, int meshOutside, int meshInside)
    {
        //if (myNumVerts[meshOutside] > MAXTVERTS) return;

        myVerts[meshOutside].Add(v0);
        myVerts[meshOutside].Add(v1);
        myVerts[meshOutside].Add(v2);

        myVerts[meshInside].Add(v2);
        myVerts[meshInside].Add(v1);
        myVerts[meshInside].Add(v0);

        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 0);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 1);
        myTriangles[meshOutside].Add(myNumVerts[meshOutside] + 2);

        // Other side;
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 0);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 1);
        myTriangles[meshInside].Add(myNumVerts[meshInside] + 2);

        myNumVerts[meshOutside] += 3;
        myNumVerts[meshInside] += 3;
    }
}
