namespace openHistorianShell
{
    partial class FrmMain
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
            this.BtnStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtMaxMB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtLocalPort = new System.Windows.Forms.TextBox();
            this.TxtArchivePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(493, 56);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(75, 23);
            this.BtnStart.TabIndex = 0;
            this.BtnStart.Text = "Start";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Archive Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Local Port";
            // 
            // TxtMaxMB
            // 
            this.TxtMaxMB.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::openHistorianShell.Properties.Settings.Default, "MaxMB", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TxtMaxMB.Location = new System.Drawing.Point(144, 58);
            this.TxtMaxMB.Name = "TxtMaxMB";
            this.TxtMaxMB.Size = new System.Drawing.Size(76, 20);
            this.TxtMaxMB.TabIndex = 6;
            this.TxtMaxMB.Text = global::openHistorianShell.Properties.Settings.Default.MaxMB;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Max Memory Usage (MB)";
            // 
            // TxtLocalPort
            // 
            this.TxtLocalPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::openHistorianShell.Properties.Settings.Default, "PortNumber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TxtLocalPort.Location = new System.Drawing.Point(86, 32);
            this.TxtLocalPort.Name = "TxtLocalPort";
            this.TxtLocalPort.Size = new System.Drawing.Size(76, 20);
            this.TxtLocalPort.TabIndex = 4;
            this.TxtLocalPort.Text = global::openHistorianShell.Properties.Settings.Default.PortNumber;
            // 
            // TxtArchivePath
            // 
            this.TxtArchivePath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::openHistorianShell.Properties.Settings.Default, "ArchivePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TxtArchivePath.Location = new System.Drawing.Point(86, 6);
            this.TxtArchivePath.Name = "TxtArchivePath";
            this.TxtArchivePath.Size = new System.Drawing.Size(482, 20);
            this.TxtArchivePath.TabIndex = 2;
            this.TxtArchivePath.Text = global::openHistorianShell.Properties.Settings.Default.ArchivePath;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 95);
            this.Controls.Add(this.TxtMaxMB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtLocalPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtArchivePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnStart);
            this.Name = "FrmMain";
            this.Text = "openHistorian Shell";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtArchivePath;
        private System.Windows.Forms.TextBox TxtLocalPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtMaxMB;
        private System.Windows.Forms.Label label3;
    }
}

