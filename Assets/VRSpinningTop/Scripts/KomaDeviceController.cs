using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class KomaDeviceController : MonoBehaviour
{
    static SerialPort port;
    static int[] recvData;
    
    Thread receivingThread;
    int cp;

    // Use this for initialization
    void Start()
    {
        recvData = new int[3];
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ReadThread()
    {
        int cp = 0;

        port.ReadTimeout = 1;

        while (true)
        {
            try
            {
                int c = port.ReadByte();
                if (c == 0)
                    cp = 0;
                else
                {
                    recvData[cp] = c;
                    cp = (cp + 1) % 3;
                }
            }
            catch (System.TimeoutException err)
            {
                // do nothing
            }
            catch (System.Exception err)
            {
                print(err.ToString());
                throw;
            }
        }
    }

    private void OnDestroy()
    {
        if (receivingThread != null)
            receivingThread.Abort();
        if (port != null)
            port.Close();
    }

    public void Connect(int portNr)
    {
        if (port != null)
            port.Close();


        port = new SerialPort();
        port.PortName = "COM" + portNr;
        port.BaudRate = 4800;
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
            Debug.Log(port.PortName + ": Koma Failed to open");
            return;
        }

        Debug.Log("Connected to Koma on Port " + portNr);

        receivingThread = new Thread(new ThreadStart(ReadThread));
        receivingThread.IsBackground = true;
        receivingThread.Start();
    }

    public Vector3 getAcceleration()
    {
        float ax = ((float)recvData[0] - 128) / 25.0f;
        float ay = ((float)recvData[1] - 128) / 25.0f;
        float az = ((float)recvData[2] - 128) / 25.0f;
        Vector3 result = new Vector3(ax, ay, az);

        //Debug.Log(recvData[0] + "/" + recvData[1] + "/" + recvData[2] +":" + result);

        return result;
    }
}
