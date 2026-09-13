namespace ZeroTalk.Shared.Constants
{
    /// <summary>
    /// Hằng số dùng chung toàn ứng dụng
    /// </summary>
    public static class AppConstants
    {
        /// <summary>Port mặc định cho TCP Server</summary>
        public const int DefaultTcpPort = 9000;

        /// <summary>Port mặc định cho UDP (Video/Audio)</summary>
        public const int DefaultUdpPort = 9001;

        /// <summary>Kích thước buffer mặc định (64KB)</summary>
        public const int DefaultBufferSize = 65536;

        /// <summary>Kích thước chunk file (64KB)</summary>
        public const int FileChunkSize = 65536;

        /// <summary>Kích thước tối đa của 1 packet (10MB)</summary>
        public const int MaxPacketSize = 10 * 1024 * 1024;

        /// <summary>Server address mặc định</summary>
        public const string DefaultServerAddress = "127.0.0.1";

        /// <summary>Phiên bản ứng dụng</summary>
        public const string AppVersion = "1.0.0";

        /// <summary>Tên ứng dụng</summary>
        public const string AppName = "ZeroTalk";
    }
}
