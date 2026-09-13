namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Interface cho dịch vụ chat
    /// </summary>
    public interface IChatService
    {
        /// <summary>Gửi tin nhắn text (tự mã hoá nếu có key)</summary>
        void SendMessage(string receiverId, string message);

        /// <summary>ID user hiện tại</summary>
        string CurrentUserId { get; set; }
    }
}
