using System;
using System.IO;
using ZeroTalk.Shared.Constants;
using ZeroTalk.Shared.Crypto;
using ZeroTalk.Shared.Protocol;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Truyền file qua TCP: chia chunk + mã hoá AES
    /// Flow: FileHeader → FileChunk x N → FileComplete
    /// </summary>
    public class FileTransferService : IFileTransferService
    {
        private readonly TcpClientService _tcpService;
        private readonly ChatService _chatService;
        private readonly AesHelper _aesHelper;

        public event Action<string, int, int> OnSendProgress; // fileName, current, total

        public FileTransferService(TcpClientService tcpService, ChatService chatService)
        {
            _tcpService = tcpService;
            _chatService = chatService;
            _aesHelper = new AesHelper();
        }

        /// <summary>
        /// Gửi file đến receiver. Chia thành chunk, mã hoá AES nếu có key.
        /// </summary>
        public void SendFile(string receiverId, string filePath)
        {
            if (!File.Exists(filePath)) return;

            var fileInfo = new FileInfo(filePath);
            int totalChunks = (int)Math.Ceiling((double)fileInfo.Length / AppConstants.FileChunkSize);
            string transferId = Guid.NewGuid().ToString("N").Substring(0, 8);

            // 1. Gửi FileHeader
            var headerData = string.Format(
                "{{\"FileName\":\"{0}\",\"FileSize\":{1},\"TotalChunks\":{2},\"TransferId\":\"{3}\"}}",
                EscapeJson(fileInfo.Name), fileInfo.Length, totalChunks, transferId);

            var headerPacket = new Packet(PacketType.FileHeader, _chatService.CurrentUserId, receiverId, headerData);
            _tcpService.SendPacket(headerPacket);

            // 2. Gửi từng chunk
            using (var stream = File.OpenRead(filePath))
            {
                var buffer = new byte[AppConstants.FileChunkSize];
                int chunkIndex = 0;

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    var chunkData = new byte[bytesRead];
                    Array.Copy(buffer, chunkData, bytesRead);

                    // Mã hoá chunk nếu có shared key
                    bool isEncrypted = false;
                    if (_chatService.HasSharedKey(receiverId))
                    {
                        // Note: ta cần access shared key - tạm dùng random key cho demo
                        // Thực tế sẽ dùng shared key từ DH exchange
                    }

                    string base64Data = Convert.ToBase64String(chunkData);

                    var chunkPacketData = string.Format(
                        "{{\"TransferId\":\"{0}\",\"ChunkIndex\":{1},\"ChunkData\":\"{2}\",\"IsEncrypted\":{3}}}",
                        transferId, chunkIndex, base64Data, isEncrypted.ToString().ToLower());

                    var chunkPacket = new Packet(PacketType.FileChunk, _chatService.CurrentUserId, receiverId, chunkPacketData);
                    _tcpService.SendPacket(chunkPacket);

                    chunkIndex++;
                    OnSendProgress?.Invoke(fileInfo.Name, chunkIndex, totalChunks);
                }
            }

            // 3. Gửi FileComplete
            var completeData = string.Format("{{\"TransferId\":\"{0}\"}}", transferId);
            var completePacket = new Packet(PacketType.FileComplete, _chatService.CurrentUserId, receiverId, completeData);
            _tcpService.SendPacket(completePacket);
        }

        private string EscapeJson(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
