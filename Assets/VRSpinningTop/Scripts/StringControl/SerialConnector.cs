using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // for RS-232C
using System.Threading;

public class SerialConnector : MonoBehaviour {

    // 使い方：
    //      Connect(int portNr)
    //          portNr のシリアルポートに接続する。接続に成功したら、その旨を Debug Log に出力後、受信スレッドを起動してポートの監視を開始する。
    //      SendChar(char c)
    //          c をポートに送信する。チップ側で現在受け付けられるのは、＋（正回転）、ー（逆回転）、０（フリー停止）、＝（ブレーキ）
    //          注意：正回転・逆回転・ブレーキの間には、いちどフリーを送信することが強く望まれる
    //      public int rotationCount
    //          現在の回転ステップ数。別スレッドで自動的に更新されている。直接書き換え可能なので注意。（Editor で監視できるので、しばらくこのまま行く） 

    static SerialPort port;
    byte[] sendbuf;

    static public int rotationCount;
    Thread receivingThread;

	// Use this for initialization
	void Start () {
        sendbuf = new byte[1];
        rotationCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
	}

    private static void ReadThread()
    {
        port.ReadTimeout = 1;

        while( true )
        {
            if (port == null)
                break;

            try
            {
                char c = (char)port.ReadByte();
                switch (c)
                {
                    case 'a':
                        rotationCount--;
                        break;
                    case 'A':
                        rotationCount -= 10;
                        break;
                    case 'b':
                        rotationCount++;
                        break;
                    case 'B':
                        rotationCount += 10;
                        break;
                    case 'R':
                        rotationCount = 0;
                        break;
                }
            }
            catch(System.TimeoutException)
            {
                // do nothing
            }
            catch(System.InvalidOperationException)
            {
                // when port is closed
                break;
            }
            catch (System.Exception)
            {
                
                break;
            }
        }
    }

    private void OnDestroy()
    {
        if (receivingThread != null)
        {
            receivingThread.Abort();
            Debug.Log("Serial Connecter thread status: " + receivingThread.IsAlive);
        }
        if (port != null)
        {
            Debug.Log("Serial Connector port status: " + port.IsOpen);
            // not closing here, keep the static connection
        }
    }

    public void OnApplicationQuit()
    {
        port.Close();
        // when application closes in standalone mode, close it
    }

    public void Connect(int portNr)
    {
        if (port != null)
        {
            Debug.Log("Avoiding Reinitializaton of COM port");
            return;
        }

        Debug.Log("Start connecting to port:" + portNr);

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
        catch (System.Exception e)
        {
            Debug.LogException(e);
            Debug.Log(port.PortName + ": Failed to open");
            return;
        }

        Debug.Log(port.PortName + port.IsOpen);

        receivingThread = new Thread(new ThreadStart(ReadThread));
        receivingThread.IsBackground = true;
        receivingThread.Start();
    }

    public void SendChar(char c)
    {
        if (port == null || !port.IsOpen)
            return;
        sendbuf[0] = (byte)c;
        port.Write(sendbuf, 0, 1);
    }

    public int GetRotationCount()
    {
        return rotationCount;
    }

    public void SetRotationCount(int rc)
    {
        rotationCount = rc;
    }

}
