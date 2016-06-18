namespace ComparisonUtility
{
    partial class ComparisonUtility
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComparisonUtility));
            this.groupBoxSourceHistorian = new System.Windows.Forms.GroupBox();
            this.labelSourceHistorianInstanceName = new System.Windows.Forms.Label();
            this.textBoxSourceHistorianInstanceName = new System.Windows.Forms.TextBox();
            this.labelSourceHistorianHostAddress = new System.Windows.Forms.Label();
            this.textBoxSourceHistorianHostAddress = new System.Windows.Forms.TextBox();
            this.maskedTextBoxSourceHistorianDataPort = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxSourceHistorianMetaDataPort = new System.Windows.Forms.MaskedTextBox();
            this.labelSourceHistorianDataPort = new System.Windows.Forms.Label();
            this.labelSourceHistorianMetaDataPort = new System.Windows.Forms.Label();
            this.groupBoxDestinationHistorian = new System.Windows.Forms.GroupBox();
            this.labelDestinationHistorianInstanceName = new System.Windows.Forms.Label();
            this.textBoxDestinationHistorianInstanceName = new System.Windows.Forms.TextBox();
            this.labelDestinationHistorianHostAddress = new System.Windows.Forms.Label();
            this.textBoxDestinationHistorianHostAddress = new System.Windows.Forms.TextBox();
            this.maskedTextBoxDestinationHistorianDataPort = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxDestinationHistorianMetaDataPort = new System.Windows.Forms.MaskedTextBox();
            this.labelDestinationHistorianDataPort = new System.Windows.Forms.Label();
            this.labelDestinationHistorianMetaDataPort = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBoxMessages = new System.Windows.Forms.GroupBox();
            this.textBoxMessageOutput = new System.Windows.Forms.TextBox();
            this.buttonGo = new System.Windows.Forms.Button();
            this.groupBoxComparisonOptions = new System.Windows.Forms.GroupBox();
            this.maskedTextBoxMessageInterval = new System.Windows.Forms.MaskedTextBox();
            this.labelMessageInterval = new System.Windows.Forms.Label();
            this.labelSeconds = new System.Windows.Forms.Label();
            this.labelPerSec = new System.Windows.Forms.Label();
            this.maskedTextBoxMetaDataTimeout = new System.Windows.Forms.MaskedTextBox();
            this.labelMetaDataTimeout = new System.Windows.Forms.Label();
            this.maskedTextBoxFrameRate = new System.Windows.Forms.MaskedTextBox();
            this.labelFrameRate = new System.Windows.Forms.Label();
            this.labelEndTime = new System.Windows.Forms.Label();
            this.dateTimePickerEndTime = new System.Windows.Forms.DateTimePicker();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.dateTimePickerSourceTime = new System.Windows.Forms.DateTimePicker();
            this.checkBoxEnableLogging = new System.Windows.Forms.CheckBox();
            this.groupBoxSourceHistorian.SuspendLayout();
            this.groupBoxDestinationHistorian.SuspendLayout();
            this.groupBoxMessages.SuspendLayout();
            this.groupBoxComparisonOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSourceHistorian
            // 
            this.groupBoxSourceHistorian.Controls.Add(this.labelSourceHistorianInstanceName);
            this.groupBoxSourceHistorian.Controls.Add(this.textBoxSourceHistorianInstanceName);
            this.groupBoxSourceHistorian.Controls.Add(this.labelSourceHistorianHostAddress);
            this.groupBoxSourceHistorian.Controls.Add(this.textBoxSourceHistorianHostAddress);
            this.groupBoxSourceHistorian.Controls.Add(this.maskedTextBoxSourceHistorianDataPort);
            this.groupBoxSourceHistorian.Controls.Add(this.maskedTextBoxSourceHistorianMetaDataPort);
            this.groupBoxSourceHistorian.Controls.Add(this.labelSourceHistorianDataPort);
            this.groupBoxSourceHistorian.Controls.Add(this.labelSourceHistorianMetaDataPort);
            this.groupBoxSourceHistorian.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSourceHistorian.Name = "groupBoxSourceHistorian";
            this.groupBoxSourceHistorian.Size = new System.Drawing.Size(310, 178);
            this.groupBoxSourceHistorian.TabIndex = 0;
            this.groupBoxSourceHistorian.TabStop = false;
            this.groupBoxSourceHistorian.Text = "&Source Historian";
            // 
            // labelSourceHistorianInstanceName
            // 
            this.labelSourceHistorianInstanceName.AutoSize = true;
            this.labelSourceHistorianInstanceName.Location = new System.Drawing.Point(22, 136);
            this.labelSourceHistorianInstanceName.Name = "labelSourceHistorianInstanceName";
            this.labelSourceHistorianInstanceName.Size = new System.Drawing.Size(121, 20);
            this.labelSourceHistorianInstanceName.TabIndex = 6;
            this.labelSourceHistorianInstanceName.Text = "Instance Name:";
            this.labelSourceHistorianInstanceName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxSourceHistorianInstanceName
            // 
            this.textBoxSourceHistorianInstanceName.Location = new System.Drawing.Point(149, 133);
            this.textBoxSourceHistorianInstanceName.Name = "textBoxSourceHistorianInstanceName";
            this.textBoxSourceHistorianInstanceName.Size = new System.Drawing.Size(59, 26);
            this.textBoxSourceHistorianInstanceName.TabIndex = 7;
            this.textBoxSourceHistorianInstanceName.Text = "PPA";
            // 
            // labelSourceHistorianHostAddress
            // 
            this.labelSourceHistorianHostAddress.AutoSize = true;
            this.labelSourceHistorianHostAddress.Location = new System.Drawing.Point(33, 40);
            this.labelSourceHistorianHostAddress.Name = "labelSourceHistorianHostAddress";
            this.labelSourceHistorianHostAddress.Size = new System.Drawing.Size(110, 20);
            this.labelSourceHistorianHostAddress.TabIndex = 0;
            this.labelSourceHistorianHostAddress.Text = "Host Address:";
            this.labelSourceHistorianHostAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxSourceHistorianHostAddress
            // 
            this.textBoxSourceHistorianHostAddress.Location = new System.Drawing.Point(149, 37);
            this.textBoxSourceHistorianHostAddress.Name = "textBoxSourceHistorianHostAddress";
            this.textBoxSourceHistorianHostAddress.Size = new System.Drawing.Size(130, 26);
            this.textBoxSourceHistorianHostAddress.TabIndex = 1;
            this.textBoxSourceHistorianHostAddress.Text = "localhost";
            // 
            // maskedTextBoxSourceHistorianDataPort
            // 
            this.maskedTextBoxSourceHistorianDataPort.Location = new System.Drawing.Point(149, 101);
            this.maskedTextBoxSourceHistorianDataPort.Mask = "00000";
            this.maskedTextBoxSourceHistorianDataPort.Name = "maskedTextBoxSourceHistorianDataPort";
            this.maskedTextBoxSourceHistorianDataPort.Size = new System.Drawing.Size(59, 26);
            this.maskedTextBoxSourceHistorianDataPort.TabIndex = 5;
            this.maskedTextBoxSourceHistorianDataPort.Text = "6356";
            this.maskedTextBoxSourceHistorianDataPort.ValidatingType = typeof(int);
            // 
            // maskedTextBoxSourceHistorianMetaDataPort
            // 
            this.maskedTextBoxSourceHistorianMetaDataPort.Location = new System.Drawing.Point(149, 69);
            this.maskedTextBoxSourceHistorianMetaDataPort.Mask = "00000";
            this.maskedTextBoxSourceHistorianMetaDataPort.Name = "maskedTextBoxSourceHistorianMetaDataPort";
            this.maskedTextBoxSourceHistorianMetaDataPort.Size = new System.Drawing.Size(59, 26);
            this.maskedTextBoxSourceHistorianMetaDataPort.TabIndex = 3;
            this.maskedTextBoxSourceHistorianMetaDataPort.Text = "6355";
            this.maskedTextBoxSourceHistorianMetaDataPort.ValidatingType = typeof(int);
            // 
            // labelSourceHistorianDataPort
            // 
            this.labelSourceHistorianDataPort.AutoSize = true;
            this.labelSourceHistorianDataPort.Location = new System.Drawing.Point(62, 104);
            this.labelSourceHistorianDataPort.Name = "labelSourceHistorianDataPort";
            this.labelSourceHistorianDataPort.Size = new System.Drawing.Size(81, 20);
            this.labelSourceHistorianDataPort.TabIndex = 4;
            this.labelSourceHistorianDataPort.Text = "Data Port:";
            this.labelSourceHistorianDataPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSourceHistorianMetaDataPort
            // 
            this.labelSourceHistorianMetaDataPort.AutoSize = true;
            this.labelSourceHistorianMetaDataPort.Location = new System.Drawing.Point(24, 72);
            this.labelSourceHistorianMetaDataPort.Name = "labelSourceHistorianMetaDataPort";
            this.labelSourceHistorianMetaDataPort.Size = new System.Drawing.Size(119, 20);
            this.labelSourceHistorianMetaDataPort.TabIndex = 2;
            this.labelSourceHistorianMetaDataPort.Text = "Meta-data Port:";
            this.labelSourceHistorianMetaDataPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBoxDestinationHistorian
            // 
            this.groupBoxDestinationHistorian.Controls.Add(this.labelDestinationHistorianInstanceName);
            this.groupBoxDestinationHistorian.Controls.Add(this.textBoxDestinationHistorianInstanceName);
            this.groupBoxDestinationHistorian.Controls.Add(this.labelDestinationHistorianHostAddress);
            this.groupBoxDestinationHistorian.Controls.Add(this.textBoxDestinationHistorianHostAddress);
            this.groupBoxDestinationHistorian.Controls.Add(this.maskedTextBoxDestinationHistorianDataPort);
            this.groupBoxDestinationHistorian.Controls.Add(this.maskedTextBoxDestinationHistorianMetaDataPort);
            this.groupBoxDestinationHistorian.Controls.Add(this.labelDestinationHistorianDataPort);
            this.groupBoxDestinationHistorian.Controls.Add(this.labelDestinationHistorianMetaDataPort);
            this.groupBoxDestinationHistorian.Location = new System.Drawing.Point(334, 12);
            this.groupBoxDestinationHistorian.Name = "groupBoxDestinationHistorian";
            this.groupBoxDestinationHistorian.Size = new System.Drawing.Size(310, 178);
            this.groupBoxDestinationHistorian.TabIndex = 1;
            this.groupBoxDestinationHistorian.TabStop = false;
            this.groupBoxDestinationHistorian.Text = "&Destination Historian";
            // 
            // labelDestinationHistorianInstanceName
            // 
            this.labelDestinationHistorianInstanceName.AutoSize = true;
            this.labelDestinationHistorianInstanceName.Location = new System.Drawing.Point(22, 136);
            this.labelDestinationHistorianInstanceName.Name = "labelDestinationHistorianInstanceName";
            this.labelDestinationHistorianInstanceName.Size = new System.Drawing.Size(121, 20);
            this.labelDestinationHistorianInstanceName.TabIndex = 6;
            this.labelDestinationHistorianInstanceName.Text = "Instance Name:";
            this.labelDestinationHistorianInstanceName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxDestinationHistorianInstanceName
            // 
            this.textBoxDestinationHistorianInstanceName.Location = new System.Drawing.Point(149, 133);
            this.textBoxDestinationHistorianInstanceName.Name = "textBoxDestinationHistorianInstanceName";
            this.textBoxDestinationHistorianInstanceName.Size = new System.Drawing.Size(59, 26);
            this.textBoxDestinationHistorianInstanceName.TabIndex = 7;
            this.textBoxDestinationHistorianInstanceName.Text = "PPA";
            // 
            // labelDestinationHistorianHostAddress
            // 
            this.labelDestinationHistorianHostAddress.AutoSize = true;
            this.labelDestinationHistorianHostAddress.Location = new System.Drawing.Point(33, 40);
            this.labelDestinationHistorianHostAddress.Name = "labelDestinationHistorianHostAddress";
            this.labelDestinationHistorianHostAddress.Size = new System.Drawing.Size(110, 20);
            this.labelDestinationHistorianHostAddress.TabIndex = 0;
            this.labelDestinationHistorianHostAddress.Text = "Host Address:";
            this.labelDestinationHistorianHostAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxDestinationHistorianHostAddress
            // 
            this.textBoxDestinationHistorianHostAddress.Location = new System.Drawing.Point(149, 37);
            this.textBoxDestinationHistorianHostAddress.Name = "textBoxDestinationHistorianHostAddress";
            this.textBoxDestinationHistorianHostAddress.Size = new System.Drawing.Size(130, 26);
            this.textBoxDestinationHistorianHostAddress.TabIndex = 1;
            this.textBoxDestinationHistorianHostAddress.Text = "localhost";
            // 
            // maskedTextBoxDestinationHistorianDataPort
            // 
            this.maskedTextBoxDestinationHistorianDataPort.Location = new System.Drawing.Point(149, 101);
            this.maskedTextBoxDestinationHistorianDataPort.Mask = "00000";
            this.maskedTextBoxDestinationHistorianDataPort.Name = "maskedTextBoxDestinationHistorianDataPort";
            this.maskedTextBoxDestinationHistorianDataPort.Size = new System.Drawing.Size(59, 26);
            this.maskedTextBoxDestinationHistorianDataPort.TabIndex = 5;
            this.maskedTextBoxDestinationHistorianDataPort.Text = "6356";
            this.maskedTextBoxDestinationHistorianDataPort.ValidatingType = typeof(int);
            // 
            // maskedTextBoxDestinationHistorianMetaDataPort
            // 
            this.maskedTextBoxDestinationHistorianMetaDataPort.Location = new System.Drawing.Point(149, 69);
            this.maskedTextBoxDestinationHistorianMetaDataPort.Mask = "00000";
            this.maskedTextBoxDestinationHistorianMetaDataPort.Name = "maskedTextBoxDestinationHistorianMetaDataPort";
            this.maskedTextBoxDestinationHistorianMetaDataPort.Size = new System.Drawing.Size(59, 26);
            this.maskedTextBoxDestinationHistorianMetaDataPort.TabIndex = 3;
            this.maskedTextBoxDestinationHistorianMetaDataPort.Text = "6355";
            this.maskedTextBoxDestinationHistorianMetaDataPort.ValidatingType = typeof(int);
            // 
            // labelDestinationHistorianDataPort
            // 
            this.labelDestinationHistorianDataPort.AutoSize = true;
            this.labelDestinationHistorianDataPort.Location = new System.Drawing.Point(62, 104);
            this.labelDestinationHistorianDataPort.Name = "labelDestinationHistorianDataPort";
            this.labelDestinationHistorianDataPort.Size = new System.Drawing.Size(81, 20);
            this.labelDestinationHistorianDataPort.TabIndex = 4;
            this.labelDestinationHistorianDataPort.Text = "Data Port:";
            this.labelDestinationHistorianDataPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDestinationHistorianMetaDataPort
            // 
            this.labelDestinationHistorianMetaDataPort.AutoSize = true;
            this.labelDestinationHistorianMetaDataPort.Location = new System.Drawing.Point(24, 72);
            this.labelDestinationHistorianMetaDataPort.Name = "labelDestinationHistorianMetaDataPort";
            this.labelDestinationHistorianMetaDataPort.Size = new System.Drawing.Size(119, 20);
            this.labelDestinationHistorianMetaDataPort.TabIndex = 2;
            this.labelDestinationHistorianMetaDataPort.Text = "Meta-data Port:";
            this.labelDestinationHistorianMetaDataPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 603);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(548, 42);
            this.progressBar.TabIndex = 4;
            // 
            // groupBoxMessages
            // 
            this.groupBoxMessages.Controls.Add(this.textBoxMessageOutput);
            this.groupBoxMessages.Location = new System.Drawing.Point(12, 347);
            this.groupBoxMessages.Name = "groupBoxMessages";
            this.groupBoxMessages.Size = new System.Drawing.Size(632, 250);
            this.groupBoxMessages.TabIndex = 3;
            this.groupBoxMessages.TabStop = false;
            this.groupBoxMessages.Text = "Messages";
            // 
            // textBoxMessageOutput
            // 
            this.textBoxMessageOutput.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBoxMessageOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMessageOutput.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMessageOutput.ForeColor = System.Drawing.SystemColors.Window;
            this.textBoxMessageOutput.Location = new System.Drawing.Point(3, 22);
            this.textBoxMessageOutput.Multiline = true;
            this.textBoxMessageOutput.Name = "textBoxMessageOutput";
            this.textBoxMessageOutput.ReadOnly = true;
            this.textBoxMessageOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessageOutput.Size = new System.Drawing.Size(626, 225);
            this.textBoxMessageOutput.TabIndex = 0;
            this.textBoxMessageOutput.TabStop = false;
            // 
            // buttonGo
            // 
            this.buttonGo.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGo.Location = new System.Drawing.Point(566, 603);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 42);
            this.buttonGo.TabIndex = 5;
            this.buttonGo.Text = "&Go!";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // groupBoxComparisonOptions
            // 
            this.groupBoxComparisonOptions.Controls.Add(this.checkBoxEnableLogging);
            this.groupBoxComparisonOptions.Controls.Add(this.maskedTextBoxMessageInterval);
            this.groupBoxComparisonOptions.Controls.Add(this.labelMessageInterval);
            this.groupBoxComparisonOptions.Controls.Add(this.labelSeconds);
            this.groupBoxComparisonOptions.Controls.Add(this.labelPerSec);
            this.groupBoxComparisonOptions.Controls.Add(this.maskedTextBoxMetaDataTimeout);
            this.groupBoxComparisonOptions.Controls.Add(this.labelMetaDataTimeout);
            this.groupBoxComparisonOptions.Controls.Add(this.maskedTextBoxFrameRate);
            this.groupBoxComparisonOptions.Controls.Add(this.labelFrameRate);
            this.groupBoxComparisonOptions.Controls.Add(this.labelEndTime);
            this.groupBoxComparisonOptions.Controls.Add(this.dateTimePickerEndTime);
            this.groupBoxComparisonOptions.Controls.Add(this.labelStartTime);
            this.groupBoxComparisonOptions.Controls.Add(this.dateTimePickerSourceTime);
            this.groupBoxComparisonOptions.Location = new System.Drawing.Point(12, 197);
            this.groupBoxComparisonOptions.Name = "groupBoxComparisonOptions";
            this.groupBoxComparisonOptions.Size = new System.Drawing.Size(632, 144);
            this.groupBoxComparisonOptions.TabIndex = 2;
            this.groupBoxComparisonOptions.TabStop = false;
            this.groupBoxComparisonOptions.Text = "&Comparison Options";
            // 
            // maskedTextBoxMessageInterval
            // 
            this.maskedTextBoxMessageInterval.Location = new System.Drawing.Point(163, 100);
            this.maskedTextBoxMessageInterval.Mask = "0000000";
            this.maskedTextBoxMessageInterval.Name = "maskedTextBoxMessageInterval";
            this.maskedTextBoxMessageInterval.Size = new System.Drawing.Size(78, 26);
            this.maskedTextBoxMessageInterval.TabIndex = 7;
            this.maskedTextBoxMessageInterval.Text = "10000";
            this.maskedTextBoxMessageInterval.ValidatingType = typeof(int);
            // 
            // labelMessageInterval
            // 
            this.labelMessageInterval.AutoSize = true;
            this.labelMessageInterval.Location = new System.Drawing.Point(23, 103);
            this.labelMessageInterval.Name = "labelMessageInterval";
            this.labelMessageInterval.Size = new System.Drawing.Size(134, 20);
            this.labelMessageInterval.TabIndex = 6;
            this.labelMessageInterval.Text = "Message Interval:";
            this.labelMessageInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSeconds
            // 
            this.labelSeconds.AutoSize = true;
            this.labelSeconds.Location = new System.Drawing.Point(214, 71);
            this.labelSeconds.Name = "labelSeconds";
            this.labelSeconds.Size = new System.Drawing.Size(69, 20);
            this.labelSeconds.TabIndex = 5;
            this.labelSeconds.Text = "seconds";
            // 
            // labelPerSec
            // 
            this.labelPerSec.AutoSize = true;
            this.labelPerSec.Location = new System.Drawing.Point(214, 39);
            this.labelPerSec.Name = "labelPerSec";
            this.labelPerSec.Size = new System.Drawing.Size(61, 20);
            this.labelPerSec.TabIndex = 2;
            this.labelPerSec.Text = "per sec";
            // 
            // maskedTextBoxMetaDataTimeout
            // 
            this.maskedTextBoxMetaDataTimeout.Location = new System.Drawing.Point(163, 68);
            this.maskedTextBoxMetaDataTimeout.Mask = "000";
            this.maskedTextBoxMetaDataTimeout.Name = "maskedTextBoxMetaDataTimeout";
            this.maskedTextBoxMetaDataTimeout.Size = new System.Drawing.Size(45, 26);
            this.maskedTextBoxMetaDataTimeout.TabIndex = 4;
            this.maskedTextBoxMetaDataTimeout.Text = "60";
            this.maskedTextBoxMetaDataTimeout.ValidatingType = typeof(int);
            // 
            // labelMetaDataTimeout
            // 
            this.labelMetaDataTimeout.AutoSize = true;
            this.labelMetaDataTimeout.Location = new System.Drawing.Point(10, 71);
            this.labelMetaDataTimeout.Name = "labelMetaDataTimeout";
            this.labelMetaDataTimeout.Size = new System.Drawing.Size(147, 20);
            this.labelMetaDataTimeout.TabIndex = 3;
            this.labelMetaDataTimeout.Text = "Meta-data Timeout:";
            this.labelMetaDataTimeout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // maskedTextBoxFrameRate
            // 
            this.maskedTextBoxFrameRate.Location = new System.Drawing.Point(163, 36);
            this.maskedTextBoxFrameRate.Mask = "000";
            this.maskedTextBoxFrameRate.Name = "maskedTextBoxFrameRate";
            this.maskedTextBoxFrameRate.Size = new System.Drawing.Size(45, 26);
            this.maskedTextBoxFrameRate.TabIndex = 1;
            this.maskedTextBoxFrameRate.Text = "30";
            this.maskedTextBoxFrameRate.ValidatingType = typeof(int);
            // 
            // labelFrameRate
            // 
            this.labelFrameRate.AutoSize = true;
            this.labelFrameRate.Location = new System.Drawing.Point(59, 39);
            this.labelFrameRate.Name = "labelFrameRate";
            this.labelFrameRate.Size = new System.Drawing.Size(98, 20);
            this.labelFrameRate.TabIndex = 0;
            this.labelFrameRate.Text = "Frame Rate:";
            this.labelFrameRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelEndTime
            // 
            this.labelEndTime.AutoSize = true;
            this.labelEndTime.Location = new System.Drawing.Point(296, 71);
            this.labelEndTime.Name = "labelEndTime";
            this.labelEndTime.Size = new System.Drawing.Size(80, 20);
            this.labelEndTime.TabIndex = 10;
            this.labelEndTime.Text = "End Time:";
            this.labelEndTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerEndTime
            // 
            this.dateTimePickerEndTime.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dateTimePickerEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEndTime.Location = new System.Drawing.Point(382, 66);
            this.dateTimePickerEndTime.Name = "dateTimePickerEndTime";
            this.dateTimePickerEndTime.Size = new System.Drawing.Size(219, 26);
            this.dateTimePickerEndTime.TabIndex = 11;
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(290, 39);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(86, 20);
            this.labelStartTime.TabIndex = 8;
            this.labelStartTime.Text = "Start Time:";
            this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerSourceTime
            // 
            this.dateTimePickerSourceTime.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dateTimePickerSourceTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSourceTime.Location = new System.Drawing.Point(382, 34);
            this.dateTimePickerSourceTime.Name = "dateTimePickerSourceTime";
            this.dateTimePickerSourceTime.Size = new System.Drawing.Size(219, 26);
            this.dateTimePickerSourceTime.TabIndex = 9;
            // 
            // checkBoxEnableDetailedLogging
            // 
            this.checkBoxEnableLogging.AutoSize = true;
            this.checkBoxEnableLogging.Checked = true;
            this.checkBoxEnableLogging.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnableLogging.Location = new System.Drawing.Point(382, 102);
            this.checkBoxEnableLogging.Name = "checkBoxEnableLogging";
            this.checkBoxEnableLogging.Size = new System.Drawing.Size(209, 24);
            this.checkBoxEnableLogging.TabIndex = 12;
            this.checkBoxEnableLogging.Text = "Enable Detailed Logging";
            this.checkBoxEnableLogging.UseVisualStyleBackColor = true;
            // 
            // ComparisonUtility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 657);
            this.Controls.Add(this.groupBoxComparisonOptions);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.groupBoxMessages);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.groupBoxDestinationHistorian);
            this.Controls.Add(this.groupBoxSourceHistorian);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ComparisonUtility";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "openHistorian Archive Comparison Utility";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ComparisonUtility_FormClosing);
            this.Load += new System.EventHandler(this.ComparisonUtility_Load);
            this.groupBoxSourceHistorian.ResumeLayout(false);
            this.groupBoxSourceHistorian.PerformLayout();
            this.groupBoxDestinationHistorian.ResumeLayout(false);
            this.groupBoxDestinationHistorian.PerformLayout();
            this.groupBoxMessages.ResumeLayout(false);
            this.groupBoxMessages.PerformLayout();
            this.groupBoxComparisonOptions.ResumeLayout(false);
            this.groupBoxComparisonOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSourceHistorian;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxSourceHistorianDataPort;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxSourceHistorianMetaDataPort;
        private System.Windows.Forms.Label labelSourceHistorianDataPort;
        private System.Windows.Forms.Label labelSourceHistorianMetaDataPort;
        private System.Windows.Forms.Label labelSourceHistorianHostAddress;
        private System.Windows.Forms.TextBox textBoxSourceHistorianHostAddress;
        private System.Windows.Forms.GroupBox groupBoxDestinationHistorian;
        private System.Windows.Forms.Label labelDestinationHistorianHostAddress;
        private System.Windows.Forms.TextBox textBoxDestinationHistorianHostAddress;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxDestinationHistorianDataPort;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxDestinationHistorianMetaDataPort;
        private System.Windows.Forms.Label labelDestinationHistorianDataPort;
        private System.Windows.Forms.Label labelDestinationHistorianMetaDataPort;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox groupBoxMessages;
        private System.Windows.Forms.TextBox textBoxMessageOutput;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Label labelSourceHistorianInstanceName;
        private System.Windows.Forms.TextBox textBoxSourceHistorianInstanceName;
        private System.Windows.Forms.Label labelDestinationHistorianInstanceName;
        private System.Windows.Forms.TextBox textBoxDestinationHistorianInstanceName;
        private System.Windows.Forms.GroupBox groupBoxComparisonOptions;
        private System.Windows.Forms.Label labelEndTime;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndTime;
        private System.Windows.Forms.Label labelStartTime;
        private System.Windows.Forms.DateTimePicker dateTimePickerSourceTime;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxFrameRate;
        private System.Windows.Forms.Label labelFrameRate;
        private System.Windows.Forms.Label labelSeconds;
        private System.Windows.Forms.Label labelPerSec;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxMetaDataTimeout;
        private System.Windows.Forms.Label labelMetaDataTimeout;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxMessageInterval;
        private System.Windows.Forms.Label labelMessageInterval;
        private System.Windows.Forms.CheckBox checkBoxEnableLogging;
    }
}