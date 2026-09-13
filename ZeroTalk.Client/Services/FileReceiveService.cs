using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Nhận file từ sender: tích luỹ chunks → ghép lại → lưu file → cho mở xem.
    /// </summary>
    public class FileReceiveService
    {
        private readonly string _downloadDir;

        /// <summary>transferId → thông tin file đang nhận</summary>
        private readonly ConcurrentDictionary<string, IncomingFile> _incomingFiles
            = new ConcurrentDictionary<string, IncomingFile>();

        /// <summary>Event khi nhận file xong (filePath, senderName)</summary>
        public event Action<string, string> OnFileReceived;

        /// <summary>Event khi nhận chunk (transferId, chunkIndex, totalChunks)</summary>
        public event Action<string, int, int> OnReceiveProgress;

        public FileReceiveService()
        {
            _downloadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
            if (!Directory.Exists(_downloadDir))
                Directory.CreateDirectory(_downloadDir);
        }

        /// <summary>
        /// Xử lý FileHeader → khởi tạo session nhận file
        /// </summary>
        public void HandleFileHeader(string senderId, string senderName, string fileName, long fileSize, int totalChunks, string transferId)
        {
            var incoming = new IncomingFile
            {
                SenderId = senderId,
                SenderName = senderName,
                FileName = fileName,
                FileSize = fileSize,
                TotalChunks = totalChunks,
                TransferId = transferId,
                Chunks = new Dictionary<int, byte[]>()
            };

            _incomingFiles[transferId] = incoming;
        }

        /// <summary>
        /// Xử lý FileChunk → tích luỹ chunk data
        /// </summary>
        public void HandleFileChunk(string transferId, int chunkIndex, string chunkDataBase64)
        {
            IncomingFile incoming;
            if (!_incomingFiles.TryGetValue(transferId, out incoming))
                return;

            try
            {
                byte[] chunkData = Convert.FromBase64String(chunkDataBase64);
                incoming.Chunks[chunkIndex] = chunkData;
                OnReceiveProgress?.Invoke(transferId, incoming.Chunks.Count, incoming.TotalChunks);
            }
            catch { }
        }

        /// <summary>
        /// Xử lý FileComplete → ghép chunks lại → lưu file
        /// Returns: đường dẫn file đã lưu, hoặc null nếu lỗi
        /// </summary>
        public string HandleFileComplete(string transferId)
        {
            IncomingFile incoming;
            if (!_incomingFiles.TryRemove(transferId, out incoming))
                return null;

            try
            {
                // Tạo tên file unique để tránh ghi đè
                string fileName = incoming.FileName;
                string filePath = Path.Combine(_downloadDir, fileName);
                int counter = 1;
                while (File.Exists(filePath))
                {
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);
                    filePath = Path.Combine(_downloadDir, $"{nameWithoutExt}_{counter}{ext}");
                    counter++;
                }

                // Ghép chunks theo thứ tự
                using (var fs = File.Create(filePath))
                {
                    for (int i = 0; i < incoming.TotalChunks; i++)
                    {
                        byte[] chunk;
                        if (incoming.Chunks.TryGetValue(i, out chunk))
                        {
                            fs.Write(chunk, 0, chunk.Length);
                        }
                    }
                }

                OnFileReceived?.Invoke(filePath, incoming.SenderName);
                return filePath;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Mở file bằng ứng dụng mặc định của hệ thống
        /// </summary>
        public static void OpenFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch { }
        }

        /// <summary>
        /// Mở thư mục chứa file
        /// </summary>
        public static void OpenFolder(string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            catch { }
        }

        /// <summary>Thông tin 1 file đang nhận</summary>
        private class IncomingFile
        {
            public string SenderId;
            public string SenderName;
            public string FileName;
            public long FileSize;
            public int TotalChunks;
            public string TransferId;
            public Dictionary<int, byte[]> Chunks;
        }
    }
}
