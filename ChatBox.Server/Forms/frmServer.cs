using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ChatBox.Server.Data;
using ChatBox.Server.Services;
using ChatBox.Shared.Network;

namespace ChatBox.Server.Forms
{
    /// <summary>
    /// Server dashboard - hiển thị trạng thái, danh sách client, log
    /// </summary>
    public partial class frmServer : Form
    {
        private TcpServerService _serverService;
        private UserStore _userStore;

        public frmServer()
        {
            InitializeComponent();
            _userStore = new UserStore();
            _serverService = new TcpServerService(_userStore);

            // Subscribe events
            _serverService.OnLog += AppendLog;
            _serverService.OnClientListChanged += RefreshClientList;

            // Hiển thị local IP lúc mở
            string localIp = StunClient.GetLocalIPAddress()?.ToString() ?? "127.0.0.1";
            lblStats.Text = $"⏱ Uptime: --:--:-- | 📦 Gói tin đã chuyển: 0 | 🌐 Local IP: {localIp}:{(int)nudPort.Value}";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                int port = (int)nudPort.Value;
                _serverService.Start(port);

                btnStart.Enabled = false;
                btnStop.Enabled = true;
                nudPort.Enabled = false;
                lblStatus.Text = "● Server đang chạy";
                lblStatus.ForeColor = System.Drawing.Color.LimeGreen;

                tmrStats.Start();
                tmrStats_Tick(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể khởi động server: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tmrStats.Stop();
            _serverService.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            nudPort.Enabled = true;
            lblStatus.Text = "● Server dừng";
            lblStatus.ForeColor = System.Drawing.Color.Gray;
            RefreshClientList();
            lblStats.Text = "⏱ Uptime: --:--:-- | 📦 Gói tin đã chuyển: 0 | 🌐 Server đã dừng";
        }

        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }

            txtLog.AppendText(message + Environment.NewLine);
            txtLog.ScrollToCaret();
        }

        private void RefreshClientList()
        {
            if (lvClients.InvokeRequired)
            {
                lvClients.BeginInvoke(new Action(RefreshClientList));
                return;
            }

            lvClients.BeginUpdate();
            lvClients.Items.Clear();

            var clients = _serverService.GetClientsSnapshot();
            foreach (var client in clients)
            {
                var item = new ListViewItem(client.IsAuthenticated ? "🟢 Online" : "🟡 Connecting");
                item.Tag = client.UserId;
                item.SubItems.Add(client.Username ?? client.UserId);
                item.SubItems.Add(client.DisplayName ?? "-");
                item.SubItems.Add(client.EndPoint);
                item.SubItems.Add(client.ConnectedAt.ToString("HH:mm:ss"));
                item.SubItems.Add($"{client.PacketsReceived} / {client.PacketsSent}");

                if (!client.IsAuthenticated)
                    item.ForeColor = System.Drawing.Color.Gold;

                lvClients.Items.Add(item);
            }
            lvClients.EndUpdate();

            lblClients.Text = $"📡 Clients Online ({clients.Count(c => c.IsAuthenticated)})";
        }

        private void tsmiKick_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0) return;

            var selectedItem = lvClients.SelectedItems[0];
            string userId = selectedItem.Tag as string;
            string userName = selectedItem.SubItems[1].Text;

            if (!string.IsNullOrEmpty(userId))
            {
                var confirm = MessageBox.Show(
                    $"Bạn có chắc chắn muốn ngắt kết nối client '{userName}' không?",
                    "Xác nhận Kick", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    _serverService.DisconnectClient(userId);
                    AppendLog($"[ADMIN] Đã ngắt kết nối client: {userName} (ID: {userId})");
                }
            }
        }

        private void tsmiRefresh_Click(object sender, EventArgs e)
        {
            RefreshClientList();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void btnLaunchClients_Click(object sender, EventArgs e)
        {
            try
            {
                string clientExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatBox.Client.exe");
                if (!File.Exists(clientExe))
                {
                    clientExe = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\ChatBox.Client\bin\Debug\ChatBox.Client.exe"));
                }

                if (!File.Exists(clientExe))
                {
                    MessageBox.Show($"Không tìm thấy file ChatBox.Client.exe tại: {clientExe}\nVui lòng build solution trước.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Khởi chạy Alice
                System.Diagnostics.Process.Start(clientExe, "demo_alice");

                // Khởi chạy Bob sau 400ms
                System.Threading.Tasks.Task.Delay(400).ContinueWith(_ =>
                {
                    try { System.Diagnostics.Process.Start(clientExe, "demo_bob"); } catch { }
                });

                AppendLog("[ADMIN] Đã gửi lệnh khởi chạy 2 client demo (Alice & Bob)");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi chạy client: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tmrStats_Tick(object sender, EventArgs e)
        {
            if (_serverService.IsRunning && _serverService.StartTime.HasValue)
            {
                var uptime = DateTime.Now - _serverService.StartTime.Value;
                string uptimeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)uptime.TotalHours, uptime.Minutes, uptime.Seconds);
                string localIp = StunClient.GetLocalIPAddress()?.ToString() ?? "127.0.0.1";
                lblStats.Text = $"⏱ Uptime: {uptimeStr} | 📦 Gói tin đã chuyển: {_serverService.TotalMessagesRouted} | 👥 Đang kết nối: {_serverService.ConnectedCount} | 🌐 Local IP: {localIp}:{(int)nudPort.Value}";
            }
        }

        private void frmServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serverService.IsRunning)
            {
                var result = MessageBox.Show(
                    "Server đang chạy. Bạn có muốn dừng và thoát?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                tmrStats.Stop();
                _serverService.Stop();
            }
        }
    }
}
