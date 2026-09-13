using System;

namespace ZeroTalk.Client.Helpers
{
    /// <summary>
    /// Ghi lại cuộc gọi video.
    /// Skeleton - sẽ implement chi tiết ở Phase 4 với AForge.Video.FFMPEG.
    /// </summary>
    public class VideoRecorder : IDisposable
    {
        private bool _isRecording;
        private string _outputPath;

        public bool IsRecording => _isRecording;

        /// <summary>
        /// Bắt đầu ghi video
        /// </summary>
        public void StartRecording(string outputPath, int width = 640, int height = 480, int fps = 15)
        {
            _outputPath = outputPath;
            _isRecording = true;
            // TODO: Initialize FFMPEG writer
        }

        /// <summary>
        /// Thêm 1 frame vào video
        /// </summary>
        public void AddFrame(byte[] frameData)
        {
            if (!_isRecording) return;
            // TODO: Write frame to FFMPEG
        }

        /// <summary>
        /// Dừng ghi video
        /// </summary>
        public void StopRecording()
        {
            _isRecording = false;
            // TODO: Close FFMPEG writer
        }

        public void Dispose()
        {
            StopRecording();
        }
    }
}
