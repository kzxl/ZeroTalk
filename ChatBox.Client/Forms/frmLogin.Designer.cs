namespace ChatBox.Client.Forms
{
    partial class frmLogin
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlDemo = new System.Windows.Forms.Panel();
            this.btnDemoCharlie = new System.Windows.Forms.Button();
            this.btnDemoBob = new System.Windows.Forms.Button();
            this.btnDemoAlice = new System.Windows.Forms.Button();
            this.lblDemo = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.pnlDemo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.pnlMain.Controls.Add(this.lblStatus);
            this.pnlMain.Controls.Add(this.pnlDemo);
            this.pnlMain.Controls.Add(this.lblDemo);
            this.pnlMain.Controls.Add(this.btnRegister);
            this.pnlMain.Controls.Add(this.btnLogin);
            this.pnlMain.Controls.Add(this.nudPort);
            this.pnlMain.Controls.Add(this.lblPort);
            this.pnlMain.Controls.Add(this.txtServer);
            this.pnlMain.Controls.Add(this.lblServer);
            this.pnlMain.Controls.Add(this.txtPassword);
            this.pnlMain.Controls.Add(this.lblPassword);
            this.pnlMain.Controls.Add(this.txtUsername);
            this.pnlMain.Controls.Add(this.lblUsername);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Location = new System.Drawing.Point(80, 20);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(25, 20, 25, 20);
            this.pnlMain.Size = new System.Drawing.Size(340, 480);
            this.pnlMain.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(25, 438);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(290, 22);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlDemo
            // 
            this.pnlDemo.Controls.Add(this.btnDemoCharlie);
            this.pnlDemo.Controls.Add(this.btnDemoBob);
            this.pnlDemo.Controls.Add(this.btnDemoAlice);
            this.pnlDemo.Location = new System.Drawing.Point(25, 395);
            this.pnlDemo.Name = "pnlDemo";
            this.pnlDemo.Size = new System.Drawing.Size(290, 34);
            this.pnlDemo.TabIndex = 12;
            // 
            // btnDemoCharlie
            // 
            this.btnDemoCharlie.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(75)))));
            this.btnDemoCharlie.FlatAppearance.BorderSize = 0;
            this.btnDemoCharlie.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDemoCharlie.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnDemoCharlie.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnDemoCharlie.Location = new System.Drawing.Point(196, 0);
            this.btnDemoCharlie.Name = "btnDemoCharlie";
            this.btnDemoCharlie.Size = new System.Drawing.Size(92, 34);
            this.btnDemoCharlie.TabIndex = 2;
            this.btnDemoCharlie.Text = "👤 Charlie";
            this.btnDemoCharlie.UseVisualStyleBackColor = false;
            this.btnDemoCharlie.Click += new System.EventHandler(this.btnDemoCharlie_Click);
            // 
            // btnDemoBob
            // 
            this.btnDemoBob.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(75)))));
            this.btnDemoBob.FlatAppearance.BorderSize = 0;
            this.btnDemoBob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDemoBob.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnDemoBob.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnDemoBob.Location = new System.Drawing.Point(98, 0);
            this.btnDemoBob.Name = "btnDemoBob";
            this.btnDemoBob.Size = new System.Drawing.Size(92, 34);
            this.btnDemoBob.TabIndex = 1;
            this.btnDemoBob.Text = "👤 Bob";
            this.btnDemoBob.UseVisualStyleBackColor = false;
            this.btnDemoBob.Click += new System.EventHandler(this.btnDemoBob_Click);
            // 
            // btnDemoAlice
            // 
            this.btnDemoAlice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(75)))));
            this.btnDemoAlice.FlatAppearance.BorderSize = 0;
            this.btnDemoAlice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDemoAlice.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnDemoAlice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnDemoAlice.Location = new System.Drawing.Point(0, 0);
            this.btnDemoAlice.Name = "btnDemoAlice";
            this.btnDemoAlice.Size = new System.Drawing.Size(92, 34);
            this.btnDemoAlice.TabIndex = 0;
            this.btnDemoAlice.Text = "👤 Alice";
            this.btnDemoAlice.UseVisualStyleBackColor = false;
            this.btnDemoAlice.Click += new System.EventHandler(this.btnDemoAlice_Click);
            // 
            // lblDemo
            // 
            this.lblDemo.AutoSize = true;
            this.lblDemo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDemo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.lblDemo.Location = new System.Drawing.Point(25, 376);
            this.lblDemo.Name = "lblDemo";
            this.lblDemo.Size = new System.Drawing.Size(201, 13);
            this.lblDemo.TabIndex = 11;
            this.lblDemo.Text = "⚡ Quick Demo Accounts (1-Click):";
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnRegister.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btnRegister.Location = new System.Drawing.Point(25, 334);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(290, 30);
            this.btnRegister.TabIndex = 10;
            this.btnRegister.Text = "Register New Account";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(25, 290);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(290, 38);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Sign In";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // nudPort
            // 
            this.nudPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.nudPort.ForeColor = System.Drawing.Color.White;
            this.nudPort.Location = new System.Drawing.Point(215, 247);
            this.nudPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudPort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(100, 23);
            this.nudPort.TabIndex = 8;
            this.nudPort.Value = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.lblPort.Location = new System.Drawing.Point(215, 227);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 15);
            this.lblPort.TabIndex = 7;
            this.lblPort.Text = "Port:";
            // 
            // txtServer
            // 
            this.txtServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.txtServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServer.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtServer.ForeColor = System.Drawing.Color.White;
            this.txtServer.Location = new System.Drawing.Point(25, 245);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(175, 26);
            this.txtServer.TabIndex = 6;
            this.txtServer.Text = "127.0.0.1";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.lblServer.Location = new System.Drawing.Point(25, 227);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(42, 15);
            this.lblServer.TabIndex = 5;
            this.lblServer.Text = "Server:";
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtPassword.ForeColor = System.Drawing.Color.White;
            this.txtPassword.Location = new System.Drawing.Point(25, 185);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(290, 26);
            this.txtPassword.TabIndex = 4;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.lblPassword.Location = new System.Drawing.Point(25, 165);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(60, 15);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtUsername.ForeColor = System.Drawing.Color.White;
            this.txtUsername.Location = new System.Drawing.Point(25, 125);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(290, 26);
            this.txtUsername.TabIndex = 2;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.lblUsername.Location = new System.Drawing.Point(25, 105);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(63, 15);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Username:";
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(255)))));
            this.lblTitle.Location = new System.Drawing.Point(25, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(290, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "💬 ChatBox";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmLogin
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(500, 525);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChatBox - Sign In";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlDemo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label lblDemo;
        private System.Windows.Forms.Panel pnlDemo;
        private System.Windows.Forms.Button btnDemoAlice;
        private System.Windows.Forms.Button btnDemoBob;
        private System.Windows.Forms.Button btnDemoCharlie;
        private System.Windows.Forms.Label lblStatus;
    }
}
