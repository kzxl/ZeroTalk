using System;
using System.Net;
using System.Net.Sockets;

namespace ZeroTalk.Shared.Network
{
    /// <summary>
    /// Minimal STUN client (RFC 5389) - chỉ dùng Binding Request
    /// để discover public IP:Port qua NAT.
    /// </summary>
    public static class StunClient
    {
        // Public STUN servers
        private static readonly string[] StunServers = new[]
        {
            "stun.l.google.com",
            "stun1.l.google.com",
            "stun2.l.google.com"
        };
        private const int StunPort = 19302;
        private const int TimeoutMs = 3000;
        private const int MaxRetries = 2;

        // STUN message constants
        private const ushort BindingRequest = 0x0001;
        private const ushort BindingResponse = 0x0101;
        private const ushort MappedAddressAttr = 0x0001;
        private const ushort XorMappedAddressAttr = 0x0020;
        private const uint MagicCookie = 0x2112A442;

        /// <summary>
        /// Discover public endpoint (IP:Port) qua STUN server.
        /// Trả về null nếu thất bại.
        /// </summary>
        public static IPEndPoint GetPublicEndPoint(int localPort = 0)
        {
            foreach (var server in StunServers)
            {
                for (int retry = 0; retry < MaxRetries; retry++)
                {
                    try
                    {
                        var result = SendBindingRequest(server, StunPort, localPort);
                        if (result != null)
                            return result;
                    }
                    catch { }
                }
            }
            return null;
        }

        /// <summary>
        /// Discover public endpoint sử dụng UdpClient có sẵn (reuse socket).
        /// </summary>
        public static IPEndPoint GetPublicEndPoint(UdpClient udpClient)
        {
            foreach (var server in StunServers)
            {
                for (int retry = 0; retry < MaxRetries; retry++)
                {
                    try
                    {
                        var result = SendBindingRequest(udpClient, server, StunPort);
                        if (result != null)
                            return result;
                    }
                    catch { }
                }
            }
            return null;
        }

        private static IPEndPoint SendBindingRequest(string server, int port, int localPort)
        {
            using (var client = new UdpClient(localPort))
            {
                client.Client.ReceiveTimeout = TimeoutMs;
                return SendBindingRequest(client, server, port);
            }
        }

        private static IPEndPoint SendBindingRequest(UdpClient client, string server, int port)
        {
            client.Client.ReceiveTimeout = TimeoutMs;

            // Build STUN Binding Request
            //  0                   1                   2                   3
            //  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            // |0 0|     STUN Message Type     |         Message Length        |
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            // |                         Magic Cookie                          |
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            // |                     Transaction ID (96 bits)                   |
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

            var transactionId = new byte[12];
            new Random().NextBytes(transactionId);

            var request = new byte[20];
            // Message Type: Binding Request (0x0001)
            request[0] = 0x00; request[1] = 0x01;
            // Message Length: 0 (no attributes)
            request[2] = 0x00; request[3] = 0x00;
            // Magic Cookie
            request[4] = 0x21; request[5] = 0x12;
            request[6] = 0xA4; request[7] = 0x42;
            // Transaction ID
            Array.Copy(transactionId, 0, request, 8, 12);

            // Resolve server
            var serverEp = new IPEndPoint(Dns.GetHostAddresses(server)[0], port);
            client.Send(request, request.Length, serverEp);

            // Receive response
            var remoteEp = new IPEndPoint(IPAddress.Any, 0);
            var response = client.Receive(ref remoteEp);

            return ParseBindingResponse(response, transactionId);
        }

        internal static IPEndPoint ParseBindingResponse(byte[] data, byte[] transactionId)
        {
            if (data == null || data.Length < 20)
                return null;

            // Verify Binding Response
            ushort msgType = (ushort)((data[0] << 8) | data[1]);
            if (msgType != BindingResponse)
                return null;

            // Verify Magic Cookie
            uint cookie = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
            if (cookie != MagicCookie)
                return null;

            // Verify Transaction ID
            for (int i = 0; i < 12; i++)
            {
                if (data[8 + i] != transactionId[i])
                    return null;
            }

            ushort msgLength = (ushort)((data[2] << 8) | data[3]);
            int offset = 20;

            // Parse attributes
            while (offset + 4 <= 20 + msgLength)
            {
                ushort attrType = (ushort)((data[offset] << 8) | data[offset + 1]);
                ushort attrLength = (ushort)((data[offset + 2] << 8) | data[offset + 3]);
                offset += 4;

                if (attrType == XorMappedAddressAttr && attrLength >= 8)
                {
                    return ParseXorMappedAddress(data, offset);
                }
                else if (attrType == MappedAddressAttr && attrLength >= 8)
                {
                    return ParseMappedAddress(data, offset);
                }

                // Align to 4 bytes
                offset += attrLength;
                if (offset % 4 != 0)
                    offset += 4 - (offset % 4);
            }

            return null;
        }

        private static IPEndPoint ParseXorMappedAddress(byte[] data, int offset)
        {
            // Family: 0x01 = IPv4
            byte family = data[offset + 1];
            if (family != 0x01) return null; // Only IPv4

            // XOR Port with Magic Cookie high 16 bits
            ushort xorPort = (ushort)((data[offset + 2] << 8) | data[offset + 3]);
            int port = xorPort ^ 0x2112;

            // XOR IP with Magic Cookie
            byte[] ipBytes = new byte[4];
            ipBytes[0] = (byte)(data[offset + 4] ^ 0x21);
            ipBytes[1] = (byte)(data[offset + 5] ^ 0x12);
            ipBytes[2] = (byte)(data[offset + 6] ^ 0xA4);
            ipBytes[3] = (byte)(data[offset + 7] ^ 0x42);

            return new IPEndPoint(new IPAddress(ipBytes), port);
        }

        private static IPEndPoint ParseMappedAddress(byte[] data, int offset)
        {
            byte family = data[offset + 1];
            if (family != 0x01) return null;

            int port = (data[offset + 2] << 8) | data[offset + 3];
            byte[] ipBytes = new byte[] { data[offset + 4], data[offset + 5], data[offset + 6], data[offset + 7] };

            return new IPEndPoint(new IPAddress(ipBytes), port);
        }

        /// <summary>
        /// Lấy local IP address (không phải loopback)
        /// </summary>
        public static IPAddress GetLocalIPAddress()
        {
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    // Connect to a public IP (doesn't actually send data)
                    socket.Connect("8.8.8.8", 80);
                    return ((IPEndPoint)socket.LocalEndPoint).Address;
                }
            }
            catch
            {
                return IPAddress.Loopback;
            }
        }
    }
}
