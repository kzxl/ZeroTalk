using System;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Contract nguồn phát video frame (Webcam, Synthetic test pattern, Screen Share)
    /// </summary>
    public interface IVideoSource : IDisposable
    {
        /// <summary>Event kích hoạt khi có frame JPEG mới</summary>
        event Action<byte[]> OnFrameCaptured;

        /// <summary>Nguồn video có đang chạy</summary>
        bool IsRunning { get; }

        /// <summary>Tên mô tả nguồn video</summary>
        string SourceName { get; }

        /// <summary>Khởi động tạo frame</summary>
        void Start(int width = 320, int height = 240, int targetFps = 15);

        /// <summary>Dừng tạo frame</summary>
        void Stop();
    }
}
