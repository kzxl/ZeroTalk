namespace ZeroTalk.Client.Forms
{
    partial class frmImagePreview
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
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnOpenExternal = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // picPreview
            // 
            this.picPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(20)))));
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.Location = new System.Drawing.Point(0, 0);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(684, 461);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 0;
            this.picPreview.TabStop = false;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(34)))));
            this.pnlBottom.Controls.Add(this.lblInfo);
            this.pnlBottom.Controls.Add(this.btnOpenExternal);
            this.pnlBottom.Controls.Add(this.btnOpenFolder);
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 461);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlBottom.Size = new System.Drawing.Size(684, 48);
            this.pnlBottom.TabIndex = 1;
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblInfo.Location = new System.Drawing.Point(12, 8);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(370, 32);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Image details";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOpenExternal
            // 
            this.btnOpenExternal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.btnOpenExternal.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOpenExternal.FlatAppearance.BorderSize = 0;
            this.btnOpenExternal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenExternal.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenExternal.ForeColor = System.Drawing.Color.White;
            this.btnOpenExternal.Location = new System.Drawing.Point(382, 8);
            this.btnOpenExternal.Name = "btnOpenExternal";
            this.btnOpenExternal.Size = new System.Drawing.Size(100, 32);
            this.btnOpenExternal.TabIndex = 1;
            this.btnOpenExternal.Text = "Open File";
            this.btnOpenExternal.UseVisualStyleBackColor = false;
            this.btnOpenExternal.Click += new System.EventHandler(this.btnOpenExternal_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(68)))));
            this.btnOpenFolder.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOpenFolder.FlatAppearance.BorderSize = 0;
            this.btnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenFolder.ForeColor = System.Drawing.Color.White;
            this.btnOpenFolder.Location = new System.Drawing.Point(490, 8);
            this.btnOpenFolder.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(105, 32);
            this.btnOpenFolder.TabIndex = 2;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(603, 8);
            this.btnClose.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(69, 32);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmImagePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(684, 509);
            this.Controls.Add(this.picPreview);
            this.Controls.Add(this.pnlBottom);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(480, 360);
            this.Name = "frmImagePreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Image Preview";
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnOpenExternal;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnClose;
    }
}
