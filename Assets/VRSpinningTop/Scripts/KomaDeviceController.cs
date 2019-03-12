using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class KomaDeviceController : MonoBehaviour
{
    static SerialPort port;
    static int[] recvData;
    
    static Thread receivingThread;

    // Use this for initialization
    void Start()
    {
        if (recvData == null || recvData.Length == 0)
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
            catch (System.TimeoutException)
            {
                // do nothing
            }
            catch (System.Exception err)
            {
                print(err.ToString());
                break;
            }
        }
    }

    private void OnDestroy()
    {
    }

    public void OnApplicationQuit()
    {
        if (receivingThread != null)
            receivingThread.Abort();
        port.Close();
    }

    public void Connect(int portNr)
    {
        // if port is already open, just keep it
        if (port == null)
        {
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
