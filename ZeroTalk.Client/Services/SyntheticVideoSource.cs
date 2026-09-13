using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroTalk.Client.Services
{
    /// <summary>
    /// Nguồn phát video giả lập động cho môi trường thử nghiệm và demo.
    /// Tạo các frame hoạt họa 15-30 FPS với đồng hồ thời gian thực, sóng âm và avatar,
    /// nén JPEG gửi trực tiếp qua P2P UDP / TCP Relay mà không cần webcam vật lý.
    /// </summary>
    public class SyntheticVideoSource : IVideoSource
    {
        private CancellationTokenSource _cts;
        private Task _workerTask;
        private readonly string _displayName;
        private int _frameIndex;
        private readonly ImageCodecInfo _jpegEncoder;
        private readonly EncoderParameters _encoderParams;

        public event Action<byte[]> OnFrameCaptured;
        public bool IsRunning { get; private set; }
        public string SourceName => "Synthetic Demo Camera";

        public SyntheticVideoSource(string displayName = "Demo User")
        {
            _displayName = displayName;

            // Thiết lập JPEG encoder chất lượng 65% để tối ưu kích thước gói tin qua UDP (5-10 KB/frame)
            _jpegEncoder = GetEncoder(ImageFormat.Jpeg);
            _encoderParams = new EncoderParameters(1);
            _encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 65L);
        }

        public void Start(int width = 320, int height = 240, int targetFps = 15)
        {
            if (IsRunning) return;

            IsRunning = true;
            _cts = new CancellationTokenSource();
            int delayMs = Math.Max(10, 1000 / targetFps);

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

        private void CaptureLoop(int width, int height, int delayMs, CancellationToken ct)
        {
            using (var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var rnd = new Random();

                while (!ct.IsCancellationRequested && IsRunning)
                {
                    var startTime = DateTime.UtcNow;
                    _frameIndex++;

                    // 1. Vẽ nền gradient chuyển màu theo thời gian
                    float hueAngle = (_frameIndex * 2f) % 360f;
                    using (var bgBrush = new LinearGradientBrush(
                        new Rectangle(0, 0, width, height),
                        Color.FromArgb(20, 24, 38),
                        Color.FromArgb(12, 14, 20),
                        45f))
                    {
                        g.FillRectangle(bgBrush, 0, 0, width, height);
                    }

                    // 2. Vẽ lưới radar mờ
                    using (var gridPen = new Pen(Color.FromArgb(30, 80, 140, 220), 1f))
                    {
                        for (int x = 20; x < width; x += 30)
                            g.DrawLine(gridPen, x, 0, x, height);
                        for (int y = 20; y < height; y += 30)
                            g.DrawLine(gridPen, 0, y, width, y);
                    }

                    // 3. Vẽ vòng tròn sóng âm / radar xoay
                    int centerX = width / 2;
                    int centerY = height / 2 - 15;
                    float pulseRadius = 35 + (float)(Math.Sin(_frameIndex * 0.15) * 8);

                    using (var pulsePen = new Pen(Color.FromArgb(120, 0, 180, 255), 2f))
                    {
                        g.DrawEllipse(pulsePen, centerX - pulseRadius, centerY - pulseRadius, pulseRadius * 2, pulseRadius * 2);
                    }

                    // 4. Vẽ Avatar hình tròn
                    int avatarRadius = 26;
                    using (var avatarBrush = new LinearGradientBrush(
                        new Rectangle(centerX - avatarRadius, centerY - avatarRadius, avatarRadius * 2, avatarRadius * 2),
                        Color.FromArgb(0, 140, 255),
                        Color.FromArgb(120, 40, 200),
                        -45f))
                    {
                        g.FillEllipse(avatarBrush, centerX - avatarRadius, centerY - avatarRadius, avatarRadius * 2, avatarRadius * 2);
                    }

                    // Ký tự đại diện người dùng
                    string initial = !string.IsNullOrEmpty(_displayName) ? _displayName.Substring(0, 1).ToUpper() : "U";
                    using (var fontInitial = new Font("Segoe UI", 18F, FontStyle.Bold))
                    using (var textBrush = new SolidBrush(Color.White))
                    {
                        var size = g.MeasureString(initial, fontInitial);
                        g.DrawString(initial, fontInitial, textBrush, centerX - size.Width / 2, centerY - size.Height / 2);
                    }

                    // 5. Tên người dùng bên dưới avatar
                    using (var fontName = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold))
                    using (var nameBrush = new SolidBrush(Color.White))
                    {
                        var size = g.MeasureString(_displayName, fontName);
                        g.DrawString(_displayName, fontName, nameBrush, centerX - size.Width / 2, centerY + avatarRadius + 8);
                    }

                    // 6. Thanh sóng âm động giả lập giọng nói
                    int barCount = 14;
                    int barWidth = 4;
                    int barGap = 3;
                    int totalBarWidth = barCount * (barWidth + barGap);
                    int startBarX = centerX - totalBarWidth / 2;
                    int barBaseY = height - 38;

                    using (var barBrush = new SolidBrush(Color.FromArgb(70, 200, 120)))
                    {
                        for (int i = 0; i < barCount; i++)
                        {
                            float h = (float)(Math.Sin(_frameIndex * 0.25 + i * 0.5) * 10 + 12);
                            g.FillRectangle(barBrush, startBarX + i * (barWidth + barGap), barBaseY - h, barWidth, h);
                        }
                    }

                    // 7. Header thanh trạng thái: LIVE STREAM và Đồng hồ Live ms
                    using (var badgeBrush = new SolidBrush(Color.FromArgb(220, 40, 40)))
                    {
                        g.FillEllipse(badgeBrush, 12, 12, 8, 8);
                    }

                    using (var hudFont = new Font("Consolas", 8.5F))
                    using (var hudBrush = new SolidBrush(Color.FromArgb(220, 220, 230)))
                    {
                        string liveTag = $"LIVE • {1000 / delayMs} FPS";
                        g.DrawString(liveTag, hudFont, hudBrush, 24, 9);

                        string clockText = DateTime.Now.ToString("HH:mm:ss.fff");
                        var clockSize = g.MeasureString(clockText, hudFont);
                        g.DrawString(clockText, hudFont, hudBrush, width - clockSize.Width - 10, 9);
                    }

                    // 8. Nén thành JPEG byte array và phát event
                    using (var ms = new MemoryStream())
                    {
                        if (_jpegEncoder != null)
                            bmp.Save(ms, _jpegEncoder, _encoderParams);
                        else
                            bmp.Save(ms, ImageFormat.Jpeg);

                        var bytes = ms.ToArray();
                        OnFrameCaptured?.Invoke(bytes);
                    }

                    // Điều tiết FPS
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
