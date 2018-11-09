
/*
 
    -----------------------
    UDP-Send
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
    // > gesendetes unter
    // 127.0.0.1 : 8050 empfangen
   
    // nc -lu 127.0.0.1 8050
 
        // todo: shutdown thread at the end
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSender : MonoBehaviour
{
    private static int localPort;

	public bool DEBUG = false;

    // prefs
    private string IP;  // define in init
	private IPAddress broadcast;
    public int port = 28080;  // define in init

	// sendout timer (to prevent too many packets)
	float timer;

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    string strMessage = "";

    // call it from shell (as program)
    private static void Main()
    {
        UDPSender sendObj = new UDPSender();
        sendObj.init();

        // testing via console
        // sendObj.inputFromConsole();

        // as server sending endless
        sendObj.sendEndless(" endless infos \n");

    }
    // start from unity3d
    public void Start()
    {
        init();
    }
	
	public void Update()
	{
		if (!DEBUG)
			return;

		timer += Time.deltaTime;
		if (timer > 1.0f)
		{
			Debug.Log("test");
			sendString("QTC");
			timer = 0;
		}
	}

    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(0, 0, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPSend-Data\n " + IP + ":" + port + " #\n"
                    + "shell> nc -lu   " + IP + ":" + port + " \n"
                , style);

        // ------------------------
        // send it
        // ------------------------
        strMessage = GUI.TextField(new Rect(0, 220, 140, 20), strMessage);
        if (GUI.Button(new Rect(0, 250, 40, 20), "send"))
        {
            sendString(strMessage + "\n");
        }
    }

    // init
    public void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

		var host = Dns.GetHostEntry(Dns.GetHostName());
		IP = "";

		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				Debug.Log("Found MyIP : " + ip.ToString());

				byte[] broadcastaddress = ip.GetAddressBytes();
				broadcastaddress[3] = 255;  // assume 24 bit mask
				if (IP.Length == 0)
				{
					broadcast = new IPAddress(broadcastaddress);
					IP = broadcast.ToString();
					Debug.Log("Broadcast to : " + IP);
				}

			}
		}

		// define
		// IP = "127.0.0.1";
        // defined on Editor
        // port = 8051;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        Debug.Log("Sending to " + IP + " : " + port);
        Debug.Log("Testing: nc -lu " + IP + " : " + port);

    }

    // inputFromConsole
    private void inputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();

                // Den Text zum Remote-Client senden.
                if (text != "")
                {

                    // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
                    byte[] data = Encoding.UTF8.GetBytes(text);

                    // Den Text zum Remote-Client senden.
                    client.Send(data, data.Length, remoteEndPoint);
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    public void sendString(string message)
    {
        try
        {
            //if (message != "")
            //{

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }


    // endless test
    private void sendEndless(string testStr)
    {
        do
        {
            sendString(testStr);


        }
        while (true);

    }

    void OnApplicationQuit()
    {
        client.Close();
    }

}
