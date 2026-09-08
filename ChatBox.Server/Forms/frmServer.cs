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
    /// Server dashboard - monitors server status, active connections, and activity logs.
    /// </summary>
    public partial class frmServer : Form
    {
        private TcpServerService _serverService;
        private UserStore _userStore;
        private readonly bool _autoStart;

        public frmServer(bool autoStart = false)
        {
            InitializeComponent();
            _autoStart = autoStart;
            _userStore = new UserStore();
            _serverService = new TcpServerService(_userStore);

            // Subscribe events
            _serverService.OnLog += AppendLog;
            _serverService.OnClientListChanged += RefreshClientList;

            // Display local IP upon startup
            string localIp = StunClient.GetLocalIPAddress()?.ToString() ?? "127.0.0.1";
            lblStats.Text = $"⏱ Uptime: --:--:-- | 📦 Routed Packets: 0 | 🌐 Local IP: {localIp}:{(int)nudPort.Value}";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_autoStart)
            {
                btnStart_Click(this, EventArgs.Empty);
            }
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
                lblStatus.Text = "● Server Running";
                lblStatus.ForeColor = System.Drawing.Color.LimeGreen;

                tmrStats.Start();
                tmrStats_Tick(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot start server: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tmrStats.Stop();
            _serverService.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            nudPort.Enabled = true;
            lblStatus.Text = "● Server Stopped";
            lblStatus.ForeColor = System.Drawing.Color.Gray;
            RefreshClientList();
            lblStats.Text = "⏱ Uptime: --:--:-- | 📦 Routed Packets: 0 | 🌐 Server Stopped";
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
                    $"Are you sure you want to disconnect client '{userName}'?",
                    "Confirm Disconnect", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    _serverService.DisconnectClient(userId);
                    AppendLog($"[ADMIN] Disconnected client: {userName} (ID: {userId})");
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
                    MessageBox.Show($"ChatBox.Client.exe not found at: {clientExe}\nPlease build the solution first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Launch Alice
                System.Diagnostics.Process.Start(clientExe, "demo_alice");

                // Launch Bob after 400ms
                System.Threading.Tasks.Task.Delay(400).ContinueWith(_ =>
                {
                    try { System.Diagnostics.Process.Start(clientExe, "demo_bob"); } catch { }
                });

                AppendLog("[ADMIN] Dispatched launch command for 2 demo clients (Alice & Bob)");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching clients: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tmrStats_Tick(object sender, EventArgs e)
        {
            if (_serverService.IsRunning && _serverService.StartTime.HasValue)
            {
                var uptime = DateTime.Now - _serverService.StartTime.Value;
                string uptimeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)uptime.TotalHours, uptime.Minutes, uptime.Seconds);
                string localIp = StunClient.GetLocalIPAddress()?.ToString() ?? "127.0.0.1";
                lblStats.Text = $"⏱ Uptime: {uptimeStr} | 📦 Routed Packets: {_serverService.TotalMessagesRouted} | 👥 Connected: {_serverService.ConnectedCount} | 🌐 Local IP: {localIp}:{(int)nudPort.Value}";
            }
        }

        private void frmServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serverService.IsRunning)
            {
                var result = MessageBox.Show(
                    "Server is running. Are you sure you want to stop and exit?",
                    "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
