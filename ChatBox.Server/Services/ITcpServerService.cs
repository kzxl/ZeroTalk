using System;

namespace ChatBox.Server.Services
{
    /// <summary>
    /// Interface quản lý TCP Server
    /// </summary>
    public interface ITcpServerService
    {
        /// <summary>Khởi động server</summary>
        void Start(int port);

        /// <summary>Dừng server</summary>
        void Stop();

        /// <summary>Server có đang chạy</summary>
        bool IsRunning { get; }

        /// <summary>Số client đang kết nối</summary>
        int ConnectedCount { get; }

        /// <summary>Tổng số tin nhắn/packet đã route</summary>
        int TotalMessagesRouted { get; }

        /// <summary>Thời gian server khởi động</summary>
        DateTime? StartTime { get; }

        /// <summary>Lấy danh sách snapshot các client đang kết nối</summary>
        System.Collections.Generic.List<ChatBox.Server.Models.ConnectedClient> GetClientsSnapshot();

        /// <summary>Ngắt kết nối/kick một client</summary>
        void DisconnectClient(string userId);

        /// <summary>Event log message</summary>
        event Action<string> OnLog;

        /// <summary>Event khi danh sách client thay đổi</summary>
        event Action OnClientListChanged;
    }
}
