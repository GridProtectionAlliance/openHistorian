namespace OHTransfer
{
    partial class OHTransfer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OHTransfer));
            this.groupBoxMessages = new System.Windows.Forms.GroupBox();
            this.textBoxMessageOutput = new System.Windows.Forms.TextBox();
            this.groupBoxTransferConfiguration = new System.Windows.Forms.GroupBox();
            this.labelToUTC = new System.Windows.Forms.Label();
            this.labelFromUTC = new System.Windows.Forms.Label();
            this.labelToDate = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.labelFromDate = new System.Windows.Forms.Label();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.textBoxDestinationCSVMeasurements = new System.Windows.Forms.TextBox();
            this.labelDestinationCSVMeasurements = new System.Windows.Forms.Label();
            this.buttonOpenDestinationCSVMeasurementsFile = new System.Windows.Forms.Button();
            this.textBoxSourceCSVMeasurements = new System.Windows.Forms.TextBox();
            this.labelSourceCSVMeasurements = new System.Windows.Forms.Label();
            this.buttonOpenSourceCSVMeasurementsFile = new System.Windows.Forms.Button();
            this.textBoxDestinationFiles = new System.Windows.Forms.TextBox();
            this.labelDestinationFilesLocation = new System.Windows.Forms.Label();
            this.buttonOpenDestinationFilesLocation = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonGo = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxSourceFiles = new System.Windows.Forms.TextBox();
            this.labelSourceFilesLocation = new System.Windows.Forms.Label();
            this.buttonOpenSourceFilesLocation = new System.Windows.Forms.Button();
            this.groupBoxMessages.SuspendLayout();
            this.groupBoxTransferConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMessages
            // 
            this.groupBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMessages.Controls.Add(this.textBoxMessageOutput);
            this.groupBoxMessages.Location = new System.Drawing.Point(15, 196);
            this.groupBoxMessages.Name = "groupBoxMessages";
            this.groupBoxMessages.Size = new System.Drawing.Size(777, 304);
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
            this.textBoxMessageOutput.Location = new System.Drawing.Point(3, 16);
            this.textBoxMessageOutput.Multiline = true;
            this.textBoxMessageOutput.Name = "textBoxMessageOutput";
            this.textBoxMessageOutput.ReadOnly = true;
            this.textBoxMessageOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessageOutput.Size = new System.Drawing.Size(771, 285);
            this.textBoxMessageOutput.TabIndex = 0;
            this.textBoxMessageOutput.TabStop = false;
            // 
            // groupBoxTransferConfiguration
            // 
            this.groupBoxTransferConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTransferConfiguration.Controls.Add(this.textBoxSourceFiles);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelSourceFilesLocation);
            this.groupBoxTransferConfiguration.Controls.Add(this.buttonOpenSourceFilesLocation);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelToUTC);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelFromUTC);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelToDate);
            this.groupBoxTransferConfiguration.Controls.Add(this.dateTimePickerTo);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelFromDate);
            this.groupBoxTransferConfiguration.Controls.Add(this.dateTimePickerFrom);
            this.groupBoxTransferConfiguration.Controls.Add(this.textBoxDestinationCSVMeasurements);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelDestinationCSVMeasurements);
            this.groupBoxTransferConfiguration.Controls.Add(this.buttonOpenDestinationCSVMeasurementsFile);
            this.groupBoxTransferConfiguration.Controls.Add(this.textBoxSourceCSVMeasurements);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelSourceCSVMeasurements);
            this.groupBoxTransferConfiguration.Controls.Add(this.buttonOpenSourceCSVMeasurementsFile);
            this.groupBoxTransferConfiguration.Controls.Add(this.textBoxDestinationFiles);
            this.groupBoxTransferConfiguration.Controls.Add(this.labelDestinationFilesLocation);
            this.groupBoxTransferConfiguration.Controls.Add(this.buttonOpenDestinationFilesLocation);
            this.groupBoxTransferConfiguration.Location = new System.Drawing.Point(18, 12);
            this.groupBoxTransferConfiguration.Name = "groupBoxTransferConfiguration";
            this.groupBoxTransferConfiguration.Size = new System.Drawing.Size(775, 178);
            this.groupBoxTransferConfiguration.TabIndex = 1;
            this.groupBoxTransferConfiguration.TabStop = false;
            this.groupBoxTransferConfiguration.Text = "Transfer Configuration";
            // 
            // labelToUTC
            // 
            this.labelToUTC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelToUTC.AutoSize = true;
            this.labelToUTC.Location = new System.Drawing.Point(727, 144);
            this.labelToUTC.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelToUTC.Name = "labelToUTC";
            this.labelToUTC.Size = new System.Drawing.Size(29, 13);
            this.labelToUTC.TabIndex = 17;
            this.labelToUTC.Text = "UTC";
            this.labelToUTC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFromUTC
            // 
            this.labelFromUTC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFromUTC.AutoSize = true;
            this.labelFromUTC.Location = new System.Drawing.Point(517, 144);
            this.labelFromUTC.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFromUTC.Name = "labelFromUTC";
            this.labelFromUTC.Size = new System.Drawing.Size(29, 13);
            this.labelFromUTC.TabIndex = 16;
            this.labelFromUTC.Text = "UTC";
            this.labelFromUTC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelToDate
            // 
            this.labelToDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelToDate.AutoSize = true;
            this.labelToDate.Location = new System.Drawing.Point(559, 144);
            this.labelToDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelToDate.Name = "labelToDate";
            this.labelToDate.Size = new System.Drawing.Size(23, 13);
            this.labelToDate.TabIndex = 14;
            this.labelToDate.Text = "&To:";
            this.labelToDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerTo.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerTo.Location = new System.Drawing.Point(584, 141);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(142, 20);
            this.dateTimePickerTo.TabIndex = 15;
            // 
            // labelFromDate
            // 
            this.labelFromDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFromDate.AutoSize = true;
            this.labelFromDate.Location = new System.Drawing.Point(339, 144);
            this.labelFromDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFromDate.Name = "labelFromDate";
            this.labelFromDate.Size = new System.Drawing.Size(33, 13);
            this.labelFromDate.TabIndex = 12;
            this.labelFromDate.Text = "&From:";
            this.labelFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerFrom.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerFrom.Location = new System.Drawing.Point(375, 141);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(142, 20);
            this.dateTimePickerFrom.TabIndex = 13;
            // 
            // textBoxDestinationCSVMeasurements
            // 
            this.textBoxDestinationCSVMeasurements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDestinationCSVMeasurements.Location = new System.Drawing.Point(179, 109);
            this.textBoxDestinationCSVMeasurements.Name = "textBoxDestinationCSVMeasurements";
            this.textBoxDestinationCSVMeasurements.Size = new System.Drawing.Size(547, 20);
            this.textBoxDestinationCSVMeasurements.TabIndex = 10;
            // 
            // labelDestinationCSVMeasurements
            // 
            this.labelDestinationCSVMeasurements.AutoSize = true;
            this.labelDestinationCSVMeasurements.Location = new System.Drawing.Point(19, 112);
            this.labelDestinationCSVMeasurements.Name = "labelDestinationCSVMeasurements";
            this.labelDestinationCSVMeasurements.Size = new System.Drawing.Size(159, 13);
            this.labelDestinationCSVMeasurements.TabIndex = 9;
            this.labelDestinationCSVMeasurements.Text = "&Destination CSV Measurements:";
            this.labelDestinationCSVMeasurements.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOpenDestinationCSVMeasurementsFile
            // 
            this.buttonOpenDestinationCSVMeasurementsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenDestinationCSVMeasurementsFile.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenDestinationCSVMeasurementsFile.Location = new System.Drawing.Point(726, 108);
            this.buttonOpenDestinationCSVMeasurementsFile.Name = "buttonOpenDestinationCSVMeasurementsFile";
            this.buttonOpenDestinationCSVMeasurementsFile.Size = new System.Drawing.Size(28, 23);
            this.buttonOpenDestinationCSVMeasurementsFile.TabIndex = 11;
            this.buttonOpenDestinationCSVMeasurementsFile.Text = "...";
            this.buttonOpenDestinationCSVMeasurementsFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonOpenDestinationCSVMeasurementsFile.UseVisualStyleBackColor = true;
            this.buttonOpenDestinationCSVMeasurementsFile.Click += new System.EventHandler(this.buttonOpenDestinationCSVMeasurementsFile_Click);
            // 
            // textBoxSourceCSVMeasurements
            // 
            this.textBoxSourceCSVMeasurements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSourceCSVMeasurements.Location = new System.Drawing.Point(179, 80);
            this.textBoxSourceCSVMeasurements.Name = "textBoxSourceCSVMeasurements";
            this.textBoxSourceCSVMeasurements.Size = new System.Drawing.Size(547, 20);
            this.textBoxSourceCSVMeasurements.TabIndex = 7;
            // 
            // labelSourceCSVMeasurements
            // 
            this.labelSourceCSVMeasurements.AutoSize = true;
            this.labelSourceCSVMeasurements.Location = new System.Drawing.Point(38, 83);
            this.labelSourceCSVMeasurements.Name = "labelSourceCSVMeasurements";
            this.labelSourceCSVMeasurements.Size = new System.Drawing.Size(140, 13);
            this.labelSourceCSVMeasurements.TabIndex = 6;
            this.labelSourceCSVMeasurements.Text = "&Source CSV Measurements:";
            this.labelSourceCSVMeasurements.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOpenSourceCSVMeasurementsFile
            // 
            this.buttonOpenSourceCSVMeasurementsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenSourceCSVMeasurementsFile.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenSourceCSVMeasurementsFile.Location = new System.Drawing.Point(726, 79);
            this.buttonOpenSourceCSVMeasurementsFile.Name = "buttonOpenSourceCSVMeasurementsFile";
            this.buttonOpenSourceCSVMeasurementsFile.Size = new System.Drawing.Size(28, 23);
            this.buttonOpenSourceCSVMeasurementsFile.TabIndex = 8;
            this.buttonOpenSourceCSVMeasurementsFile.Text = "...";
            this.buttonOpenSourceCSVMeasurementsFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonOpenSourceCSVMeasurementsFile.UseVisualStyleBackColor = true;
            this.buttonOpenSourceCSVMeasurementsFile.Click += new System.EventHandler(this.buttonOpenSourceCSVMeasurementsFile_Click);
            // 
            // textBoxDestinationFiles
            // 
            this.textBoxDestinationFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDestinationFiles.Location = new System.Drawing.Point(178, 53);
            this.textBoxDestinationFiles.Name = "textBoxDestinationFiles";
            this.textBoxDestinationFiles.Size = new System.Drawing.Size(547, 20);
            this.textBoxDestinationFiles.TabIndex = 4;
            // 
            // labelDestinationFilesLocation
            // 
            this.labelDestinationFilesLocation.AutoSize = true;
            this.labelDestinationFilesLocation.Location = new System.Drawing.Point(54, 56);
            this.labelDestinationFilesLocation.Name = "labelDestinationFilesLocation";
            this.labelDestinationFilesLocation.Size = new System.Drawing.Size(124, 13);
            this.labelDestinationFilesLocation.TabIndex = 3;
            this.labelDestinationFilesLocation.Text = "&Destination files location:";
            this.labelDestinationFilesLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOpenDestinationFilesLocation
            // 
            this.buttonOpenDestinationFilesLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenDestinationFilesLocation.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenDestinationFilesLocation.Location = new System.Drawing.Point(726, 50);
            this.buttonOpenDestinationFilesLocation.Name = "buttonOpenDestinationFilesLocation";
            this.buttonOpenDestinationFilesLocation.Size = new System.Drawing.Size(28, 23);
            this.buttonOpenDestinationFilesLocation.TabIndex = 5;
            this.buttonOpenDestinationFilesLocation.Text = "...";
            this.buttonOpenDestinationFilesLocation.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonOpenDestinationFilesLocation.UseVisualStyleBackColor = true;
            this.buttonOpenDestinationFilesLocation.Click += new System.EventHandler(this.buttonOpenDestinationFilesLocation_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 200;
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 200;
            this.toolTip.ReshowDelay = 40;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csv";
            this.openFileDialog.Filter = "\"CSV files|*.csv|All files|*.*";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(15, 506);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(697, 23);
            this.progressBar.TabIndex = 4;
            // 
            // buttonGo
            // 
            this.buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGo.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGo.Location = new System.Drawing.Point(718, 506);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 23);
            this.buttonGo.TabIndex = 2;
            this.buttonGo.Text = "&Go!";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(718, 506);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxSourceFiles
            // 
            this.textBoxSourceFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSourceFiles.Location = new System.Drawing.Point(178, 25);
            this.textBoxSourceFiles.Name = "textBoxSourceFiles";
            this.textBoxSourceFiles.Size = new System.Drawing.Size(547, 20);
            this.textBoxSourceFiles.TabIndex = 1;
            // 
            // labelSourceFilesLocation
            // 
            this.labelSourceFilesLocation.AutoSize = true;
            this.labelSourceFilesLocation.Location = new System.Drawing.Point(73, 28);
            this.labelSourceFilesLocation.Name = "labelSourceFilesLocation";
            this.labelSourceFilesLocation.Size = new System.Drawing.Size(105, 13);
            this.labelSourceFilesLocation.TabIndex = 0;
            this.labelSourceFilesLocation.Text = "&Source files location:";
            this.labelSourceFilesLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOpenSourceFilesLocation
            // 
            this.buttonOpenSourceFilesLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenSourceFilesLocation.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenSourceFilesLocation.Location = new System.Drawing.Point(726, 22);
            this.buttonOpenSourceFilesLocation.Name = "buttonOpenSourceFilesLocation";
            this.buttonOpenSourceFilesLocation.Size = new System.Drawing.Size(28, 23);
            this.buttonOpenSourceFilesLocation.TabIndex = 2;
            this.buttonOpenSourceFilesLocation.Text = "...";
            this.buttonOpenSourceFilesLocation.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonOpenSourceFilesLocation.UseVisualStyleBackColor = true;
            this.buttonOpenSourceFilesLocation.Click += new System.EventHandler(this.buttonOpenSourceFilesLocation_Click);
            // 
            // OHTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 541);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.groupBoxTransferConfiguration);
            this.Controls.Add(this.groupBoxMessages);
            this.Controls.Add(this.buttonCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(720, 480);
            this.Name = "OHTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "openHistorian Data Transfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OHTransfer_FormClosing);
            this.groupBoxMessages.ResumeLayout(false);
            this.groupBoxMessages.PerformLayout();
            this.groupBoxTransferConfiguration.ResumeLayout(false);
            this.groupBoxTransferConfiguration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBoxMessages;
        private System.Windows.Forms.TextBox textBoxMessageOutput;
        private System.Windows.Forms.GroupBox groupBoxTransferConfiguration;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox textBoxDestinationCSVMeasurements;
        private System.Windows.Forms.Label labelDestinationCSVMeasurements;
        private System.Windows.Forms.Button buttonOpenDestinationCSVMeasurementsFile;
        private System.Windows.Forms.TextBox textBoxSourceCSVMeasurements;
        private System.Windows.Forms.Label labelSourceCSVMeasurements;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Button buttonOpenSourceCSVMeasurementsFile;
        private System.Windows.Forms.TextBox textBoxDestinationFiles;
        private System.Windows.Forms.Label labelDestinationFilesLocation;
        private System.Windows.Forms.Button buttonOpenDestinationFilesLocation;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Label labelToDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.Label labelFromDate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelToUTC;
        private System.Windows.Forms.Label labelFromUTC;
        private System.Windows.Forms.TextBox textBoxSourceFiles;
        private System.Windows.Forms.Label labelSourceFilesLocation;
        private System.Windows.Forms.Button buttonOpenSourceFilesLocation;
    }
}

