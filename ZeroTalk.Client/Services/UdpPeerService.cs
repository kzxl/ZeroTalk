using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeroTalk.Shared.Network;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Quản lý kết nối UDP P2P cho video call.
    /// Thực hiện hole punching và streaming video frames.
    /// </summary>
    public class UdpPeerService : IDisposable
    {
        private UdpClient _udpClient;
        private CancellationTokenSource _cts;
        private IPEndPoint _peerEndPoint;
        private bool _isConnected;
        private readonly object _lock = new object();

        /// <summary>Local UDP port đang lắng nghe</summary>
        public int LocalPort { get; private set; }

        /// <summary>Public endpoint (từ STUN)</summary>
        public IPEndPoint PublicEndPoint { get; private set; }

        /// <summary>Local endpoint</summary>
        public IPEndPoint LocalEndPoint { get; private set; }

        /// <summary>Đã kết nối P2P thành công chưa</summary>
        public bool IsConnected => _isConnected;

        /// <summary>Event nhận data từ peer</summary>
        public event Action<byte[]> OnDataReceived;

        /// <summary>Event khi P2P connection thành công</summary>
        public event Action OnConnected;

        /// <summary>
        /// Khởi tạo UDP socket và discover endpoints
        /// </summary>
        public void Initialize()
        {
            _udpClient = new UdpClient(0); // Random available port
            LocalPort = ((IPEndPoint)_udpClient.Client.LocalEndPoint).Port;

            // Discover local IP
            var localIp = StunClient.GetLocalIPAddress();
            LocalEndPoint = new IPEndPoint(localIp, LocalPort);

            // Discover public endpoint qua STUN (dùng cùng socket)
            PublicEndPoint = StunClient.GetPublicEndPoint(_udpClient);
        }

        /// <summary>
        /// Thực hiện UDP hole punching đến peer.
        /// Thử cả public và local endpoint, timeout 5 giây.
        /// </summary>
        public async Task<bool> HolePunchAsync(
            string peerPublicIp, int peerPublicPort,
            string peerLocalIp, int peerLocalPort)
        {
            _cts = new CancellationTokenSource();
            _isConnected = false;

            // Endpoints to try
            IPEndPoint publicEp = null;
            IPEndPoint localEp = null;

            if (!string.IsNullOrEmpty(peerPublicIp) && peerPublicPort > 0)
                publicEp = new IPEndPoint(IPAddress.Parse(peerPublicIp), peerPublicPort);

            if (!string.IsNullOrEmpty(peerLocalIp) && peerLocalPort > 0)
                localEp = new IPEndPoint(IPAddress.Parse(peerLocalIp), peerLocalPort);

            // Start listening for pings
            var listenTask = Task.Run(() => ListenForPing(_cts.Token));

            // Send pings to both endpoints
            var pingData = System.Text.Encoding.UTF8.GetBytes("CHATBOX_PING");
            var pongData = System.Text.Encoding.UTF8.GetBytes("CHATBOX_PONG");

            int attempts = 0;
            while (!_isConnected && attempts < 25) // 25 x 200ms = 5 seconds
            {
                try
                {
                    // Ping public endpoint
                    if (publicEp != null)
                        _udpClient.Send(pingData, pingData.Length, publicEp);

                    // Ping local endpoint
                    if (localEp != null)
                        _udpClient.Send(pingData, pingData.Length, localEp);
                }
                catch { }

                await Task.Delay(200);
                attempts++;
            }

            if (!_isConnected)
            {
                _cts.Cancel();
            }

            return _isConnected;
        }

        /// <summary>
        /// Lắng nghe ping từ peer, trả lời pong, thiết lập connection
        /// </summary>
        private void ListenForPing(CancellationToken ct)
        {
            var pongData = System.Text.Encoding.UTF8.GetBytes("CHATBOX_PONG");

            while (!ct.IsCancellationRequested && !_isConnected)
            {
                try
                {
                    _udpClient.Client.ReceiveTimeout = 500;
                    var remoteEp = new IPEndPoint(IPAddress.Any, 0);
                    var data = _udpClient.Receive(ref remoteEp);

                    var message = System.Text.Encoding.UTF8.GetString(data);

                    if (message == "CHATBOX_PING" || message == "CHATBOX_PONG")
                    {
                        _peerEndPoint = remoteEp;
                        _isConnected = true;

                        // Reply pong
                        if (message == "CHATBOX_PING")
                        {
                            _udpClient.Send(pongData, pongData.Length, remoteEp);
                        }

                        OnConnected?.Invoke();
                        break;
                    }
                }
                catch (SocketException)
                {
                    // Timeout, try again
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Bắt đầu nhận data từ peer (sau khi P2P connected)
        /// </summary>
        public void StartReceiving()
        {
            if (!_isConnected) return;

            _cts = new CancellationTokenSource();
            Task.Run(() => ReceiveLoop(_cts.Token));
        }

        private void ReceiveLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && _isConnected)
            {
                try
                {
                    _udpClient.Client.ReceiveTimeout = 5000;
                    var remoteEp = new IPEndPoint(IPAddress.Any, 0);
                    var data = _udpClient.Receive(ref remoteEp);

                    if (data.Length > 0)
                    {
                        OnDataReceived?.Invoke(data);
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                        continue;
                    break;
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gửi data đến peer qua UDP
        /// </summary>
        public void SendData(byte[] data)
        {
            if (!_isConnected || _peerEndPoint == null) return;

            lock (_lock)
            {
                try
                {
                    _udpClient.Send(data, data.Length, _peerEndPoint);
                }
                catch { }
            }
        }

        /// <summary>
        /// Dừng kết nối
        /// </summary>
        public void Stop()
        {
            _isConnected = false;
            _cts?.Cancel();
        }

        public void Dispose()
        {
            Stop();
            try { _udpClient?.Close(); } catch { }
        }
    }
}
