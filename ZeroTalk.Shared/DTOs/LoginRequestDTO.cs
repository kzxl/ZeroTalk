using System;

namespace ZeroTalk.Shared.DTOs
{
    /// <summary>
    /// DTO gửi từ Client khi đăng nhập
    /// </summary>
    [Serializable]
    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
