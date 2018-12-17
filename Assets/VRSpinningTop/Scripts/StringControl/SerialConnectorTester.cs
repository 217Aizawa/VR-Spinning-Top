using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialConnectorTester : MonoBehaviour {

    SerialConnector serial;

	// Use this for initialization
	void Start () {
        serial = gameObject.GetComponent<SerialConnector>();
        serial.Connect(3);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Alpha1))
            serial.SendChar('+');
        if (Input.GetKey(KeyCode.Minus))
            serial.SendChar('-');
        if (Input.GetKey(KeyCode.Alpha0))
            serial.SendChar('0');
    }
}
