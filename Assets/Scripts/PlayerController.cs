using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CellData
{
    public int status;
    public Vector3 worldCoords;
}


// Player controller
public class PlayerController : MonoBehaviour
{
    public Material vertexMaterial;

    public GameObject goFrame;

    // External game objects.
    public Light directionalLight;

    // Internal cache for building meshes.
    public int[] myNumVerts;
    public int[] myNumTriangles;
    public List<Vector3>[] myVerts;
    public List<int>[] myTriangles;

    // Internal parameters.
    private float size;
    private float sizeOnTwo;

    bool displayVertices;
    float vertexSize;

    private float max;

    private float scale;

    // Mesh gameobjects.
    public GameObject mfMain;

    public MeshFilter[] mfSub;  // Point to the 14 "sub meshes"

    public MeshFilter mfMain0;
    public MeshFilter mfMain1;
    public MeshFilter mfMain2;
    public MeshFilter mfMain3;
    public MeshFilter mfMain4;
    public MeshFilter mfMain5;
    public MeshFilter mfMain6;
    public MeshFilter mfMain7;
    public MeshFilter mfMain8;
    public MeshFilter mfMain9;
    public MeshFilter mfMain10;
    public MeshFilter mfMain11;
    public MeshFilter mfMain12;
    public MeshFilter mfMain13;

    private int MAXTVERTS = 65530;

    // List of vertex spheres.
    private GameObject s;
    private ArrayList myList;


    public CameraController mainCamController;
    public Camera mainCam;



    // Phi and phi squared.
    float p;
    float p2;

    Vector3[] dodecVerts;       // Vertices of a dodecahedron.

    Vector3[] fullDodecVerts;   // Vertices of the dodecahedra at each of the 20 vertices.

    int[,] fullDodecVertFaces;
    //int[,] dodecCubeConnectors;


    void Start()
    {
        p = 1.618033988749894848204586834f;
        p2 = 2.61803398875f;

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

        for (int i = 0; i < 20; i++)
        {
            AddFullDodecVertVerts(i * 20, dodecVerts[i], 0.1f);
        }


        fullDodecVertFaces = new int[20 * 12, 5];     // Faces of the dodecahedra at each vertex.

        for (int i = 0; i < 20; i++)
        {
            AddFullDodecVertFaces(i * 12, i * 20);
        }

        //int[,] dodecCubeConnectors;


        // Create the array of meshes.
        mfSub = new MeshFilter[14];

        mfSub[0] = mfMain0;
        mfSub[1] = mfMain1;
        mfSub[2] = mfMain2;
        mfSub[3] = mfMain3;
        mfSub[4] = mfMain4;
        mfSub[5] = mfMain5;
        mfSub[6] = mfMain6;
        mfSub[7] = mfMain7;
        mfSub[8] = mfMain8;
        mfSub[9] = mfMain9;
        mfSub[10] = mfMain10;
        mfSub[11] = mfMain11;
        mfSub[12] = mfMain12;
        mfSub[13] = mfMain13;

        // Create the builder info for each of the meshes.
        myNumVerts = new int[14];
        myNumTriangles = new int[14];
        myVerts = new List<Vector3>[14];
        myTriangles = new List<int>[14];

        // Create the list of vertex spheres.
        myList = new ArrayList();
        scale = 1.0f;

        // Set the basic size of the figure to match the cube frame.
        size = 10.0f;               // Size of the "configuration cube".
        sizeOnTwo = size / 2.0f;    // Used to center the cube.

        //        GetParametersFromControls();
        //        SetLightFromControls();
        ComputeGeometry();
    }



