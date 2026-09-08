using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatBox.Server.Data;
using ChatBox.Server.Models;
using ChatBox.Shared.Constants;
using ChatBox.Shared.DTOs;
using ChatBox.Shared.Protocol;

namespace ChatBox.Server.Services
{
    /// <summary>
    /// TCP Server xử lý nhiều client đồng thời.
    /// Mỗi client kết nối → chạy trong Task riêng (async).
    /// </summary>
    public class TcpServerService : ITcpServerService
    {
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private readonly ConcurrentDictionary<string, ConnectedClient> _clients;
        private readonly AuthService _authService;
        private readonly MessageRouter _messageRouter;
        private readonly MessageStore _messageStore;
        private int _connectionCounter;

        public bool IsRunning { get; private set; }
        public int ConnectedCount => _clients.Count;
        public int TotalMessagesRouted { get; private set; }
        public DateTime? StartTime { get; private set; }

        public event Action<string> OnLog;
        public event Action OnClientListChanged;

        public List<ConnectedClient> GetClientsSnapshot()
        {
            return new List<ConnectedClient>(_clients.Values);
        }

        public TcpServerService(UserStore userStore)
        {
            _clients = new ConcurrentDictionary<string, ConnectedClient>();
            _authService = new AuthService(userStore);
            _messageRouter = new MessageRouter(_clients, msg => OnLog?.Invoke(msg));
            _messageStore = new MessageStore();
        }

        /// <summary>
        /// Khởi động server trên port chỉ định
        /// </summary>
        public void Start(int port)
        {
            if (IsRunning) return;

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            IsRunning = true;
            StartTime = DateTime.Now;

            Log($"Server started on port {port}");

            // Accept connections in background
            Task.Run(() => AcceptClientsAsync(_cts.Token));
        }

        /// <summary>
        /// Stop server and disconnect all clients
        /// </summary>
        public void Stop()
        {
            if (!IsRunning) return;

            _cts?.Cancel();
            IsRunning = false;

            // Disconnect all clients
            foreach (var kvp in _clients)
            {
                try
                {
                    kvp.Value.TcpClient?.Close();
                }
                catch { }
            }
            _clients.Clear();

            try
            {
                _listener?.Stop();
            }
            catch { }

            Log("Server stopped");
            OnClientListChanged?.Invoke();
        }

        /// <summary>
        /// Asynchronously accept new clients
        /// </summary>
        private async Task AcceptClientsAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    var connectionId = "conn_" + Interlocked.Increment(ref _connectionCounter);

                    var client = new ConnectedClient
                    {
                        TcpClient = tcpClient,
                        Stream = tcpClient.GetStream(),
                        UserId = connectionId
                    };

                    _clients.TryAdd(connectionId, client);
                    Log($"[CONNECT] New client connected: {client.EndPoint} (ID: {connectionId})");
                    OnClientListChanged?.Invoke();

                    // Xử lý client trong Task riêng
                    var _ = Task.Run(() => HandleClientAsync(connectionId, client, ct));
                }
                catch (ObjectDisposedException)
                {
                    break; // Server stopped
                }
                catch (Exception ex)
                {
                    if (!ct.IsCancellationRequested)
                        Log($"[ERROR] Accept failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Xử lý 1 client: đọc packet và route
        /// </summary>
        private async Task HandleClientAsync(string connectionId, ConnectedClient client, CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested && client.TcpClient.Connected)
                {
                    var packet = await Task.Run(() => PacketSerializer.ReceivePacket(client.Stream));
                    if (packet == null)
                        break; // Client disconnected

                    ProcessPacket(connectionId, client, packet);
                }
            }
            catch (Exception ex)
            {
                if (!ct.IsCancellationRequested)
                    Log($"[ERROR] Client {connectionId}: {ex.Message}");
            }
            finally
            {
                // Dùng client.UserId thay vì connectionId cũ 
                // (vì HandleLogin đổi key từ conn_X → userId)
                DisconnectClient(client.UserId ?? connectionId);
            }
        }

