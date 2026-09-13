using System;
using System.Collections.Concurrent;
using ZeroTalk.Shared.Crypto;
using ZeroTalk.Shared.Protocol;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Xử lý gửi/nhận tin nhắn text, quản lý mã hoá AES với từng user
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly TcpClientService _tcpService;
        private readonly AesHelper _aesHelper;

        /// <summary>Shared keys cho từng user (userId → AES key)</summary>
        private readonly ConcurrentDictionary<string, byte[]> _sharedKeys;

        public string CurrentUserId { get; set; }

        public ChatService(TcpClientService tcpService)
        {
            _tcpService = tcpService;
            _aesHelper = new AesHelper();
            _sharedKeys = new ConcurrentDictionary<string, byte[]>();
        }

        /// <summary>
        /// Gửi tin nhắn text. Nếu có shared key với receiver → mã hoá AES.
        /// </summary>
        public void SendMessage(string receiverId, string message)
        {
            string content = message;
            bool isEncrypted = false;

            byte[] key;
            if (_sharedKeys.TryGetValue(receiverId, out key))
            {
                content = _aesHelper.Encrypt(message, key);
                isEncrypted = true;
            }

            var data = string.Format(
                "{{\"Content\":\"{0}\",\"IsEncrypted\":{1},\"SentAt\":\"{2:O}\"}}",
                EscapeJson(content),
                isEncrypted.ToString().ToLower(),
                DateTime.Now);

            var packet = new Packet(PacketType.Message, CurrentUserId, receiverId, data);
            _tcpService.SendPacket(packet);
        }

        /// <summary>
        /// Giải mã tin nhắn nhận được
        /// </summary>
        public string DecryptMessage(string senderId, string content, bool isEncrypted)
        {
            if (!isEncrypted) return content;

            byte[] key;
            if (_sharedKeys.TryGetValue(senderId, out key))
            {
                try
                {
                    return _aesHelper.Decrypt(content, key);
                }
                catch
                {
                    return "[Không thể giải mã]";
                }
            }

            return "[Chưa có key để giải mã]";
        }

        /// <summary>
        /// Lưu shared key cho 1 user (sau khi DH key exchange hoàn tất)
        /// </summary>
        public void SetSharedKey(string userId, byte[] key)
        {
            _sharedKeys.AddOrUpdate(userId, key, (k, v) => key);
        }

        /// <summary>
        /// Kiểm tra đã có shared key với user chưa
        /// </summary>
        public bool HasSharedKey(string userId)
        {
            return _sharedKeys.ContainsKey(userId);
        }

        private string EscapeJson(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
}
