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
    private float fullMax;

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

    Vector3[] dodecVerts;

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


    // A geometry control has changed.
    // Get the new parameters, and recompute the geometry.
    //    public void CheckGeometryControls()
    //    {
    //        bool changed = GetParametersFromControls();
    //        if (changed)
    //        {
    //            ComputeGeometry();
    //        }
    //    }

    /*
        // Read the geometry parameters from the controls,
        // and work out the internal parameters.
        public bool GetParametersFromControls()
        {
            bool changed = false;

            // Lattice divisions.
            if (nDivisions != (int)sliderDivisions.value)
            {
                nDivisions = (int)sliderDivisions.value;
                changed = true;
            }

            textDivisions.text = "Divisions: " + nDivisions.ToString();

            // Edges
            if (dropdownEdgesInt != DropdownEdges.value)
            {
                dropdownEdgesInt = DropdownEdges.value;
                changed = true;
            }

            // 4th edge
            float sliderFloat = slider4thEdge.value;
            int sliderInt = (int)(sliderFloat * (nDivisions + 1));
            if (sliderInt > nDivisions) sliderInt = nDivisions;

            // 5th edge
            float sliderFloat5thEdge = slider5thEdge.value;
            int sliderInt5thEdge = (int)(sliderFloat5thEdge * (nDivisions + 1));
            if (sliderInt5thEdge > nDivisions) sliderInt5thEdge = nDivisions;


            text4thEdge.text = "Edges: " + sliderInt.ToString() + " " + sliderInt5thEdge.ToString();

            // Vertices
            if (displayVertices != togglePoints.isOn)
            {
                displayVertices = togglePoints.isOn;
                changed = true;
            }

            if (vertexSize != sliderVertexSize.value)
            {
                vertexSize = sliderVertexSize.value;
                changed = true;
            }


            if (doClosure != toggleClosure.isOn)
            {
                doClosure = toggleClosure.isOn;
                changed = true;
            }


            // Internal parameters.
            nFullDivisions = nDivisions * 12;
            if (sliderFullInt != sliderInt * 12)
            {
                sliderFullInt = sliderInt * 12;
                changed = true;
            }
            if (sliderFullInt5thEdge != sliderInt5thEdge * 12)
            {
                sliderFullInt5thEdge = sliderInt5thEdge * 12;
                changed = true;
            }

            max = (float)nDivisions;
            fullMax = (float)nFullDivisions;

            scale = size / max * vertexSize + 0.05f;

            return changed;
        }


        public void CheckFrameToggle()
        {
            goFrame.SetActive(toggleFrame.isOn);
        }

        public void CheckFlipToggle()
        {
            if (toggleFlip.isOn)
            {
                mfMain.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
            }
            else
            {
                mfMain.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
        */

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


        for (int i = 0; i < 20; i++)
        {
            DoShape(dodecVerts[i], 0.2f);
        }

        // Now put the list of triangles in each mesh.
        for (int i = 0; i < 14; i++)
        {
            ProcessMesh(i);
        }


        //TextStatus.text = "F: " + nFullFlats.ToString() + " D:" + nFullDiagonals.ToString() + " C:" + nFullCorners.ToString() + " IO:" + nFullyInOrOut.ToString();
    }



    void DoShape(Vector3 pos, float scale)
    {
        Vector3[] v = new Vector3[20];
        for (int i = 0; i < 20; i++)
        {
            v[i] = dodecVerts[i] * scale + pos;
        }

        AddPentBoth(v[9], v[1], v[18], v[19], v[2], 1);
        AddPentBoth(v[11], v[6], v[19], v[18], v[5], 1);
        AddPentBoth(v[12], v[13], v[5], v[18], v[1], 1);
        AddPentBoth(v[15], v[14], v[2], v[19], v[6], 1);
        AddPentBoth(v[12], v[1], v[9], v[8], v[0], 1);
        AddPentBoth(v[14], v[3], v[8], v[9], v[2], 1);
        AddPentBoth(v[10], v[11], v[5], v[13], v[4], 1);
        AddPentBoth(v[11], v[10], v[7], v[15], v[6], 1);
        AddPentBoth(v[13], v[12], v[0], v[16], v[4], 1);
        AddPentBoth(v[14], v[15], v[7], v[17], v[3], 1);
        AddPentBoth(v[8], v[3], v[17], v[16], v[0], 1);
        AddPentBoth(v[10], v[4], v[16], v[17], v[7], 1);
    }

/*
    void DoShape(Vector3 pos, float scale)
    {
        AddPentBoth(v9, v1, v18, v19, v2, 1);
        AddPentBoth(v11, v6, v19, v18, v5, 1);
        AddPentBoth(v12, v13, v5, v18, v1, 1);
        AddPentBoth(v15, v14, v2, v19, v6, 1);
        AddPentBoth(v12, v1, v9, v8, v0, 1);
        AddPentBoth(v14, v3, v8, v9, v2, 1);
        AddPentBoth(v10, v11, v5, v13, v4, 1);
        AddPentBoth(v11, v10, v7, v15, v6, 1);
        AddPentBoth(v13, v12, v0, v16, v4, 1);
        AddPentBoth(v14, v15, v7, v17, v3, 1);
        AddPentBoth(v8, v3, v17, v16, v0, 1);
        AddPentBoth(v10, v4, v16, v17, v7, 1);
    }
*/

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

    // Convert a full integer coord to a float from 0 to 1.
    private float IntToFloat(int coord)
    {
        return (float)coord / fullMax;
    }


    // Convert float coord (0 - 1) into world coordinate.
    private float CubeToWorld(float coord)
    {
        return coord * size - sizeOnTwo;
    }


    private float GridToWorld(int coord)
    {
        return ((float)coord / fullMax) * size - sizeOnTwo;
    }


    public Vector3Int MixVectors3Int(Vector3Int baseVector, Vector3Int dir1, int mag1, Vector3Int dir2, int mag2)
    {
        Vector3Int result = new Vector3Int(baseVector.x + dir1.x * mag1 + dir2.x * mag2, baseVector.y + dir1.y * mag1 + dir2.y * mag2, baseVector.z + dir1.z * mag1 + dir2.z * mag2);

        return result;
    }


    public Vector3 IntVectorToWorld(Vector3Int intVector)
    {
        Vector3 result = new Vector3(GridToWorld(intVector.x), GridToWorld(intVector.y), GridToWorld(intVector.z));

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
