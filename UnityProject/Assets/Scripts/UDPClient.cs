using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using UnityEngine.UI;

public class UDPClient : MonoBehaviour {

    #region Private Fields

    private IPEndPoint serverEndPoint;
    private UdpClient client;

    #endregion

    public int Port;
    public UnityEvent ConnectedCallback;
    public UnityEvent ConnectionFailed;

    public float Timeout = 5.0f;

    // Use this for initialization
    void Start () {
        client = new UdpClient();
        client.Client.Bind(new IPEndPoint(IPAddress.Any, Port + 1));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        client.Close();
    }

    public void SendEmptyMessage(UDPHost.PacketHeader.PacketTypes packetType)
    {
        var packet = new UDPHost.PacketHeader()
        {
            PacketType = packetType,
            Payload = 0
        };

        var buffer = packet.Encode();

        client.Send(buffer, buffer.Length, serverEndPoint);
    }

    public void Connect(string ipaddress)
    {
        try
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse(ipaddress), Port);

            var connectPacket = new UDPHost.PacketHeader()
            {
                PacketType = UDPHost.PacketHeader.PacketTypes.Announce,
                Payload = 0
            };

            var buffer = connectPacket.Encode();

            client.Send(buffer, buffer.Length, serverEndPoint);

            var connected = false;
            var startTime = DateTime.Now;

            do
            {
                if(client.Available == 0) continue;

                connected = true;
                client.Receive(ref serverEndPoint);
                
                // Send the packet
                var response = new UDPHost.PacketHeader()
                {
                    PacketType = UDPHost.PacketHeader.PacketTypes.PatientId,
                    Payload = 2744010
                };

                var responseBuffer = response.Encode();

                client.Send(responseBuffer, responseBuffer.Length, serverEndPoint);

            } while (!connected && DateTime.Now.Subtract(startTime).TotalSeconds < Timeout);

            if(connected)
                ConnectedCallback.Invoke();
            else
                ConnectionFailed.Invoke();

        }
        catch (Exception ex)
        {
            print(ex.Message);

            ConnectionFailed.Invoke();
        }
    }
}
