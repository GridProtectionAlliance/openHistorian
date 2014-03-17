namespace DataExtractionUtility
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
            this.label1 = new System.Windows.Forms.Label();
            this.BtnGetMetadata = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtGEPPort = new System.Windows.Forms.TextBox();
            this.TxtServerIP = new System.Windows.Forms.TextBox();
            this.TxtHistorianInstance = new System.Windows.Forms.TextBox();
            this.TxtHistorianPort = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCsvMaxFileCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCsvMaxColumnCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMaxFileSize = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCsvMaxRowCount = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.LblEstimatedSize = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbResolution = new System.Windows.Forms.ComboBox();
            this.dateStopTime = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.dateStartTime = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.BtnExport = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslStatusUpdate = new System.Windows.Forms.ToolStripStatusLabel();
            this.ChkSignalType = new System.Windows.Forms.CheckedListBox();
            this.GrpPointSelection = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lblPointCount = new System.Windows.Forms.Label();
            this.btnCategoryCheckSelected = new System.Windows.Forms.Button();
            this.btnCategoryUncheckSelected = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.GrpPointSelection.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server IP";
            // 
            // BtnGetMetadata
            // 
            this.BtnGetMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnGetMetadata.Location = new System.Drawing.Point(141, 127);
            this.BtnGetMetadata.Name = "BtnGetMetadata";
            this.BtnGetMetadata.Size = new System.Drawing.Size(100, 23);
            this.BtnGetMetadata.TabIndex = 0;
            this.BtnGetMetadata.Text = "Get Metadata";
            this.BtnGetMetadata.UseVisualStyleBackColor = true;
            this.BtnGetMetadata.Click += new System.EventHandler(this.BtnGetMetadata_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(84, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "GEP Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Historian Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Historian Instance Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TxtGEPPort);
            this.groupBox1.Controls.Add(this.BtnGetMetadata);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TxtServerIP);
            this.groupBox1.Controls.Add(this.TxtHistorianInstance);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TxtHistorianPort);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 162);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Settings";
            // 
            // TxtGEPPort
            // 
            this.TxtGEPPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "HistorianGatewayPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TxtGEPPort.Location = new System.Drawing.Point(141, 75);
            this.TxtGEPPort.Name = "TxtGEPPort";
            this.TxtGEPPort.Size = new System.Drawing.Size(100, 20);
            this.TxtGEPPort.TabIndex = 8;
            this.TxtGEPPort.Text = global::DataExtractionUtility.Properties.Settings.Default.HistorianGatewayPort.ToString();
            // 
            // TxtServerIP
            // 
            this.TxtServerIP.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "ServerIP", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TxtServerIP.Location = new System.Drawing.Point(141, 23);
            this.TxtServerIP.Name = "TxtServerIP";
            this.TxtServerIP.Size = new System.Drawing.Size(100, 20);
            this.TxtServerIP.TabIndex = 2;
            this.TxtServerIP.Text = global::DataExtractionUtility.Properties.Settings.Default.ServerIP;
            // 
            // TxtHistorianInstance
            // 
            this.TxtHistorianInstance.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "HistorianInstanceName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TxtHistorianInstance.Location = new System.Drawing.Point(141, 101);
            this.TxtHistorianInstance.Name = "TxtHistorianInstance";
            this.TxtHistorianInstance.Size = new System.Drawing.Size(100, 20);
            this.TxtHistorianInstance.TabIndex = 6;
            this.TxtHistorianInstance.Text = global::DataExtractionUtility.Properties.Settings.Default.HistorianInstanceName;
            // 
            // TxtHistorianPort
            // 
            this.TxtHistorianPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "HistorianStreamingPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TxtHistorianPort.Location = new System.Drawing.Point(141, 49);
            this.TxtHistorianPort.Name = "TxtHistorianPort";
            this.TxtHistorianPort.Size = new System.Drawing.Size(100, 20);
            this.TxtHistorianPort.TabIndex = 4;
            this.TxtHistorianPort.Text = global::DataExtractionUtility.Properties.Settings.Default.HistorianStreamingPort.ToString();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtCsvMaxFileCount);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtCsvMaxColumnCount);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtMaxFileSize);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtCsvMaxRowCount);
            this.groupBox2.Location = new System.Drawing.Point(12, 180);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(253, 138);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CSV Options";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Maximum Files To Export";
            // 
            // txtCsvMaxFileCount
            // 
            this.txtCsvMaxFileCount.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "CsvMaxFileCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtCsvMaxFileCount.Location = new System.Drawing.Point(141, 101);
            this.txtCsvMaxFileCount.Name = "txtCsvMaxFileCount";
            this.txtCsvMaxFileCount.Size = new System.Drawing.Size(100, 20);
            this.txtCsvMaxFileCount.TabIndex = 14;
            this.txtCsvMaxFileCount.Text = global::DataExtractionUtility.Properties.Settings.Default.CsvMaxFileCount.ToString();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(120, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Maximum Column Count";
            // 
            // txtCsvMaxColumnCount
            // 
            this.txtCsvMaxColumnCount.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "CsvMaximumColumnCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtCsvMaxColumnCount.Location = new System.Drawing.Point(141, 75);
            this.txtCsvMaxColumnCount.Name = "txtCsvMaxColumnCount";
            this.txtCsvMaxColumnCount.Size = new System.Drawing.Size(100, 20);
            this.txtCsvMaxColumnCount.TabIndex = 12;
            this.txtCsvMaxColumnCount.Text = global::DataExtractionUtility.Properties.Settings.Default.CsvMaximumColumnCount.ToString();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Maximum File Size (MB)";
            // 
            // txtMaxFileSize
            // 
            this.txtMaxFileSize.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "CsvMaxFileSizeMB", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtMaxFileSize.Location = new System.Drawing.Point(141, 23);
            this.txtMaxFileSize.Name = "txtMaxFileSize";
            this.txtMaxFileSize.Size = new System.Drawing.Size(100, 20);
            this.txtMaxFileSize.TabIndex = 2;
            this.txtMaxFileSize.Text = global::DataExtractionUtility.Properties.Settings.Default.CsvMaxFileSizeMB.ToString();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Maximum Row Count";
            // 
            // txtCsvMaxRowCount
            // 
            this.txtCsvMaxRowCount.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DataExtractionUtility.Properties.Settings.Default, "CsvMaxRowCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtCsvMaxRowCount.Location = new System.Drawing.Point(141, 49);
            this.txtCsvMaxRowCount.Name = "txtCsvMaxRowCount";
            this.txtCsvMaxRowCount.Size = new System.Drawing.Size(100, 20);
            this.txtCsvMaxRowCount.TabIndex = 4;
            this.txtCsvMaxRowCount.Text = global::DataExtractionUtility.Properties.Settings.Default.CsvMaxRowCount.ToString();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.LblEstimatedSize);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.cmbResolution);
            this.groupBox3.Controls.Add(this.dateStopTime);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.dateStartTime);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.BtnExport);
            this.groupBox3.Location = new System.Drawing.Point(12, 326);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(253, 137);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Export Settings";
            // 
            // LblEstimatedSize
            // 
            this.LblEstimatedSize.AutoSize = true;
            this.LblEstimatedSize.Location = new System.Drawing.Point(15, 105);
            this.LblEstimatedSize.Name = "LblEstimatedSize";
            this.LblEstimatedSize.Size = new System.Drawing.Size(79, 13);
            this.LblEstimatedSize.TabIndex = 7;
            this.LblEstimatedSize.Text = "Estimated Size:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(17, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Resolution";
            // 
            // cmbResolution
            // 
            this.cmbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResolution.FormattingEnabled = true;
            this.cmbResolution.Location = new System.Drawing.Point(80, 71);
            this.cmbResolution.Name = "cmbResolution";
            this.cmbResolution.Size = new System.Drawing.Size(161, 21);
            this.cmbResolution.TabIndex = 5;
            this.cmbResolution.SelectedIndexChanged += new System.EventHandler(this.cmbResolution_SelectedIndexChanged);
            // 
            // dateStopTime
            // 
            this.dateStopTime.CustomFormat = "MM/dd/yyyy h:mm:ss tt";
            this.dateStopTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateStopTime.Location = new System.Drawing.Point(80, 45);
            this.dateStopTime.Name = "dateStopTime";
            this.dateStopTime.Size = new System.Drawing.Size(161, 20);
            this.dateStopTime.TabIndex = 4;
            this.dateStopTime.ValueChanged += new System.EventHandler(this.dateStopTime_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(45, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Stop";
            // 
            // dateStartTime
            // 
            this.dateStartTime.CustomFormat = "MM/dd/yyyy h:mm:ss tt";
            this.dateStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateStartTime.Location = new System.Drawing.Point(80, 19);
            this.dateStartTime.Name = "dateStartTime";
            this.dateStartTime.Size = new System.Drawing.Size(161, 20);
            this.dateStartTime.TabIndex = 2;
            this.dateStartTime.ValueChanged += new System.EventHandler(this.dateStartTime_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(45, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Start";
            // 
            // BtnExport
            // 
            this.BtnExport.Location = new System.Drawing.Point(166, 100);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(75, 23);
            this.BtnExport.TabIndex = 0;
            this.BtnExport.Text = "Export";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslStatusUpdate});
            this.statusStrip1.Location = new System.Drawing.Point(0, 470);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(620, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tslStatusUpdate
            // 
            this.tslStatusUpdate.Name = "tslStatusUpdate";
            this.tslStatusUpdate.Size = new System.Drawing.Size(42, 17);
            this.tslStatusUpdate.Text = "Status:";
            // 
            // ChkSignalType
            // 
            this.ChkSignalType.Dock = System.Windows.Forms.DockStyle.Top;
            this.ChkSignalType.FormattingEnabled = true;
            this.ChkSignalType.Location = new System.Drawing.Point(3, 3);
            this.ChkSignalType.Name = "ChkSignalType";
            this.ChkSignalType.Size = new System.Drawing.Size(318, 334);
            this.ChkSignalType.TabIndex = 15;
            this.ChkSignalType.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ChkSignalType_ItemCheck);
            // 
            // GrpPointSelection
            // 
            this.GrpPointSelection.Controls.Add(this.lblPointCount);
            this.GrpPointSelection.Controls.Add(this.tabControl1);
            this.GrpPointSelection.Location = new System.Drawing.Point(271, 12);
            this.GrpPointSelection.Name = "GrpPointSelection";
            this.GrpPointSelection.Size = new System.Drawing.Size(338, 451);
            this.GrpPointSelection.TabIndex = 17;
            this.GrpPointSelection.TabStop = false;
            this.GrpPointSelection.Text = "Point Selection";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(3, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(332, 401);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnCategoryUncheckSelected);
            this.tabPage1.Controls.Add(this.btnCategoryCheckSelected);
            this.tabPage1.Controls.Add(this.ChkSignalType);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(324, 375);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Category";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(324, 387);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Device";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(324, 387);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Points";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(324, 387);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Summary";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lblPointCount
            // 
            this.lblPointCount.AutoSize = true;
            this.lblPointCount.Location = new System.Drawing.Point(6, 424);
            this.lblPointCount.Name = "lblPointCount";
            this.lblPointCount.Size = new System.Drawing.Size(65, 13);
            this.lblPointCount.TabIndex = 8;
            this.lblPointCount.Text = "Point Count:";
            // 
            // btnCategoryCheckSelected
            // 
            this.btnCategoryCheckSelected.Location = new System.Drawing.Point(6, 343);
            this.btnCategoryCheckSelected.Name = "btnCategoryCheckSelected";
            this.btnCategoryCheckSelected.Size = new System.Drawing.Size(113, 23);
            this.btnCategoryCheckSelected.TabIndex = 16;
            this.btnCategoryCheckSelected.Text = "Check Selected";
            this.btnCategoryCheckSelected.UseVisualStyleBackColor = true;
            this.btnCategoryCheckSelected.Visible = false;
            // 
            // btnCategoryUncheckSelected
            // 
            this.btnCategoryUncheckSelected.Location = new System.Drawing.Point(125, 343);
            this.btnCategoryUncheckSelected.Name = "btnCategoryUncheckSelected";
            this.btnCategoryUncheckSelected.Size = new System.Drawing.Size(113, 23);
            this.btnCategoryUncheckSelected.TabIndex = 17;
            this.btnCategoryUncheckSelected.Text = "Uncheck Selected";
            this.btnCategoryUncheckSelected.UseVisualStyleBackColor = true;
            this.btnCategoryUncheckSelected.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(17, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Comming Soon";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(22, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(78, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Comming Soon";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(25, 28);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(78, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Comming Soon";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 492);
            this.Controls.Add(this.GrpPointSelection);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "FrmMain";
            this.Text = "Data Extraction Utility";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.GrpPointSelection.ResumeLayout(false);
            this.GrpPointSelection.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtGEPPort;
        private System.Windows.Forms.Button BtnGetMetadata;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtServerIP;
        private System.Windows.Forms.TextBox TxtHistorianInstance;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtHistorianPort;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCsvMaxFileCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCsvMaxColumnCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxFileSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCsvMaxRowCount;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tslStatusUpdate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbResolution;
        private System.Windows.Forms.DateTimePicker dateStopTime;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dateStartTime;
        private System.Windows.Forms.Label LblEstimatedSize;
        private System.Windows.Forms.CheckedListBox ChkSignalType;
        private System.Windows.Forms.GroupBox GrpPointSelection;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblPointCount;
        private System.Windows.Forms.Button btnCategoryUncheckSelected;
        private System.Windows.Forms.Button btnCategoryCheckSelected;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
    }
}

