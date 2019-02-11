namespace DataExtractor
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBoxServerConnection = new System.Windows.Forms.GroupBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.labelSourceHistorianInstanceName = new System.Windows.Forms.Label();
            this.textBoxHistorianInstanceName = new System.Windows.Forms.TextBox();
            this.labelSourceHistorianHostAddress = new System.Windows.Forms.Label();
            this.textBoxHistorianHostAddress = new System.Windows.Forms.TextBox();
            this.maskedTextBoxHistorianPort = new System.Windows.Forms.MaskedTextBox();
            this.labelSourceHistorianMetaDataPort = new System.Windows.Forms.Label();
            this.buttonGo = new System.Windows.Forms.Button();
            this.groupBoxMessages = new System.Windows.Forms.GroupBox();
            this.textBoxMessageOutput = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.tabControlOptions = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridViewDevices = new System.Windows.Forms.DataGridView();
            this.labelChevron1 = new System.Windows.Forms.Label();
            this.labelDevices = new System.Windows.Forms.Label();
            this.labelDataTypes = new System.Windows.Forms.Label();
            this.checkedListBoxDataTypes = new System.Windows.Forms.CheckedListBox();
            this.labelEndTime = new System.Windows.Forms.Label();
            this.dateTimePickerEndTime = new System.Windows.Forms.DateTimePicker();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.dateTimePickerSourceTime = new System.Windows.Forms.DateTimePicker();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.labelPointList = new System.Windows.Forms.Label();
            this.textBoxPointList = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxFillInMissingTimestamps = new System.Windows.Forms.CheckBox();
            this.checkBoxAlignTimestamps = new System.Windows.Forms.CheckBox();
            this.checkBoxExportMissingAsNaN = new System.Windows.Forms.CheckBox();
            this.labelReadIntervalDetails = new System.Windows.Forms.Label();
            this.labelSecondsReadInterval = new System.Windows.Forms.Label();
            this.maskedTextBoxReadInterval = new System.Windows.Forms.MaskedTextBox();
            this.labelReadInterval = new System.Windows.Forms.Label();
            this.maskedTextBoxFrameRate = new System.Windows.Forms.MaskedTextBox();
            this.checkBoxEnableLogging = new System.Windows.Forms.CheckBox();
            this.maskedTextBoxMessageInterval = new System.Windows.Forms.MaskedTextBox();
            this.labelMessageInterval = new System.Windows.Forms.Label();
            this.labelSecondsMetadataTimeout = new System.Windows.Forms.Label();
            this.labelPerSec = new System.Windows.Forms.Label();
            this.maskedTextBoxMetadataTimeout = new System.Windows.Forms.MaskedTextBox();
            this.labelFrameRate = new System.Windows.Forms.Label();
            this.labelMetaDataTimeout = new System.Windows.Forms.Label();
            this.checkBoxSelectAllDevices = new System.Windows.Forms.CheckBox();
            this.groupBoxServerConnection.SuspendLayout();
            this.groupBoxMessages.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.tabControlOptions.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevices)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxServerConnection
            // 
            this.groupBoxServerConnection.Controls.Add(this.buttonConnect);
            this.groupBoxServerConnection.Controls.Add(this.labelSourceHistorianInstanceName);
            this.groupBoxServerConnection.Controls.Add(this.textBoxHistorianInstanceName);
            this.groupBoxServerConnection.Controls.Add(this.labelSourceHistorianHostAddress);
            this.groupBoxServerConnection.Controls.Add(this.textBoxHistorianHostAddress);
            this.groupBoxServerConnection.Controls.Add(this.maskedTextBoxHistorianPort);
            this.groupBoxServerConnection.Controls.Add(this.labelSourceHistorianMetaDataPort);
            this.groupBoxServerConnection.Location = new System.Drawing.Point(12, 12);
            this.groupBoxServerConnection.Name = "groupBoxServerConnection";
            this.groupBoxServerConnection.Size = new System.Drawing.Size(201, 250);
            this.groupBoxServerConnection.TabIndex = 0;
            this.groupBoxServerConnection.TabStop = false;
            this.groupBoxServerConnection.Text = "&Server Connection";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.Location = new System.Drawing.Point(101, 132);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(2);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(88, 27);
            this.buttonConnect.TabIndex = 16;
            this.buttonConnect.Text = "&Connect >>";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // labelSourceHistorianInstanceName
            // 
            this.labelSourceHistorianInstanceName.AutoSize = true;
            this.labelSourceHistorianInstanceName.Location = new System.Drawing.Point(15, 85);
            this.labelSourceHistorianInstanceName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSourceHistorianInstanceName.Name = "labelSourceHistorianInstanceName";
            this.labelSourceHistorianInstanceName.Size = new System.Drawing.Size(82, 13);
            this.labelSourceHistorianInstanceName.TabIndex = 14;
            this.labelSourceHistorianInstanceName.Text = "Instance Name:";
            this.labelSourceHistorianInstanceName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxHistorianInstanceName
            // 
            this.textBoxHistorianInstanceName.Location = new System.Drawing.Point(101, 83);
            this.textBoxHistorianInstanceName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxHistorianInstanceName.Name = "textBoxHistorianInstanceName";
            this.textBoxHistorianInstanceName.Size = new System.Drawing.Size(41, 20);
            this.textBoxHistorianInstanceName.TabIndex = 15;
            this.textBoxHistorianInstanceName.Text = "PPA";
            this.textBoxHistorianInstanceName.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // labelSourceHistorianHostAddress
            // 
            this.labelSourceHistorianHostAddress.AutoSize = true;
            this.labelSourceHistorianHostAddress.Location = new System.Drawing.Point(24, 32);
            this.labelSourceHistorianHostAddress.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSourceHistorianHostAddress.Name = "labelSourceHistorianHostAddress";
            this.labelSourceHistorianHostAddress.Size = new System.Drawing.Size(73, 13);
            this.labelSourceHistorianHostAddress.TabIndex = 8;
            this.labelSourceHistorianHostAddress.Text = "Host Address:";
            this.labelSourceHistorianHostAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxHistorianHostAddress
            // 
            this.textBoxHistorianHostAddress.Location = new System.Drawing.Point(101, 29);
            this.textBoxHistorianHostAddress.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxHistorianHostAddress.Name = "textBoxHistorianHostAddress";
            this.textBoxHistorianHostAddress.Size = new System.Drawing.Size(88, 20);
            this.textBoxHistorianHostAddress.TabIndex = 9;
            this.textBoxHistorianHostAddress.Text = "localhost";
            this.textBoxHistorianHostAddress.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // maskedTextBoxHistorianPort
            // 
            this.maskedTextBoxHistorianPort.Location = new System.Drawing.Point(101, 56);
            this.maskedTextBoxHistorianPort.Margin = new System.Windows.Forms.Padding(2);
            this.maskedTextBoxHistorianPort.Mask = "00000";
            this.maskedTextBoxHistorianPort.Name = "maskedTextBoxHistorianPort";
            this.maskedTextBoxHistorianPort.Size = new System.Drawing.Size(41, 20);
            this.maskedTextBoxHistorianPort.TabIndex = 13;
            this.maskedTextBoxHistorianPort.Text = "6175";
            this.maskedTextBoxHistorianPort.ValidatingType = typeof(int);
            this.maskedTextBoxHistorianPort.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // labelSourceHistorianMetaDataPort
            // 
            this.labelSourceHistorianMetaDataPort.AutoSize = true;
            this.labelSourceHistorianMetaDataPort.Location = new System.Drawing.Point(68, 59);
            this.labelSourceHistorianMetaDataPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSourceHistorianMetaDataPort.Name = "labelSourceHistorianMetaDataPort";
            this.labelSourceHistorianMetaDataPort.Size = new System.Drawing.Size(29, 13);
            this.labelSourceHistorianMetaDataPort.TabIndex = 12;
            this.labelSourceHistorianMetaDataPort.Text = "Port:";
            this.labelSourceHistorianMetaDataPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonGo
            // 
            this.buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGo.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGo.Location = new System.Drawing.Point(628, 533);
            this.buttonGo.Margin = new System.Windows.Forms.Padding(2);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(52, 27);
            this.buttonGo.TabIndex = 7;
            this.buttonGo.Text = "&Go!";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // groupBoxMessages
            // 
            this.groupBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMessages.Controls.Add(this.textBoxMessageOutput);
            this.groupBoxMessages.Location = new System.Drawing.Point(12, 268);
            this.groupBoxMessages.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxMessages.Name = "groupBoxMessages";
            this.groupBoxMessages.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxMessages.Size = new System.Drawing.Size(668, 263);
            this.groupBoxMessages.TabIndex = 5;
            this.groupBoxMessages.TabStop = false;
            this.groupBoxMessages.Text = "Messages";
            // 
            // textBoxMessageOutput
            // 
            this.textBoxMessageOutput.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBoxMessageOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMessageOutput.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMessageOutput.ForeColor = System.Drawing.SystemColors.Window;
            this.textBoxMessageOutput.Location = new System.Drawing.Point(2, 15);
            this.textBoxMessageOutput.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMessageOutput.Multiline = true;
            this.textBoxMessageOutput.Name = "textBoxMessageOutput";
            this.textBoxMessageOutput.ReadOnly = true;
            this.textBoxMessageOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessageOutput.Size = new System.Drawing.Size(664, 246);
            this.textBoxMessageOutput.TabIndex = 0;
            this.textBoxMessageOutput.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(14, 533);
            this.progressBar.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(610, 27);
            this.progressBar.TabIndex = 6;
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOptions.Controls.Add(this.tabControlOptions);
            this.groupBoxOptions.Location = new System.Drawing.Point(218, 11);
            this.groupBoxOptions.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxOptions.Size = new System.Drawing.Size(462, 253);
            this.groupBoxOptions.TabIndex = 8;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "&Options";
            // 
            // tabControlOptions
            // 
            this.tabControlOptions.Controls.Add(this.tabPage1);
            this.tabControlOptions.Controls.Add(this.tabPage2);
            this.tabControlOptions.Controls.Add(this.tabPage3);
            this.tabControlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlOptions.Location = new System.Drawing.Point(2, 15);
            this.tabControlOptions.Name = "tabControlOptions";
            this.tabControlOptions.SelectedIndex = 0;
            this.tabControlOptions.Size = new System.Drawing.Size(458, 236);
            this.tabControlOptions.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridViewDevices);
            this.tabPage1.Controls.Add(this.labelDevices);
            this.tabPage1.Controls.Add(this.labelDataTypes);
            this.tabPage1.Controls.Add(this.checkedListBoxDataTypes);
            this.tabPage1.Controls.Add(this.labelEndTime);
            this.tabPage1.Controls.Add(this.dateTimePickerEndTime);
            this.tabPage1.Controls.Add(this.labelStartTime);
            this.tabPage1.Controls.Add(this.dateTimePickerSourceTime);
            this.tabPage1.Controls.Add(this.labelChevron1);
            this.tabPage1.Controls.Add(this.checkBoxSelectAllDevices);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(450, 210);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Time Range";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDevices
            // 
            this.dataGridViewDevices.AllowUserToAddRows = false;
            this.dataGridViewDevices.AllowUserToDeleteRows = false;
            this.dataGridViewDevices.AllowUserToOrderColumns = true;
            this.dataGridViewDevices.AllowUserToResizeRows = false;
            this.dataGridViewDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewDevices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewDevices.Location = new System.Drawing.Point(118, 66);
            this.dataGridViewDevices.Name = "dataGridViewDevices";
            this.dataGridViewDevices.RowHeadersVisible = false;
            this.dataGridViewDevices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDevices.Size = new System.Drawing.Size(319, 138);
            this.dataGridViewDevices.TabIndex = 15;
            this.dataGridViewDevices.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDevices_CellContentClick);
            this.dataGridViewDevices.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewDevices_ColumnHeaderMouseClick);
            this.dataGridViewDevices.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridViewDevices_DataBindingComplete);
            // 
            // labelChevron1
            // 
            this.labelChevron1.AutoSize = true;
            this.labelChevron1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelChevron1.Location = new System.Drawing.Point(92, 103);
            this.labelChevron1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelChevron1.Name = "labelChevron1";
            this.labelChevron1.Size = new System.Drawing.Size(25, 13);
            this.labelChevron1.TabIndex = 13;
            this.labelChevron1.Text = ">>";
            this.labelChevron1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDevices
            // 
            this.labelDevices.AutoSize = true;
            this.labelDevices.Location = new System.Drawing.Point(115, 50);
            this.labelDevices.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDevices.Name = "labelDevices";
            this.labelDevices.Size = new System.Drawing.Size(49, 13);
            this.labelDevices.TabIndex = 11;
            this.labelDevices.Text = "Devices:";
            this.labelDevices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDataTypes
            // 
            this.labelDataTypes.AutoSize = true;
            this.labelDataTypes.Location = new System.Drawing.Point(9, 50);
            this.labelDataTypes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDataTypes.Name = "labelDataTypes";
            this.labelDataTypes.Size = new System.Drawing.Size(65, 13);
            this.labelDataTypes.TabIndex = 9;
            this.labelDataTypes.Text = "Data Types:";
            this.labelDataTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkedListBoxDataTypes
            // 
            this.checkedListBoxDataTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxDataTypes.CheckOnClick = true;
            this.checkedListBoxDataTypes.FormattingEnabled = true;
            this.checkedListBoxDataTypes.Location = new System.Drawing.Point(12, 66);
            this.checkedListBoxDataTypes.Name = "checkedListBoxDataTypes";
            this.checkedListBoxDataTypes.Size = new System.Drawing.Size(79, 139);
            this.checkedListBoxDataTypes.TabIndex = 8;
            // 
            // labelEndTime
            // 
            this.labelEndTime.AutoSize = true;
            this.labelEndTime.Location = new System.Drawing.Point(232, 17);
            this.labelEndTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelEndTime.Name = "labelEndTime";
            this.labelEndTime.Size = new System.Drawing.Size(55, 13);
            this.labelEndTime.TabIndex = 6;
            this.labelEndTime.Text = "End Time:";
            this.labelEndTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerEndTime
            // 
            this.dateTimePickerEndTime.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dateTimePickerEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEndTime.Location = new System.Drawing.Point(290, 14);
            this.dateTimePickerEndTime.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerEndTime.Name = "dateTimePickerEndTime";
            this.dateTimePickerEndTime.Size = new System.Drawing.Size(147, 20);
            this.dateTimePickerEndTime.TabIndex = 7;
            this.dateTimePickerEndTime.Value = new System.DateTime(2017, 1, 1, 0, 10, 0, 0);
            this.dateTimePickerEndTime.ValueChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(9, 17);
            this.labelStartTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(58, 13);
            this.labelStartTime.TabIndex = 4;
            this.labelStartTime.Text = "Start Time:";
            this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerSourceTime
            // 
            this.dateTimePickerSourceTime.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dateTimePickerSourceTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSourceTime.Location = new System.Drawing.Point(71, 14);
            this.dateTimePickerSourceTime.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerSourceTime.Name = "dateTimePickerSourceTime";
            this.dateTimePickerSourceTime.Size = new System.Drawing.Size(147, 20);
            this.dateTimePickerSourceTime.TabIndex = 5;
            this.dateTimePickerSourceTime.Value = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerSourceTime.ValueChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.labelPointList);
            this.tabPage2.Controls.Add(this.textBoxPointList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(450, 210);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Data Selection";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // labelPointList
            // 
            this.labelPointList.AutoSize = true;
            this.labelPointList.Location = new System.Drawing.Point(5, 81);
            this.labelPointList.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPointList.Name = "labelPointList";
            this.labelPointList.Size = new System.Drawing.Size(140, 13);
            this.labelPointList.TabIndex = 6;
            this.labelPointList.Text = "Point List / Filter Expression:";
            this.labelPointList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxPointList
            // 
            this.textBoxPointList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPointList.Location = new System.Drawing.Point(7, 97);
            this.textBoxPointList.Multiline = true;
            this.textBoxPointList.Name = "textBoxPointList";
            this.textBoxPointList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPointList.Size = new System.Drawing.Size(430, 49);
            this.textBoxPointList.TabIndex = 7;
            this.textBoxPointList.Text = "FILTER MeasurementDetail WHERE SignalAcronym IN (\'IPHM\', \'IPHA\', \'VPHM\', \'VPHA\', " +
    "\'FREQ\', \'DFDT\', \'ALOG\', \'CALC\')";
            this.textBoxPointList.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBoxFillInMissingTimestamps);
            this.tabPage3.Controls.Add(this.checkBoxAlignTimestamps);
            this.tabPage3.Controls.Add(this.checkBoxExportMissingAsNaN);
            this.tabPage3.Controls.Add(this.labelReadIntervalDetails);
            this.tabPage3.Controls.Add(this.labelSecondsReadInterval);
            this.tabPage3.Controls.Add(this.maskedTextBoxReadInterval);
            this.tabPage3.Controls.Add(this.labelReadInterval);
            this.tabPage3.Controls.Add(this.maskedTextBoxFrameRate);
            this.tabPage3.Controls.Add(this.checkBoxEnableLogging);
            this.tabPage3.Controls.Add(this.maskedTextBoxMessageInterval);
            this.tabPage3.Controls.Add(this.labelMessageInterval);
            this.tabPage3.Controls.Add(this.labelSecondsMetadataTimeout);
            this.tabPage3.Controls.Add(this.labelPerSec);
            this.tabPage3.Controls.Add(this.maskedTextBoxMetadataTimeout);
            this.tabPage3.Controls.Add(this.labelFrameRate);
            this.tabPage3.Controls.Add(this.labelMetaDataTimeout);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(450, 210);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxFillInMissingTimestamps
            // 
            this.checkBoxFillInMissingTimestamps.AutoSize = true;
            this.checkBoxFillInMissingTimestamps.Checked = true;
            this.checkBoxFillInMissingTimestamps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFillInMissingTimestamps.Location = new System.Drawing.Point(8, 143);
            this.checkBoxFillInMissingTimestamps.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxFillInMissingTimestamps.Name = "checkBoxFillInMissingTimestamps";
            this.checkBoxFillInMissingTimestamps.Size = new System.Drawing.Size(146, 17);
            this.checkBoxFillInMissingTimestamps.TabIndex = 43;
            this.checkBoxFillInMissingTimestamps.Text = "Fill-in Missing Timestamps";
            this.checkBoxFillInMissingTimestamps.UseVisualStyleBackColor = true;
            this.checkBoxFillInMissingTimestamps.CheckedChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // checkBoxAlignTimestamps
            // 
            this.checkBoxAlignTimestamps.AutoSize = true;
            this.checkBoxAlignTimestamps.Checked = true;
            this.checkBoxAlignTimestamps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAlignTimestamps.Location = new System.Drawing.Point(8, 101);
            this.checkBoxAlignTimestamps.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxAlignTimestamps.Name = "checkBoxAlignTimestamps";
            this.checkBoxAlignTimestamps.Size = new System.Drawing.Size(108, 17);
            this.checkBoxAlignTimestamps.TabIndex = 42;
            this.checkBoxAlignTimestamps.Text = "Align Timestamps";
            this.checkBoxAlignTimestamps.UseVisualStyleBackColor = true;
            this.checkBoxAlignTimestamps.CheckedChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // checkBoxExportMissingAsNaN
            // 
            this.checkBoxExportMissingAsNaN.AutoSize = true;
            this.checkBoxExportMissingAsNaN.Checked = true;
            this.checkBoxExportMissingAsNaN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExportMissingAsNaN.Location = new System.Drawing.Point(8, 122);
            this.checkBoxExportMissingAsNaN.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxExportMissingAsNaN.Name = "checkBoxExportMissingAsNaN";
            this.checkBoxExportMissingAsNaN.Size = new System.Drawing.Size(168, 17);
            this.checkBoxExportMissingAsNaN.TabIndex = 41;
            this.checkBoxExportMissingAsNaN.Text = "Export Missing Values as NaN";
            this.checkBoxExportMissingAsNaN.UseVisualStyleBackColor = true;
            this.checkBoxExportMissingAsNaN.CheckedChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // labelReadIntervalDetails
            // 
            this.labelReadIntervalDetails.AutoSize = true;
            this.labelReadIntervalDetails.Location = new System.Drawing.Point(201, 64);
            this.labelReadIntervalDetails.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelReadIntervalDetails.Name = "labelReadIntervalDetails";
            this.labelReadIntervalDetails.Size = new System.Drawing.Size(226, 13);
            this.labelReadIntervalDetails.TabIndex = 40;
            this.labelReadIntervalDetails.Text = "( if > 0, read window then skip to next interval )";
            // 
            // labelSecondsReadInterval
            // 
            this.labelSecondsReadInterval.AutoSize = true;
            this.labelSecondsReadInterval.Location = new System.Drawing.Point(156, 64);
            this.labelSecondsReadInterval.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSecondsReadInterval.Name = "labelSecondsReadInterval";
            this.labelSecondsReadInterval.Size = new System.Drawing.Size(47, 13);
            this.labelSecondsReadInterval.TabIndex = 39;
            this.labelSecondsReadInterval.Text = "seconds";
            // 
            // maskedTextBoxReadInterval
            // 
            this.maskedTextBoxReadInterval.Location = new System.Drawing.Point(111, 61);
            this.maskedTextBoxReadInterval.Margin = new System.Windows.Forms.Padding(2);
            this.maskedTextBoxReadInterval.Mask = "00000";
            this.maskedTextBoxReadInterval.Name = "maskedTextBoxReadInterval";
            this.maskedTextBoxReadInterval.Size = new System.Drawing.Size(43, 20);
            this.maskedTextBoxReadInterval.TabIndex = 38;
            this.maskedTextBoxReadInterval.Text = "0";
            this.maskedTextBoxReadInterval.ValidatingType = typeof(int);
            this.maskedTextBoxReadInterval.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // labelReadInterval
            // 
            this.labelReadInterval.AutoSize = true;
            this.labelReadInterval.Location = new System.Drawing.Point(39, 64);
            this.labelReadInterval.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelReadInterval.Name = "labelReadInterval";
            this.labelReadInterval.Size = new System.Drawing.Size(74, 13);
            this.labelReadInterval.TabIndex = 37;
            this.labelReadInterval.Text = "Read Interval:";
            this.labelReadInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // maskedTextBoxFrameRate
            // 
            this.maskedTextBoxFrameRate.Location = new System.Drawing.Point(111, 13);
            this.maskedTextBoxFrameRate.Margin = new System.Windows.Forms.Padding(2);
            this.maskedTextBoxFrameRate.Mask = "000";
            this.maskedTextBoxFrameRate.Name = "maskedTextBoxFrameRate";
            this.maskedTextBoxFrameRate.Size = new System.Drawing.Size(31, 20);
            this.maskedTextBoxFrameRate.TabIndex = 26;
            this.maskedTextBoxFrameRate.Text = "30";
            this.maskedTextBoxFrameRate.ValidatingType = typeof(int);
            this.maskedTextBoxFrameRate.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // checkBoxEnableLogging
            // 
            this.checkBoxEnableLogging.AutoSize = true;
            this.checkBoxEnableLogging.Checked = true;
            this.checkBoxEnableLogging.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnableLogging.Location = new System.Drawing.Point(339, 119);
            this.checkBoxEnableLogging.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxEnableLogging.Name = "checkBoxEnableLogging";
            this.checkBoxEnableLogging.Size = new System.Drawing.Size(100, 17);
            this.checkBoxEnableLogging.TabIndex = 33;
            this.checkBoxEnableLogging.Text = "Enable Logging";
            this.checkBoxEnableLogging.UseVisualStyleBackColor = true;
            this.checkBoxEnableLogging.CheckedChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // maskedTextBoxMessageInterval
            // 
            this.maskedTextBoxMessageInterval.Location = new System.Drawing.Point(111, 37);
            this.maskedTextBoxMessageInterval.Margin = new System.Windows.Forms.Padding(2);
            this.maskedTextBoxMessageInterval.Mask = "0000000";
            this.maskedTextBoxMessageInterval.Name = "maskedTextBoxMessageInterval";
            this.maskedTextBoxMessageInterval.Size = new System.Drawing.Size(53, 20);
            this.maskedTextBoxMessageInterval.TabIndex = 32;
            this.maskedTextBoxMessageInterval.Text = "2000";
            this.maskedTextBoxMessageInterval.ValidatingType = typeof(int);
            this.maskedTextBoxMessageInterval.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // labelMessageInterval
            // 
            this.labelMessageInterval.AutoSize = true;
            this.labelMessageInterval.Location = new System.Drawing.Point(22, 40);
            this.labelMessageInterval.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMessageInterval.Name = "labelMessageInterval";
            this.labelMessageInterval.Size = new System.Drawing.Size(91, 13);
            this.labelMessageInterval.TabIndex = 31;
            this.labelMessageInterval.Text = "Message Interval:";
            this.labelMessageInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSecondsMetadataTimeout
            // 
            this.labelSecondsMetadataTimeout.AutoSize = true;
            this.labelSecondsMetadataTimeout.Location = new System.Drawing.Point(388, 145);
            this.labelSecondsMetadataTimeout.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSecondsMetadataTimeout.Name = "labelSecondsMetadataTimeout";
            this.labelSecondsMetadataTimeout.Size = new System.Drawing.Size(47, 13);
            this.labelSecondsMetadataTimeout.TabIndex = 30;
            this.labelSecondsMetadataTimeout.Text = "seconds";
            // 
            // labelPerSec
            // 
            this.labelPerSec.AutoSize = true;
            this.labelPerSec.Location = new System.Drawing.Point(143, 16);
            this.labelPerSec.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPerSec.Name = "labelPerSec";
            this.labelPerSec.Size = new System.Drawing.Size(101, 13);
            this.labelPerSec.TabIndex = 27;
            this.labelPerSec.Text = "samples per second";
            // 
            // maskedTextBoxMetadataTimeout
            // 
            this.maskedTextBoxMetadataTimeout.Location = new System.Drawing.Point(356, 142);
            this.maskedTextBoxMetadataTimeout.Margin = new System.Windows.Forms.Padding(2);
            this.maskedTextBoxMetadataTimeout.Mask = "000";
            this.maskedTextBoxMetadataTimeout.Name = "maskedTextBoxMetadataTimeout";
            this.maskedTextBoxMetadataTimeout.Size = new System.Drawing.Size(31, 20);
            this.maskedTextBoxMetadataTimeout.TabIndex = 29;
            this.maskedTextBoxMetadataTimeout.Text = "60";
            this.maskedTextBoxMetadataTimeout.ValidatingType = typeof(int);
            this.maskedTextBoxMetadataTimeout.TextChanged += new System.EventHandler(this.FormElementChanged);
            // 
            // labelFrameRate
            // 
            this.labelFrameRate.AutoSize = true;
            this.labelFrameRate.Location = new System.Drawing.Point(5, 16);
            this.labelFrameRate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFrameRate.Name = "labelFrameRate";
            this.labelFrameRate.Size = new System.Drawing.Size(108, 13);
            this.labelFrameRate.TabIndex = 25;
            this.labelFrameRate.Text = "Frame Rate Estimate:";
            this.labelFrameRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelMetaDataTimeout
            // 
            this.labelMetaDataTimeout.AutoSize = true;
            this.labelMetaDataTimeout.Location = new System.Drawing.Point(263, 145);
            this.labelMetaDataTimeout.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMetaDataTimeout.Name = "labelMetaDataTimeout";
            this.labelMetaDataTimeout.Size = new System.Drawing.Size(96, 13);
            this.labelMetaDataTimeout.TabIndex = 28;
            this.labelMetaDataTimeout.Text = "Metadata Timeout:";
            this.labelMetaDataTimeout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxSelectAllDevices
            // 
            this.checkBoxSelectAllDevices.AutoSize = true;
            this.checkBoxSelectAllDevices.Location = new System.Drawing.Point(168, 50);
            this.checkBoxSelectAllDevices.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSelectAllDevices.Name = "checkBoxSelectAllDevices";
            this.checkBoxSelectAllDevices.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllDevices.TabIndex = 44;
            this.checkBoxSelectAllDevices.Text = "Select all";
            this.checkBoxSelectAllDevices.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllDevices.CheckedChanged += new System.EventHandler(this.checkBoxSelectAllDevices_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 571);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.groupBoxMessages);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.groupBoxServerConnection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(710, 610);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IMS Data Extractor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxServerConnection.ResumeLayout(false);
            this.groupBoxServerConnection.PerformLayout();
            this.groupBoxMessages.ResumeLayout(false);
            this.groupBoxMessages.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.tabControlOptions.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevices)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxServerConnection;
        private System.Windows.Forms.Label labelSourceHistorianInstanceName;
        public System.Windows.Forms.TextBox textBoxHistorianInstanceName;
        private System.Windows.Forms.Label labelSourceHistorianHostAddress;
        public System.Windows.Forms.TextBox textBoxHistorianHostAddress;
        public System.Windows.Forms.MaskedTextBox maskedTextBoxHistorianPort;
        private System.Windows.Forms.Label labelSourceHistorianMetaDataPort;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.GroupBox groupBoxMessages;
        private System.Windows.Forms.TextBox textBoxMessageOutput;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TabControl tabControlOptions;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label labelEndTime;
        public System.Windows.Forms.DateTimePicker dateTimePickerEndTime;
        private System.Windows.Forms.Label labelStartTime;
        public System.Windows.Forms.DateTimePicker dateTimePickerSourceTime;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label labelPointList;
        public System.Windows.Forms.TextBox textBoxPointList;
        private System.Windows.Forms.TabPage tabPage3;
        public System.Windows.Forms.CheckBox checkBoxFillInMissingTimestamps;
        public System.Windows.Forms.CheckBox checkBoxAlignTimestamps;
        public System.Windows.Forms.CheckBox checkBoxExportMissingAsNaN;
        private System.Windows.Forms.Label labelReadIntervalDetails;
        private System.Windows.Forms.Label labelSecondsReadInterval;
        public System.Windows.Forms.MaskedTextBox maskedTextBoxReadInterval;
        private System.Windows.Forms.Label labelReadInterval;
        public System.Windows.Forms.MaskedTextBox maskedTextBoxFrameRate;
        public System.Windows.Forms.CheckBox checkBoxEnableLogging;
        public System.Windows.Forms.MaskedTextBox maskedTextBoxMessageInterval;
        private System.Windows.Forms.Label labelMessageInterval;
        private System.Windows.Forms.Label labelSecondsMetadataTimeout;
        private System.Windows.Forms.Label labelPerSec;
        public System.Windows.Forms.MaskedTextBox maskedTextBoxMetadataTimeout;
        private System.Windows.Forms.Label labelFrameRate;
        private System.Windows.Forms.Label labelMetaDataTimeout;
        private System.Windows.Forms.Label labelDataTypes;
        private System.Windows.Forms.CheckedListBox checkedListBoxDataTypes;
        private System.Windows.Forms.Label labelDevices;
        private System.Windows.Forms.Label labelChevron1;
        private System.Windows.Forms.DataGridView dataGridViewDevices;
        public System.Windows.Forms.CheckBox checkBoxSelectAllDevices;
    }
}

