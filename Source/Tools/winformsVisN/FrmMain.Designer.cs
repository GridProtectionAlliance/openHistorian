namespace winformsVisN
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
            this.colorWheel1 = new openVisN.Components.ColorWheel(this.components);
            this.visualizationFramework1 = new openVisN.Components.VisualizationFramework(this.components);
            this.signalGroupTextLegend1 = new openVisN.Components.SignalGroupTextLegend();
            this.setTimeWindowCalendarControl1 = new openVisN.Components.SetTimeWindowCalendarControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.manualAutomaticModeSelectorButton1 = new openVisN.Components.ManualAutomaticModeSelectorButton();
            this.button1 = new System.Windows.Forms.Button();
            this.BtnEvents = new System.Windows.Forms.Button();
            this.referenceSignalSelectionComboBox1 = new openVisN.Components.ReferenceSignalSelectionComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.signalGroupSelectionCheckedListBox1 = new openVisN.Components.SignalGroupSelectionCheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.signalPlots1 = new openVisN.Components.SignalPlots();
            this.signalPlots2 = new openVisN.Components.SignalPlots();
            this.signalPlots3 = new openVisN.Components.SignalPlots();
            this.signalPlots4 = new openVisN.Components.SignalPlots();
            this.signalPlots5 = new openVisN.Components.SignalPlots();
            this.signalPlots6 = new openVisN.Components.SignalPlots();
            this.BtnExport = new System.Windows.Forms.Button();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // visualizationFramework1
            // 
            this.visualizationFramework1.Paths = new string[] {
        "H:\\OGE 2009.d2"};
            this.visualizationFramework1.UseNetworkHistorian = false;
            // 
            // signalGroupTextLegend1
            // 
            this.signalGroupTextLegend1.Colors = this.colorWheel1;
            this.signalGroupTextLegend1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalGroupTextLegend1.Framework = this.visualizationFramework1;
            this.signalGroupTextLegend1.Location = new System.Drawing.Point(3, 3);
            this.signalGroupTextLegend1.Name = "signalGroupTextLegend1";
            this.signalGroupTextLegend1.Size = new System.Drawing.Size(266, 291);
            this.signalGroupTextLegend1.TabIndex = 4;
            // 
            // setTimeWindowCalendarControl1
            // 
            this.setTimeWindowCalendarControl1.AutoSize = true;
            this.setTimeWindowCalendarControl1.Framework = this.visualizationFramework1;
            this.setTimeWindowCalendarControl1.Location = new System.Drawing.Point(25, 3);
            this.setTimeWindowCalendarControl1.Name = "setTimeWindowCalendarControl1";
            this.setTimeWindowCalendarControl1.Size = new System.Drawing.Size(232, 170);
            this.setTimeWindowCalendarControl1.TabIndex = 5;
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
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 172F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(286, 639);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnExport);
            this.groupBox1.Controls.Add(this.manualAutomaticModeSelectorButton1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.BtnEvents);
            this.groupBox1.Controls.Add(this.referenceSignalSelectionComboBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 397);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 67);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Angle Reference";
            // 
            // manualAutomaticModeSelectorButton1
            // 
            this.manualAutomaticModeSelectorButton1.Framework = this.visualizationFramework1;
            this.manualAutomaticModeSelectorButton1.Location = new System.Drawing.Point(209, 41);
            this.manualAutomaticModeSelectorButton1.Name = "manualAutomaticModeSelectorButton1";
            this.manualAutomaticModeSelectorButton1.Size = new System.Drawing.Size(68, 23);
            this.manualAutomaticModeSelectorButton1.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(67, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BtnEvents
            // 
            this.BtnEvents.Location = new System.Drawing.Point(3, 42);
            this.BtnEvents.Name = "BtnEvents";
            this.BtnEvents.Size = new System.Drawing.Size(58, 23);
            this.BtnEvents.TabIndex = 4;
            this.BtnEvents.Text = "Events";
            this.BtnEvents.UseVisualStyleBackColor = true;
            this.BtnEvents.Click += new System.EventHandler(this.BtnEvents_Click);
            // 
            // referenceSignalSelectionComboBox1
            // 
            this.referenceSignalSelectionComboBox1.AutoSize = true;
            this.referenceSignalSelectionComboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referenceSignalSelectionComboBox1.Framework = this.visualizationFramework1;
            this.referenceSignalSelectionComboBox1.Location = new System.Drawing.Point(3, 16);
            this.referenceSignalSelectionComboBox1.Name = "referenceSignalSelectionComboBox1";
            this.referenceSignalSelectionComboBox1.Size = new System.Drawing.Size(274, 48);
            this.referenceSignalSelectionComboBox1.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(280, 59);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 68);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(280, 323);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(272, 297);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Select";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.signalGroupSelectionCheckedListBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(266, 291);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Terminals";
            // 
            // signalGroupSelectionCheckedListBox1
            // 
            this.signalGroupSelectionCheckedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalGroupSelectionCheckedListBox1.Framework = this.visualizationFramework1;
            this.signalGroupSelectionCheckedListBox1.Location = new System.Drawing.Point(3, 16);
            this.signalGroupSelectionCheckedListBox1.Name = "signalGroupSelectionCheckedListBox1";
            this.signalGroupSelectionCheckedListBox1.Size = new System.Drawing.Size(260, 272);
            this.signalGroupSelectionCheckedListBox1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.signalGroupTextLegend1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(272, 297);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Legend";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.setTimeWindowCalendarControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 470);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 166);
            this.panel1.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1125, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1125, 639);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1125, 686);
            this.toolStripContainer1.TabIndex = 9;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1125, 639);
            this.splitContainer1.SplitterDistance = 286;
            this.splitContainer1.TabIndex = 8;
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
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(835, 639);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // signalPlots1
            // 
            this.signalPlots1.Colors = this.colorWheel1;
            this.signalPlots1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots1.Framework = this.visualizationFramework1;
            this.signalPlots1.Location = new System.Drawing.Point(3, 3);
            this.signalPlots1.Name = "signalPlots1";
            this.signalPlots1.PlotTitle = "Voltage Magnitude";
            this.signalPlots1.SignalTypeToPlot = "Voltage Magnitude Per Unit";
            this.signalPlots1.Size = new System.Drawing.Size(411, 207);
            this.signalPlots1.TabIndex = 0;
            // 
            // signalPlots2
            // 
            this.signalPlots2.Colors = this.colorWheel1;
            this.signalPlots2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots2.Framework = this.visualizationFramework1;
            this.signalPlots2.Location = new System.Drawing.Point(420, 3);
            this.signalPlots2.Name = "signalPlots2";
            this.signalPlots2.PlotTitle = "Voltage Angle";
            this.signalPlots2.SignalTypeToPlot = "Voltage Angle Reference";
            this.signalPlots2.Size = new System.Drawing.Size(412, 207);
            this.signalPlots2.TabIndex = 0;
            // 
            // signalPlots3
            // 
            this.signalPlots3.Colors = this.colorWheel1;
            this.signalPlots3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots3.Framework = this.visualizationFramework1;
            this.signalPlots3.Location = new System.Drawing.Point(3, 216);
            this.signalPlots3.Name = "signalPlots3";
            this.signalPlots3.PlotTitle = "MVAR";
            this.signalPlots3.ScalingFactor = 1E-06D;
            this.signalPlots3.SignalTypeToPlot = "Volt Ampre Reactive";
            this.signalPlots3.Size = new System.Drawing.Size(411, 207);
            this.signalPlots3.TabIndex = 0;
            // 
            // signalPlots4
            // 
            this.signalPlots4.Colors = this.colorWheel1;
            this.signalPlots4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots4.Framework = this.visualizationFramework1;
            this.signalPlots4.Location = new System.Drawing.Point(420, 216);
            this.signalPlots4.Name = "signalPlots4";
            this.signalPlots4.PlotTitle = "MW";
            this.signalPlots4.ScalingFactor = 1E-06D;
            this.signalPlots4.SignalTypeToPlot = "Watt";
            this.signalPlots4.Size = new System.Drawing.Size(412, 207);
            this.signalPlots4.TabIndex = 0;
            // 
            // signalPlots5
            // 
            this.signalPlots5.Colors = this.colorWheel1;
            this.signalPlots5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots5.Framework = this.visualizationFramework1;
            this.signalPlots5.Location = new System.Drawing.Point(3, 429);
            this.signalPlots5.Name = "signalPlots5";
            this.signalPlots5.PlotTitle = "Frequency";
            this.signalPlots5.SignalTypeToPlot = "Frequency";
            this.signalPlots5.Size = new System.Drawing.Size(411, 207);
            this.signalPlots5.TabIndex = 0;
            // 
            // signalPlots6
            // 
            this.signalPlots6.Colors = this.colorWheel1;
            this.signalPlots6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots6.Framework = this.visualizationFramework1;
            this.signalPlots6.Location = new System.Drawing.Point(420, 429);
            this.signalPlots6.Name = "signalPlots6";
            this.signalPlots6.PlotTitle = "DFDT";
            this.signalPlots6.SignalTypeToPlot = "DFDT";
            this.signalPlots6.Size = new System.Drawing.Size(412, 207);
            this.signalPlots6.TabIndex = 0;
            // 
            // BtnExport
            // 
            this.BtnExport.Location = new System.Drawing.Point(128, 41);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(75, 23);
            this.BtnExport.TabIndex = 7;
            this.BtnExport.Text = "Export";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1125, 686);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "openVisN";
            this.Load += new System.EventHandler(this.FrmMain_Load);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private openVisN.Components.VisualizationFramework visualizationFramework1;
        private openVisN.Components.SignalGroupSelectionCheckedListBox signalGroupSelectionCheckedListBox1;
        private openVisN.Components.ReferenceSignalSelectionComboBox referenceSignalSelectionComboBox1;
        private openVisN.Components.ColorWheel colorWheel1;
        private openVisN.Components.SignalGroupTextLegend signalGroupTextLegend1;
        private openVisN.Components.SetTimeWindowCalendarControl setTimeWindowCalendarControl1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BtnEvents;
        private System.Windows.Forms.Button button1;
        private openVisN.Components.ManualAutomaticModeSelectorButton manualAutomaticModeSelectorButton1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private openVisN.Components.SignalPlots signalPlots1;
        private openVisN.Components.SignalPlots signalPlots2;
        private openVisN.Components.SignalPlots signalPlots3;
        private openVisN.Components.SignalPlots signalPlots4;
        private openVisN.Components.SignalPlots signalPlots5;
        private openVisN.Components.SignalPlots signalPlots6;
        private System.Windows.Forms.Button BtnExport;
    }
}

