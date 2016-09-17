using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Schema;
using UnityEngine.UI;

public class UDPHost : MonoBehaviour
{
    public Text StatusText;

    

    public struct PacketHeader
    {
        public PacketTypes id;
        public int length;

        public enum PacketTypes
        {
            Announce = 0,
            Response = 1
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

    Thread rxThread;
    bool running = true;

    #endregion

    public int port;

	// Use this for initialization
	void Start () {
        rxThread = new Thread(RXStart);
	    rxThread.IsBackground = true;
        rxThread.Start();
	}

    void OnDestroy()
    {
        running = false;
        rxThread.Join(1000);
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
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // Main loop
        while (running)
        {
            // Only try to get packets if they're available
            if (udpClient.Available > 0)
            {
                // Get the packet bytes
                var buffer = udpClient.Receive(ref remoteEndPoint);
                var header = PacketHeader.Decode(buffer);

                switch (header.id)
                {
                    case PacketHeader.PacketTypes.Announce:
                        // Send back an announcement
                        var newPacket = new PacketHeader()
                        {
                            id = PacketHeader.PacketTypes.Response,
                            length = 0
                        };
                        var data = newPacket.Encode();
                        udpClient.Send(data, data.Length, new IPEndPoint(remoteEndPoint.Address, port));

                        StatusText.text = string.Format("Connected to - {0}", remoteEndPoint.Address);
                        break;
                }
            }
        }

        // At the end of the program, close the client
        udpClient.Close();
    }
    
    // Update is called once per frame
	void Update () {
	    
	}
}
