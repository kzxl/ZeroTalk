using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatBox.Client.Services;
using ChatBox.Shared.Protocol;

namespace ChatBox.Client.Forms
{
    /// <summary>
    /// Form đăng nhập / đăng ký, hỗ trợ đăng nhập 1-click cho tài khoản Demo.
    /// </summary>
    public partial class frmLogin : Form
    {
        private TcpClientService _tcpService;
        private readonly string _autoLoginUser;

        public string LoggedInUserId { get; private set; }
        public string LoggedInDisplayName { get; private set; }
        public TcpClientService TcpService => _tcpService;

        public frmLogin(string autoLoginUser = null)
        {
            InitializeComponent();
            _autoLoginUser = autoLoginUser;
            _tcpService = new TcpClientService();
        }

        private bool _autoLoginTriggered;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            TryAutoLogin();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            TryAutoLogin();
        }

        private async void TryAutoLogin()
        {
            if (_autoLoginTriggered || string.IsNullOrEmpty(_autoLoginUser)) return;
            _autoLoginTriggered = true;

            string username = _autoLoginUser.Replace("demo_", "").Trim().ToLower();
            txtUsername.Text = username;
            txtPassword.Text = "123";
            await Task.Delay(200);
            await DoAuth(PacketType.Login);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await DoAuth(PacketType.Login);
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            await DoAuth(PacketType.Register);
        }

        private async void btnDemoAlice_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "alice";
            txtPassword.Text = "123";
            await DoAuth(PacketType.Login);
        }

        private async void btnDemoBob_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "bob";
            txtPassword.Text = "123";
            await DoAuth(PacketType.Login);
        }

        private async void btnDemoCharlie_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "charlie";
            txtPassword.Text = "123";
            await DoAuth(PacketType.Login);
        }

        private async Task DoAuth(PacketType authType)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Vui lòng nhập username và password";
                lblStatus.ForeColor = System.Drawing.Color.Orange;
                return;
            }

            btnLogin.Enabled = false;
            btnRegister.Enabled = false;
            pnlDemo.Enabled = false;
            lblStatus.Text = "Đang kết nối...";
            lblStatus.ForeColor = System.Drawing.Color.Gray;

            try
            {
                // 1. Kết nối TCP
                if (!_tcpService.IsConnected)
                {
                    var connected = await _tcpService.ConnectAsync(txtServer.Text, (int)nudPort.Value);
                    if (!connected)
                    {
                        lblStatus.Text = "Không thể kết nối đến server";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        return;
                    }
                }

                // 2. Hash password
                string passwordHash;
                using (var sha256 = SHA256.Create())
                {
                    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text));
                    passwordHash = Convert.ToBase64String(bytes);
                }

                // 3. Gửi packet Login/Register
                var authPayload = new Dictionary<string, object>
                {
                    { "Username", txtUsername.Text.Trim() },
                    { "PasswordHash", passwordHash }
                };

                var data = PacketSerializer.ToJson(authPayload);
                var packet = new Packet(authType, null, null, data);

                // Subscribe nhận response 1 lần
                Action<Packet> handler = null;
                var tcs = new TaskCompletionSource<Packet>();
                handler = p =>
                {
                    if (p.Type == PacketType.LoginResponse || p.Type == PacketType.RegisterResponse)
                    {
                        _tcpService.OnPacketReceived -= handler;
                        tcs.TrySetResult(p);
                    }
                };
                _tcpService.OnPacketReceived += handler;

                _tcpService.SendPacket(packet);
                lblStatus.Text = authType == PacketType.Login ? "Đang đăng nhập..." : "Đang đăng ký...";

                // 4. Chờ response (timeout 10s)
                var timeoutTask = Task.Delay(10000);
                var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    _tcpService.OnPacketReceived -= handler;
                    lblStatus.Text = "Timeout - server không phản hồi";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                var response = tcs.Task.Result;

                // 5. Parse response
                var respData = PacketSerializer.FromJson<Dictionary<string, object>>(response.Data);
                bool success = false;
                string message = null;
                string userId = null;
                string displayName = null;

                if (respData != null)
                {
                    if (respData.ContainsKey("Success") && respData["Success"] != null)
                        bool.TryParse(respData["Success"].ToString(), out success);
                    if (respData.ContainsKey("Message") && respData["Message"] != null)
                        message = respData["Message"].ToString();
                    if (respData.ContainsKey("UserId") && respData["UserId"] != null)
                        userId = respData["UserId"].ToString();
                    if (respData.ContainsKey("DisplayName") && respData["DisplayName"] != null)
                        displayName = respData["DisplayName"].ToString();
                }

                if (success)
                {
                    LoggedInUserId = userId;
                    LoggedInDisplayName = displayName;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.Text = message ?? "Đăng nhập thất bại";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Lỗi: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                btnLogin.Enabled = true;
                btnRegister.Enabled = true;
                pnlDemo.Enabled = true;
            }
        }
    }
}
