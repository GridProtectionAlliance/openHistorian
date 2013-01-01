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
            this.setTimeWindowCalendarControl1 = new openVisN.Components.SetTimeWindowCalendarControl();
            this.visualizationFramework1 = new openVisN.Components.VisualizationFramework(this.components);
            this.signalGroupTextLegend1 = new openVisN.Components.SignalGroupTextLegend();
            this.colorWheel1 = new openVisN.Components.ColorWheel(this.components);
            this.referenceSignalSelectionComboBox1 = new openVisN.Components.ReferenceSignalSelectionComboBox();
            this.signalGroupSelectionCheckedListBox1 = new openVisN.Components.SignalGroupSelectionCheckedListBox();
            this.signalPlots6 = new openVisN.Components.SignalPlots();
            this.signalPlots5 = new openVisN.Components.SignalPlots();
            this.signalPlots4 = new openVisN.Components.SignalPlots();
            this.signalPlots3 = new openVisN.Components.SignalPlots();
            this.signalPlots2 = new openVisN.Components.SignalPlots();
            this.signalPlots1 = new openVisN.Components.SignalPlots();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnEvents = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // setTimeWindowCalendarControl1
            // 
            this.setTimeWindowCalendarControl1.AutoSize = true;
            this.setTimeWindowCalendarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setTimeWindowCalendarControl1.Framework = this.visualizationFramework1;
            this.setTimeWindowCalendarControl1.Location = new System.Drawing.Point(0, 0);
            this.setTimeWindowCalendarControl1.Name = "setTimeWindowCalendarControl1";
            this.setTimeWindowCalendarControl1.Size = new System.Drawing.Size(234, 169);
            this.setTimeWindowCalendarControl1.TabIndex = 5;
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
            this.signalGroupTextLegend1.Location = new System.Drawing.Point(0, 0);
            this.signalGroupTextLegend1.Name = "signalGroupTextLegend1";
            this.signalGroupTextLegend1.Size = new System.Drawing.Size(234, 485);
            this.signalGroupTextLegend1.TabIndex = 4;
            // 
            // referenceSignalSelectionComboBox1
            // 
            this.referenceSignalSelectionComboBox1.AutoSize = true;
            this.referenceSignalSelectionComboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referenceSignalSelectionComboBox1.Framework = this.visualizationFramework1;
            this.referenceSignalSelectionComboBox1.Location = new System.Drawing.Point(3, 16);
            this.referenceSignalSelectionComboBox1.Name = "referenceSignalSelectionComboBox1";
            this.referenceSignalSelectionComboBox1.Size = new System.Drawing.Size(238, 60);
            this.referenceSignalSelectionComboBox1.TabIndex = 3;
            // 
            // signalGroupSelectionCheckedListBox1
            // 
            this.signalGroupSelectionCheckedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalGroupSelectionCheckedListBox1.Framework = this.visualizationFramework1;
            this.signalGroupSelectionCheckedListBox1.Location = new System.Drawing.Point(3, 16);
            this.signalGroupSelectionCheckedListBox1.Name = "signalGroupSelectionCheckedListBox1";
            this.signalGroupSelectionCheckedListBox1.Size = new System.Drawing.Size(238, 556);
            this.signalGroupSelectionCheckedListBox1.TabIndex = 1;
            // 
            // signalPlots6
            // 
            this.signalPlots6.Colors = this.colorWheel1;
            this.signalPlots6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots6.Framework = this.visualizationFramework1;
            this.signalPlots6.Location = new System.Drawing.Point(415, 441);
            this.signalPlots6.Name = "signalPlots6";
            this.signalPlots6.PlotTitle = "DFDT";
            this.signalPlots6.SignalTypeToPlot = "DFDT";
            this.signalPlots6.Size = new System.Drawing.Size(406, 214);
            this.signalPlots6.TabIndex = 0;
            // 
            // signalPlots5
            // 
            this.signalPlots5.Colors = this.colorWheel1;
            this.signalPlots5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots5.Framework = this.visualizationFramework1;
            this.signalPlots5.Location = new System.Drawing.Point(3, 441);
            this.signalPlots5.Name = "signalPlots5";
            this.signalPlots5.PlotTitle = "Frequency";
            this.signalPlots5.SignalTypeToPlot = "Frequency";
            this.signalPlots5.Size = new System.Drawing.Size(406, 214);
            this.signalPlots5.TabIndex = 0;
            // 
            // signalPlots4
            // 
            this.signalPlots4.Colors = this.colorWheel1;
            this.signalPlots4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots4.Framework = this.visualizationFramework1;
            this.signalPlots4.Location = new System.Drawing.Point(415, 222);
            this.signalPlots4.Name = "signalPlots4";
            this.signalPlots4.PlotTitle = "MW";
            this.signalPlots4.ScalingFactor = 1E-06D;
            this.signalPlots4.SignalTypeToPlot = "Watt";
            this.signalPlots4.Size = new System.Drawing.Size(406, 213);
            this.signalPlots4.TabIndex = 0;
            // 
            // signalPlots3
            // 
            this.signalPlots3.Colors = this.colorWheel1;
            this.signalPlots3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots3.Framework = this.visualizationFramework1;
            this.signalPlots3.Location = new System.Drawing.Point(3, 222);
            this.signalPlots3.Name = "signalPlots3";
            this.signalPlots3.PlotTitle = "MVAR";
            this.signalPlots3.ScalingFactor = 1E-06D;
            this.signalPlots3.SignalTypeToPlot = "Volt Ampre Reactive";
            this.signalPlots3.Size = new System.Drawing.Size(406, 213);
            this.signalPlots3.TabIndex = 0;
            // 
            // signalPlots2
            // 
            this.signalPlots2.Colors = this.colorWheel1;
            this.signalPlots2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalPlots2.Framework = this.visualizationFramework1;
            this.signalPlots2.Location = new System.Drawing.Point(415, 3);
            this.signalPlots2.Name = "signalPlots2";
            this.signalPlots2.PlotTitle = "Voltage Angle";
            this.signalPlots2.SignalTypeToPlot = "Voltage Angle Reference";
            this.signalPlots2.Size = new System.Drawing.Size(406, 213);
            this.signalPlots2.TabIndex = 0;
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
            this.signalPlots1.Size = new System.Drawing.Size(406, 213);
            this.signalPlots1.TabIndex = 0;
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(253, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(824, 658);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel2.Controls.Add(this.splitContainer1, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.splitContainer2, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1320, 664);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(1083, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.signalGroupTextLegend1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.setTimeWindowCalendarControl1);
            this.splitContainer1.Size = new System.Drawing.Size(234, 658);
            this.splitContainer1.SplitterDistance = 485;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Size = new System.Drawing.Size(244, 658);
            this.splitContainer2.SplitterDistance = 575;
            this.splitContainer2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.signalGroupSelectionCheckedListBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 575);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Terminals";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.BtnEvents);
            this.groupBox1.Controls.Add(this.referenceSignalSelectionComboBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(244, 79);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Angle Reference";
            // 
            // BtnEvents
            // 
            this.BtnEvents.Location = new System.Drawing.Point(9, 47);
            this.BtnEvents.Name = "BtnEvents";
            this.BtnEvents.Size = new System.Drawing.Size(91, 23);
            this.BtnEvents.TabIndex = 4;
            this.BtnEvents.Text = "Events";
            this.BtnEvents.UseVisualStyleBackColor = true;
            this.BtnEvents.Click += new System.EventHandler(this.BtnEvents_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(163, 47);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 664);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "FrmMain";
            this.Text = "openVisN";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private openVisN.Components.VisualizationFramework visualizationFramework1;
        private openVisN.Components.SignalPlots signalPlots1;
        private openVisN.Components.SignalGroupSelectionCheckedListBox signalGroupSelectionCheckedListBox1;
        private openVisN.Components.SignalPlots signalPlots2;
        private openVisN.Components.SignalPlots signalPlots3;
        private openVisN.Components.SignalPlots signalPlots4;
        private openVisN.Components.SignalPlots signalPlots5;
        private openVisN.Components.SignalPlots signalPlots6;
        private openVisN.Components.ReferenceSignalSelectionComboBox referenceSignalSelectionComboBox1;
        private openVisN.Components.ColorWheel colorWheel1;
        private openVisN.Components.SignalGroupTextLegend signalGroupTextLegend1;
        private openVisN.Components.SetTimeWindowCalendarControl setTimeWindowCalendarControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BtnEvents;
        private System.Windows.Forms.Button button1;
    }
}