    // We have the internal parameters set.
    // Now, compute the geometry of the figure.
    public void ComputeGeometry()
    {
        foreach (GameObject s in myList)
        {
            Destroy(s);
        }

        for (int i = 0; i < 14; i++)
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
                        fullDodecVerts[fullDodecVertFaces[i, 4]], 1);
        }


        // Now put the list of triangles in each mesh.
        for (int i = 0; i < 14; i++)
        {
            ProcessMesh(i);
        }


        //TextStatus.text = "F: " + nFullFlats.ToString() + " D:" + nFullDiagonals.ToString() + " C:" + nFullCorners.ToString() + " IO:" + nFullyInOrOut.ToString();
    }



    void AddFullDodecVertVerts(int index, Vector3 pos, float vertScale)
    {
        for (int i = 0; i < 20; i++)
        {
            fullDodecVerts[i + index] = dodecVerts[i] * vertScale + pos;
        }
    }

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

    void AddFullDodecVertFace(int faceIndex, int[] vertexIndices, int vertexOffset)
    {
        for (int i = 0; i < 5; i++)
        {
            fullDodecVertFaces[faceIndex, i] = vertexIndices[i] + vertexOffset;
        }
    }






    // called per frame, before performing physics
    void FixedUpdate()
    {
    }


    // called per frame, before performing physics
    void Update()
    {
        //if (toggleAnimate.isOn)
        {
            mfMain.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 10.0f);
        }
    }


    // Read the light controls, and update the directional light.
    public void SetLightFromControls()
    {
        /*
        float y = 360.0f - sliderLightAzimuth.value;
        float x = sliderLightElevation.value;
        float z = 0.0f;

        textTitleLightAzimuthElevation.text = "Azm: " + y.ToString() + " Ele: " + x.ToString();

        directionalLight.transform.localRotation = Quaternion.Euler(x, y, z);
        directionalLight.transform.localPosition = Vector3.zero;
        */
    }


    public void ResetAnimation()
    {
        //toggleAnimate.isOn = false;
        //mfMain.transform.localEulerAngles = Vector3.zero;
    }

    public void ResetCamera()
    {
        mainCamController.azimuthElevation.azimuth = 0;
        mainCamController.azimuthElevation.elevation = 0;
        mainCamController.SetCameraAzimuthElevation(mainCamController.azimuthElevation);
    }

    public void CountSubCellOccupancy()
    {

    }


    // Draw a vertex at the "zero surface", if applicable.
    public void DrawVertex(int xFull, int yFull, int zFull, float x0, float y0, float z0)
    {
        //if (CanFormTriangleVertex(xFull, yFull, zFull) == 0)
        {
            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.parent = mfMain.transform;

            s.transform.localPosition = new Vector3(x0, y0, z0);
            s.transform.localScale = new Vector3(scale, scale, scale);

            s.GetComponent<Renderer>().material = vertexMaterial;
            s.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            myList.Add(s);
        }
    }


    public void ResetMesh(int mesh)
    {
        myNumVerts[mesh] = 0;
        myNumTriangles[mesh] = 0;
        myVerts[mesh] = new List<Vector3>();
        myTriangles[mesh] = new List<int>();
        mfSub[mesh].mesh.Clear();
    }


    public void ProcessMesh(int mesh)
    {
        mfSub[mesh].mesh.vertices = myVerts[mesh].ToArray();
        mfSub[mesh].mesh.triangles = myTriangles[mesh].ToArray();
        mfSub[mesh].mesh.RecalculateBounds();
        mfSub[mesh].mesh.RecalculateNormals();
    }


    //
    // Utils for vectors
    //


    public Vector3Int MixVectors3Int(Vector3Int baseVector, Vector3Int dir1, int mag1, Vector3Int dir2, int mag2)
    {
        Vector3Int result = new Vector3Int(baseVector.x + dir1.x * mag1 + dir2.x * mag2, baseVector.y + dir1.y * mag1 + dir2.y * mag2, baseVector.z + dir1.z * mag1 + dir2.z * mag2);

        return result;
    }



    public void AddPentBoth(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int mesh)
    {
        if (myNumVerts[mesh] > MAXTVERTS) return;

        myVerts[mesh].Add(v0);
        myVerts[mesh].Add(v1);
        myVerts[mesh].Add(v2);
        myVerts[mesh].Add(v3);
        myVerts[mesh].Add(v4);

        myVerts[mesh].Add(v4);
        myVerts[mesh].Add(v3);
        myVerts[mesh].Add(v2);
        myVerts[mesh].Add(v1);
        myVerts[mesh].Add(v0);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3);
        myTriangles[mesh].Add(myNumVerts[mesh] + 4);

        // Other side;
        myTriangles[mesh].Add(myNumVerts[mesh] + 0 + 5);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1 + 5);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 5);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0 + 5);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 5);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3 + 5);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0 + 5);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3 + 5);
        myTriangles[mesh].Add(myNumVerts[mesh] + 4 + 5);

        myNumVerts[mesh] += 10;
    }


    public void AddQuadBoth(Vector3 v00, Vector3 v01, Vector3 v10, Vector3 v11, int mesh)
    {
        if (myNumVerts[mesh] > MAXTVERTS) return;

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v10);
        myVerts[mesh].Add(v01);
        myVerts[mesh].Add(v11);

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v10);
        myVerts[mesh].Add(v01);
        myVerts[mesh].Add(v11);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1);

        myTriangles[mesh].Add(myNumVerts[mesh] + 2);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1);

        // Other side;
        myTriangles[mesh].Add(myNumVerts[mesh] + 0 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 4);

        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1 + 4);
        myTriangles[mesh].Add(myNumVerts[mesh] + 3 + 4);

        myNumVerts[mesh] += 8;
    }


    public void AddTriangleBoth(Vector3 v00, Vector3 v01, Vector3 v10, int mesh)
    {
        if (myNumVerts[mesh] > MAXTVERTS) return;

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v01);
        myVerts[mesh].Add(v10);

        myVerts[mesh].Add(v00);
        myVerts[mesh].Add(v10);
        myVerts[mesh].Add(v01);

        myTriangles[mesh].Add(myNumVerts[mesh] + 0);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1);

        // Other side;
        myTriangles[mesh].Add(myNumVerts[mesh] + 0 + 3);
        myTriangles[mesh].Add(myNumVerts[mesh] + 2 + 3);
        myTriangles[mesh].Add(myNumVerts[mesh] + 1 + 3);

        myNumVerts[mesh] += 6;
    }

}
