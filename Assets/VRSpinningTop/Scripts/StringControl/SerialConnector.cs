using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // for RS-232C

public class SerialConnector : MonoBehaviour {

    SerialPort port;
    byte[] buf;

	// Use this for initialization
	void Start () {
        buf = new byte[1];
        Connect(3);	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Alpha1))
            SendChar('+');
        if (Input.GetKey(KeyCode.Minus))
            SendChar('-');
        if (Input.GetKey(KeyCode.Alpha0))
            SendChar('0');
	}

    private void OnDestroy()
    {
        if( port != null )
            port.Close();
    }

    public void Connect(int portNr)
    {
        if (port != null)
            port.Close();

        port = new SerialPort();
        port.PortName = "COM" + portNr;
        port.BaudRate = 19200;
        port.DataBits = 8;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.Handshake = Handshake.None;

        try
        {
            port.Open();
        }
        catch (System.Exception)
        {
            Debug.Log(port.PortName + ": Failed to open");
            throw;
        }

        Debug.Log(port.PortName + ": Connected");
    }

    private void SendChar(char c)
    {
        buf[0] = (byte)c;
        port.Write(buf, 0, 1);
    }
    
}


