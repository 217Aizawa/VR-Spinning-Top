using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinController : MonoBehaviour {

    public bool isThrown;
    public Vector3 velocity;
    public Vector3 Axis;

	// Use this for initialization
	void Start () {
        ResetSpin();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            velocity = new Vector3(0, 0, 5);
            isThrown = true;
        }
	}

    public void ResetSpin()
    {
        isThrown = false;
        velocity = Vector3.zero;
        Axis = Vector3.up;
    }
}
