using System.Collections.Generic;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Interface lưu lịch sử tin nhắn
    /// </summary>
    public interface IMessageHistoryService
    {
        /// <summary>Lưu 1 tin nhắn</summary>
        void SaveMessage(string conversationId, string senderId, string senderName, string content, bool isFile);

        /// <summary>Lấy lịch sử chat với 1 user</summary>
        List<MessageHistoryItem> GetHistory(string conversationId, int limit = 100);
    }

    /// <summary>
    /// 1 tin nhắn trong lịch sử
    /// </summary>
    public class MessageHistoryItem
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public string Timestamp { get; set; }
        public bool IsFile { get; set; }
    }
}
