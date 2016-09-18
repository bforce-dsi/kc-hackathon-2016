using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Schema;
using Assets;
using UnityEngine.Events;
using Text = UnityEngine.UI.Text;

[Serializable]
public class AlergyEvent: UnityEvent<FhirWrapper> { }

public class UDPHost : MonoBehaviour
{
    public struct PacketHeader
    {
        public PacketTypes PacketType;
        public int Payload;

        public enum PacketTypes
        {
            Announce = 0,
            Response = 1,
            PatientId = 2,
            Allergy = 3,
            Distraction = 4,
            Welcome = 5,
            Facts = 6
        }

        public byte[] Encode()
        {
            var size = Marshal.SizeOf(this);
            var result = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, result, 0, size);
            Marshal.FreeHGlobal(ptr);
            return result;
        }

        /// <summary>
        /// Decodes the packet header from a byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static PacketHeader Decode(Byte[] buffer)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var result = (PacketHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(PacketHeader));
            handle.Free();
            return result;
        }
    }

    #region Private Fields

    Thread _rxThread;
    bool _running = true;

    FhirWrapper _patientData;

    IPEndPoint _remoteEndPoint;

    #endregion

    public int Port;

    public UnityEvent ConnectedEvent;
    public UnityEvent PlayDistraction;
    public UnityEvent PlayWelcome;
    public UnityEvent PlayFacts;
    public AlergyEvent PlayAlergy;

	// Use this for initialization
	void Start () {
        _packets = new Queue<PacketHeader>();
        _rxThread = new Thread(RXStart);
	    _rxThread.IsBackground = true;
        _rxThread.Start();
	}

    void OnDestroy()
    {
        _running = false;
        _rxThread.Join(1000);
    }

    /// <summary>
    /// Recieve thread main loop
    /// </summary>
    private void RXStart()
    {
        // Initialize the udp client
        var udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.ExclusiveAddressUse = false;

        // Bind
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));

        _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // Main loop
        while (_running)
        {
            // Only try to get _packets if they're available
            if (udpClient.Available > 0)
            {
                // Get the packet bytes
                var buffer = udpClient.Receive(ref _remoteEndPoint);
                var header = PacketHeader.Decode(buffer);

                if (header.PacketType == PacketHeader.PacketTypes.Announce)
                {
                    // Send back an announcement
                    var newPacket = new PacketHeader()
                    {
                        PacketType = PacketHeader.PacketTypes.Response,
                        Payload = 0
                    };
                    var data = newPacket.Encode();
                    udpClient.Send(data, data.Length, _remoteEndPoint);
                }
                else if (header.PacketType == PacketHeader.PacketTypes.PatientId)
                {
                    _patientData = new FhirWrapper(header.Payload.ToString());
                }

                _packets.Enqueue(header);
            }
        }

        // At the end of the program, close the client
        udpClient.Close();
    }

    private Queue<PacketHeader> _packets;
    
    // Update is called once per frame
	void Update () {

	    while (_packets.Count > 0)
	    {
	        var nextPacket = _packets.Dequeue();

	        switch (nextPacket.PacketType)
	        {
                case PacketHeader.PacketTypes.Announce:
                    ConnectedEvent.Invoke();
	                break;

                case PacketHeader.PacketTypes.Allergy:
                    PlayAlergy.Invoke(_patientData);
                    break;
                case PacketHeader.PacketTypes.Distraction:
                    PlayDistraction.Invoke();
                    break;
                case PacketHeader.PacketTypes.Welcome:
                    PlayWelcome.Invoke();
                    break;
                case PacketHeader.PacketTypes.Facts:
                    PlayFacts.Invoke();
                    break;
	        }
	    }

        Canvas.ForceUpdateCanvases();
	}
}
