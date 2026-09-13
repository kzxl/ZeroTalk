using System;

namespace ZeroTalk.Shared.DTOs
{
    /// <summary>
    /// DTO cho tin nhắn chat
    /// </summary>
    [Serializable]
    public class MessageDTO
    {
        /// <summary>Nội dung tin nhắn (đã mã hoá AES nếu có key)</summary>
        public string Content { get; set; }

        /// <summary>Có phải tin nhắn đã mã hoá không</summary>
        public bool IsEncrypted { get; set; }

        /// <summary>Thời gian gửi</summary>
        public DateTime SentAt { get; set; }

        public MessageDTO()
        {
            SentAt = DateTime.Now;
        }
    }
}
