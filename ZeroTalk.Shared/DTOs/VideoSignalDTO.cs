using System;

namespace ZeroTalk.Shared.DTOs
{
    /// <summary>
    /// DTO cho signaling video call.
    /// Chứa cả endpoint info cho P2P UDP connection.
    /// </summary>
    public class VideoSignalDTO
    {
        /// <summary>Loại signal: Request, Accept, Reject, End</summary>
        public string SignalType { get; set; }

        // === P2P Endpoint Info ===

        /// <summary>Local IP (LAN)</summary>
        public string LocalIp { get; set; }

        /// <summary>Local UDP port</summary>
        public int LocalPort { get; set; }

        /// <summary>Public IP (từ STUN)</summary>
        public string PublicIp { get; set; }

        /// <summary>Public UDP port (từ STUN)</summary>
        public int PublicPort { get; set; }

        /// <summary>True nếu phải dùng server relay (P2P fail)</summary>
        public bool UseRelay { get; set; }

        /// <summary>Thời gian bắt đầu cuộc gọi</summary>
        public DateTime CallStartTime { get; set; }
    }
}
