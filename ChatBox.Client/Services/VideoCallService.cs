using System;
using System.Threading.Tasks;
using ChatBox.Shared.Network;
using ChatBox.Shared.Protocol;

namespace ChatBox.Client.Services
{
    /// <summary>
    /// Quản lý video call: P2P UDP với STUN NAT traversal.
    /// Fallback về server relay (TCP) khi P2P thất bại.
    /// 
    /// Flow:
    /// 1. Discover public endpoint qua STUN
    /// 2. Gửi signaling (Request/Accept) qua TCP (kèm endpoint info)
    /// 3. UDP hole punching (thử cả public + local endpoint)
    /// 4. Nếu thành công → P2P streaming
    /// 5. Nếu thất bại → fallback TCP relay
    /// </summary>
    public class VideoCallService : IVideoCallService
    {
        private readonly TcpClientService _tcpService;
        private readonly ChatService _chatService;
        private UdpPeerService _udpPeer;

        public bool IsInCall { get; private set; }
        public string CurrentCallPartner { get; private set; }
        public string CurrentUserId => _chatService?.CurrentUserId;
        public bool IsP2PConnected => _udpPeer?.IsConnected ?? false;
        public bool UseRelay { get; private set; }
        public bool IsVideoEnabled { get; set; } = true;

        public event Action<string> OnIncomingCall;        // callerUserId
        public event Action OnCallAccepted;
        public event Action<string> OnCallRejected;        // reason
        public event Action OnCallEnded;
        public event Action<byte[]> OnVideoFrameReceived;
        public event Action<byte[]> OnLocalFrameCaptured;  // local camera / PIP
        public event Action<string> OnLog;                 // log message

        private IVideoSource _videoSource;

        // Lưu endpoint info của peer (nhận từ signaling)
        private string _peerPublicIp;
        private int _peerPublicPort;
        private string _peerLocalIp;
        private int _peerLocalPort;

        public VideoCallService(TcpClientService tcpService, ChatService chatService)
        {
            _tcpService = tcpService;
            _chatService = chatService;
        }

        /// <summary>
        /// Thay đổi nguồn phát video (Synthetic / Screen Share / Webcam)
        /// </summary>
        public void SetVideoSource(IVideoSource source)
        {
            if (_videoSource != null)
            {
                _videoSource.OnFrameCaptured -= HandleLocalFrameCaptured;
                _videoSource.Stop();
                _videoSource.Dispose();
            }

            _videoSource = source;
            if (_videoSource != null)
            {
                _videoSource.OnFrameCaptured += HandleLocalFrameCaptured;
                if (IsInCall)
                {
                    _videoSource.Start(320, 240, 15);
                }
            }
        }

        /// <summary>
        /// Bật/tắt truyền camera
        /// </summary>
        public void ToggleVideo(bool enabled)
        {
            IsVideoEnabled = enabled;
            Log(enabled ? "📷 Đã bật camera" : "📷 Đã tắt camera");
        }

        private void StartCapturing()
        {
            if (_videoSource == null)
            {
                _videoSource = new SyntheticVideoSource(_chatService.CurrentUserId);
                _videoSource.OnFrameCaptured += HandleLocalFrameCaptured;
            }

            if (!_videoSource.IsRunning)
            {
                _videoSource.Start(320, 240, 15);
                Log($"▶ Đã khởi động nguồn video: {_videoSource.SourceName}");
            }
        }

