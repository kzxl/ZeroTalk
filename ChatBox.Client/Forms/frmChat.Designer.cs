namespace ChatBox.Client.Forms
{
    partial class frmChat
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.lblUsers = new System.Windows.Forms.Label();
            this.pnlChatBottom = new System.Windows.Forms.Panel();
            this.btnEmoji = new System.Windows.Forms.Button();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.btnVideoCall = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.rtbChat = new System.Windows.Forms.RichTextBox();
            this.lblChatWith = new System.Windows.Forms.Label();
            this.lblTyping = new System.Windows.Forms.Label();
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.lblCurrentUser = new System.Windows.Forms.Label();
            this.btnGroupChat = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlChatBottom.SuspendLayout();
            this.pnlTopBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
            this.pnlTopBar.Controls.Add(this.lblCurrentUser);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pnlTopBar.Size = new System.Drawing.Size(900, 35);
            this.pnlTopBar.TabIndex = 2;
            // 
            // lblCurrentUser
            // 
            this.lblCurrentUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentUser.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCurrentUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(255)))));
            this.lblCurrentUser.Location = new System.Drawing.Point(10, 5);
            this.lblCurrentUser.Name = "lblCurrentUser";
            this.lblCurrentUser.Size = new System.Drawing.Size(880, 25);
            this.lblCurrentUser.TabIndex = 0;
            this.lblCurrentUser.Text = "💬 ChatBox";
            this.lblCurrentUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 35);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1 - User list
            // 
            this.splitMain.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(35)))));
            this.splitMain.Panel1.Controls.Add(this.lstUsers);
            this.splitMain.Panel1.Controls.Add(this.btnGroupChat);
            this.splitMain.Panel1.Controls.Add(this.lblUsers);
            this.splitMain.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // splitMain.Panel2 - Chat area
            // 
            this.splitMain.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(30)))));
            this.splitMain.Panel2.Controls.Add(this.rtbChat);
            this.splitMain.Panel2.Controls.Add(this.lblChatWith);
            this.splitMain.Panel2.Controls.Add(this.lblTyping);
            this.splitMain.Panel2.Controls.Add(this.pnlChatBottom);
            this.splitMain.Size = new System.Drawing.Size(900, 515);
            this.splitMain.SplitterDistance = 200;
            this.splitMain.TabIndex = 0;
            // 
            // lstUsers
            // 
            this.lstUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.lstUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsers.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lstUsers.ForeColor = System.Drawing.Color.White;
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.ItemHeight = 20;
            this.lstUsers.Location = new System.Drawing.Point(5, 30);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(190, 480);
            this.lstUsers.TabIndex = 1;
            this.lstUsers.SelectedIndexChanged += new System.EventHandler(this.lstUsers_SelectedIndexChanged);
            // 
            // lblUsers
            // 
            this.lblUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblUsers.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.lblUsers.Location = new System.Drawing.Point(5, 5);
            this.lblUsers.Name = "lblUsers";
            this.lblUsers.Size = new System.Drawing.Size(190, 25);
            this.lblUsers.TabIndex = 0;
            this.lblUsers.Text = "👥 Online (0)";
            // 
            // pnlChatBottom
            // 
            this.pnlChatBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.pnlChatBottom.Controls.Add(this.txtMessage);
            this.pnlChatBottom.Controls.Add(this.btnEmoji);
            this.pnlChatBottom.Controls.Add(this.btnSend);
            this.pnlChatBottom.Controls.Add(this.btnSendFile);
            this.pnlChatBottom.Controls.Add(this.btnVideoCall);
            this.pnlChatBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlChatBottom.Location = new System.Drawing.Point(0, 465);
            this.pnlChatBottom.Name = "pnlChatBottom";
            this.pnlChatBottom.Padding = new System.Windows.Forms.Padding(8);
            this.pnlChatBottom.Size = new System.Drawing.Size(696, 50);
            this.pnlChatBottom.TabIndex = 2;
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtMessage.ForeColor = System.Drawing.Color.White;
            this.txtMessage.Location = new System.Drawing.Point(8, 8);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(480, 27);
            this.txtMessage.TabIndex = 0;
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(488, 8);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(70, 34);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send ➤";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnSendFile
            // 
            this.btnSendFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(75)))));
            this.btnSendFile.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSendFile.FlatAppearance.BorderSize = 0;
            this.btnSendFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendFile.ForeColor = System.Drawing.Color.White;
            this.btnSendFile.Location = new System.Drawing.Point(558, 8);
            this.btnSendFile.Name = "btnSendFile";
            this.btnSendFile.Size = new System.Drawing.Size(60, 34);
            this.btnSendFile.TabIndex = 2;
            this.btnSendFile.Text = "📎";
            this.btnSendFile.UseVisualStyleBackColor = false;
            this.btnSendFile.Click += new System.EventHandler(this.btnSendFile_Click);
            // 
            // btnVideoCall
            // 
            this.btnVideoCall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(150)))), ((int)(((byte)(50)))));
            this.btnVideoCall.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnVideoCall.FlatAppearance.BorderSize = 0;
            this.btnVideoCall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVideoCall.ForeColor = System.Drawing.Color.White;
            this.btnVideoCall.Location = new System.Drawing.Point(618, 8);
            this.btnVideoCall.Name = "btnVideoCall";
            this.btnVideoCall.Size = new System.Drawing.Size(70, 34);
            this.btnVideoCall.TabIndex = 3;
            this.btnVideoCall.Text = "📹";
            this.btnVideoCall.UseVisualStyleBackColor = false;
            this.btnVideoCall.Click += new System.EventHandler(this.btnVideoCall_Click);
            // 
            // rtbChat
            // 
            this.rtbChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(30)))));
            this.rtbChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbChat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rtbChat.ForeColor = System.Drawing.Color.White;
            this.rtbChat.Location = new System.Drawing.Point(0, 25);
            this.rtbChat.Name = "rtbChat";
            this.rtbChat.ReadOnly = true;
            this.rtbChat.Size = new System.Drawing.Size(696, 440);
            this.rtbChat.TabIndex = 0;
            this.rtbChat.Text = "";
            // 
            // lblChatWith
            // 
            this.lblChatWith.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblChatWith.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblChatWith.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.lblChatWith.Location = new System.Drawing.Point(0, 0);
            this.lblChatWith.Name = "lblChatWith";
            this.lblChatWith.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblChatWith.Size = new System.Drawing.Size(696, 25);
            this.lblChatWith.TabIndex = 1;
            this.lblChatWith.Text = "Select a user to start chat";
            // 
            // btnEmoji
            // 
            this.btnEmoji.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.btnEmoji.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnEmoji.FlatAppearance.BorderSize = 0;
            this.btnEmoji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmoji.Font = new System.Drawing.Font("Segoe UI Emoji", 12F);
            this.btnEmoji.ForeColor = System.Drawing.Color.White;
            this.btnEmoji.Location = new System.Drawing.Point(488, 8);
            this.btnEmoji.Name = "btnEmoji";
            this.btnEmoji.Size = new System.Drawing.Size(40, 34);
            this.btnEmoji.TabIndex = 4;
            this.btnEmoji.Text = "😀";
            this.btnEmoji.UseVisualStyleBackColor = false;
            this.btnEmoji.Click += new System.EventHandler(this.btnEmoji_Click);
            // 
            // lblTyping
            // 
            this.lblTyping.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblTyping.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblTyping.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTyping.Location = new System.Drawing.Point(0, 450);
            this.lblTyping.Name = "lblTyping";
            this.lblTyping.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblTyping.Size = new System.Drawing.Size(696, 15);
            this.lblTyping.TabIndex = 5;
            this.lblTyping.Text = "";
            // 
            // btnGroupChat
            // 
            this.btnGroupChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(100)))), ((int)(((byte)(160)))));
            this.btnGroupChat.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGroupChat.FlatAppearance.BorderSize = 0;
            this.btnGroupChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGroupChat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGroupChat.ForeColor = System.Drawing.Color.White;
            this.btnGroupChat.Location = new System.Drawing.Point(5, 30);
            this.btnGroupChat.Name = "btnGroupChat";
            this.btnGroupChat.Size = new System.Drawing.Size(190, 28);
            this.btnGroupChat.TabIndex = 5;
            this.btnGroupChat.Text = "📢 Group Chat";
            this.btnGroupChat.UseVisualStyleBackColor = false;
            this.btnGroupChat.Click += new System.EventHandler(this.btnGroupChat_Click);
            //
            // frmChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlTopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "frmChat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChatBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmChat_FormClosing);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlChatBottom.ResumeLayout(false);
            this.pnlChatBottom.PerformLayout();
            this.pnlTopBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Label lblUsers;
        private System.Windows.Forms.Panel pnlChatBottom;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.Button btnVideoCall;
        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.Label lblChatWith;
        private System.Windows.Forms.Label lblTyping;
        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Button btnEmoji;
        private System.Windows.Forms.Button btnGroupChat;
        private System.Windows.Forms.Label lblCurrentUser;
    }
}
