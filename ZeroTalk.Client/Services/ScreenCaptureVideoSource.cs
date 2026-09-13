using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Nguồn phát video chia sẻ màn hình máy tính thời gian thực.
    /// Sử dụng GDI+ CopyFromScreen, thu nhỏ và nén JPEG stream qua P2P/Relay.
    /// </summary>
    public class ScreenCaptureVideoSource : IVideoSource
    {
        private CancellationTokenSource _cts;
        private Task _workerTask;
        private readonly ImageCodecInfo _jpegEncoder;
        private readonly EncoderParameters _encoderParams;

        public event Action<byte[]> OnFrameCaptured;
        public bool IsRunning { get; private set; }
        public string SourceName => "Screen Share";

        public ScreenCaptureVideoSource()
        {
            _jpegEncoder = GetEncoder(ImageFormat.Jpeg);
            _encoderParams = new EncoderParameters(1);
            _encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 60L);
        }

        public void Start(int width = 480, int height = 270, int targetFps = 12)
        {
            if (IsRunning) return;

            IsRunning = true;
            _cts = new CancellationTokenSource();
            int delayMs = Math.Max(20, 1000 / targetFps);

            _workerTask = Task.Run(() => CaptureLoop(width, height, delayMs, _cts.Token));
        }

        public void Stop()
        {
            if (!IsRunning) return;

            IsRunning = false;
            try
            {
                _cts?.Cancel();
                _workerTask?.Wait(500);
            }
            catch { }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        private void CaptureLoop(int targetWidth, int targetHeight, int delayMs, CancellationToken ct)
        {
            var bounds = Screen.PrimaryScreen.Bounds;

            using (var screenBmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb))
            using (var screenG = Graphics.FromImage(screenBmp))
            using (var scaledBmp = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb))
            using (var scaledG = Graphics.FromImage(scaledBmp))
            {
                scaledG.InterpolationMode = InterpolationMode.Bilinear;

                while (!ct.IsCancellationRequested && IsRunning)
                {
                    var startTime = DateTime.UtcNow;

                    try
                    {
                        // 1. Chụp toàn màn hình
                        screenG.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);

                        // 2. Scale về kích thước truyền stream
                        scaledG.DrawImage(screenBmp, 0, 0, targetWidth, targetHeight);

                        // 3. Nén JPEG
                        using (var ms = new MemoryStream())
                        {
                            if (_jpegEncoder != null)
                                scaledBmp.Save(ms, _jpegEncoder, _encoderParams);
                            else
                                scaledBmp.Save(ms, ImageFormat.Jpeg);

                            var bytes = ms.ToArray();
                            OnFrameCaptured?.Invoke(bytes);
                        }
                    }
                    catch { }

                    var elapsed = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
                    int sleepTime = Math.Max(1, delayMs - elapsed);
                    if (ct.WaitHandle.WaitOne(sleepTime))
                        break;
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }

        public void Dispose()
        {
            Stop();
            _encoderParams?.Dispose();
        }
    }
}
