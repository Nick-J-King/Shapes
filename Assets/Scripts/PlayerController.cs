using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


// Player controller
public class PlayerController : MonoBehaviour
{
    public FrameController frame;

    public PanelControlsController panelControls;

    public ShapeDodec dodec;

    public Light directionalLight;

    public CameraController mainCamController;



    void Start()
    {
        GetParametersFromControls();

        SetLightFromControls();

        dodec.ComputeGeometry();
    }


    // called per frame, before performing physics
    void FixedUpdate()
    {
    }


    // called per frame, before performing physics
    void Update()
    {
        if (panelControls.toggleAnimate.isOn)
        {
            frame.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 100.0f * panelControls.SliderAnimateSpeed.value);
            dodec.mfMain.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 100.0f * panelControls.SliderAnimateSpeed.value);
        }
    }


    // Read the light controls, and update the directional light.
    public void SetLightFromControls()
    {
        float y = 360.0f - panelControls.sliderLightAzimuth.value;
        float x = panelControls.sliderLightElevation.value;
        float z = 0.0f;

        panelControls.textTitleLightAzimuthElevation.text = "Azm: " + y.ToString() + " Ele: " + x.ToString();

        directionalLight.transform.localRotation = Quaternion.Euler(x, y, z);
        directionalLight.transform.localPosition = Vector3.zero;
    }


    void ComputeGeometryAndGetStats()
    {
        //frame.SetEdges(zeroTriangles.parameters.sliderFullInt, zeroTriangles.parameters.sliderFullInt5thEdge, zeroTriangles.parameters.nFullDivisions, zeroTriangles.parameters.dropdownEdgesInt + 3);
        //zeroTriangles.ComputeGeometry();

        //ZeroTriangleStats stats = zeroTriangles.GetStats();

        //panelStatus.SetStats(stats);
    }


    // A geometry control has changed.
    // Get the new parameters, and recompute the geometry.

    public void CheckGeometryControls()
    {
        bool changed = GetParametersFromControls();
        if (changed)
        {
            //ComputeGeometryAndGetStats();
            dodec.ComputeGeometry();
        }
    }


    // Read the geometry parameters from the controls,
    // and work out the internal parameters.
    public bool GetParametersFromControls()
    {
        bool changed = false;

        if (dodec.parameters.masterScale != panelControls.SliderMasterScale.value)
        {
            dodec.parameters.masterScale = panelControls.SliderMasterScale.value;
            changed = true;
        }

        if (dodec.parameters.nodeScale != panelControls.SliderNodeScale.value)
        {
            dodec.parameters.nodeScale = panelControls.SliderNodeScale.value;
            changed = true;
        }

        return changed;
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


    public void CheckFrameToggle()
    {
        frame.SetActive(panelControls.toggleFrame.isOn);
    }

}
