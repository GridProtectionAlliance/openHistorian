namespace openVisN
{
    partial class FrmConfigure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConfigure));
            this.BtnGetMetadata = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtServerIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtHistorianPort = new System.Windows.Forms.TextBox();
            this.TxtHistorianInstance = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtGEPPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.BtnRefreshGroups = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dgvMeasurements = new System.Windows.Forms.DataGridView();
            this.dgvTerminals = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMeasurements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTerminals)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnGetMetadata
            // 
            this.BtnGetMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnGetMetadata.Location = new System.Drawing.Point(519, 12);
            this.BtnGetMetadata.Name = "BtnGetMetadata";
            this.BtnGetMetadata.Size = new System.Drawing.Size(100, 23);
            this.BtnGetMetadata.TabIndex = 0;
            this.BtnGetMetadata.Text = "Refresh Metadata";
            this.BtnGetMetadata.UseVisualStyleBackColor = true;
            this.BtnGetMetadata.Click += new System.EventHandler(this.BtnGetMetadata_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server IP";
            // 
            // TxtServerIP
            // 
            this.TxtServerIP.Location = new System.Drawing.Point(154, 11);
            this.TxtServerIP.Name = "TxtServerIP";
            this.TxtServerIP.Size = new System.Drawing.Size(100, 20);
            this.TxtServerIP.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Historian Port";
            // 
            // TxtHistorianPort
            // 
            this.TxtHistorianPort.Location = new System.Drawing.Point(154, 40);
            this.TxtHistorianPort.Name = "TxtHistorianPort";
            this.TxtHistorianPort.Size = new System.Drawing.Size(100, 20);
            this.TxtHistorianPort.TabIndex = 4;
            this.TxtHistorianPort.Text = "38402";
            // 
            // TxtHistorianInstance
            // 
            this.TxtHistorianInstance.Location = new System.Drawing.Point(154, 66);
            this.TxtHistorianInstance.Name = "TxtHistorianInstance";
            this.TxtHistorianInstance.Size = new System.Drawing.Size(100, 20);
            this.TxtHistorianInstance.TabIndex = 6;
            this.TxtHistorianInstance.Text = "PPA";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Historian Instance Name";
            // 
            // TxtGEPPort
            // 
            this.TxtGEPPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtGEPPort.Location = new System.Drawing.Point(457, 14);
            this.TxtGEPPort.Name = "TxtGEPPort";
            this.TxtGEPPort.Size = new System.Drawing.Size(56, 20);
            this.TxtGEPPort.TabIndex = 8;
            this.TxtGEPPort.Text = "6175";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(400, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "GEP Port";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.BtnRefreshGroups);
            this.splitContainer1.Panel1.Controls.Add(this.BtnClose);
            this.splitContainer1.Panel1.Controls.Add(this.BtnSave);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.TxtGEPPort);
            this.splitContainer1.Panel1.Controls.Add(this.BtnGetMetadata);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.TxtServerIP);
            this.splitContainer1.Panel1.Controls.Add(this.TxtHistorianInstance);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.TxtHistorianPort);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(631, 571);
            this.splitContainer1.SplitterDistance = 92;
            this.splitContainer1.TabIndex = 9;
            // 
            // BtnRefreshGroups
            // 
            this.BtnRefreshGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRefreshGroups.Location = new System.Drawing.Point(359, 63);
            this.BtnRefreshGroups.Name = "BtnRefreshGroups";
            this.BtnRefreshGroups.Size = new System.Drawing.Size(100, 23);
            this.BtnRefreshGroups.TabIndex = 10;
            this.BtnRefreshGroups.Text = "Refresh Groups";
            this.BtnRefreshGroups.UseVisualStyleBackColor = true;
            this.BtnRefreshGroups.Click += new System.EventHandler(this.BtnRefreshGroups_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(545, 63);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(74, 23);
            this.BtnClose.TabIndex = 9;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(465, 63);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(74, 23);
            this.BtnSave.TabIndex = 9;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dgvMeasurements);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dgvTerminals);
            this.splitContainer2.Size = new System.Drawing.Size(631, 475);
            this.splitContainer2.SplitterDistance = 226;
            this.splitContainer2.TabIndex = 0;
            // 
            // dgvMeasurements
            // 
            this.dgvMeasurements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMeasurements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMeasurements.Location = new System.Drawing.Point(0, 0);
            this.dgvMeasurements.Name = "dgvMeasurements";
            this.dgvMeasurements.Size = new System.Drawing.Size(631, 226);
            this.dgvMeasurements.TabIndex = 0;
            // 
            // dgvTerminals
            // 
            this.dgvTerminals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTerminals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTerminals.Location = new System.Drawing.Point(0, 0);
            this.dgvTerminals.Name = "dgvTerminals";
            this.dgvTerminals.Size = new System.Drawing.Size(631, 245);
            this.dgvTerminals.TabIndex = 0;
            this.dgvTerminals.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvTerminals_KeyDown);
            // 
            // FrmConfigure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 571);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmConfigure";
            this.Text = "Configure";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMeasurements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTerminals)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnGetMetadata;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtServerIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtHistorianPort;
        private System.Windows.Forms.TextBox TxtHistorianInstance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtGEPPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgvMeasurements;
        private System.Windows.Forms.DataGridView dgvTerminals;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnRefreshGroups;
    }
}