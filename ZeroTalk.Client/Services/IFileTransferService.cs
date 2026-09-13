namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Interface truyền file
    /// </summary>
    public interface IFileTransferService
    {
        /// <summary>Gửi file đến 1 user</summary>
        void SendFile(string receiverId, string filePath);
    }
}
