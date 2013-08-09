namespace HistorianPlaybackUtility
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.SplitContainerFull = new System.Windows.Forms.SplitContainer();
            this.SplitContainerTop = new System.Windows.Forms.SplitContainer();
            this.InputSelectionContainer = new System.Windows.Forms.GroupBox();
            this.SearchPhraseClear = new System.Windows.Forms.LinkLabel();
            this.SearchPhraseFind = new System.Windows.Forms.LinkLabel();
            this.ArchiveLocationBrowse = new System.Windows.Forms.LinkLabel();
            this.SearchPhraseLabel = new System.Windows.Forms.Label();
            this.SearchPhraseInput = new System.Windows.Forms.TextBox();
            this.EndTimeInput = new System.Windows.Forms.DateTimePicker();
            this.ArchiveLocationInput = new System.Windows.Forms.TextBox();
            this.ArchiveLocationLabel = new System.Windows.Forms.Label();
            this.StartTimeInput = new System.Windows.Forms.DateTimePicker();
            this.EndTimeLabel = new System.Windows.Forms.Label();
            this.StartTimeLabel = new System.Windows.Forms.Label();
            this.IDInput = new System.Windows.Forms.CheckedListBox();
            this.OutputSelectionContainer = new System.Windows.Forms.GroupBox();
            this.OutputChannelTabs = new System.Windows.Forms.TabControl();
            this.TCPSettingsTab = new System.Windows.Forms.TabPage();
            this.TCPPortInput = new System.Windows.Forms.TextBox();
            this.TCPPortLabel = new System.Windows.Forms.Label();
            this.TCPServerInput = new System.Windows.Forms.TextBox();
            this.TCPServerLabel = new System.Windows.Forms.Label();
            this.UDPSettingsTab = new System.Windows.Forms.TabPage();
            this.UDPPortInput = new System.Windows.Forms.TextBox();
            this.UDPPortLabel = new System.Windows.Forms.Label();
            this.UDPServerInput = new System.Windows.Forms.TextBox();
            this.UDPServerLabel = new System.Windows.Forms.Label();
            this.FileSettingsTab = new System.Windows.Forms.TabPage();
            this.FileNameBrowse = new System.Windows.Forms.Button();
            this.FileNameInput = new System.Windows.Forms.TextBox();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.SerialSettingsTab = new System.Windows.Forms.TabPage();
            this.SerialRtsEnable = new System.Windows.Forms.CheckBox();
            this.SerialDtrEnable = new System.Windows.Forms.CheckBox();
            this.SerialDataBitsInput = new System.Windows.Forms.TextBox();
            this.SerialDataBitsLabel = new System.Windows.Forms.Label();
            this.SerialStopBitsInput = new System.Windows.Forms.ComboBox();
            this.SerialStopBitsLabel = new System.Windows.Forms.Label();
            this.SerialParityInput = new System.Windows.Forms.ComboBox();
            this.SerialParityLabel = new System.Windows.Forms.Label();
            this.SerialBaudRateInput = new System.Windows.Forms.ComboBox();
            this.SerialBaudRateLabel = new System.Windows.Forms.Label();
            this.SerialPortInput = new System.Windows.Forms.ComboBox();
            this.SerialPortLabel = new System.Windows.Forms.Label();
            this.ProcessingSpeedContainer = new System.Windows.Forms.Panel();
            this.ProcessDataFullSpeed = new System.Windows.Forms.RadioButton();
            this.ProcessDataAtIntervalSampleRate = new System.Windows.Forms.TextBox();
            this.ProcessDataAtInterval = new System.Windows.Forms.RadioButton();
            this.RepeatDataProcessing = new System.Windows.Forms.CheckBox();
            this.ProcessDataInParallel = new System.Windows.Forms.CheckBox();
            this.OutputFormatContainer = new System.Windows.Forms.Panel();
            this.OutputPlainTextDataFormat = new System.Windows.Forms.RichTextBox();
            this.OutputPlainTextData = new System.Windows.Forms.RadioButton();
            this.OutputBinaryData = new System.Windows.Forms.RadioButton();
            this.OutputChannelLabel = new System.Windows.Forms.Label();
            this.MessagesContainer = new System.Windows.Forms.GroupBox();
            this.MessagesOutput = new System.Windows.Forms.TextBox();
            this.StartProcessing = new System.Windows.Forms.Button();
            this.StopProcessing = new System.Windows.Forms.Button();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.AppendToExisting = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerFull)).BeginInit();
            this.SplitContainerFull.Panel1.SuspendLayout();
            this.SplitContainerFull.Panel2.SuspendLayout();
            this.SplitContainerFull.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerTop)).BeginInit();
            this.SplitContainerTop.Panel1.SuspendLayout();
            this.SplitContainerTop.Panel2.SuspendLayout();
            this.SplitContainerTop.SuspendLayout();
            this.InputSelectionContainer.SuspendLayout();
            this.OutputSelectionContainer.SuspendLayout();
            this.OutputChannelTabs.SuspendLayout();
            this.TCPSettingsTab.SuspendLayout();
            this.UDPSettingsTab.SuspendLayout();
            this.FileSettingsTab.SuspendLayout();
            this.SerialSettingsTab.SuspendLayout();
            this.ProcessingSpeedContainer.SuspendLayout();
            this.OutputFormatContainer.SuspendLayout();
            this.MessagesContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainerFull
            // 
            this.SplitContainerFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainerFull.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainerFull.Location = new System.Drawing.Point(0, 0);
            this.SplitContainerFull.Name = "SplitContainerFull";
            this.SplitContainerFull.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainerFull.Panel1
            // 
            this.SplitContainerFull.Panel1.Controls.Add(this.SplitContainerTop);
            // 
            // SplitContainerFull.Panel2
            // 
            this.SplitContainerFull.Panel2.Controls.Add(this.MessagesContainer);
            this.SplitContainerFull.Panel2.Controls.Add(this.StartProcessing);
            this.SplitContainerFull.Panel2.Controls.Add(this.StopProcessing);
            this.SplitContainerFull.Size = new System.Drawing.Size(684, 562);
            this.SplitContainerFull.SplitterDistance = 350;
            this.SplitContainerFull.TabIndex = 0;
            // 
            // SplitContainerTop
            // 
            this.SplitContainerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainerTop.Location = new System.Drawing.Point(0, 0);
            this.SplitContainerTop.Name = "SplitContainerTop";
            // 
            // SplitContainerTop.Panel1
            // 
            this.SplitContainerTop.Panel1.Controls.Add(this.InputSelectionContainer);
            // 
            // SplitContainerTop.Panel2
            // 
            this.SplitContainerTop.Panel2.Controls.Add(this.OutputSelectionContainer);
            this.SplitContainerTop.Size = new System.Drawing.Size(684, 350);
            this.SplitContainerTop.SplitterDistance = 342;
            this.SplitContainerTop.TabIndex = 0;
            // 
            // InputSelectionContainer
            // 
            this.InputSelectionContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputSelectionContainer.Controls.Add(this.SearchPhraseClear);
            this.InputSelectionContainer.Controls.Add(this.SearchPhraseFind);
            this.InputSelectionContainer.Controls.Add(this.ArchiveLocationBrowse);
            this.InputSelectionContainer.Controls.Add(this.SearchPhraseLabel);
            this.InputSelectionContainer.Controls.Add(this.SearchPhraseInput);
            this.InputSelectionContainer.Controls.Add(this.EndTimeInput);
            this.InputSelectionContainer.Controls.Add(this.ArchiveLocationInput);
            this.InputSelectionContainer.Controls.Add(this.ArchiveLocationLabel);
            this.InputSelectionContainer.Controls.Add(this.StartTimeInput);
            this.InputSelectionContainer.Controls.Add(this.EndTimeLabel);
            this.InputSelectionContainer.Controls.Add(this.StartTimeLabel);
            this.InputSelectionContainer.Controls.Add(this.IDInput);
            this.InputSelectionContainer.Location = new System.Drawing.Point(12, 12);
            this.InputSelectionContainer.Name = "InputSelectionContainer";
            this.InputSelectionContainer.Size = new System.Drawing.Size(324, 335);
            this.InputSelectionContainer.TabIndex = 1001;
            this.InputSelectionContainer.TabStop = false;
            this.InputSelectionContainer.Text = "Input Selection";
            // 
            // SearchPhraseClear
            // 
            this.SearchPhraseClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchPhraseClear.AutoSize = true;
            this.SearchPhraseClear.Location = new System.Drawing.Point(260, 59);
            this.SearchPhraseClear.Name = "SearchPhraseClear";
            this.SearchPhraseClear.Size = new System.Drawing.Size(32, 13);
            this.SearchPhraseClear.TabIndex = 4;
            this.SearchPhraseClear.TabStop = true;
            this.SearchPhraseClear.Text = "Clear";
            this.SearchPhraseClear.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SearchPhraseClear_LinkClicked);
            // 
            // SearchPhraseFind
            // 
            this.SearchPhraseFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchPhraseFind.AutoSize = true;
            this.SearchPhraseFind.Location = new System.Drawing.Point(293, 59);
            this.SearchPhraseFind.Name = "SearchPhraseFind";
            this.SearchPhraseFind.Size = new System.Drawing.Size(27, 13);
            this.SearchPhraseFind.TabIndex = 3;
            this.SearchPhraseFind.TabStop = true;
            this.SearchPhraseFind.Text = "Find";
            this.SearchPhraseFind.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SearchPhraseFind_LinkClicked);
            // 
            // ArchiveLocationBrowse
            // 
            this.ArchiveLocationBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ArchiveLocationBrowse.AutoSize = true;
            this.ArchiveLocationBrowse.Location = new System.Drawing.Point(278, 17);
            this.ArchiveLocationBrowse.Name = "ArchiveLocationBrowse";
            this.ArchiveLocationBrowse.Size = new System.Drawing.Size(42, 13);
            this.ArchiveLocationBrowse.TabIndex = 1;
            this.ArchiveLocationBrowse.TabStop = true;
            this.ArchiveLocationBrowse.Text = "Browse";
            this.ArchiveLocationBrowse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ArchiveLocationBrowse_LinkClicked);
            // 
            // SearchPhraseLabel
            // 
            this.SearchPhraseLabel.AutoSize = true;
            this.SearchPhraseLabel.Location = new System.Drawing.Point(6, 59);
            this.SearchPhraseLabel.Name = "SearchPhraseLabel";
            this.SearchPhraseLabel.Size = new System.Drawing.Size(80, 13);
            this.SearchPhraseLabel.TabIndex = 13;
            this.SearchPhraseLabel.Text = "Search Phrase:";
            // 
            // SearchPhraseInput
            // 
            this.SearchPhraseInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchPhraseInput.Location = new System.Drawing.Point(6, 75);
            this.SearchPhraseInput.Name = "SearchPhraseInput";
            this.SearchPhraseInput.Size = new System.Drawing.Size(311, 21);
            this.SearchPhraseInput.TabIndex = 2;
            this.SearchPhraseInput.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchPhraseInput_MouseClick);
            this.SearchPhraseInput.TextChanged += new System.EventHandler(this.SearchPhraseInput_TextChanged);
            this.SearchPhraseInput.Leave += new System.EventHandler(this.SearchPhraseInput_Leave);
            // 
            // EndTimeInput
            // 
            this.EndTimeInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EndTimeInput.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.EndTimeInput.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.EndTimeInput.Location = new System.Drawing.Point(172, 308);
            this.EndTimeInput.Name = "EndTimeInput";
            this.EndTimeInput.Size = new System.Drawing.Size(145, 21);
            this.EndTimeInput.TabIndex = 7;
            // 
            // ArchiveLocationInput
            // 
            this.ArchiveLocationInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ArchiveLocationInput.Location = new System.Drawing.Point(6, 33);
            this.ArchiveLocationInput.Name = "ArchiveLocationInput";
            this.ArchiveLocationInput.Size = new System.Drawing.Size(311, 21);
            this.ArchiveLocationInput.TabIndex = 0;
            this.ArchiveLocationInput.TextChanged += new System.EventHandler(this.ArchiveLocationInput_TextChanged);
            // 
            // ArchiveLocationLabel
            // 
            this.ArchiveLocationLabel.AutoSize = true;
            this.ArchiveLocationLabel.Location = new System.Drawing.Point(6, 17);
            this.ArchiveLocationLabel.Name = "ArchiveLocationLabel";
            this.ArchiveLocationLabel.Size = new System.Drawing.Size(90, 13);
            this.ArchiveLocationLabel.TabIndex = 0;
            this.ArchiveLocationLabel.Text = "Archive Location:";
            // 
            // StartTimeInput
            // 
            this.StartTimeInput.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.StartTimeInput.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.StartTimeInput.Location = new System.Drawing.Point(6, 308);
            this.StartTimeInput.Name = "StartTimeInput";
            this.StartTimeInput.Size = new System.Drawing.Size(145, 21);
            this.StartTimeInput.TabIndex = 6;
            // 
            // EndTimeLabel
            // 
            this.EndTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EndTimeLabel.AutoSize = true;
            this.EndTimeLabel.Location = new System.Drawing.Point(169, 292);
            this.EndTimeLabel.Name = "EndTimeLabel";
            this.EndTimeLabel.Size = new System.Drawing.Size(96, 13);
            this.EndTimeLabel.TabIndex = 0;
            this.EndTimeLabel.Text = "End Time (in UTC):";
            // 
            // StartTimeLabel
            // 
            this.StartTimeLabel.AutoSize = true;
            this.StartTimeLabel.Location = new System.Drawing.Point(3, 292);
            this.StartTimeLabel.Name = "StartTimeLabel";
            this.StartTimeLabel.Size = new System.Drawing.Size(102, 13);
            this.StartTimeLabel.TabIndex = 0;
            this.StartTimeLabel.Text = "Start Time (in UTC):";
            // 
            // IDInput
            // 
            this.IDInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IDInput.CheckOnClick = true;
            this.IDInput.FormattingEnabled = true;
            this.IDInput.HorizontalScrollbar = true;
            this.IDInput.Location = new System.Drawing.Point(6, 102);
            this.IDInput.Name = "IDInput";
            this.IDInput.Size = new System.Drawing.Size(311, 180);
            this.IDInput.TabIndex = 5;
            // 
            // OutputSelectionContainer
            // 
            this.OutputSelectionContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputSelectionContainer.Controls.Add(this.OutputChannelTabs);
            this.OutputSelectionContainer.Controls.Add(this.ProcessingSpeedContainer);
            this.OutputSelectionContainer.Controls.Add(this.RepeatDataProcessing);
            this.OutputSelectionContainer.Controls.Add(this.ProcessDataInParallel);
            this.OutputSelectionContainer.Controls.Add(this.OutputFormatContainer);
            this.OutputSelectionContainer.Controls.Add(this.OutputChannelLabel);
            this.OutputSelectionContainer.Location = new System.Drawing.Point(3, 12);
            this.OutputSelectionContainer.Name = "OutputSelectionContainer";
            this.OutputSelectionContainer.Size = new System.Drawing.Size(324, 335);
            this.OutputSelectionContainer.TabIndex = 1001;
            this.OutputSelectionContainer.TabStop = false;
            this.OutputSelectionContainer.Text = "Output Selection";
            // 
            // OutputChannelTabs
            // 
            this.OutputChannelTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputChannelTabs.Controls.Add(this.TCPSettingsTab);
            this.OutputChannelTabs.Controls.Add(this.UDPSettingsTab);
            this.OutputChannelTabs.Controls.Add(this.FileSettingsTab);
            this.OutputChannelTabs.Controls.Add(this.SerialSettingsTab);
            this.OutputChannelTabs.Location = new System.Drawing.Point(6, 33);
            this.OutputChannelTabs.Name = "OutputChannelTabs";
            this.OutputChannelTabs.SelectedIndex = 0;
            this.OutputChannelTabs.Size = new System.Drawing.Size(314, 109);
            this.OutputChannelTabs.TabIndex = 8;
            // 
            // TCPSettingsTab
            // 
            this.TCPSettingsTab.Controls.Add(this.TCPPortInput);
            this.TCPSettingsTab.Controls.Add(this.TCPPortLabel);
            this.TCPSettingsTab.Controls.Add(this.TCPServerInput);
            this.TCPSettingsTab.Controls.Add(this.TCPServerLabel);
            this.TCPSettingsTab.Location = new System.Drawing.Point(4, 22);
            this.TCPSettingsTab.Name = "TCPSettingsTab";
            this.TCPSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.TCPSettingsTab.Size = new System.Drawing.Size(306, 83);
            this.TCPSettingsTab.TabIndex = 0;
            this.TCPSettingsTab.Text = "TCP";
            this.TCPSettingsTab.UseVisualStyleBackColor = true;
            // 
            // TCPPortInput
            // 
            this.TCPPortInput.Location = new System.Drawing.Point(197, 6);
            this.TCPPortInput.Name = "TCPPortInput";
            this.TCPPortInput.Size = new System.Drawing.Size(50, 21);
            this.TCPPortInput.TabIndex = 10;
            this.TCPPortInput.Text = "8989";
            // 
            // TCPPortLabel
            // 
            this.TCPPortLabel.AutoSize = true;
            this.TCPPortLabel.Location = new System.Drawing.Point(160, 9);
            this.TCPPortLabel.Name = "TCPPortLabel";
            this.TCPPortLabel.Size = new System.Drawing.Size(31, 13);
            this.TCPPortLabel.TabIndex = 19;
            this.TCPPortLabel.Text = "Port:";
            // 
            // TCPServerInput
            // 
            this.TCPServerInput.Location = new System.Drawing.Point(52, 6);
            this.TCPServerInput.Name = "TCPServerInput";
            this.TCPServerInput.Size = new System.Drawing.Size(100, 21);
            this.TCPServerInput.TabIndex = 9;
            this.TCPServerInput.Text = "localhost";
            // 
            // TCPServerLabel
            // 
            this.TCPServerLabel.AutoSize = true;
            this.TCPServerLabel.Location = new System.Drawing.Point(3, 9);
            this.TCPServerLabel.Name = "TCPServerLabel";
            this.TCPServerLabel.Size = new System.Drawing.Size(43, 13);
            this.TCPServerLabel.TabIndex = 18;
            this.TCPServerLabel.Text = "Server:";
            // 
            // UDPSettingsTab
            // 
            this.UDPSettingsTab.Controls.Add(this.UDPPortInput);
            this.UDPSettingsTab.Controls.Add(this.UDPPortLabel);
            this.UDPSettingsTab.Controls.Add(this.UDPServerInput);
            this.UDPSettingsTab.Controls.Add(this.UDPServerLabel);
            this.UDPSettingsTab.Location = new System.Drawing.Point(4, 22);
            this.UDPSettingsTab.Name = "UDPSettingsTab";
            this.UDPSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.UDPSettingsTab.Size = new System.Drawing.Size(306, 83);
            this.UDPSettingsTab.TabIndex = 1;
            this.UDPSettingsTab.Text = "UDP";
            this.UDPSettingsTab.UseVisualStyleBackColor = true;
            // 
            // UDPPortInput
            // 
            this.UDPPortInput.Location = new System.Drawing.Point(197, 6);
            this.UDPPortInput.Name = "UDPPortInput";
            this.UDPPortInput.Size = new System.Drawing.Size(50, 21);
            this.UDPPortInput.TabIndex = 12;
            this.UDPPortInput.Text = "8989";
            // 
            // UDPPortLabel
            // 
            this.UDPPortLabel.AutoSize = true;
            this.UDPPortLabel.Location = new System.Drawing.Point(160, 9);
            this.UDPPortLabel.Name = "UDPPortLabel";
            this.UDPPortLabel.Size = new System.Drawing.Size(31, 13);
            this.UDPPortLabel.TabIndex = 27;
            this.UDPPortLabel.Text = "Port:";
            // 
            // UDPServerInput
            // 
            this.UDPServerInput.Location = new System.Drawing.Point(52, 6);
            this.UDPServerInput.Name = "UDPServerInput";
            this.UDPServerInput.Size = new System.Drawing.Size(100, 21);
            this.UDPServerInput.TabIndex = 11;
            this.UDPServerInput.Text = "localhost";
            // 
            // UDPServerLabel
            // 
            this.UDPServerLabel.AutoSize = true;
            this.UDPServerLabel.Location = new System.Drawing.Point(3, 9);
            this.UDPServerLabel.Name = "UDPServerLabel";
            this.UDPServerLabel.Size = new System.Drawing.Size(43, 13);
            this.UDPServerLabel.TabIndex = 26;
            this.UDPServerLabel.Text = "Server:";
            // 
            // FileSettingsTab
            // 
            this.FileSettingsTab.Controls.Add(this.AppendToExisting);
            this.FileSettingsTab.Controls.Add(this.FileNameBrowse);
            this.FileSettingsTab.Controls.Add(this.FileNameInput);
            this.FileSettingsTab.Controls.Add(this.FileNameLabel);
            this.FileSettingsTab.Location = new System.Drawing.Point(4, 22);
            this.FileSettingsTab.Name = "FileSettingsTab";
            this.FileSettingsTab.Size = new System.Drawing.Size(306, 83);
            this.FileSettingsTab.TabIndex = 2;
            this.FileSettingsTab.Text = "File";
            this.FileSettingsTab.UseVisualStyleBackColor = true;
            // 
            // FileNameBrowse
            // 
            this.FileNameBrowse.Location = new System.Drawing.Point(240, 6);
            this.FileNameBrowse.Name = "FileNameBrowse";
            this.FileNameBrowse.Size = new System.Drawing.Size(63, 23);
            this.FileNameBrowse.TabIndex = 14;
            this.FileNameBrowse.Text = "Browse";
            this.FileNameBrowse.UseVisualStyleBackColor = true;
            this.FileNameBrowse.Click += new System.EventHandler(this.FileNameBrowse_Click);
            // 
            // FileNameInput
            // 
            this.FileNameInput.Location = new System.Drawing.Point(60, 6);
            this.FileNameInput.Name = "FileNameInput";
            this.FileNameInput.Size = new System.Drawing.Size(172, 21);
            this.FileNameInput.TabIndex = 13;
            this.FileNameInput.Text = "output.csv";
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.AutoSize = true;
            this.FileNameLabel.Location = new System.Drawing.Point(3, 9);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(53, 13);
            this.FileNameLabel.TabIndex = 21;
            this.FileNameLabel.Text = "Filename:";
            // 
            // SerialSettingsTab
            // 
            this.SerialSettingsTab.Controls.Add(this.SerialRtsEnable);
            this.SerialSettingsTab.Controls.Add(this.SerialDtrEnable);
            this.SerialSettingsTab.Controls.Add(this.SerialDataBitsInput);
            this.SerialSettingsTab.Controls.Add(this.SerialDataBitsLabel);
            this.SerialSettingsTab.Controls.Add(this.SerialStopBitsInput);
            this.SerialSettingsTab.Controls.Add(this.SerialStopBitsLabel);
            this.SerialSettingsTab.Controls.Add(this.SerialParityInput);
            this.SerialSettingsTab.Controls.Add(this.SerialParityLabel);
            this.SerialSettingsTab.Controls.Add(this.SerialBaudRateInput);
            this.SerialSettingsTab.Controls.Add(this.SerialBaudRateLabel);
            this.SerialSettingsTab.Controls.Add(this.SerialPortInput);
            this.SerialSettingsTab.Controls.Add(this.SerialPortLabel);
            this.SerialSettingsTab.Location = new System.Drawing.Point(4, 22);
            this.SerialSettingsTab.Name = "SerialSettingsTab";
            this.SerialSettingsTab.Size = new System.Drawing.Size(306, 83);
            this.SerialSettingsTab.TabIndex = 3;
            this.SerialSettingsTab.Text = "Serial";
            this.SerialSettingsTab.UseVisualStyleBackColor = true;
            // 
            // SerialRtsEnable
            // 
            this.SerialRtsEnable.AutoSize = true;
            this.SerialRtsEnable.Location = new System.Drawing.Point(264, 60);
            this.SerialRtsEnable.Name = "SerialRtsEnable";
            this.SerialRtsEnable.Size = new System.Drawing.Size(48, 17);
            this.SerialRtsEnable.TabIndex = 30;
            this.SerialRtsEnable.Text = "RTS";
            this.SerialRtsEnable.UseVisualStyleBackColor = true;
            // 
            // SerialDtrEnable
            // 
            this.SerialDtrEnable.AutoSize = true;
            this.SerialDtrEnable.Location = new System.Drawing.Point(222, 60);
            this.SerialDtrEnable.Name = "SerialDtrEnable";
            this.SerialDtrEnable.Size = new System.Drawing.Size(49, 17);
            this.SerialDtrEnable.TabIndex = 29;
            this.SerialDtrEnable.Text = "DTR";
            this.SerialDtrEnable.UseVisualStyleBackColor = true;
            // 
            // SerialDataBitsInput
            // 
            this.SerialDataBitsInput.Location = new System.Drawing.Point(222, 31);
            this.SerialDataBitsInput.Name = "SerialDataBitsInput";
            this.SerialDataBitsInput.Size = new System.Drawing.Size(76, 21);
            this.SerialDataBitsInput.TabIndex = 28;
            this.SerialDataBitsInput.Text = "8";
            // 
            // SerialDataBitsLabel
            // 
            this.SerialDataBitsLabel.AutoSize = true;
            this.SerialDataBitsLabel.Location = new System.Drawing.Point(164, 34);
            this.SerialDataBitsLabel.Name = "SerialDataBitsLabel";
            this.SerialDataBitsLabel.Size = new System.Drawing.Size(54, 13);
            this.SerialDataBitsLabel.TabIndex = 8;
            this.SerialDataBitsLabel.Text = "Data Bits:";
            // 
            // SerialStopBitsInput
            // 
            this.SerialStopBitsInput.FormattingEnabled = true;
            this.SerialStopBitsInput.Items.AddRange(new object[] {
            "None",
            "One",
            "Two",
            "OnePointFive"});
            this.SerialStopBitsInput.Location = new System.Drawing.Point(222, 6);
            this.SerialStopBitsInput.Name = "SerialStopBitsInput";
            this.SerialStopBitsInput.Size = new System.Drawing.Size(76, 21);
            this.SerialStopBitsInput.TabIndex = 7;
            // 
            // SerialStopBitsLabel
            // 
            this.SerialStopBitsLabel.AutoSize = true;
            this.SerialStopBitsLabel.Location = new System.Drawing.Point(163, 9);
            this.SerialStopBitsLabel.Name = "SerialStopBitsLabel";
            this.SerialStopBitsLabel.Size = new System.Drawing.Size(53, 13);
            this.SerialStopBitsLabel.TabIndex = 6;
            this.SerialStopBitsLabel.Text = "Stop Bits:";
            // 
            // SerialParityInput
            // 
            this.SerialParityInput.FormattingEnabled = true;
            this.SerialParityInput.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.SerialParityInput.Location = new System.Drawing.Point(70, 56);
            this.SerialParityInput.Name = "SerialParityInput";
            this.SerialParityInput.Size = new System.Drawing.Size(76, 21);
            this.SerialParityInput.TabIndex = 5;
            // 
            // SerialParityLabel
            // 
            this.SerialParityLabel.AutoSize = true;
            this.SerialParityLabel.Location = new System.Drawing.Point(25, 60);
            this.SerialParityLabel.Name = "SerialParityLabel";
            this.SerialParityLabel.Size = new System.Drawing.Size(39, 13);
            this.SerialParityLabel.TabIndex = 4;
            this.SerialParityLabel.Text = "Parity:";
            // 
            // SerialBaudRateInput
            // 
            this.SerialBaudRateInput.FormattingEnabled = true;
            this.SerialBaudRateInput.Items.AddRange(new object[] {
            "115200",
            "57600",
            "38400",
            "19200",
            "9600",
            "4800",
            "2400",
            "1200"});
            this.SerialBaudRateInput.Location = new System.Drawing.Point(70, 31);
            this.SerialBaudRateInput.Name = "SerialBaudRateInput";
            this.SerialBaudRateInput.Size = new System.Drawing.Size(76, 21);
            this.SerialBaudRateInput.TabIndex = 3;
            // 
            // SerialBaudRateLabel
            // 
            this.SerialBaudRateLabel.AutoSize = true;
            this.SerialBaudRateLabel.Location = new System.Drawing.Point(3, 34);
            this.SerialBaudRateLabel.Name = "SerialBaudRateLabel";
            this.SerialBaudRateLabel.Size = new System.Drawing.Size(61, 13);
            this.SerialBaudRateLabel.TabIndex = 2;
            this.SerialBaudRateLabel.Text = "Baud Rate:";
            // 
            // SerialPortInput
            // 
            this.SerialPortInput.FormattingEnabled = true;
            this.SerialPortInput.Location = new System.Drawing.Point(70, 6);
            this.SerialPortInput.Name = "SerialPortInput";
            this.SerialPortInput.Size = new System.Drawing.Size(76, 21);
            this.SerialPortInput.TabIndex = 1;
            // 
            // SerialPortLabel
            // 
            this.SerialPortLabel.AutoSize = true;
            this.SerialPortLabel.Location = new System.Drawing.Point(33, 10);
            this.SerialPortLabel.Name = "SerialPortLabel";
            this.SerialPortLabel.Size = new System.Drawing.Size(31, 13);
            this.SerialPortLabel.TabIndex = 0;
            this.SerialPortLabel.Text = "Port:";
            // 
            // ProcessingSpeedContainer
            // 
            this.ProcessingSpeedContainer.Controls.Add(this.ProcessDataFullSpeed);
            this.ProcessingSpeedContainer.Controls.Add(this.ProcessDataAtIntervalSampleRate);
            this.ProcessingSpeedContainer.Controls.Add(this.ProcessDataAtInterval);
            this.ProcessingSpeedContainer.Location = new System.Drawing.Point(5, 244);
            this.ProcessingSpeedContainer.Name = "ProcessingSpeedContainer";
            this.ProcessingSpeedContainer.Size = new System.Drawing.Size(263, 44);
            this.ProcessingSpeedContainer.TabIndex = 1001;
            // 
            // ProcessDataFullSpeed
            // 
            this.ProcessDataFullSpeed.AutoSize = true;
            this.ProcessDataFullSpeed.Checked = true;
            this.ProcessDataFullSpeed.Location = new System.Drawing.Point(3, 3);
            this.ProcessDataFullSpeed.Name = "ProcessDataFullSpeed";
            this.ProcessDataFullSpeed.Size = new System.Drawing.Size(178, 17);
            this.ProcessDataFullSpeed.TabIndex = 18;
            this.ProcessDataFullSpeed.TabStop = true;
            this.ProcessDataFullSpeed.Text = "Process data as fast as possible";
            this.ProcessDataFullSpeed.UseVisualStyleBackColor = true;
            this.ProcessDataFullSpeed.CheckedChanged += new System.EventHandler(this.ProcessDataFullSpeed_CheckedChanged);
            // 
            // ProcessDataAtIntervalSampleRate
            // 
            this.ProcessDataAtIntervalSampleRate.Enabled = false;
            this.ProcessDataAtIntervalSampleRate.Location = new System.Drawing.Point(102, 21);
            this.ProcessDataAtIntervalSampleRate.Name = "ProcessDataAtIntervalSampleRate";
            this.ProcessDataAtIntervalSampleRate.Size = new System.Drawing.Size(30, 21);
            this.ProcessDataAtIntervalSampleRate.TabIndex = 27;
            this.ProcessDataAtIntervalSampleRate.Text = "0";
            // 
            // ProcessDataAtInterval
            // 
            this.ProcessDataAtInterval.AutoSize = true;
            this.ProcessDataAtInterval.Location = new System.Drawing.Point(3, 23);
            this.ProcessDataAtInterval.Name = "ProcessDataAtInterval";
            this.ProcessDataAtInterval.Size = new System.Drawing.Size(233, 17);
            this.ProcessDataAtInterval.TabIndex = 19;
            this.ProcessDataAtInterval.Text = "Process data at             samples per second";
            this.ProcessDataAtInterval.UseVisualStyleBackColor = true;
            this.ProcessDataAtInterval.CheckedChanged += new System.EventHandler(this.ProcessDataAtInterval_CheckedChanged);
            // 
            // RepeatDataProcessing
            // 
            this.RepeatDataProcessing.AutoSize = true;
            this.RepeatDataProcessing.Location = new System.Drawing.Point(8, 313);
            this.RepeatDataProcessing.Name = "RepeatDataProcessing";
            this.RepeatDataProcessing.Size = new System.Drawing.Size(214, 17);
            this.RepeatDataProcessing.TabIndex = 21;
            this.RepeatDataProcessing.Text = "Repeat processing of data until stopped";
            this.RepeatDataProcessing.UseVisualStyleBackColor = true;
            // 
            // ProcessDataInParallel
            // 
            this.ProcessDataInParallel.AutoSize = true;
            this.ProcessDataInParallel.Checked = true;
            this.ProcessDataInParallel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ProcessDataInParallel.Location = new System.Drawing.Point(8, 294);
            this.ProcessDataInParallel.Name = "ProcessDataInParallel";
            this.ProcessDataInParallel.Size = new System.Drawing.Size(254, 17);
            this.ProcessDataInParallel.TabIndex = 20;
            this.ProcessDataInParallel.Text = "Process selected data points in time sorted order";
            this.ProcessDataInParallel.UseVisualStyleBackColor = true;
            // 
            // OutputFormatContainer
            // 
            this.OutputFormatContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputFormatContainer.Controls.Add(this.OutputPlainTextDataFormat);
            this.OutputFormatContainer.Controls.Add(this.OutputPlainTextData);
            this.OutputFormatContainer.Controls.Add(this.OutputBinaryData);
            this.OutputFormatContainer.Location = new System.Drawing.Point(5, 144);
            this.OutputFormatContainer.Name = "OutputFormatContainer";
            this.OutputFormatContainer.Size = new System.Drawing.Size(315, 97);
            this.OutputFormatContainer.TabIndex = 1001;
            // 
            // OutputPlainTextDataFormat
            // 
            this.OutputPlainTextDataFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputPlainTextDataFormat.Location = new System.Drawing.Point(21, 40);
            this.OutputPlainTextDataFormat.Name = "OutputPlainTextDataFormat";
            this.OutputPlainTextDataFormat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.OutputPlainTextDataFormat.Size = new System.Drawing.Size(290, 54);
            this.OutputPlainTextDataFormat.TabIndex = 17;
            this.OutputPlainTextDataFormat.Text = "";
            // 
            // OutputPlainTextData
            // 
            this.OutputPlainTextData.AutoSize = true;
            this.OutputPlainTextData.Location = new System.Drawing.Point(4, 23);
            this.OutputPlainTextData.Name = "OutputPlainTextData";
            this.OutputPlainTextData.Size = new System.Drawing.Size(183, 17);
            this.OutputPlainTextData.TabIndex = 16;
            this.OutputPlainTextData.Text = "Output data in plain-text format:";
            this.OutputPlainTextData.UseVisualStyleBackColor = true;
            this.OutputPlainTextData.CheckedChanged += new System.EventHandler(this.OutputPlainTextData_CheckedChanged);
            // 
            // OutputBinaryData
            // 
            this.OutputBinaryData.AutoSize = true;
            this.OutputBinaryData.Checked = true;
            this.OutputBinaryData.Location = new System.Drawing.Point(4, 3);
            this.OutputBinaryData.Name = "OutputBinaryData";
            this.OutputBinaryData.Size = new System.Drawing.Size(163, 17);
            this.OutputBinaryData.TabIndex = 15;
            this.OutputBinaryData.TabStop = true;
            this.OutputBinaryData.Text = "Output data in binary format";
            this.OutputBinaryData.UseVisualStyleBackColor = true;
            this.OutputBinaryData.CheckedChanged += new System.EventHandler(this.OutputBinaryData_CheckedChanged);
            // 
            // OutputChannelLabel
            // 
            this.OutputChannelLabel.AutoSize = true;
            this.OutputChannelLabel.Location = new System.Drawing.Point(7, 17);
            this.OutputChannelLabel.Name = "OutputChannelLabel";
            this.OutputChannelLabel.Size = new System.Drawing.Size(87, 13);
            this.OutputChannelLabel.TabIndex = 19;
            this.OutputChannelLabel.Text = "Output Channel:\r\n";
            // 
            // MessagesContainer
            // 
            this.MessagesContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessagesContainer.Controls.Add(this.MessagesOutput);
            this.MessagesContainer.Location = new System.Drawing.Point(12, 3);
            this.MessagesContainer.Name = "MessagesContainer";
            this.MessagesContainer.Size = new System.Drawing.Size(661, 168);
            this.MessagesContainer.TabIndex = 1001;
            this.MessagesContainer.TabStop = false;
            this.MessagesContainer.Text = "Messages";
            // 
            // MessagesOutput
            // 
            this.MessagesOutput.BackColor = System.Drawing.SystemColors.WindowText;
            this.MessagesOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagesOutput.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MessagesOutput.ForeColor = System.Drawing.SystemColors.Window;
            this.MessagesOutput.Location = new System.Drawing.Point(3, 16);
            this.MessagesOutput.Multiline = true;
            this.MessagesOutput.Name = "MessagesOutput";
            this.MessagesOutput.ReadOnly = true;
            this.MessagesOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MessagesOutput.Size = new System.Drawing.Size(655, 149);
            this.MessagesOutput.TabIndex = 0;
            // 
            // StartProcessing
            // 
            this.StartProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartProcessing.Location = new System.Drawing.Point(595, 177);
            this.StartProcessing.Name = "StartProcessing";
            this.StartProcessing.Size = new System.Drawing.Size(78, 25);
            this.StartProcessing.TabIndex = 22;
            this.StartProcessing.Text = "Start";
            this.StartProcessing.UseVisualStyleBackColor = true;
            this.StartProcessing.Click += new System.EventHandler(this.StartProcessing_Click);
            // 
            // StopProcessing
            // 
            this.StopProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StopProcessing.Location = new System.Drawing.Point(595, 177);
            this.StopProcessing.Name = "StopProcessing";
            this.StopProcessing.Size = new System.Drawing.Size(75, 23);
            this.StopProcessing.TabIndex = 22;
            this.StopProcessing.Text = "Stop";
            this.StopProcessing.UseVisualStyleBackColor = true;
            this.StopProcessing.Click += new System.EventHandler(this.StopProcessing_Click);
            // 
            // SaveFile
            // 
            this.SaveFile.DefaultExt = "csv";
            this.SaveFile.FileName = "Output.csv";
            this.SaveFile.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            this.SaveFile.OverwritePrompt = false;
            this.SaveFile.Title = "Save / Append to Text File";
            // 
            // AppendToExisting
            // 
            this.AppendToExisting.AutoSize = true;
            this.AppendToExisting.Location = new System.Drawing.Point(6, 38);
            this.AppendToExisting.Name = "AppendToExisting";
            this.AppendToExisting.Size = new System.Drawing.Size(284, 17);
            this.AppendToExisting.TabIndex = 22;
            this.AppendToExisting.Text = "Append export to existing file (uncheck for overwrite)";
            this.AppendToExisting.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 562);
            this.Controls.Add(this.SplitContainerFull);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Historian Playback / Export Utility v{0}";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.SplitContainerFull.Panel1.ResumeLayout(false);
            this.SplitContainerFull.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerFull)).EndInit();
            this.SplitContainerFull.ResumeLayout(false);
            this.SplitContainerTop.Panel1.ResumeLayout(false);
            this.SplitContainerTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerTop)).EndInit();
            this.SplitContainerTop.ResumeLayout(false);
            this.InputSelectionContainer.ResumeLayout(false);
            this.InputSelectionContainer.PerformLayout();
            this.OutputSelectionContainer.ResumeLayout(false);
            this.OutputSelectionContainer.PerformLayout();
            this.OutputChannelTabs.ResumeLayout(false);
            this.TCPSettingsTab.ResumeLayout(false);
            this.TCPSettingsTab.PerformLayout();
            this.UDPSettingsTab.ResumeLayout(false);
            this.UDPSettingsTab.PerformLayout();
            this.FileSettingsTab.ResumeLayout(false);
            this.FileSettingsTab.PerformLayout();
            this.SerialSettingsTab.ResumeLayout(false);
            this.SerialSettingsTab.PerformLayout();
            this.ProcessingSpeedContainer.ResumeLayout(false);
            this.ProcessingSpeedContainer.PerformLayout();
            this.OutputFormatContainer.ResumeLayout(false);
            this.OutputFormatContainer.PerformLayout();
            this.MessagesContainer.ResumeLayout(false);
            this.MessagesContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog FolderBrowser;
        private System.Windows.Forms.SplitContainer SplitContainerFull;
        private System.Windows.Forms.SplitContainer SplitContainerTop;
        private System.Windows.Forms.GroupBox MessagesContainer;
        private System.Windows.Forms.TextBox MessagesOutput;
        private System.Windows.Forms.Button StartProcessing;
        private System.Windows.Forms.Button StopProcessing;
        private System.Windows.Forms.GroupBox InputSelectionContainer;
        private System.Windows.Forms.LinkLabel SearchPhraseClear;
        private System.Windows.Forms.LinkLabel SearchPhraseFind;
        private System.Windows.Forms.LinkLabel ArchiveLocationBrowse;
        private System.Windows.Forms.Label SearchPhraseLabel;
        private System.Windows.Forms.TextBox SearchPhraseInput;
        private System.Windows.Forms.DateTimePicker EndTimeInput;
        private System.Windows.Forms.TextBox ArchiveLocationInput;
        private System.Windows.Forms.Label ArchiveLocationLabel;
        private System.Windows.Forms.DateTimePicker StartTimeInput;
        private System.Windows.Forms.Label EndTimeLabel;
        private System.Windows.Forms.Label StartTimeLabel;
        private System.Windows.Forms.CheckedListBox IDInput;
        private System.Windows.Forms.GroupBox OutputSelectionContainer;
        private System.Windows.Forms.CheckBox RepeatDataProcessing;
        private System.Windows.Forms.CheckBox ProcessDataInParallel;
        private System.Windows.Forms.Panel OutputFormatContainer;
        private System.Windows.Forms.RadioButton OutputPlainTextData;
        private System.Windows.Forms.RadioButton OutputBinaryData;
        private System.Windows.Forms.Label OutputChannelLabel;
        private System.Windows.Forms.TabControl OutputChannelTabs;
        private System.Windows.Forms.TabPage TCPSettingsTab;
        private System.Windows.Forms.TextBox TCPPortInput;
        private System.Windows.Forms.Label TCPPortLabel;
        private System.Windows.Forms.TextBox TCPServerInput;
        private System.Windows.Forms.Label TCPServerLabel;
        private System.Windows.Forms.TabPage UDPSettingsTab;
        private System.Windows.Forms.TextBox UDPPortInput;
        private System.Windows.Forms.Label UDPPortLabel;
        private System.Windows.Forms.TextBox UDPServerInput;
        private System.Windows.Forms.Label UDPServerLabel;
        private System.Windows.Forms.TabPage FileSettingsTab;
        private System.Windows.Forms.Button FileNameBrowse;
        private System.Windows.Forms.TextBox FileNameInput;
        private System.Windows.Forms.Label FileNameLabel;
        private System.Windows.Forms.Panel ProcessingSpeedContainer;
        private System.Windows.Forms.RadioButton ProcessDataFullSpeed;
        private System.Windows.Forms.TextBox ProcessDataAtIntervalSampleRate;
        private System.Windows.Forms.RadioButton ProcessDataAtInterval;
        private System.Windows.Forms.RichTextBox OutputPlainTextDataFormat;
        private System.Windows.Forms.TabPage SerialSettingsTab;
        private System.Windows.Forms.ComboBox SerialPortInput;
        private System.Windows.Forms.Label SerialPortLabel;
        private System.Windows.Forms.CheckBox SerialRtsEnable;
        private System.Windows.Forms.CheckBox SerialDtrEnable;
        private System.Windows.Forms.TextBox SerialDataBitsInput;
        private System.Windows.Forms.Label SerialDataBitsLabel;
        private System.Windows.Forms.ComboBox SerialStopBitsInput;
        private System.Windows.Forms.Label SerialStopBitsLabel;
        private System.Windows.Forms.ComboBox SerialParityInput;
        private System.Windows.Forms.Label SerialParityLabel;
        private System.Windows.Forms.ComboBox SerialBaudRateInput;
        private System.Windows.Forms.Label SerialBaudRateLabel;
        private System.Windows.Forms.SaveFileDialog SaveFile;
        private System.Windows.Forms.CheckBox AppendToExisting;
    }
}

