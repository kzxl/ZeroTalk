namespace ChatBox.Client.Forms
{
    partial class frmVideoCall
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
            this.pnlRemoteVideo = new System.Windows.Forms.PictureBox();
            this.pnlLocalVideo = new System.Windows.Forms.PictureBox();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.lblConnectionMode = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.cmbVideoSource = new System.Windows.Forms.ComboBox();
            this.btnToggleVideo = new System.Windows.Forms.Button();
            this.btnEndCall = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tmrCallDuration = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pnlRemoteVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlLocalVideo)).BeginInit();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRemoteVideo
            // 
            this.pnlRemoteVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(22)))));
            this.pnlRemoteVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRemoteVideo.Location = new System.Drawing.Point(0, 0);
            this.pnlRemoteVideo.Name = "pnlRemoteVideo";
            this.pnlRemoteVideo.Size = new System.Drawing.Size(720, 465);
            this.pnlRemoteVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pnlRemoteVideo.TabIndex = 0;
            this.pnlRemoteVideo.TabStop = false;
            // 
            // pnlLocalVideo
            // 
            this.pnlLocalVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLocalVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(34)))));
            this.pnlLocalVideo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLocalVideo.Location = new System.Drawing.Point(540, 325);
            this.pnlLocalVideo.Name = "pnlLocalVideo";
            this.pnlLocalVideo.Size = new System.Drawing.Size(165, 125);
            this.pnlLocalVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pnlLocalVideo.TabIndex = 1;
            this.pnlLocalVideo.TabStop = false;
            // 
            // pnlControls
            // 
            this.pnlControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
            this.pnlControls.Controls.Add(this.lblConnectionMode);
            this.pnlControls.Controls.Add(this.lblDuration);
            this.pnlControls.Controls.Add(this.cmbVideoSource);
            this.pnlControls.Controls.Add(this.btnToggleVideo);
            this.pnlControls.Controls.Add(this.btnRecord);
            this.pnlControls.Controls.Add(this.btnEndCall);
            this.pnlControls.Controls.Add(this.lblStatus);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControls.Location = new System.Drawing.Point(0, 465);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlControls.Size = new System.Drawing.Size(720, 55);
            this.pnlControls.TabIndex = 2;
            // 
            // lblConnectionMode
            // 
            this.lblConnectionMode.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblConnectionMode.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblConnectionMode.ForeColor = System.Drawing.Color.LightGreen;
            this.lblConnectionMode.Location = new System.Drawing.Point(210, 8);
            this.lblConnectionMode.Name = "lblConnectionMode";
            this.lblConnectionMode.Size = new System.Drawing.Size(110, 39);
            this.lblConnectionMode.TabIndex = 6;
            this.lblConnectionMode.Text = "🟢 P2P Direct";
            this.lblConnectionMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDuration
            // 
            this.lblDuration.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDuration.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDuration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblDuration.Location = new System.Drawing.Point(145, 8);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(65, 39);
            this.lblDuration.TabIndex = 5;
            this.lblDuration.Text = "⏱ 00:00";
            this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbVideoSource
            // 
            this.cmbVideoSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(52)))));
            this.cmbVideoSource.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmbVideoSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVideoSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVideoSource.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbVideoSource.ForeColor = System.Drawing.Color.White;
            this.cmbVideoSource.FormattingEnabled = true;
            this.cmbVideoSource.Items.AddRange(new object[] {
            "Synthetic Camera",
            "Screen Share"});
            this.cmbVideoSource.Location = new System.Drawing.Point(325, 8);
            this.cmbVideoSource.Name = "cmbVideoSource";
            this.cmbVideoSource.Size = new System.Drawing.Size(130, 23);
            this.cmbVideoSource.TabIndex = 4;
            this.cmbVideoSource.SelectedIndexChanged += new System.EventHandler(this.cmbVideoSource_SelectedIndexChanged);
            // 
            // btnToggleVideo
            // 
            this.btnToggleVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.btnToggleVideo.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnToggleVideo.FlatAppearance.BorderSize = 0;
            this.btnToggleVideo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleVideo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnToggleVideo.ForeColor = System.Drawing.Color.White;
            this.btnToggleVideo.Location = new System.Drawing.Point(460, 8);
            this.btnToggleVideo.Name = "btnToggleVideo";
            this.btnToggleVideo.Size = new System.Drawing.Size(85, 39);
            this.btnToggleVideo.TabIndex = 3;
            this.btnToggleVideo.Text = "📷 Tắt Cam";
            this.btnToggleVideo.UseVisualStyleBackColor = false;
            this.btnToggleVideo.Click += new System.EventHandler(this.btnToggleVideo_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(72)))));
            this.btnRecord.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRecord.FlatAppearance.BorderSize = 0;
            this.btnRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecord.ForeColor = System.Drawing.Color.White;
            this.btnRecord.Location = new System.Drawing.Point(550, 8);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(75, 39);
            this.btnRecord.TabIndex = 1;
            this.btnRecord.Text = "⏺ Ghi";
            this.btnRecord.UseVisualStyleBackColor = false;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnEndCall
            // 
            this.btnEndCall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnEndCall.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnEndCall.FlatAppearance.BorderSize = 0;
            this.btnEndCall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEndCall.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnEndCall.ForeColor = System.Drawing.Color.White;
            this.btnEndCall.Location = new System.Drawing.Point(630, 8);
            this.btnEndCall.Name = "btnEndCall";
            this.btnEndCall.Size = new System.Drawing.Size(80, 39);
            this.btnEndCall.TabIndex = 0;
            this.btnEndCall.Text = "📞 Dừng";
            this.btnEndCall.UseVisualStyleBackColor = false;
            this.btnEndCall.Click += new System.EventHandler(this.btnEndCall_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(10, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(135, 39);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "📹 Đang đàm thoại";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tmrCallDuration
            // 
            this.tmrCallDuration.Interval = 1000;
            this.tmrCallDuration.Tick += new System.EventHandler(this.tmrCallDuration_Tick);
            // 
            // frmVideoCall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(16)))));
            this.ClientSize = new System.Drawing.Size(720, 520);
            this.Controls.Add(this.pnlLocalVideo);
            this.Controls.Add(this.pnlRemoteVideo);
            this.Controls.Add(this.pnlControls);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(600, 420);
            this.Name = "frmVideoCall";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Video Call";
            ((System.ComponentModel.ISupportInitialize)(this.pnlRemoteVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlLocalVideo)).EndInit();
            this.pnlControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pnlRemoteVideo;
        private System.Windows.Forms.PictureBox pnlLocalVideo;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Button btnEndCall;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnToggleVideo;
        private System.Windows.Forms.ComboBox cmbVideoSource;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblConnectionMode;
        private System.Windows.Forms.Timer tmrCallDuration;
    }
}
