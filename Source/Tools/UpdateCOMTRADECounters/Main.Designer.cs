
namespace UpdateCOMTRADECounters
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.maskedTextBoxBinaryByteCount = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxEndSampleCount = new System.Windows.Forms.MaskedTextBox();
            this.labelBinaryByteCount = new System.Windows.Forms.Label();
            this.labelEndSampleCount = new System.Windows.Forms.Label();
            this.buttonOpenSourceCFFLocation = new System.Windows.Forms.Button();
            this.labelSourceCFF = new System.Windows.Forms.Label();
            this.textBoxSourceCFF = new System.Windows.Forms.TextBox();
            this.openFileDialogCFF = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSelectDownloadedCFF = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timerFlashButton = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // maskedTextBoxBinaryByteCount
            // 
            this.maskedTextBoxBinaryByteCount.Location = new System.Drawing.Point(351, 19);
            this.maskedTextBoxBinaryByteCount.Margin = new System.Windows.Forms.Padding(2);
            this.maskedTextBoxBinaryByteCount.Mask = "000000000000000";
            this.maskedTextBoxBinaryByteCount.Name = "maskedTextBoxBinaryByteCount";
            this.maskedTextBoxBinaryByteCount.Size = new System.Drawing.Size(106, 20);
            this.maskedTextBoxBinaryByteCount.TabIndex = 3;
            this.maskedTextBoxBinaryByteCount.Text = "0";
            this.maskedTextBoxBinaryByteCount.ValidatingType = typeof(int);
            this.maskedTextBoxBinaryByteCount.Enter += new System.EventHandler(this.textBox_Enter);
            // 
            // maskedTextBoxEndSampleCount
            // 
            this.maskedTextBoxEndSampleCount.Location = new System.Drawing.Point(166, 19);
            this.maskedTextBoxEndSampleCount.Margin = new System.Windows.Forms.Padding(2);
            this.maskedTextBoxEndSampleCount.Mask = "0000000000";
            this.maskedTextBoxEndSampleCount.Name = "maskedTextBoxEndSampleCount";
            this.maskedTextBoxEndSampleCount.Size = new System.Drawing.Size(76, 20);
            this.maskedTextBoxEndSampleCount.TabIndex = 1;
            this.maskedTextBoxEndSampleCount.Text = "0";
            this.maskedTextBoxEndSampleCount.ValidatingType = typeof(int);
            this.maskedTextBoxEndSampleCount.Enter += new System.EventHandler(this.textBox_Enter);
            // 
            // labelBinaryByteCount
            // 
            this.labelBinaryByteCount.AutoSize = true;
            this.labelBinaryByteCount.Location = new System.Drawing.Point(253, 22);
            this.labelBinaryByteCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBinaryByteCount.Name = "labelBinaryByteCount";
            this.labelBinaryByteCount.Size = new System.Drawing.Size(94, 13);
            this.labelBinaryByteCount.TabIndex = 2;
            this.labelBinaryByteCount.Text = "&Binary Byte Count:";
            this.labelBinaryByteCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelEndSampleCount
            // 
            this.labelEndSampleCount.AutoSize = true;
            this.labelEndSampleCount.Location = new System.Drawing.Point(64, 22);
            this.labelEndSampleCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelEndSampleCount.Name = "labelEndSampleCount";
            this.labelEndSampleCount.Size = new System.Drawing.Size(98, 13);
            this.labelEndSampleCount.TabIndex = 0;
            this.labelEndSampleCount.Text = "&End Sample Count:";
            this.labelEndSampleCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOpenSourceCFFLocation
            // 
            this.buttonOpenSourceCFFLocation.BackColor = System.Drawing.SystemColors.Control;
            this.buttonOpenSourceCFFLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOpenSourceCFFLocation.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenSourceCFFLocation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonOpenSourceCFFLocation.Location = new System.Drawing.Point(456, 61);
            this.buttonOpenSourceCFFLocation.Margin = new System.Windows.Forms.Padding(0);
            this.buttonOpenSourceCFFLocation.Name = "buttonOpenSourceCFFLocation";
            this.buttonOpenSourceCFFLocation.Size = new System.Drawing.Size(28, 20);
            this.buttonOpenSourceCFFLocation.TabIndex = 6;
            this.buttonOpenSourceCFFLocation.Text = "...";
            this.buttonOpenSourceCFFLocation.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip.SetToolTip(this.buttonOpenSourceCFFLocation, "Select last downloaded COMTRADE CFF...");
            this.buttonOpenSourceCFFLocation.UseVisualStyleBackColor = false;
            this.buttonOpenSourceCFFLocation.Click += new System.EventHandler(this.buttonOpenSourceCFFLocation_Click);
            this.buttonOpenSourceCFFLocation.MouseEnter += new System.EventHandler(this.buttonOpenSourceCFFLocation_MouseEnter);
            // 
            // labelSourceCFF
            // 
            this.labelSourceCFF.AutoSize = true;
            this.labelSourceCFF.Location = new System.Drawing.Point(64, 64);
            this.labelSourceCFF.Name = "labelSourceCFF";
            this.labelSourceCFF.Size = new System.Drawing.Size(110, 13);
            this.labelSourceCFF.TabIndex = 4;
            this.labelSourceCFF.Text = "&Source CFF Location:";
            this.labelSourceCFF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxSourceCFF
            // 
            this.textBoxSourceCFF.Location = new System.Drawing.Point(174, 61);
            this.textBoxSourceCFF.Name = "textBoxSourceCFF";
            this.textBoxSourceCFF.Size = new System.Drawing.Size(283, 20);
            this.textBoxSourceCFF.TabIndex = 5;
            this.textBoxSourceCFF.Enter += new System.EventHandler(this.textBox_Enter);
            // 
            // openFileDialogCFF
            // 
            this.openFileDialogCFF.DefaultExt = "CFF";
            this.openFileDialogCFF.Filter = "COMTRADE CFF (*.CFF)|*.CFF|All Files (*.*)|*.*";
            this.openFileDialogCFF.Title = "Select Downloaded COMTRADE CFF to Apply Counter Updates";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.label1.Location = new System.Drawing.Point(350, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Set to 0 for non-binary";
            // 
            // labelSelectDownloadedCFF
            // 
            this.labelSelectDownloadedCFF.AutoSize = true;
            this.labelSelectDownloadedCFF.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectDownloadedCFF.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelSelectDownloadedCFF.Location = new System.Drawing.Point(174, 81);
            this.labelSelectDownloadedCFF.Name = "labelSelectDownloadedCFF";
            this.labelSelectDownloadedCFF.Size = new System.Drawing.Size(138, 13);
            this.labelSelectDownloadedCFF.TabIndex = 8;
            this.labelSelectDownloadedCFF.Text = "Select last downloaded CFF";
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.BackgroundImage")));
            this.pictureBoxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxLogo.Location = new System.Drawing.Point(4, 21);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(62, 54);
            this.pictureBoxLogo.TabIndex = 9;
            this.pictureBoxLogo.TabStop = false;
            this.pictureBoxLogo.Click += new System.EventHandler(this.pictureBoxLogo_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 5000;
            this.toolTip.AutoPopDelay = 50000;
            this.toolTip.BackColor = System.Drawing.Color.Wheat;
            this.toolTip.ForeColor = System.Drawing.Color.Black;
            this.toolTip.InitialDelay = 5000;
            this.toolTip.ReshowDelay = 10;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.toolTip.ToolTipTitle = "⤊ Action Required:";
            // 
            // timerFlashButton
            // 
            this.timerFlashButton.Interval = 350;
            this.timerFlashButton.Tick += new System.EventHandler(this.timerFlashButton_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 115);
            this.Controls.Add(this.labelSelectDownloadedCFF);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOpenSourceCFFLocation);
            this.Controls.Add(this.labelSourceCFF);
            this.Controls.Add(this.textBoxSourceCFF);
            this.Controls.Add(this.maskedTextBoxBinaryByteCount);
            this.Controls.Add(this.maskedTextBoxEndSampleCount);
            this.Controls.Add(this.labelBinaryByteCount);
            this.Controls.Add(this.labelEndSampleCount);
            this.Controls.Add(this.pictureBoxLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update COMTRADE Counters";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.hideToolTip);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.Move += new System.EventHandler(this.showToolTip);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox maskedTextBoxBinaryByteCount;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxEndSampleCount;
        private System.Windows.Forms.Label labelBinaryByteCount;
        private System.Windows.Forms.Label labelEndSampleCount;
        private System.Windows.Forms.Button buttonOpenSourceCFFLocation;
        private System.Windows.Forms.Label labelSourceCFF;
        private System.Windows.Forms.TextBox textBoxSourceCFF;
        private System.Windows.Forms.OpenFileDialog openFileDialogCFF;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSelectDownloadedCFF;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer timerFlashButton;
    }
}

