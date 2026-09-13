using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeroTalk.Shared.Protocol;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Quản lý TCP connection đến server.
    /// Nhận packet trong background thread, gửi packet thread-safe.
    /// </summary>
    public class TcpClientService : ITcpClientService
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;
        private readonly object _sendLock = new object();

        public bool IsConnected { get; private set; }

        public event Action<Packet> OnPacketReceived;
        public event Action OnDisconnected;

        /// <summary>
        /// Kết nối đến server
        /// </summary>
        public async Task<bool> ConnectAsync(string host, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(host, port);
                _stream = _client.GetStream();
                IsConnected = true;

                _cts = new CancellationTokenSource();
                var _ = Task.Run(() => ReceiveLoopAsync(_cts.Token));

                return true;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Ngắt kết nối
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
            _cts?.Cancel();

            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
        }

        /// <summary>
        /// Gửi packet (thread-safe)
        /// </summary>
        public void SendPacket(Packet packet)
        {
            if (!IsConnected || _stream == null) return;

            lock (_sendLock)
            {
                try
                {
                    PacketSerializer.SendPacket(_stream, packet);
                }
                catch
                {
                    IsConnected = false;
                    OnDisconnected?.Invoke();
                }
            }
        }

        /// <summary>
        /// Vòng lặp nhận packet từ server
        /// </summary>
        private async Task ReceiveLoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested && IsConnected)
                {
                    var packet = await Task.Run(() => PacketSerializer.ReceivePacket(_stream));
                    if (packet == null)
                        break;

                    OnPacketReceived?.Invoke(packet);
                }
            }
            catch
            {
                // Connection lost
            }
            finally
            {
                if (IsConnected)
                {
                    IsConnected = false;
                    OnDisconnected?.Invoke();
                }
            }
        }
    }
}
