using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameController : MonoBehaviour {


    public GameObject frame;
    public TextMeshPro label;

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

        frame.SetActive(active);
        label.enabled = active;
    }
}
