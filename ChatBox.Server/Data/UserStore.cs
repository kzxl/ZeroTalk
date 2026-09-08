using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ChatBox.Server.Models;

namespace ChatBox.Server.Data
{
    /// <summary>
    /// Lưu trữ user accounts trên server.
    /// Dùng file JSON đơn giản (có thể nâng cấp lên SQLite sau).
    /// </summary>
    public class UserStore
    {
        private readonly string _filePath;
        private List<UserAccount> _users;
        private readonly object _lock = new object();

        public UserStore(string filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
            LoadUsers();
        }

        /// <summary>
        /// Xác thực user bằng username và password hash
        /// </summary>
        public UserAccount Authenticate(string username, string passwordHash)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    u.PasswordHash == passwordHash);
            }
        }

        /// <summary>
        /// Đăng ký user mới
        /// </summary>
        public bool Register(string username, string passwordHash, string displayName)
        {
            lock (_lock)
            {
                // Kiểm tra trùng username
                if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    return false;

                var user = new UserAccount
                {
                    UserId = Guid.NewGuid().ToString("N").Substring(0, 8),
                    Username = username,
                    PasswordHash = passwordHash,
                    DisplayName = displayName ?? username,
                    CreatedAt = DateTime.Now
                };

                _users.Add(user);
                SaveUsers();
                return true;
            }
        }

        /// <summary>
        /// Lấy user theo username
        /// </summary>
        public UserAccount GetByUsername(string username)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Hash password bằng SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void LoadUsers()
        {
            _users = new List<UserAccount>();

            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath, Encoding.UTF8);
                    _users = ChatBox.Shared.Protocol.PacketSerializer.FromJson<List<UserAccount>>(json) ?? new List<UserAccount>();
                }
                catch
                {
                    _users = new List<UserAccount>();
                }
            }

            // Ensure default demo accounts (alice, bob, charlie) always exist with password "123"
            EnsureDemoUsers();
            SaveUsers();
        }

        private void EnsureDemoUsers()
        {
            EnsureUser("user_alice", "alice", "123", "Alice Johnson");
            EnsureUser("user_bob", "bob", "123", "Bob Williams");
            EnsureUser("user_charlie", "charlie", "123", "Charlie Davis");
        }

        private void EnsureUser(string id, string username, string password, string displayName)
        {
            var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                _users.Add(new UserAccount
                {
                    UserId = id,
                    Username = username,
                    PasswordHash = HashPassword(password),
                    DisplayName = displayName,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                // Guarantee demo password is always valid
                user.PasswordHash = HashPassword(password);
                if (string.IsNullOrEmpty(user.DisplayName))
                    user.DisplayName = displayName;
            }
        }

        private void SaveUsers()
        {
            try
            {
                var json = ChatBox.Shared.Protocol.PacketSerializer.ToJson(_users);
                File.WriteAllText(_filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving users: {ex.Message}");
            }
        }
    }
}
