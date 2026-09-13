using System;

namespace ZeroTalk.Shared.Protocol
{
    /// <summary>
    /// Gói tin truyền qua TCP. Mọi dữ liệu đều được wrap trong Packet.
    /// Format: [4 bytes length][JSON payload]
    /// </summary>
    [Serializable]
    public class Packet
    {
        /// <summary>Loại packet</summary>
        public PacketType Type { get; set; }

        /// <summary>ID người gửi</summary>
        public string SenderId { get; set; }

        /// <summary>ID người nhận (null = broadcast)</summary>
        public string ReceiverId { get; set; }

        /// <summary>Dữ liệu payload (JSON string của DTO tương ứng)</summary>
        public string Data { get; set; }

        /// <summary>Thời gian gửi</summary>
        public DateTime Timestamp { get; set; }

        public Packet()
        {
            Timestamp = DateTime.Now;
        }

        public Packet(PacketType type, string senderId, string receiverId, string data)
        {
            Type = type;
            SenderId = senderId;
            ReceiverId = receiverId;
            Data = data;
            Timestamp = DateTime.Now;
        }
    }
}
