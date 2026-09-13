using System;

namespace ZeroTalk.Server.Models
{
    /// <summary>
    /// Thông tin tài khoản user (lưu trên server)
    /// </summary>
    public class UserAccount
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserAccount()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
