namespace openVisN
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.signalPlots1 = new openVisN.Components.SignalPlots();
            this.colorWheel1 = new openVisN.Components.ColorWheel(this.components);
            this.visualizationFramework1 = new openVisN.Components.VisualizationFramework(this.components);
            this.signalPlots2 = new openVisN.Components.SignalPlots();
            this.signalPlots3 = new openVisN.Components.SignalPlots();
            this.signalPlots4 = new openVisN.Components.SignalPlots();
            this.signalPlots5 = new openVisN.Components.SignalPlots();
            this.signalPlots6 = new openVisN.Components.SignalPlots();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.manualAutomaticModeSelectorButton1 = new openVisN.Components.ManualAutomaticModeSelectorButton();
            this.BtnConfig = new System.Windows.Forms.Button();
            this.referenceSignalSelectionComboBox1 = new openVisN.Components.ReferenceSignalSelectionComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.signalGroupSelectionCheckedListBox1 = new openVisN.Components.SignalGroupSelectionCheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.signalGroupTextLegend1 = new openVisN.Components.SignalGroupTextLegend();
            this.panel1 = new System.Windows.Forms.Panel();
            this.setTimeWindowCalendarControl1 = new openVisN.Components.SetTimeWindowCalendarControl();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.signalPlots1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.signalPlots2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.signalPlots3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.signalPlots4, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.signalPlots5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.signalPlots6, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1121, 1071);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // signalPlots1
            // 
            this.signalPlots1.Colors = this.colorWheel1;
            this.signalPlots1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots1.Framework = this.visualizationFramework1;
            this.signalPlots1.Location = new System.Drawing.Point(6, 8);
            this.signalPlots1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalPlots1.Name = "signalPlots1";
            this.signalPlots1.PlotTitle = "Voltage Magnitude";
            this.signalPlots1.SignalTypeToPlot = "Voltage Magnitude Per Unit";
            this.signalPlots1.Size = new System.Drawing.Size(548, 341);
            this.signalPlots1.TabIndex = 0;
            // 
            // visualizationFramework1
            // 
            this.visualizationFramework1.Enabled = true;
            this.visualizationFramework1.Paths = new string[] {
        ""};
            this.visualizationFramework1.UseNetworkHistorian = false;
            // 
            // signalPlots2
            // 
            this.signalPlots2.Colors = this.colorWheel1;
            this.signalPlots2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots2.Framework = this.visualizationFramework1;
            this.signalPlots2.Location = new System.Drawing.Point(566, 8);
            this.signalPlots2.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalPlots2.Name = "signalPlots2";
            this.signalPlots2.PlotTitle = "Voltage Angle";
            this.signalPlots2.SignalTypeToPlot = "Voltage Angle Reference";
            this.signalPlots2.Size = new System.Drawing.Size(549, 341);
            this.signalPlots2.TabIndex = 0;
            // 
            // signalPlots3
            // 
            this.signalPlots3.Colors = this.colorWheel1;
            this.signalPlots3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots3.Framework = this.visualizationFramework1;
            this.signalPlots3.Location = new System.Drawing.Point(6, 365);
            this.signalPlots3.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalPlots3.Name = "signalPlots3";
            this.signalPlots3.PlotTitle = "MVAR";
            this.signalPlots3.ScalingFactor = 1E-06D;
            this.signalPlots3.SignalTypeToPlot = "Volt Ampre Reactive";
            this.signalPlots3.Size = new System.Drawing.Size(548, 341);
            this.signalPlots3.TabIndex = 0;
            // 
            // signalPlots4
            // 
            this.signalPlots4.Colors = this.colorWheel1;
            this.signalPlots4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots4.Framework = this.visualizationFramework1;
            this.signalPlots4.Location = new System.Drawing.Point(566, 365);
            this.signalPlots4.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalPlots4.Name = "signalPlots4";
            this.signalPlots4.PlotTitle = "MW";
            this.signalPlots4.ScalingFactor = 1E-06D;
            this.signalPlots4.SignalTypeToPlot = "Watt";
            this.signalPlots4.Size = new System.Drawing.Size(549, 341);
            this.signalPlots4.TabIndex = 0;
            // 
            // signalPlots5
            // 
            this.signalPlots5.Colors = this.colorWheel1;
            this.signalPlots5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots5.Framework = this.visualizationFramework1;
            this.signalPlots5.Location = new System.Drawing.Point(6, 722);
            this.signalPlots5.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalPlots5.Name = "signalPlots5";
            this.signalPlots5.PlotTitle = "Frequency";
            this.signalPlots5.SignalTypeToPlot = "Frequency";
            this.signalPlots5.Size = new System.Drawing.Size(548, 341);
            this.signalPlots5.TabIndex = 0;
            // 
            // signalPlots6
            // 
            this.signalPlots6.Colors = this.colorWheel1;
            this.signalPlots6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots6.Framework = this.visualizationFramework1;
            this.signalPlots6.Location = new System.Drawing.Point(566, 722);
            this.signalPlots6.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalPlots6.Name = "signalPlots6";
            this.signalPlots6.PlotTitle = "DFDT";
            this.signalPlots6.SignalTypeToPlot = "DFDT";
            this.signalPlots6.Size = new System.Drawing.Size(549, 341);
            this.signalPlots6.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1413, 1071);
            this.splitContainer1.SplitterDistance = 286;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 8;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 265F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(286, 1071);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnConnect);
            this.groupBox1.Controls.Add(this.manualAutomaticModeSelectorButton1);
            this.groupBox1.Controls.Add(this.BtnConfig);
            this.groupBox1.Controls.Add(this.referenceSignalSelectionComboBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(4, 696);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(278, 105);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Angle Reference";
            // 
            // BtnConnect
            // 
            this.BtnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnConnect.Location = new System.Drawing.Point(50, 64);
            this.BtnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(112, 35);
            this.BtnConnect.TabIndex = 7;
            this.BtnConnect.Text = "Connect";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // manualAutomaticModeSelectorButton1
            // 
            this.manualAutomaticModeSelectorButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.manualAutomaticModeSelectorButton1.Enabled = false;
            this.manualAutomaticModeSelectorButton1.Framework = this.visualizationFramework1;
            this.manualAutomaticModeSelectorButton1.Location = new System.Drawing.Point(172, 64);
            this.manualAutomaticModeSelectorButton1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.manualAutomaticModeSelectorButton1.Name = "manualAutomaticModeSelectorButton1";
            this.manualAutomaticModeSelectorButton1.Size = new System.Drawing.Size(102, 35);
            this.manualAutomaticModeSelectorButton1.TabIndex = 6;
            // 
            // BtnConfig
            // 
            this.BtnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnConfig.Location = new System.Drawing.Point(6, 64);
            this.BtnConfig.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.Size = new System.Drawing.Size(94, 35);
            this.BtnConfig.TabIndex = 5;
            this.BtnConfig.Text = "Config";
            this.BtnConfig.UseVisualStyleBackColor = true;
            this.BtnConfig.Click += new System.EventHandler(this.BtnConfig_Click);
            // 
            // referenceSignalSelectionComboBox1
            // 
            this.referenceSignalSelectionComboBox1.AutoSize = true;
            this.referenceSignalSelectionComboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referenceSignalSelectionComboBox1.Framework = this.visualizationFramework1;
            this.referenceSignalSelectionComboBox1.Location = new System.Drawing.Point(4, 24);
            this.referenceSignalSelectionComboBox1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.referenceSignalSelectionComboBox1.Name = "referenceSignalSelectionComboBox1";
            this.referenceSignalSelectionComboBox1.Size = new System.Drawing.Size(270, 76);
            this.referenceSignalSelectionComboBox1.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 5);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(278, 90);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(4, 105);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(278, 581);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(270, 548);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Select";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.signalGroupSelectionCheckedListBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(4, 5);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(262, 538);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Terminals";
            // 
            // signalGroupSelectionCheckedListBox1
            // 
            this.signalGroupSelectionCheckedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalGroupSelectionCheckedListBox1.Enabled = false;
            this.signalGroupSelectionCheckedListBox1.Framework = this.visualizationFramework1;
            this.signalGroupSelectionCheckedListBox1.Location = new System.Drawing.Point(4, 24);
            this.signalGroupSelectionCheckedListBox1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalGroupSelectionCheckedListBox1.Name = "signalGroupSelectionCheckedListBox1";
            this.signalGroupSelectionCheckedListBox1.Size = new System.Drawing.Size(254, 509);
            this.signalGroupSelectionCheckedListBox1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.signalGroupTextLegend1);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Size = new System.Drawing.Size(270, 548);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Legend";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // signalGroupTextLegend1
            // 
            this.signalGroupTextLegend1.Colors = this.colorWheel1;
            this.signalGroupTextLegend1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalGroupTextLegend1.Framework = this.visualizationFramework1;
            this.signalGroupTextLegend1.Location = new System.Drawing.Point(4, 5);
            this.signalGroupTextLegend1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.signalGroupTextLegend1.Name = "signalGroupTextLegend1";
            this.signalGroupTextLegend1.Size = new System.Drawing.Size(262, 538);
            this.signalGroupTextLegend1.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.setTimeWindowCalendarControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 811);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(278, 255);
            this.panel1.TabIndex = 9;
            // 
            // setTimeWindowCalendarControl1
            // 
            this.setTimeWindowCalendarControl1.AutoSize = true;
            this.setTimeWindowCalendarControl1.Framework = this.visualizationFramework1;
            this.setTimeWindowCalendarControl1.Location = new System.Drawing.Point(38, 5);
            this.setTimeWindowCalendarControl1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.setTimeWindowCalendarControl1.Name = "setTimeWindowCalendarControl1";
            this.setTimeWindowCalendarControl1.Size = new System.Drawing.Size(348, 262);
            this.setTimeWindowCalendarControl1.TabIndex = 5;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1413, 1071);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1413, 1118);
            this.toolStripContainer1.TabIndex = 10;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1413, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1413, 1118);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmMain";
            this.Text = "openVisN 1.0";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Components.SignalPlots signalPlots1;
        private Components.ColorWheel colorWheel1;
        private Components.VisualizationFramework visualizationFramework1;
        private Components.SignalPlots signalPlots2;
        private Components.SignalPlots signalPlots3;
        private Components.SignalPlots signalPlots4;
        private Components.SignalPlots signalPlots6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Components.SignalPlots signalPlots5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private Components.ManualAutomaticModeSelectorButton manualAutomaticModeSelectorButton1;
        private System.Windows.Forms.Button BtnConfig;
        private Components.ReferenceSignalSelectionComboBox referenceSignalSelectionComboBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Components.SignalGroupSelectionCheckedListBox signalGroupSelectionCheckedListBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private Components.SignalGroupTextLegend signalGroupTextLegend1;
        private System.Windows.Forms.Panel panel1;
        private Components.SetTimeWindowCalendarControl setTimeWindowCalendarControl1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button BtnConnect;

    }
}

