using System;

namespace ZeroTalk.Shared.DTOs
{
    /// <summary>
    /// DTO trả về cho Client sau khi đăng nhập
    /// </summary>
    [Serializable]
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }
}
