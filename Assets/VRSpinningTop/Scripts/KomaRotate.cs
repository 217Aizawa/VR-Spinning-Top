using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KomaRotate : MonoBehaviour {

    public Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        /*if(SpinController.isThrown == true)
        {
            rb.angularVelocity = Vector3.up * 3.14f;//πは180°/sec3.14
            //rb.AddTorque(Vector3.up * 3.14f);
        }*/
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F");
            rb.angularVelocity = Vector3.up * 3.14f;//πは180°/sec3.14
            //rb.AddTorque(Vector3.up * 3.14f);
        }
	}
}
