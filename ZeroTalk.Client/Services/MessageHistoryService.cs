using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Lưu lịch sử tin nhắn local bằng file JSON.
    /// Mỗi conversation = 1 file: history_{conversationId}.json
    /// </summary>
    public class MessageHistoryService : IMessageHistoryService
    {
        private readonly string _historyDir;

        public MessageHistoryService()
        {
            _historyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatHistory");
            if (!Directory.Exists(_historyDir))
                Directory.CreateDirectory(_historyDir);
        }

        /// <summary>
        /// Lưu 1 tin nhắn vào file lịch sử
        /// </summary>
        public void SaveMessage(string conversationId, string senderId, string senderName, string content, bool isFile)
        {
            var filePath = GetFilePath(conversationId);
            var line = string.Format(
                "{{\"SenderId\":\"{0}\",\"SenderName\":\"{1}\",\"Content\":\"{2}\",\"Timestamp\":\"{3:O}\",\"IsFile\":{4}}}",
                Escape(senderId), Escape(senderName), Escape(content), DateTime.Now, isFile.ToString().ToLower());

            try
            {
                File.AppendAllText(filePath, line + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }

        /// <summary>
        /// Lấy lịch sử chat từ file
        /// </summary>
        public List<MessageHistoryItem> GetHistory(string conversationId, int limit = 100)
        {
            var result = new List<MessageHistoryItem>();
            var filePath = GetFilePath(conversationId);

            if (!File.Exists(filePath))
                return result;

            try
            {
                var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                var recent = lines.Skip(Math.Max(0, lines.Length - limit)).Take(limit);

                foreach (var line in recent)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var item = new MessageHistoryItem
                    {
                        SenderId = ExtractField(line, "SenderId"),
                        SenderName = ExtractField(line, "SenderName"),
                        Content = ExtractField(line, "Content"),
                        Timestamp = ExtractField(line, "Timestamp"),
                        IsFile = ExtractField(line, "IsFile") == "true"
                    };
                    result.Add(item);
                }
            }
            catch { }

            return result;
        }

        private string GetFilePath(string conversationId)
        {
            return Path.Combine(_historyDir, $"history_{conversationId}.json");
        }

        private string Escape(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        private string ExtractField(string json, string field)
        {
            var search = "\"" + field + "\":";
            int idx = json.IndexOf(search, StringComparison.Ordinal);
            if (idx < 0) return null;

            idx += search.Length;
            while (idx < json.Length && json[idx] == ' ') idx++;

            if (idx >= json.Length) return null;

            if (json[idx] == '"')
            {
                idx++;
                var sb = new StringBuilder();
                bool escaped = false;
                while (idx < json.Length)
                {
                    char c = json[idx];
                    if (escaped) { sb.Append(c); escaped = false; }
                    else if (c == '\\') { escaped = true; }
                    else if (c == '"') { break; }
                    else { sb.Append(c); }
                    idx++;
                }
                return sb.ToString();
            }
            else
            {
                // Boolean or number
                var sb = new StringBuilder();
                while (idx < json.Length && json[idx] != ',' && json[idx] != '}')
                {
                    sb.Append(json[idx]);
                    idx++;
                }
                return sb.ToString().Trim();
            }
        }
    }
}
