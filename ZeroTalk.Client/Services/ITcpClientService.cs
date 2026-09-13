using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeroTalk.Shared.Constants;
using ZeroTalk.Shared.Protocol;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Interface quản lý TCP connection đến server
    /// </summary>
    public interface ITcpClientService
    {
        /// <summary>Kết nối đến server</summary>
        Task<bool> ConnectAsync(string host, int port);

        /// <summary>Ngắt kết nối</summary>
        void Disconnect();

        /// <summary>Gửi packet</summary>
        void SendPacket(Packet packet);

        /// <summary>Có đang kết nối không</summary>
        bool IsConnected { get; }

        /// <summary>Event nhận packet</summary>
        event Action<Packet> OnPacketReceived;

        /// <summary>Event khi bị ngắt kết nối</summary>
        event Action OnDisconnected;
    }
}
