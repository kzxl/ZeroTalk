using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Text;
using System.Windows.Forms;
using ChatBox.Client.Services;
using ChatBox.Shared.Crypto;
using ChatBox.Shared.Protocol;

namespace ChatBox.Client.Forms
{
    /// <summary>
    /// Giao diện chat chính - hiển thị user online, gửi/nhận tin nhắn, file, video call
    /// </summary>
    public partial class frmChat : Form
    {
        private readonly TcpClientService _tcpService;
        private readonly ChatService _chatService;
        private readonly FileTransferService _fileTransferService;
        private readonly FileReceiveService _fileReceiveService;
        private readonly VideoCallService _videoCallService;
        private readonly MessageHistoryService _historyService;

        private string _currentUserId;
        private string _currentDisplayName;
        private string _selectedUserId;
        private string _selectedUserName;

        /// <summary>userId → displayName</summary>
        private readonly Dictionary<string, string> _onlineUsers = new Dictionary<string, string>();

        /// <summary>DH key exchange instances per user</summary>
        private readonly Dictionary<string, DiffieHellmanHelper> _dhInstances = new Dictionary<string, DiffieHellmanHelper>();

        private frmVideoCall _activeVideoCallForm;
        private Timer _typingSendTimer;
        private Timer _typingDisplayTimer;
        private bool _isGroupChatMode;
        private readonly Dictionary<string, int> _unreadCounts = new Dictionary<string, int>();

        /// <summary>Emoji phổ biến</summary>
        private static readonly string[] CommonEmojis = new[]
        {
            "😀", "😂", "🥰", "😍", "😎", "🤔", "😢", "😡",
            "👍", "👎", "👏", "🙏", "💪", "❤️", "🔥", "✨",
            "🎉", "😊", "😘", "🤣", "😭", "🥺", "👋", "🙌"
        };

        public frmChat(TcpClientService tcpService, string userId, string displayName)
        {
            InitializeComponent();

            _tcpService = tcpService;
            _currentUserId = userId;
            _currentDisplayName = displayName;

            _chatService = new ChatService(tcpService);
            _chatService.CurrentUserId = userId;

            _fileTransferService = new FileTransferService(tcpService, _chatService);
            _fileReceiveService = new FileReceiveService();
            _videoCallService = new VideoCallService(tcpService, _chatService);
            _historyService = new MessageHistoryService();

            // Lắng nghe tiến trình truyền file
            _fileTransferService.OnSendProgress += (fileName, current, total) =>
            {
                if (current == total || current % 5 == 0)
                {
                    AppendSystem($"📤 Đang gửi \"{fileName}\": {current}/{total} chunks ({(current * 100 / total)}%)");
                }
            };
            _fileReceiveService.OnReceiveProgress += (transferId, current, total) =>
            {
                if (current == total || current % 5 == 0)
                {
                    AppendSystem($"📥 Đang nhận file: {current}/{total} chunks ({(current * 100 / total)}%)");
                }
            };

            lblCurrentUser.Text = $"💬 ChatBox - Đăng nhập: {displayName}";
            this.Text = $"ChatBox - {displayName}";

            // Subscribe packet handler
            _tcpService.OnPacketReceived += HandlePacket;
            _tcpService.OnDisconnected += HandleDisconnected;

            // Video call events
            _videoCallService.OnIncomingCall += HandleIncomingCall;
            _videoCallService.OnCallAccepted += HandleCallAccepted;
            _videoCallService.OnCallEnded += HandleCallEnded;
            _videoCallService.OnCallRejected += HandleCallRejected;

            // Typing indicator timer: gửi mỗi 2s khi đang gõ
            _typingSendTimer = new Timer { Interval = 2000 };
            _typingSendTimer.Tick += (s, args) => _typingSendTimer.Stop();

            // Timer ẩn typing indicator sau 3s
            _typingDisplayTimer = new Timer { Interval = 3000 };
            _typingDisplayTimer.Tick += (s, args) =>
            {
                _typingDisplayTimer.Stop();
                lblTyping.Text = "";
            };

            // Khi form hiện lên → gửi Heartbeat để server gửi lại UserList
            this.Shown += frmChat_Shown;
        }

