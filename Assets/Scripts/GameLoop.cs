using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour {

    public SpinController spinController;
    public StringController stringController;
    public KinectController kinectController;
    public GameObject koma;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (spinController.isThrown)
        {
            koma.GetComponent<Rigidbody>().velocity = spinController.velocity;
        }
	}
}
