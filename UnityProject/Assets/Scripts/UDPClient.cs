using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

public class UDPClient : MonoBehaviour {

    #region Private Fields

    private IPEndPoint serverEndPoint;
#endregion

    public int port;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Connect()
    {
        var client = new UdpClient();
        var endPoint = new IPEndPoint(IPAddress.Broadcast, port);

        var connectPacket = new UDPHost.PacketHeader()
        {
            id = UDPHost.PacketHeader.PacketTypes.Announce,
            length = 0
        };

        var buffer = connectPacket.Encode();

        client.Send(buffer, buffer.Length, endPoint);

        var result = client.Receive(ref endPoint);

        client.Close();
    }
}
