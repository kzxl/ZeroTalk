using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZeroTalk.Server.Data
{
    /// <summary>
    /// Lưu trữ lịch sử tin nhắn trên server.
    /// Mỗi conversation (userId1_userId2) lưu vào 1 file JSON.
    /// Group chat lưu riêng file __group__.json.
    /// </summary>
    public class MessageStore
    {
        private readonly string _dataDir;
        private readonly object _lock = new object();

        public MessageStore(string dataDir = null)
        {
            _dataDir = dataDir ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatData");
            if (!Directory.Exists(_dataDir))
                Directory.CreateDirectory(_dataDir);
        }

        /// <summary>
        /// Lưu 1 tin nhắn
        /// </summary>
        public void SaveMessage(string senderId, string receiverId, string senderName, string content, bool isFile)
        {
            var record = new ChatRecord
            {
                SenderId = senderId,
                SenderName = senderName,
                Content = content,
                IsFile = isFile,
                Timestamp = DateTime.UtcNow
            };

            string key = GetConversationKey(senderId, receiverId);
            string filePath = Path.Combine(_dataDir, key + ".json");

            lock (_lock)
            {
                var records = LoadRecords(filePath);
                records.Add(record);

                // Giới hạn 500 tin nhắn gần nhất per conversation
                if (records.Count > 500)
                    records.RemoveRange(0, records.Count - 500);

                SaveRecords(filePath, records);
            }
        }

        /// <summary>
        /// Lấy lịch sử chat giữa 2 user (hoặc group)
        /// </summary>
        public List<ChatRecord> GetHistory(string userId1, string userId2, int maxCount = 50)
        {
            string key = GetConversationKey(userId1, userId2);
            string filePath = Path.Combine(_dataDir, key + ".json");

            lock (_lock)
            {
                var records = LoadRecords(filePath);
                int skip = Math.Max(0, records.Count - maxCount);
                return records.Skip(skip).ToList();
            }
        }

        /// <summary>
        /// Serialize lịch sử thành JSON string để gửi cho client
        /// </summary>
        public string SerializeHistory(List<ChatRecord> records)
        {
            if (records == null || records.Count == 0) return "[]";
            return ZeroTalk.Shared.Protocol.PacketSerializer.ToJson(records);
        }

        private string GetConversationKey(string userId1, string userId2)
        {
            if (userId2 == null || userId2 == "__group__")
                return "__group__";

            // Sort để conversation key luôn giống nhau bất kể ai gửi trước
            var ids = new[] { userId1, userId2 };
            Array.Sort(ids, StringComparer.OrdinalIgnoreCase);
            return ids[0] + "_" + ids[1];
        }

        private List<ChatRecord> LoadRecords(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<ChatRecord>();

            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                return ZeroTalk.Shared.Protocol.PacketSerializer.FromJson<List<ChatRecord>>(json) ?? new List<ChatRecord>();
            }
            catch
            {
                return new List<ChatRecord>();
            }
        }

        private void SaveRecords(string filePath, List<ChatRecord> records)
        {
            try
            {
                string json = SerializeHistory(records);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving chat records: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 1 bản ghi tin nhắn
    /// </summary>
    public class ChatRecord
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public bool IsFile { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
