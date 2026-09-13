using ZeroTalk.Server.Data;
using ZeroTalk.Shared.DTOs;

namespace ZeroTalk.Server.Services
{
    /// <summary>
    /// Xác thực user bằng DTO
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserStore _userStore;

        public AuthService(UserStore userStore)
        {
            _userStore = userStore;
        }

        /// <summary>
        /// Xác thực đăng nhập
        /// </summary>
        public LoginResponseDTO Authenticate(LoginRequestDTO request)
        {
            if (string.IsNullOrEmpty(request?.Username) || string.IsNullOrEmpty(request?.PasswordHash))
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Username và password không được trống"
                };
            }

            var user = _userStore.Authenticate(request.Username, request.PasswordHash);
            if (user == null)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Sai username hoặc password"
                };
            }

            return new LoginResponseDTO
            {
                Success = true,
                Message = "Đăng nhập thành công",
                UserId = user.UserId,
                DisplayName = user.DisplayName
            };
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        public LoginResponseDTO Register(LoginRequestDTO request)
        {
            if (string.IsNullOrEmpty(request?.Username) || string.IsNullOrEmpty(request?.PasswordHash))
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Username và password không được trống"
                };
            }

            if (request.Username.Length < 3)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Username phải có ít nhất 3 ký tự"
                };
            }

            var success = _userStore.Register(request.Username, request.PasswordHash, request.Username);
            if (!success)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Username đã tồn tại"
                };
            }

            var user = _userStore.GetByUsername(request.Username);
            return new LoginResponseDTO
            {
                Success = true,
                Message = "Đăng ký thành công",
                UserId = user.UserId,
                DisplayName = user.DisplayName
            };
        }
    }
}
