namespace ZeroTalk.Shared.Protocol
{
    /// <summary>
    /// Định nghĩa các loại packet truyền qua mạng
    /// </summary>
    public enum PacketType
    {
        // === Authentication ===
        Login = 1,
        LoginResponse = 2,
        Register = 3,
        RegisterResponse = 4,

        // === Messaging ===
        Message = 10,
        GroupMessage = 11,
        TypingIndicator = 12,

        // === File Transfer ===
        FileHeader = 20,
        FileChunk = 21,
        FileComplete = 22,

        // === Video Call ===
        VideoCallRequest = 30,
        VideoCallAccept = 31,
        VideoCallReject = 32,
        VideoCallEnd = 33,
        VideoFrame = 34,
        AudioFrame = 35,

        // === Key Exchange (Diffie-Hellman) ===
        KeyExchange = 40,
        KeyExchangeResponse = 41,

        // === System ===
        UserList = 50,
        Disconnect = 51,
        Heartbeat = 52,
        ChatHistoryRequest = 53,
        ChatHistoryResponse = 54,
        Error = 99
    }
}
