﻿namespace ArchiveIntegrityChecker
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
            this.BtnBrowse = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkQuickScan = new System.Windows.Forms.CheckBox();
            this.lblMBToScan = new System.Windows.Forms.Label();
            this.txtMBToScan = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BtnBrowse
            // 
            this.BtnBrowse.Location = new System.Drawing.Point(12, 12);
            this.BtnBrowse.Name = "BtnBrowse";
            this.BtnBrowse.Size = new System.Drawing.Size(75, 23);
            this.BtnBrowse.TabIndex = 0;
            this.BtnBrowse.Text = "Select Files";
            this.BtnBrowse.UseVisualStyleBackColor = true;
            this.BtnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(12, 66);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(458, 232);
            this.txtResults.TabIndex = 1;
            this.txtResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtResults_KeyDown);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(93, 17);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(68, 13);
            this.lblProgress.TabIndex = 2;
            this.lblProgress.Text = "Progress: 0%";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(395, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkQuickScan
            // 
            this.chkQuickScan.AutoSize = true;
            this.chkQuickScan.Location = new System.Drawing.Point(12, 43);
            this.chkQuickScan.Name = "chkQuickScan";
            this.chkQuickScan.Size = new System.Drawing.Size(82, 17);
            this.chkQuickScan.TabIndex = 4;
            this.chkQuickScan.Text = "Quick Scan";
            this.chkQuickScan.UseVisualStyleBackColor = true;
            this.chkQuickScan.CheckedChanged += new System.EventHandler(this.chkQuickScan_CheckedChanged);
            // 
            // lblMBToScan
            // 
            this.lblMBToScan.AutoSize = true;
            this.lblMBToScan.Location = new System.Drawing.Point(152, 43);
            this.lblMBToScan.Name = "lblMBToScan";
            this.lblMBToScan.Size = new System.Drawing.Size(67, 13);
            this.lblMBToScan.TabIndex = 5;
            this.lblMBToScan.Text = "MB To Scan";
            // 
            // txtMBToScan
            // 
            this.txtMBToScan.Location = new System.Drawing.Point(225, 41);
            this.txtMBToScan.Name = "txtMBToScan";
            this.txtMBToScan.Size = new System.Drawing.Size(40, 20);
            this.txtMBToScan.TabIndex = 6;
            this.txtMBToScan.Text = "1";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 310);
            this.Controls.Add(this.txtMBToScan);
            this.Controls.Add(this.lblMBToScan);
            this.Controls.Add(this.chkQuickScan);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.BtnBrowse);
            this.Name = "FrmMain";
            this.Text = "Archive Integrity Checker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnBrowse;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkQuickScan;
        private System.Windows.Forms.Label lblMBToScan;
        private System.Windows.Forms.TextBox txtMBToScan;
    }
}

