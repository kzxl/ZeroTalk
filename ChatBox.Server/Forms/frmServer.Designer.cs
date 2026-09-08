namespace ChatBox.Server.Forms
{
    partial class frmServer
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
            this.components = new System.ComponentModel.Container();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnLaunchClients = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.lblPort = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblStats = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.lvClients = new System.Windows.Forms.ListView();
            this.colStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEndpoint = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colConnected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPackets = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsClients = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiKick = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.lblClients = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.lblLog = new System.Windows.Forms.Label();
            this.tmrStats = new System.Windows.Forms.Timer(this.components);
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.cmsClients.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.pnlTop.Controls.Add(this.btnClearLog);
            this.pnlTop.Controls.Add(this.btnLaunchClients);
            this.pnlTop.Controls.Add(this.lblStatus);
            this.pnlTop.Controls.Add(this.nudPort);
            this.pnlTop.Controls.Add(this.lblPort);
            this.pnlTop.Controls.Add(this.btnStop);
            this.pnlTop.Controls.Add(this.btnStart);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTop.Size = new System.Drawing.Size(920, 60);
            this.pnlTop.TabIndex = 0;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.btnClearLog.FlatAppearance.BorderSize = 0;
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.ForeColor = System.Drawing.Color.White;
            this.btnClearLog.Location = new System.Drawing.Point(680, 15);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(85, 30);
            this.btnClearLog.TabIndex = 6;
            this.btnClearLog.Text = "🗑 Xoá Log";
            this.btnClearLog.UseVisualStyleBackColor = false;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnLaunchClients
            // 
            this.btnLaunchClients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLaunchClients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnLaunchClients.FlatAppearance.BorderSize = 0;
            this.btnLaunchClients.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLaunchClients.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnLaunchClients.ForeColor = System.Drawing.Color.White;
            this.btnLaunchClients.Location = new System.Drawing.Point(775, 15);
            this.btnLaunchClients.Name = "btnLaunchClients";
            this.btnLaunchClients.Size = new System.Drawing.Size(135, 30);
            this.btnLaunchClients.TabIndex = 5;
            this.btnLaunchClients.Text = "🚀 Mở 2 Client Demo";
            this.btnLaunchClients.UseVisualStyleBackColor = false;
            this.btnLaunchClients.Click += new System.EventHandler(this.btnLaunchClients_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(375, 21);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(95, 17);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "● Server dừng";
            // 
            // nudPort
            // 
            this.nudPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(28)))));
            this.nudPort.ForeColor = System.Drawing.Color.White;
            this.nudPort.Location = new System.Drawing.Point(55, 19);
            this.nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.nudPort.Minimum = new decimal(new int[] { 1024, 0, 0, 0 });
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(80, 23);
            this.nudPort.TabIndex = 3;
            this.nudPort.Value = new decimal(new int[] { 9000, 0, 0, 0 });
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.ForeColor = System.Drawing.Color.White;
            this.lblPort.Location = new System.Drawing.Point(15, 22);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 15);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnStop.Enabled = false;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(260, 15);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 30);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "⏹ Dừng";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(50)))));
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(150, 15);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 30);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "▶ Khởi động";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(28)))));
            this.pnlBottom.Controls.Add(this.lblStats);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 485);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pnlBottom.Size = new System.Drawing.Size(920, 30);
            this.pnlBottom.TabIndex = 2;
            // 
            // lblStats
            // 
            this.lblStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStats.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(185)))));
            this.lblStats.Location = new System.Drawing.Point(10, 5);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(900, 20);
            this.lblStats.TabIndex = 0;
            this.lblStats.Text = "⏱ Uptime: --:--:-- | 📦 Gói tin đã chuyển: 0 | 🌐 Local IP: 127.0.0.1";
            this.lblStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 60);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(34)))));
            this.splitContainer.Panel1.Controls.Add(this.lvClients);
            this.splitContainer.Panel1.Controls.Add(this.lblClients);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(8);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(34)))));
            this.splitContainer.Panel2.Controls.Add(this.txtLog);
            this.splitContainer.Panel2.Controls.Add(this.lblLog);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(8);
            this.splitContainer.Size = new System.Drawing.Size(920, 425);
            this.splitContainer.SplitterDistance = 450;
            this.splitContainer.TabIndex = 1;
            // 
            // lvClients
            // 
            this.lvClients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(24)))));
            this.lvClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colStatus,
            this.colUser,
            this.colName,
            this.colEndpoint,
            this.colConnected,
            this.colPackets});
            this.lvClients.ContextMenuStrip = this.cmsClients;
            this.lvClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvClients.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lvClients.ForeColor = System.Drawing.Color.White;
            this.lvClients.FullRowSelect = true;
            this.lvClients.GridLines = true;
            this.lvClients.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvClients.Location = new System.Drawing.Point(8, 30);
            this.lvClients.MultiSelect = false;
            this.lvClients.Name = "lvClients";
            this.lvClients.Size = new System.Drawing.Size(434, 387);
            this.lvClients.TabIndex = 1;
            this.lvClients.UseCompatibleStateImageBehavior = false;
            this.lvClients.View = System.Windows.Forms.View.Details;
            // 
            // colStatus
            // 
            this.colStatus.Text = "Trạng thái";
            this.colStatus.Width = 75;
            // 
            // colUser
            // 
            this.colUser.Text = "Username";
            this.colUser.Width = 80;
            // 
            // colName
            // 
            this.colName.Text = "Tên hiển thị";
            this.colName.Width = 95;
            // 
            // colEndpoint
            // 
            this.colEndpoint.Text = "Địa chỉ IP";
            this.colEndpoint.Width = 110;
            // 
            // colConnected
            // 
            this.colConnected.Text = "Kết nối";
            this.colConnected.Width = 70;
            // 
            // colPackets
            // 
            this.colPackets.Text = "Gói tin";
            this.colPackets.Width = 70;
            // 
            // cmsClients
            // 
            this.cmsClients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiKick,
            this.tsmiRefresh});
            this.cmsClients.Name = "cmsClients";
            this.cmsClients.Size = new System.Drawing.Size(193, 48);
            // 
            // tsmiKick
            // 
            this.tsmiKick.Name = "tsmiKick";
            this.tsmiKick.Size = new System.Drawing.Size(192, 22);
            this.tsmiKick.Text = "⛔ Kick / Ngắt kết nối";
            this.tsmiKick.Click += new System.EventHandler(this.tsmiKick_Click);
            // 
            // tsmiRefresh
            // 
            this.tsmiRefresh.Name = "tsmiRefresh";
            this.tsmiRefresh.Size = new System.Drawing.Size(192, 22);
            this.tsmiRefresh.Text = "🔄 Làm mới danh sách";
            this.tsmiRefresh.Click += new System.EventHandler(this.tsmiRefresh_Click);
            // 
            // lblClients
            // 
            this.lblClients.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblClients.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblClients.ForeColor = System.Drawing.Color.White;
            this.lblClients.Location = new System.Drawing.Point(8, 8);
            this.lblClients.Name = "lblClients";
            this.lblClients.Size = new System.Drawing.Size(434, 22);
            this.lblClients.TabIndex = 0;
            this.lblClients.Text = "📡 Clients Online (0)";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(24)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtLog.Location = new System.Drawing.Point(8, 30);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(450, 387);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // lblLog
            // 
            this.lblLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLog.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblLog.ForeColor = System.Drawing.Color.White;
            this.lblLog.Location = new System.Drawing.Point(8, 8);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(450, 22);
            this.lblLog.TabIndex = 0;
            this.lblLog.Text = "📋 Server Log";
            // 
            // tmrStats
            // 
            this.tmrStats.Interval = 1000;
            this.tmrStats.Tick += new System.EventHandler(this.tmrStats_Tick);
            // 
            // frmServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(920, 515);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(750, 450);
            this.Name = "frmServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChatBox Server Dashboard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmServer_FormClosing);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.cmsClients.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnLaunchClients;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView lvClients;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.ColumnHeader colUser;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colEndpoint;
        private System.Windows.Forms.ColumnHeader colConnected;
        private System.Windows.Forms.ColumnHeader colPackets;
        private System.Windows.Forms.ContextMenuStrip cmsClients;
        private System.Windows.Forms.ToolStripMenuItem tsmiKick;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefresh;
        private System.Windows.Forms.Label lblClients;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.Timer tmrStats;
    }
}
