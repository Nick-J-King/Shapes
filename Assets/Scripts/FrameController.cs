using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour {


    public GameObject goFrame;

    private bool active; // Whether to display.


    // Use this for initialization
    void Start ()
    {
        active = true;
	}
	

	// Update is called once per frame
	void Update () {
		
	}

    public void SetActive(bool activeIn)
    {
        active = activeIn;

        goFrame.SetActive(active);
    }

}
