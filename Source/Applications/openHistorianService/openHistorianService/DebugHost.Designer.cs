namespace openHistorianService
{
    partial class DebugHost
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugHost));
            this.LabelNotice = new System.Windows.Forms.Label();
            this.m_serviceHost = new openHistorianService.ServiceHost(this.components);
            this.m_notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.m_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelNotice
            // 
            this.LabelNotice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LabelNotice.Location = new System.Drawing.Point(10, 10);
            this.LabelNotice.Name = "LabelNotice";
            this.LabelNotice.Size = new System.Drawing.Size(324, 53);
            this.LabelNotice.TabIndex = 1;
            this.LabelNotice.Text = resources.GetString("LabelNotice.Text");
            this.LabelNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_serviceHost
            // 
            this.m_serviceHost.ExitCode = 0;
            // 
            // m_notifyIcon
            // 
            this.m_notifyIcon.ContextMenuStrip = this.m_contextMenuStrip;
            this.m_notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("m_notifyIcon.Icon")));
            this.m_notifyIcon.Text = "{0} (Debug Mode)";
            this.m_notifyIcon.Visible = true;
            // 
            // m_contextMenuStrip
            // 
            this.m_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_showToolStripMenuItem,
            this.m_toolStripSeparator1,
            this.m_exitToolStripMenuItem});
            this.m_contextMenuStrip.Name = "contextMenuStrip";
            this.m_contextMenuStrip.ShowImageMargin = false;
            this.m_contextMenuStrip.Size = new System.Drawing.Size(128, 76);
            // 
            // showToolStripMenuItem
            // 
            this.m_showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.m_showToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.m_showToolStripMenuItem.Text = "Show";
            this.m_showToolStripMenuItem.Click += new System.EventHandler(this.ShowToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.m_toolStripSeparator1.Name = "toolStripSeparator1";
            this.m_toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.m_exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.m_exitToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.m_exitToolStripMenuItem.Text = "Exit {0}";
            this.m_exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // DebugHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 73);
            this.Controls.Add(this.LabelNotice);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DebugHost";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{0} (Debug Mode)";
            this.Load += new System.EventHandler(this.DebugHost_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugHost_FormClosing);
            this.Resize += new System.EventHandler(this.DebugHost_Resize);
            this.m_contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelNotice;
        private ServiceHost m_serviceHost;
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenuStrip m_contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem m_showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator m_toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem m_exitToolStripMenuItem;
    }
}