        private void frmChat_Shown(object sender, EventArgs e)
        {
            // Gửi Heartbeat để yêu cầu server gửi danh sách user online
            var heartbeat = new Packet(PacketType.Heartbeat, _currentUserId, null, null);
            _tcpService.SendPacket(heartbeat);
        }

        #region Packet Handling

        private void HandlePacket(Packet packet)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Packet>(HandlePacket), packet);
                return;
            }

            switch (packet.Type)
            {
                case PacketType.UserList:
                    HandleUserList(packet);
                    break;

                case PacketType.Message:
                    HandleMessage(packet);
                    break;

                case PacketType.KeyExchange:
                    HandleKeyExchange(packet);
                    break;

                case PacketType.KeyExchangeResponse:
                    HandleKeyExchangeResponse(packet);
                    break;

                case PacketType.FileHeader:
                    HandleFileHeader(packet);
                    break;

                case PacketType.FileChunk:
                    HandleFileChunk(packet);
                    break;

                case PacketType.FileComplete:
                    HandleFileComplete(packet);
                    break;

                case PacketType.VideoCallRequest:
                case PacketType.VideoCallAccept:
                case PacketType.VideoCallReject:
                case PacketType.VideoCallEnd:
                case PacketType.VideoFrame:
                    _videoCallService.HandleVideoSignal(packet);
                    break;

                case PacketType.TypingIndicator:
                    HandleTypingIndicator(packet);
                    break;

                case PacketType.GroupMessage:
                    HandleGroupMessage(packet);
                    break;

                case PacketType.ChatHistoryResponse:
                    HandleChatHistoryResponse(packet);
                    break;
            }
        }

        private void HandleUserList(Packet packet)
        {
            // Lưu lại selected user trước khi clear
            string previousSelectedId = _selectedUserId;

            _onlineUsers.Clear();
            lstUsers.Items.Clear();

            if (!string.IsNullOrEmpty(packet.Data))
            {
                var users = packet.Data.Split(',');
                foreach (var user in users)
                {
                    var parts = user.Split('|');
                    if (parts.Length >= 2)
                    {
                        string uid = parts[0];
                        string name = parts[1];

                        if (uid == _currentUserId) continue; // Bỏ qua chính mình

                        _onlineUsers[uid] = name;
                        lstUsers.Items.Add($"🟢 {name}");
                    }
                }
            }

            lblUsers.Text = $"👥 Online ({_onlineUsers.Count})";

            // Kiểm tra nếu đối tượng đang chat/call đã offline
            if (!string.IsNullOrEmpty(previousSelectedId) && !_onlineUsers.ContainsKey(previousSelectedId))
            {
                // User đang chat đã offline
                AppendSystem($"⚠️ {_selectedUserName ?? previousSelectedId} đã offline");

                // Nếu đang video call với user này → kết thúc
                if (_videoCallService.IsInCall && _videoCallService.CurrentCallPartner == previousSelectedId)
                {
                    _videoCallService.EndCall();
                    CloseVideoCallForm();
                    AppendSystem("📹 Cuộc gọi video đã kết thúc do đối phương offline");
                }

                _selectedUserId = null;
                _selectedUserName = null;
                lblChatWith.Text = "Chọn user để bắt đầu chat";
            }

            // Restore selection nếu user vẫn online
            if (!string.IsNullOrEmpty(previousSelectedId) && _onlineUsers.ContainsKey(previousSelectedId))
            {
                int idx = 0;
                foreach (var kvp in _onlineUsers)
                {
                    if (kvp.Key == previousSelectedId)
                    {
                        lstUsers.SelectedIndex = idx;
                        break;
                    }
                    idx++;
                }
            }
        }

        private void HandleMessage(Packet packet)
        {
            string content = GetJsonField(packet.Data, "Content");
            bool isEncrypted = GetJsonField(packet.Data, "IsEncrypted") == "true";

            // Giải mã nếu cần
            string displayContent = _chatService.DecryptMessage(packet.SenderId, content, isEncrypted);

            string senderName = "Unknown";
            if (_onlineUsers.ContainsKey(packet.SenderId))
                senderName = _onlineUsers[packet.SenderId];

            AppendChat(senderName, displayContent, Color.FromArgb(100, 200, 255));
            PlayNotificationSound();

            // Đếm unread nếu không phải user đang chọn
            if (packet.SenderId != _selectedUserId)
            {
                int count;
                _unreadCounts.TryGetValue(packet.SenderId, out count);
                _unreadCounts[packet.SenderId] = count + 1;
                RefreshUserListBadges();
            }

            // Lưu lịch sử
            _historyService.SaveMessage(packet.SenderId, packet.SenderId, senderName, displayContent, false);
        }

        private void HandleKeyExchange(Packet packet)
        {
            // Nhận public key từ người khác → tạo DH instance, derive shared secret, gửi lại public key
            var otherPublicKey = packet.Data;
            var dh = new DiffieHellmanHelper();
            var sharedSecret = dh.DeriveSharedSecret(otherPublicKey);
            var myPublicKey = dh.GetPublicKey();

            _chatService.SetSharedKey(packet.SenderId, sharedSecret);
            _dhInstances[packet.SenderId] = dh;

            // Gửi public key response
            var responsePacket = new Packet(PacketType.KeyExchangeResponse, _currentUserId, packet.SenderId, myPublicKey);
            _tcpService.SendPacket(responsePacket);

            AppendSystem($"🔐 Đã thiết lập kênh mã hoá với {GetUserName(packet.SenderId)}");
        }

        private void HandleKeyExchangeResponse(Packet packet)
        {
            // Nhận public key response → derive shared secret
            DiffieHellmanHelper dh;
            if (_dhInstances.TryGetValue(packet.SenderId, out dh))
            {
                var sharedSecret = dh.DeriveSharedSecret(packet.Data);
                _chatService.SetSharedKey(packet.SenderId, sharedSecret);

                AppendSystem($"🔐 Đã thiết lập kênh mã hoá với {GetUserName(packet.SenderId)}");
            }
        }

        private void HandleFileHeader(Packet packet)
        {
            string fileName = GetJsonField(packet.Data, "FileName");
            string fileSizeStr = GetJsonField(packet.Data, "FileSize");
            string totalChunksStr = GetJsonField(packet.Data, "TotalChunks");
            string transferId = GetJsonField(packet.Data, "TransferId");
            string senderName = GetUserName(packet.SenderId);

            long fileSize = 0;
            long.TryParse(fileSizeStr, out fileSize);
            int totalChunks = 0;
            int.TryParse(totalChunksStr, out totalChunks);

            _fileReceiveService.HandleFileHeader(packet.SenderId, senderName, fileName, fileSize, totalChunks, transferId);

            string sizeText = FormatFileSize(fileSize);
            AppendSystem($"📎 {senderName} đang gửi file: {fileName} ({sizeText})");
        }

        private void HandleFileChunk(Packet packet)
        {
            string transferId = GetJsonField(packet.Data, "TransferId");
            string chunkIndexStr = GetJsonField(packet.Data, "ChunkIndex");
            string chunkData = GetJsonField(packet.Data, "ChunkData");

            int chunkIndex = 0;
            int.TryParse(chunkIndexStr, out chunkIndex);

            _fileReceiveService.HandleFileChunk(transferId, chunkIndex, chunkData);
        }

        private void HandleFileComplete(Packet packet)
        {
            string transferId = GetJsonField(packet.Data, "TransferId");
            string savedPath = _fileReceiveService.HandleFileComplete(transferId);

            if (savedPath != null)
            {
                string fileName = System.IO.Path.GetFileName(savedPath);
                string senderName = GetUserName(packet.SenderId);

                AppendSystem($"✅ Đã nhận file từ {senderName}: {fileName}");

                string ext = System.IO.Path.GetExtension(savedPath).ToLower();
                bool isImage = ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".bmp";

                if (isImage)
                {
                    try
                    {
                        var preview = new frmImagePreview(savedPath);
                        preview.Show(this);
                    }
                    catch
                    {
                        FileReceiveService.OpenFile(savedPath);
                    }
                }
                else
                {
                    // Hỏi user có muốn mở file không
                    var result = MessageBox.Show(
                        $"Đã nhận file \"{fileName}\" từ {senderName}.\n\nBạn có muốn mở file?",
                        "File đã nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        FileReceiveService.OpenFile(savedPath);
                    }
                }

                // Lưu lịch sử
                _historyService.SaveMessage(packet.SenderId, packet.SenderId, senderName,
                    $"[File] {fileName}", true);
            }
            else
            {
                AppendSystem($"❌ Lỗi khi nhận file từ {GetUserName(packet.SenderId)}");
            }
        }

        private void HandleTypingIndicator(Packet packet)
        {
            string senderName = GetUserName(packet.SenderId);
            lblTyping.Text = $"✏️ {senderName} đang nhập...";
            _typingDisplayTimer.Stop();
            _typingDisplayTimer.Start(); // Ẩn sau 3s
        }

        private void HandleGroupMessage(Packet packet)
        {
            string content = GetJsonField(packet.Data, "Content");
            string senderName = GetUserName(packet.SenderId);

            AppendChat($"[Nhóm] {senderName}", content, Color.FromArgb(255, 180, 100));
            PlayNotificationSound();

            // Lưu vào lịch sử group
            _historyService.SaveMessage("__group__", packet.SenderId, senderName, content, false);
        }

        private void HandleChatHistoryResponse(Packet packet)
        {
            // Parse response: {"PartnerId":"xxx","Messages":[...]}
            string partnerId = GetJsonField(packet.Data, "PartnerId");
            
            // Clear "Loading..." text
            rtbChat.Clear();

            // Parse Messages array - tìm mảng JSON
            int msgStart = packet.Data.IndexOf("\"Messages\":", StringComparison.Ordinal);
            if (msgStart < 0) return;
            msgStart += "\"Messages\":".Length;

            string messagesJson = packet.Data.Substring(msgStart).TrimEnd('}');
            if (messagesJson == "[]") 
            {
                AppendSystem("Chưa có tin nhắn nào");
                return;
            }

            // Parse từng message object
            bool isGroup = partnerId == "__group__";
            int i = 0;
            while (i < messagesJson.Length)
            {
                int objStart = messagesJson.IndexOf('{', i);
                if (objStart < 0) break;

                int objEnd = FindMatchingBrace(messagesJson, objStart);
                if (objEnd < 0) break;

                string obj = messagesJson.Substring(objStart, objEnd - objStart + 1);
                string senderId = GetJsonField(obj, "SenderId");
                string senderName2 = GetJsonField(obj, "SenderName");
                string content2 = GetJsonField(obj, "Content");

                bool isMine = senderId == _currentUserId;
                var color = isMine ? Color.LimeGreen : (isGroup ? Color.FromArgb(255, 180, 100) : Color.FromArgb(100, 200, 255));
                var name = isMine ? _currentDisplayName : senderName2;
                if (isGroup && !isMine) name = $"[Nhóm] {name}";

                AppendChat(name, content2, color);
                i = objEnd + 1;
            }
        }

        /// <summary>Tìm dấu } đóng tương ứng, bỏ qua {} con bên trong string</summary>
        private int FindMatchingBrace(string s, int openPos)
        {
            int depth = 0;
            bool inString = false;
            bool escaped = false;
            for (int i = openPos; i < s.Length; i++)
            {
                char c = s[i];
                if (escaped) { escaped = false; continue; }
                if (c == '\\') { escaped = true; continue; }
                if (c == '"') { inString = !inString; continue; }
                if (inString) continue;
                if (c == '{') depth++;
                if (c == '}') { depth--; if (depth == 0) return i; }
            }
            return -1;
        }

        private void HandleIncomingCall(string callerUserId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(HandleIncomingCall), callerUserId);
                return;
            }

            string callerName = GetUserName(callerUserId);
            var result = MessageBox.Show(
                $"📹 {callerName} đang gọi video cho bạn.\nBạn có muốn trả lời?",
                "Cuộc gọi đến", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _videoCallService.AcceptCall(callerUserId);
                // AcceptCall sẽ trigger OnCallAccepted → mở frmVideoCall
            }
            else
            {
                _videoCallService.RejectCall(callerUserId);
            }
        }

        private void HandleCallAccepted()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HandleCallAccepted));
                return;
            }

            string partnerName = GetUserName(_videoCallService.CurrentCallPartner);
            AppendSystem($"📹 Đang gọi video với {partnerName}");
            OpenVideoCallForm(partnerName);
        }

        private void HandleCallRejected(string reason)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(HandleCallRejected), reason);
                return;
            }

            AppendSystem($"📹 {reason}");
            CloseVideoCallForm();
        }

        private void HandleCallEnded()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HandleCallEnded));
                return;
            }

            AppendSystem("📹 Cuộc gọi video đã kết thúc");
            CloseVideoCallForm();
        }

        private void HandleDisconnected()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HandleDisconnected));
                return;
            }

            // Kết thúc video call nếu đang gọi
            if (_videoCallService.IsInCall)
            {
                _videoCallService.EndCall();
                CloseVideoCallForm();
            }

            AppendSystem("⚠️ Mất kết nối đến server!");
            MessageBox.Show("Mất kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region UI Events

        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedIndex < 0) return;

            int idx = 0;
            foreach (var kvp in _onlineUsers)
            {
             if (idx == lstUsers.SelectedIndex)
                {
                    // Thoát group chat mode khi chọn user cụ thể
                    if (_isGroupChatMode)
                    {
                        _isGroupChatMode = false;
                        btnGroupChat.BackColor = Color.FromArgb(60, 100, 160);
                        btnGroupChat.Text = "📢 Chat nhóm";
                    }

                    _selectedUserId = kvp.Key;
                    _selectedUserName = kvp.Value;
                    lblChatWith.Text = $"💬 Chat với {kvp.Value}";

                    // Clear unread
                    _unreadCounts.Remove(kvp.Key);
                    RefreshUserListBadges();

                    // Load lịch sử chat từ server
                    rtbChat.Clear();
                    AppendSystem("Loading...");
                    var histReq = new Packet(PacketType.ChatHistoryRequest, _currentUserId, _selectedUserId, null);
                    _tcpService.SendPacket(histReq);

                    // Khởi tạo DH key exchange nếu chưa có
                    if (!_chatService.HasSharedKey(_selectedUserId))
                    {
                        InitiateKeyExchange(_selectedUserId);
                    }

                    break;
                }
                idx++;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
            else if (!string.IsNullOrEmpty(_selectedUserId) && !_typingSendTimer.Enabled)
            {
                // Gửi typing indicator (throttle 2s)
                _typingSendTimer.Start();
                var typingPacket = new Packet(PacketType.TypingIndicator, _currentUserId, _selectedUserId, null);
                _tcpService.SendPacket(typingPacket);
            }
        }

        private void btnEmoji_Click(object sender, EventArgs e)
        {
            // Tạo popup menu emoji
            var menu = new ContextMenuStrip();
            menu.BackColor = Color.FromArgb(45, 45, 50);
            menu.ForeColor = Color.White;
            menu.ShowImageMargin = false;

            // Hiển thị emoji 6 cái mỗi hàng
            var flowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                MaximumSize = new Size(240, 200),
                BackColor = Color.FromArgb(45, 45, 50),
                Padding = new Padding(5)
            };

            foreach (var emoji in CommonEmojis)
            {
                var btn = new Button
                {
                    Text = emoji,
                    Font = new Font("Segoe UI Emoji", 14F),
                    Size = new Size(36, 36),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(55, 55, 60),
                    ForeColor = Color.White,
                    Margin = new Padding(2),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, args) =>
                {
                    txtMessage.Text += ((Button)s).Text;
                    txtMessage.SelectionStart = txtMessage.Text.Length;
                    txtMessage.Focus();
                    menu.Close();
                };
                flowPanel.Controls.Add(btn);
            }

            var host = new ToolStripControlHost(flowPanel)
            {
                AutoSize = true
            };
            menu.Items.Add(host);
            menu.Show(btnEmoji, new Point(0, -210));
        }

        private void btnGroupChat_Click(object sender, EventArgs e)
        {
            _isGroupChatMode = !_isGroupChatMode;

            if (_isGroupChatMode)
            {
                lstUsers.ClearSelected();
                _selectedUserId = null;
                _selectedUserName = null;
                lblChatWith.Text = "📢 Chat nhóm (tất cả user online)";
                btnGroupChat.BackColor = Color.FromArgb(200, 100, 50);
                btnGroupChat.Text = "📢 Đang chat nhóm";

                // Load lịch sử group từ server
                rtbChat.Clear();
                AppendSystem("Loading...");
                var histReq = new Packet(PacketType.ChatHistoryRequest, _currentUserId, "__group__", null);
                _tcpService.SendPacket(histReq);
            }
            else
            {
                lblChatWith.Text = "Chọn user để bắt đầu chat";
                btnGroupChat.BackColor = Color.FromArgb(60, 100, 160);
                btnGroupChat.Text = "📢 Chat nhóm";
                rtbChat.Clear();
            }
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                MessageBox.Show("Vui lòng chọn user để gửi file");
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file để gửi";
                ofd.Filter = "All Files|*.*|Images|*.jpg;*.png;*.gif;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _fileTransferService.SendFile(_selectedUserId, ofd.FileName);
                    string fileName = System.IO.Path.GetFileName(ofd.FileName);
                    AppendSystem($"📎 Đang gửi file: {fileName}");
                    _historyService.SaveMessage(_selectedUserId, _currentUserId, _currentDisplayName,
                        $"[File] {fileName}", true);
                }
            }
        }

        private void btnVideoCall_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                MessageBox.Show("Vui lòng chọn user để gọi video");
                return;
            }

            if (_videoCallService.IsInCall)
            {
                MessageBox.Show("Bạn đang trong cuộc gọi video!");
                return;
            }

            _videoCallService.StartCall(_selectedUserId);
            AppendSystem($"📹 Đang gọi video cho {_selectedUserName}...");
        }

        private void frmChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            _tcpService.OnPacketReceived -= HandlePacket;
            _tcpService.OnDisconnected -= HandleDisconnected;

            // Kết thúc video call nếu đang gọi
            if (_videoCallService.IsInCall)
            {
                _videoCallService.EndCall();
            }
            CloseVideoCallForm();

            // Gửi disconnect packet
            var packet = new Packet(PacketType.Disconnect, _currentUserId, null, null);
            _tcpService.SendPacket(packet);
            _tcpService.Disconnect();

            // Dispose DH instances
            foreach (var dh in _dhInstances.Values)
            {
                dh.Dispose();
            }
        }

        #endregion

        #region Video Call Helpers

        private void OpenVideoCallForm(string partnerName)
        {
            CloseVideoCallForm(); // Đóng form cũ nếu có

            _activeVideoCallForm = new frmVideoCall(_videoCallService);
            _activeVideoCallForm.Text = $"📹 Video Call - {partnerName}";
            _activeVideoCallForm.FormClosed += (s, args) =>
            {
                _activeVideoCallForm = null;
            };
            _activeVideoCallForm.Show(this); // Show as modeless, owner = frmChat
        }

        private void CloseVideoCallForm()
        {
            if (_activeVideoCallForm != null && !_activeVideoCallForm.IsDisposed)
            {
                _activeVideoCallForm.Close();
                _activeVideoCallForm = null;
            }
        }

        #endregion

        #region Helpers

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
                return;

            string message = txtMessage.Text.Trim();

            if (_isGroupChatMode)
            {
                // Group chat: broadcast tới tất cả user online
                var data = string.Format("{{\"Content\":\"{0}\"}}", EscapeJsonString(message));
                foreach (var kvp in _onlineUsers)
                {
                    var packet = new Packet(PacketType.GroupMessage, _currentUserId, kvp.Key, data);
                    _tcpService.SendPacket(packet);
                }

                AppendChat(_currentDisplayName, message, Color.LimeGreen);
                _historyService.SaveMessage("__group__", _currentUserId, _currentDisplayName, message, false);
            }
            else
            {
                if (string.IsNullOrEmpty(_selectedUserId)) return;

                _chatService.SendMessage(_selectedUserId, message);
                AppendChat(_currentDisplayName, message, Color.LimeGreen);
                _historyService.SaveMessage(_selectedUserId, _currentUserId, _currentDisplayName, message, false);
            }

            txtMessage.Clear();
            txtMessage.Focus();
        }

        private void InitiateKeyExchange(string targetUserId)
        {
            var dh = new DiffieHellmanHelper();
            _dhInstances[targetUserId] = dh;

            var publicKey = dh.GetPublicKey();
            var packet = new Packet(PacketType.KeyExchange, _currentUserId, targetUserId, publicKey);
            _tcpService.SendPacket(packet);

            AppendSystem($"🔑 Đang trao đổi khoá mã hoá với {GetUserName(targetUserId)}...");
        }

        private void AppendChat(string sender, string message, Color color)
        {
            var timestamp = DateTime.Now.ToString("HH:mm");

            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;

            // Timestamp
            rtbChat.SelectionColor = Color.Gray;
            rtbChat.AppendText($"[{timestamp}] ");

            // Sender name
            rtbChat.SelectionColor = color;
            rtbChat.AppendText($"{sender}: ");

            // Message
            rtbChat.SelectionColor = Color.White;
            rtbChat.AppendText($"{message}\n");

            rtbChat.ScrollToCaret();
        }

        private void AppendSystem(string message)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = Color.FromArgb(255, 200, 100);
            rtbChat.AppendText($"  {message}\n");
            rtbChat.ScrollToCaret();
        }

        private string GetUserName(string userId)
        {
            string name;
            return _onlineUsers.TryGetValue(userId, out name) ? name : userId;
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
            return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
        }

        private string GetJsonField(string json, string field)
        {
            if (string.IsNullOrEmpty(json)) return null;

            var search = "\"" + field + "\":";
            int idx = json.IndexOf(search, StringComparison.Ordinal);
            if (idx < 0) return null;

            idx += search.Length;
            while (idx < json.Length && json[idx] == ' ') idx++;

            if (idx >= json.Length) return null;

            if (json[idx] == '"')
            {
                idx++;
                var sb = new StringBuilder();
                bool escaped = false;
                while (idx < json.Length)
                {
                    char c = json[idx];
                    if (escaped) { sb.Append(c); escaped = false; }
                    else if (c == '\\') { escaped = true; }
                    else if (c == '"') { break; }
                    else { sb.Append(c); }
                    idx++;
                }
                return sb.ToString();
            }
            else
            {
                var sb = new StringBuilder();
                while (idx < json.Length && json[idx] != ',' && json[idx] != '}')
                {
                    sb.Append(json[idx]);
                    idx++;
                }
                return sb.ToString().Trim();
            }
        }

        #endregion

        #region Badge & Sound

        private void RefreshUserListBadges()
        {
            lstUsers.Items.Clear();
            foreach (var kvp in _onlineUsers)
            {
                int unread;
                _unreadCounts.TryGetValue(kvp.Key, out unread);
                string badge = unread > 0 ? $" ({unread})" : "";
                lstUsers.Items.Add($"🟢 {kvp.Value}{badge}");
            }
        }

        private void PlayNotificationSound()
        {
            try
            {
                SystemSounds.Asterisk.Play();
            }
            catch { }
        }

        private string EscapeJsonString(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        }

        #endregion
    }
}
