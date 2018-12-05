using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningController : MonoBehaviour {
    public Vector3 speed;

	// Use this for initialization
	void Start () {
        speed = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            speed = new Vector3(0, 0, 5);
        }
	}
}