        private void StopCapturing()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.Stop();
                Log("⏹ Đã dừng nguồn phát video");
            }
        }

        private void HandleLocalFrameCaptured(byte[] frameData)
        {
            OnLocalFrameCaptured?.Invoke(frameData);

            if (IsInCall && IsVideoEnabled)
            {
                SendVideoFrame(frameData);
            }
        }

        /// <summary>
        /// Gửi yêu cầu gọi video. Discover endpoint qua STUN trước.
        /// </summary>
        public void StartCall(string targetUserId)
        {
            if (IsInCall) return;

            CurrentCallPartner = targetUserId;
            Log("Đang khám phá network endpoint (STUN)...");

            // Discover endpoints async
            Task.Run(() =>
            {
                InitializeUdpPeer();

                string localIp = _udpPeer.LocalEndPoint?.Address?.ToString() ?? "";
                int localPort = _udpPeer.LocalEndPoint?.Port ?? 0;
                string publicIp = _udpPeer.PublicEndPoint?.Address?.ToString() ?? "";
                int publicPort = _udpPeer.PublicEndPoint?.Port ?? 0;

                if (!string.IsNullOrEmpty(publicIp))
                    Log($"Public endpoint: {publicIp}:{publicPort}");
                else
                    Log("Không thể discover public endpoint (STUN failed)");

                Log($"Local endpoint: {localIp}:{localPort}");

                var data = string.Format(
                    "{{\"SignalType\":\"Request\",\"LocalIp\":\"{0}\",\"LocalPort\":{1},\"PublicIp\":\"{2}\",\"PublicPort\":{3}}}",
                    localIp, localPort, publicIp, publicPort);

                var packet = new Packet(PacketType.VideoCallRequest, _chatService.CurrentUserId, targetUserId, data);
                _tcpService.SendPacket(packet);

                Log("Đã gửi yêu cầu gọi video...");
            });
        }

        /// <summary>
        /// Chấp nhận cuộc gọi. Discover endpoint, reply, rồi bắt đầu hole punch.
        /// </summary>
        public void AcceptCall(string callerUserId)
        {
            CurrentCallPartner = callerUserId;
            Log("Đang chuẩn bị kết nối...");

            Task.Run(async () =>
            {
                InitializeUdpPeer();

                string localIp = _udpPeer.LocalEndPoint?.Address?.ToString() ?? "";
                int localPort = _udpPeer.LocalEndPoint?.Port ?? 0;
                string publicIp = _udpPeer.PublicEndPoint?.Address?.ToString() ?? "";
                int publicPort = _udpPeer.PublicEndPoint?.Port ?? 0;

                var data = string.Format(
                    "{{\"SignalType\":\"Accept\",\"LocalIp\":\"{0}\",\"LocalPort\":{1},\"PublicIp\":\"{2}\",\"PublicPort\":{3}}}",
                    localIp, localPort, publicIp, publicPort);

                var packet = new Packet(PacketType.VideoCallAccept, _chatService.CurrentUserId, callerUserId, data);
                _tcpService.SendPacket(packet);

                IsInCall = true;
                StartCapturing();
                OnCallAccepted?.Invoke();

                // Bắt đầu hole punching
                await AttemptP2PConnection();
            });
        }

        /// <summary>
        /// Từ chối cuộc gọi
        /// </summary>
        public void RejectCall(string callerUserId)
        {
            var data = "{\"SignalType\":\"Reject\"}";
            var packet = new Packet(PacketType.VideoCallReject, _chatService.CurrentUserId, callerUserId, data);
            _tcpService.SendPacket(packet);
        }

        /// <summary>
        /// Kết thúc cuộc gọi
        /// </summary>
        public void EndCall()
        {
            if (!IsInCall && CurrentCallPartner == null) return;

            if (CurrentCallPartner != null)
            {
                var data = "{\"SignalType\":\"End\"}";
                var packet = new Packet(PacketType.VideoCallEnd, _chatService.CurrentUserId, CurrentCallPartner, data);
                _tcpService.SendPacket(packet);
            }

            Cleanup();
            OnCallEnded?.Invoke();
        }

        /// <summary>
        /// Gửi video frame đến peer (P2P hoặc relay)
        /// </summary>
        public void SendVideoFrame(byte[] frameData)
        {
            if (!IsInCall) return;

            if (IsP2PConnected && !UseRelay)
            {
                // P2P UDP
                _udpPeer.SendData(frameData);
            }
            else
            {
                // Server relay (TCP)
                var base64 = Convert.ToBase64String(frameData);
                var packet = new Packet(PacketType.VideoFrame, _chatService.CurrentUserId, CurrentCallPartner, base64);
                _tcpService.SendPacket(packet);
            }
        }

        /// <summary>
        /// Xử lý signal nhận được từ server
        /// </summary>
        public void HandleVideoSignal(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.VideoCallRequest:
                    // Lưu endpoint info của caller
                    SavePeerEndpoint(packet.Data);
                    OnIncomingCall?.Invoke(packet.SenderId);
                    break;

                case PacketType.VideoCallAccept:
                    // Lưu endpoint info của callee, bắt đầu hole punch
                    SavePeerEndpoint(packet.Data);
                    IsInCall = true;
                    CurrentCallPartner = packet.SenderId;
                    StartCapturing();
                    OnCallAccepted?.Invoke();

                    // Bắt đầu P2P connection
                    Task.Run(async () => await AttemptP2PConnection());
                    break;

                case PacketType.VideoCallReject:
                    OnCallRejected?.Invoke("Cuộc gọi bị từ chối");
                    Cleanup();
                    break;

                case PacketType.VideoCallEnd:
                    Cleanup();
                    OnCallEnded?.Invoke();
                    break;

                case PacketType.VideoFrame:
                    // Server relay frame
                    if (!string.IsNullOrEmpty(packet.Data))
                    {
                        try
                        {
                            var frameData = Convert.FromBase64String(packet.Data);
                            OnVideoFrameReceived?.Invoke(frameData);
                        }
                        catch { }
                    }
                    break;
            }
        }

        #region Private Helpers

        private void InitializeUdpPeer()
        {
            _udpPeer?.Dispose();
            _udpPeer = new UdpPeerService();
            _udpPeer.OnDataReceived += (data) => OnVideoFrameReceived?.Invoke(data);
            _udpPeer.Initialize();
        }

        /// <summary>
        /// Thử kết nối P2P qua UDP hole punching.
        /// Nếu thất bại → fallback server relay.
        /// </summary>
        private async Task AttemptP2PConnection()
        {
            if (_udpPeer == null)
            {
                UseRelay = true;
                Log("⚠️ Dùng Server Relay (UDP chưa khởi tạo)");
                return;
            }

            Log("🔗 Đang thử kết nối P2P (UDP hole punching)...");

            bool success = await _udpPeer.HolePunchAsync(
                _peerPublicIp, _peerPublicPort,
                _peerLocalIp, _peerLocalPort);

            if (success)
            {
                UseRelay = false;
                _udpPeer.StartReceiving();
                Log("✅ Kết nối P2P thành công! Video stream trực tiếp qua UDP");
            }
            else
            {
                UseRelay = true;
                Log("⚠️ P2P thất bại, chuyển sang Server Relay (TCP)");
            }
        }

        private void SavePeerEndpoint(string data)
        {
            _peerLocalIp = GetJsonField(data, "LocalIp");
            _peerPublicIp = GetJsonField(data, "PublicIp");

            int.TryParse(GetJsonField(data, "LocalPort"), out _peerLocalPort);
            int.TryParse(GetJsonField(data, "PublicPort"), out _peerPublicPort);
        }

        private void Cleanup()
        {
            StopCapturing();
            IsInCall = false;
            CurrentCallPartner = null;
            UseRelay = false;
            _peerPublicIp = null;
            _peerPublicPort = 0;
            _peerLocalIp = null;
            _peerLocalPort = 0;

            _udpPeer?.Dispose();
            _udpPeer = null;
        }

        private void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        private string GetJsonField(string json, string field)
        {
            if (string.IsNullOrEmpty(json)) return null;

            var search = "\"" + field + "\":";
            int idx = json.IndexOf(search, StringComparison.Ordinal);
            if (idx < 0) return null;

            idx += search.Length;
            while (idx < json.Length && json[idx] == ' ') idx++;

            if (idx >= json.Length) return null;

            if (json[idx] == '"')
            {
                idx++;
                int end = json.IndexOf('"', idx);
                return end < 0 ? null : json.Substring(idx, end - idx);
            }
            else
            {
                var sb = new System.Text.StringBuilder();
                while (idx < json.Length && json[idx] != ',' && json[idx] != '}')
                {
                    sb.Append(json[idx]);
                    idx++;
                }
                return sb.ToString().Trim();
            }
        }

        #endregion
    }
}
