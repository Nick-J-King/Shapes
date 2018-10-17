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
    public GameObject goFrame;

    public ShapeDodec dodec;

    // External game objects.
    public Light directionalLight;



    public CameraController mainCamController;
    public Camera mainCam;



    void Start()
    {

        dodec.ComputeGeometry();
    }


    //--------------------------------------------------------------------------------



    //--------------------------------------------------------------------------------


    // called per frame, before performing physics
    void FixedUpdate()
    {
    }


    // called per frame, before performing physics
    void Update()
    {
        //if (toggleAnimate.isOn)
        {
            goFrame.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 20.0f);
            dodec.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * 20.0f);
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




}
