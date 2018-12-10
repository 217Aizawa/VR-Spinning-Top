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

    SerialPort port;
    byte[] sendbuf;

    public int rotationCount;
    Thread receivingThread;

	// Use this for initialization
	void Start () {
        sendbuf = new byte[1];
        rotationCount = 0;
//        Connect(3); // just for test
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

    void ReadThread()
    {
        while( port != null && port.IsOpen )
        {
            char c = (char)port.ReadByte();
            switch(c)
            {
                case 'a':
                    rotationCount++;
                    break;
                case 'A':
                    rotationCount += 10;
                    break;
                case 'b':
                    rotationCount--;
                    break;
                case 'B':
                    rotationCount -= 10;
                    break;
                case 'R':
                    rotationCount = 0;
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        receivingThread.Abort();
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

        receivingThread = new Thread(ReadThread);
        receivingThread.Start();
    }

    public void SendChar(char c)
    {
        sendbuf[0] = (byte)c;
        port.Write(sendbuf, 0, 1);
    }
}
