namespace openVisN.Components
{
    partial class ManualAutomaticModeSelectorButton
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshIntervalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minuteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minuteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.playbackSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolCustomPlaybackSpeed = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.ContextMenuStrip = this.contextMenuStrip1;
            this.btnStartStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartStop.Location = new System.Drawing.Point(0, 0);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(76, 32);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Running";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshIntervalToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.playbackSpeedToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(157, 92);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // refreshIntervalToolStripMenuItem
            // 
            this.refreshIntervalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.secondToolStripMenuItem,
            this.secondsToolStripMenuItem});
            this.refreshIntervalToolStripMenuItem.Name = "refreshIntervalToolStripMenuItem";
            this.refreshIntervalToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.refreshIntervalToolStripMenuItem.Text = "Refresh Interval";
            // 
            // secondToolStripMenuItem
            // 
            this.secondToolStripMenuItem.Name = "secondToolStripMenuItem";
            this.secondToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.secondToolStripMenuItem.Text = "1 second";
            // 
            // secondsToolStripMenuItem
            // 
            this.secondsToolStripMenuItem.Name = "secondsToolStripMenuItem";
            this.secondsToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.secondsToolStripMenuItem.Text = "2 seconds";
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.minuteToolStripMenuItem,
            this.minuteToolStripMenuItem1});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // minuteToolStripMenuItem
            // 
            this.minuteToolStripMenuItem.Name = "minuteToolStripMenuItem";
            this.minuteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.minuteToolStripMenuItem.Text = "5 Minute";
            // 
            // minuteToolStripMenuItem1
            // 
            this.minuteToolStripMenuItem1.Name = "minuteToolStripMenuItem1";
            this.minuteToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.minuteToolStripMenuItem1.Text = "10 Minute";
            // 
            // playbackSpeedToolStripMenuItem
            // 
            this.playbackSpeedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xToolStripMenuItem,
            this.xToolStripMenuItem2,
            this.xToolStripMenuItem4,
            this.toolStripSeparator1,
            this.toolCustomPlaybackSpeed});
            this.playbackSpeedToolStripMenuItem.Name = "playbackSpeedToolStripMenuItem";
            this.playbackSpeedToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.playbackSpeedToolStripMenuItem.Text = "Playback Speed";
            // 
            // xToolStripMenuItem
            // 
            this.xToolStripMenuItem.Name = "xToolStripMenuItem";
            this.xToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.xToolStripMenuItem.Text = "1x";
            this.xToolStripMenuItem.Click += new System.EventHandler(this.xToolStripMenuItem_Click);
            // 
            // xToolStripMenuItem2
            // 
            this.xToolStripMenuItem2.Name = "xToolStripMenuItem2";
            this.xToolStripMenuItem2.Size = new System.Drawing.Size(160, 22);
            this.xToolStripMenuItem2.Text = "3x";
            this.xToolStripMenuItem2.Click += new System.EventHandler(this.xToolStripMenuItem2_Click);
            // 
            // xToolStripMenuItem4
            // 
            this.xToolStripMenuItem4.Name = "xToolStripMenuItem4";
            this.xToolStripMenuItem4.Size = new System.Drawing.Size(160, 22);
            this.xToolStripMenuItem4.Text = "5x";
            this.xToolStripMenuItem4.Click += new System.EventHandler(this.xToolStripMenuItem4_Click);
            // 
            // toolCustomPlaybackSpeed
            // 
            this.toolCustomPlaybackSpeed.Name = "toolCustomPlaybackSpeed";
            this.toolCustomPlaybackSpeed.Size = new System.Drawing.Size(100, 23);
            this.toolCustomPlaybackSpeed.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolCustomPlaybackSpeed_KeyDown);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // ManualAutomaticModeSelectorButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnStartStop);
            this.Name = "ManualAutomaticModeSelectorButton";
            this.Size = new System.Drawing.Size(76, 32);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshIntervalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minuteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minuteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem playbackSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox toolCustomPlaybackSpeed;
    }
}
