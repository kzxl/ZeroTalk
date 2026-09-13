using System;

namespace ZeroTalk.Shared.DTOs
{
    /// <summary>
    /// DTO cho truyền file qua mạng
    /// </summary>
    [Serializable]
    public class FileTransferDTO
    {
        /// <summary>Tên file gốc</summary>
        public string FileName { get; set; }

        /// <summary>Kích thước file (bytes)</summary>
        public long FileSize { get; set; }

        /// <summary>Tổng số chunk</summary>
        public int TotalChunks { get; set; }

        /// <summary>Index chunk hiện tại (0-based)</summary>
        public int ChunkIndex { get; set; }

        /// <summary>Dữ liệu chunk (Base64 encoded, đã mã hoá AES)</summary>
        public string ChunkData { get; set; }

        /// <summary>Có phải chunk đã mã hoá không</summary>
        public bool IsEncrypted { get; set; }

        /// <summary>ID duy nhất cho phiên truyền file</summary>
        public string TransferId { get; set; }
    }
}
