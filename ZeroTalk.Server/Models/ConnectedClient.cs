using System;
using System.Net.Sockets;

namespace ZeroTalk.Server.Models
{
    /// <summary>
    /// Đại diện cho 1 client đang kết nối đến server
    /// </summary>
    public class ConnectedClient
    {
        /// <summary>TCP client socket</summary>
        public TcpClient TcpClient { get; set; }

        /// <summary>Network stream cho TCP</summary>
        public NetworkStream Stream { get; set; }

        /// <summary>User ID sau khi authenticate</summary>
        public string UserId { get; set; }

        /// <summary>Username</summary>
        public string Username { get; set; }

        /// <summary>Display name</summary>
        public string DisplayName { get; set; }

        /// <summary>Đã xác thực chưa</summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>Thời gian kết nối</summary>
        public DateTime ConnectedAt { get; set; } = DateTime.Now;

        /// <summary>Số packet nhận được</summary>
        public int PacketsReceived { get; set; }

        /// <summary>Số packet đã gửi</summary>
        public int PacketsSent { get; set; }

        /// <summary>Endpoint address (IP:Port) để log</summary>
        public string EndPoint
        {
            get
            {
                try
                {
                    return TcpClient?.Client?.RemoteEndPoint?.ToString() ?? "Unknown";
                }
                catch
                {
                    return "Disconnected";
                }
            }
        }
    }
}
