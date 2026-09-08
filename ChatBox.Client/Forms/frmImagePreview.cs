using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ChatBox.Client.Services;

namespace ChatBox.Client.Forms
{
    /// <summary>
    /// Form xem trước ảnh đính kèm nhận qua chat.
    /// </summary>
    public partial class frmImagePreview : Form
    {
        private readonly string _filePath;

        public frmImagePreview(string filePath)
        {
            InitializeComponent();
            _filePath = filePath;
            LoadImage();
        }

        private void LoadImage()
        {
            if (!File.Exists(_filePath))
            {
                lblInfo.Text = "Không tìm thấy file hình ảnh";
                return;
            }

            try
            {
                var fi = new FileInfo(_filePath);
                using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var img = Image.FromStream(fs))
                {
                    picPreview.Image = new Bitmap(img);
                    this.Text = $"Xem ảnh: {fi.Name} ({img.Width}x{img.Height})";
                    lblInfo.Text = $"{fi.Name} — {img.Width}x{img.Height} px | {FormatBytes(fi.Length)}";
                }
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Lỗi tải ảnh: {ex.Message}";
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
        }

        private void btnOpenExternal_Click(object sender, EventArgs e)
        {
            FileReceiveService.OpenFile(_filePath);
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            FileReceiveService.OpenFolder(_filePath);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            picPreview.Image?.Dispose();
            picPreview.Image = null;
            base.OnFormClosing(e);
        }
    }
}
