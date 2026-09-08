using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ChatBox.Client.Helpers;
using ChatBox.Client.Services;

namespace ChatBox.Client.Forms
{
    /// <summary>
    /// Video call window - supports remote stream and local Picture-in-Picture (PIP).
    /// Supports Synthetic Camera and Screen Share video sources.
    /// </summary>
    public partial class frmVideoCall : Form
    {
        private readonly VideoCallService _videoCallService;
        private readonly VideoRecorder _recorder;
        private bool _isRecording;
        private DateTime _callStartTime;
        private bool _isFormClosing;

        public frmVideoCall(VideoCallService videoCallService)
        {
            InitializeComponent();
            _videoCallService = videoCallService;
            _recorder = new VideoRecorder();

            _videoCallService.OnVideoFrameReceived += DisplayRemoteFrame;
            _videoCallService.OnLocalFrameCaptured += DisplayLocalFrame;
            _videoCallService.OnCallEnded += HandleCallEnded;

            _callStartTime = DateTime.Now;
            tmrCallDuration.Start();

            // Initialize default video source
            if (cmbVideoSource.Items.Count > 0)
            {
                cmbVideoSource.SelectedIndex = 0;
            }
        }

        private void DisplayRemoteFrame(byte[] frameData)
        {
            if (_isFormClosing || IsDisposed) return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action<byte[]>(DisplayRemoteFrame), frameData);
                }
                catch { }
                return;
            }

            try
            {
                using (var ms = new MemoryStream(frameData))
                {
                    var image = Image.FromStream(ms);
                    var oldImg = pnlRemoteVideo.Image;
                    pnlRemoteVideo.Image = new Bitmap(image);
                    oldImg?.Dispose();

                    if (_isRecording)
                    {
                        _recorder.AddFrame(frameData);
                    }
                }
            }
            catch { }
        }

        private void DisplayLocalFrame(byte[] frameData)
        {
            if (_isFormClosing || IsDisposed) return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action<byte[]>(DisplayLocalFrame), frameData);
                }
                catch { }
                return;
            }

            try
            {
                using (var ms = new MemoryStream(frameData))
                {
                    var image = Image.FromStream(ms);
                    var oldImg = pnlLocalVideo.Image;
                    pnlLocalVideo.Image = new Bitmap(image);
                    oldImg?.Dispose();
                }
            }
            catch { }
        }

        private void HandleCallEnded()
        {
            if (_isFormClosing || IsDisposed) return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(HandleCallEnded));
                }
                catch { }
                return;
            }

            tmrCallDuration.Stop();

            if (_isRecording)
            {
                _recorder.StopRecording();
                _isRecording = false;
            }

            lblStatus.Text = "📹 Call Ended";
            MessageBox.Show("Call has ended.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnToggleVideo_Click(object sender, EventArgs e)
        {
            bool newState = !_videoCallService.IsVideoEnabled;
            _videoCallService.ToggleVideo(newState);

            if (newState)
            {
                btnToggleVideo.Text = "📷 Cam Off";
                btnToggleVideo.BackColor = Color.FromArgb(50, 100, 180);
            }
            else
            {
                btnToggleVideo.Text = "📷 Cam On";
                btnToggleVideo.BackColor = Color.FromArgb(80, 80, 85);
                var oldImg = pnlLocalVideo.Image;
                pnlLocalVideo.Image = null;
                oldImg?.Dispose();
            }
        }

        private void cmbVideoSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVideoSource.SelectedIndex == 0)
            {
                _videoCallService.SetVideoSource(new SyntheticVideoSource(_videoCallService.CurrentUserId ?? "User"));
            }
            else if (cmbVideoSource.SelectedIndex == 1)
            {
                _videoCallService.SetVideoSource(new ScreenCaptureVideoSource());
            }
        }

        private void tmrCallDuration_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _callStartTime;
            lblDuration.Text = $"⏱ {elapsed:mm\\:ss}";

            if (_videoCallService.IsP2PConnected && !_videoCallService.UseRelay)
            {
                lblConnectionMode.Text = "🟢 P2P Direct";
                lblConnectionMode.ForeColor = Color.LightGreen;
            }
            else
            {
                lblConnectionMode.Text = "🟠 Server Relay";
                lblConnectionMode.ForeColor = Color.Orange;
            }
        }

        private void btnEndCall_Click(object sender, EventArgs e)
        {
            _isFormClosing = true;
            tmrCallDuration.Stop();
            _videoCallService.EndCall();

            if (_isRecording)
            {
                _recorder.StopRecording();
                _isRecording = false;
            }

            this.Close();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (!_isRecording)
            {
                var outputPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Recordings",
                    $"call_{DateTime.Now:yyyyMMdd_HHmmss}.avi");

                var dir = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _recorder.StartRecording(outputPath);
                _isRecording = true;
                btnRecord.Text = "⏹ Stop";
                btnRecord.BackColor = Color.FromArgb(200, 50, 50);
                lblStatus.Text = "🔴 Recording";
            }
            else
            {
                _recorder.StopRecording();
                _isRecording = false;
                btnRecord.Text = "⏺ Record";
                btnRecord.BackColor = Color.FromArgb(65, 65, 72);
                lblStatus.Text = "📹 In Call";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _isFormClosing = true;
            tmrCallDuration.Stop();

            _videoCallService.OnVideoFrameReceived -= DisplayRemoteFrame;
            _videoCallService.OnLocalFrameCaptured -= DisplayLocalFrame;
            _videoCallService.OnCallEnded -= HandleCallEnded;

            if (_isRecording)
            {
                _recorder.StopRecording();
                _isRecording = false;
            }

            pnlRemoteVideo.Image?.Dispose();
            pnlRemoteVideo.Image = null;

            pnlLocalVideo.Image?.Dispose();
            pnlLocalVideo.Image = null;

            base.OnFormClosing(e);
        }
    }
}