        /// <summary>
        /// Xử lý packet nhận được từ client
        /// </summary>
        private void ProcessPacket(string connectionId, ConnectedClient client, Packet packet)
        {
            client.PacketsReceived++;
            TotalMessagesRouted++;

            switch (packet.Type)
            {
                case PacketType.Login:
                    HandleLogin(connectionId, client, packet);
                    break;

                case PacketType.Register:
                    HandleRegister(connectionId, client, packet);
                    break;

                case PacketType.Message:
                case PacketType.GroupMessage:
                    // Forward + lưu lịch sử
                    HandleForward(client, packet);
                    SaveMessageToStore(client, packet);
                    break;

                case PacketType.TypingIndicator:
                case PacketType.FileHeader:
                case PacketType.FileChunk:
                case PacketType.FileComplete:
                case PacketType.KeyExchange:
                case PacketType.KeyExchangeResponse:
                case PacketType.VideoCallRequest:
                case PacketType.VideoCallAccept:
                case PacketType.VideoCallReject:
                case PacketType.VideoCallEnd:
                case PacketType.VideoFrame:
                case PacketType.AudioFrame:
                    // Forward only (không lưu store)
                    HandleForward(client, packet);
                    break;

                case PacketType.ChatHistoryRequest:
                    HandleChatHistoryRequest(connectionId, client, packet);
                    break;

                case PacketType.Heartbeat:
                    // Client yêu cầu cập nhật danh sách user
                    if (client.IsAuthenticated)
                    {
                        _messageRouter.BroadcastUserList();
                    }
                    break;

                case PacketType.Disconnect:
                    DisconnectClient(connectionId);
                    break;

                default:
                    Log($"[WARN] Unknown packet type: {packet.Type} from {connectionId}");
                    break;
            }
        }

        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        private void HandleLogin(string connectionId, ConnectedClient client, Packet packet)
        {
            var request = PacketSerializer.FromJson<LoginRequestDTO>(packet.Data) ?? new LoginRequestDTO();
            var response = _authService.Authenticate(request);

            if (response.Success)
            {
                // Update client info
                ConnectedClient removed;
                _clients.TryRemove(connectionId, out removed);

                client.UserId = response.UserId;
                client.Username = request.Username;
                client.DisplayName = response.DisplayName;
                client.IsAuthenticated = true;

                _clients.TryAdd(response.UserId, client);

                Log($"[LOGIN] User '{request.Username}' authenticated successfully (ID: {response.UserId})");
            }
            else
            {
                Log($"[LOGIN FAILED] {request.Username}: {response.Message}");
            }

            // Send response back to client
            var responseData = PacketSerializer.ToJson(response);
            var responsePacket = new Packet(PacketType.LoginResponse, "server", connectionId, responseData);
            PacketSerializer.SendPacket(client.Stream, responsePacket);
            client.PacketsSent++;

            if (response.Success)
            {
                // Brief delay to allow client to open frmChat and subscribe to events
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    _messageRouter.BroadcastUserList();
                });
                OnClientListChanged?.Invoke();
            }
        }

        /// <summary>
        /// Handle registration
        /// </summary>
        private void HandleRegister(string connectionId, ConnectedClient client, Packet packet)
        {
            var request = PacketSerializer.FromJson<LoginRequestDTO>(packet.Data) ?? new LoginRequestDTO();
            var response = _authService.Register(request);
            var responseData = PacketSerializer.ToJson(response);

            var responsePacket = new Packet(PacketType.RegisterResponse, "server", connectionId, responseData);
            PacketSerializer.SendPacket(client.Stream, responsePacket);
            client.PacketsSent++;

            Log($"[REGISTER] {request.Username}: {response.Message}");
        }

        /// <summary>
        /// Forward packet from sender to receiver
        /// </summary>
        private void HandleForward(ConnectedClient sender, Packet packet)
        {
            if (!sender.IsAuthenticated)
            {
                Log($"[WARN] Unauthenticated client tried to send {packet.Type}");
                return;
            }

            // Set sender info
            packet.SenderId = sender.UserId;

            if (string.IsNullOrEmpty(packet.ReceiverId))
            {
                // Broadcast
                _messageRouter.Broadcast(packet, sender.UserId);
            }
            else
            {
                // Point-to-point
                _messageRouter.SendToClient(packet.ReceiverId, packet);
            }
        }

        /// <summary>
        /// Disconnect client (public for admin Kick support)
        /// </summary>
        public void DisconnectClient(string connectionId)
        {
            ConnectedClient client;
            if (_clients.TryRemove(connectionId, out client))
            {
                try { client.TcpClient?.Close(); } catch { }

                Log($"[DISCONNECT] Client disconnected: {client.DisplayName ?? client.Username ?? connectionId}");

                if (client.IsAuthenticated)
                {
                    _messageRouter.BroadcastUserList();
                }

                OnClientListChanged?.Invoke();
            }
        }

        private void Log(string message)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        /// <summary>
        /// Save message to store
        /// </summary>
        private void SaveMessageToStore(ConnectedClient sender, Packet packet)
        {
            if (packet.Type == PacketType.Message || packet.Type == PacketType.GroupMessage)
            {
                var msg = PacketSerializer.FromJson<MessageDTO>(packet.Data);
                string content = msg?.Content;
                if (string.IsNullOrEmpty(content)) return;

                string receiverId = packet.Type == PacketType.GroupMessage ? "__group__" : packet.ReceiverId;
                _messageStore.SaveMessage(packet.SenderId, receiverId, sender.DisplayName ?? packet.SenderId, content, false);
            }
        }

        /// <summary>
        /// Handle history request
        /// </summary>
        private void HandleChatHistoryRequest(string connectionId, ConnectedClient client, Packet packet)
        {
            if (!client.IsAuthenticated) return;

            string partnerId = packet.ReceiverId ?? "__group__";
            int maxCount = 50;

            var history = _messageStore.GetHistory(client.UserId, partnerId, maxCount);
            var historyPayload = new System.Collections.Generic.Dictionary<string, object>
            {
                { "PartnerId", partnerId },
                { "Messages", history }
            };

            var response = new Packet(PacketType.ChatHistoryResponse, "SERVER", client.UserId, PacketSerializer.ToJson(historyPayload));
            PacketSerializer.SendPacket(client.Stream, response);
            client.PacketsSent++;
            Log($"Sent {history.Count} history messages to {client.DisplayName} (partner: {partnerId})");
        }
    }
}
