namespace BulkCalculationState
{
    partial class BulkCalculationState
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulkCalculationState));
            this.checkedListBoxDevices = new System.Windows.Forms.CheckedListBox();
            this.buttonEnableSelected = new System.Windows.Forms.Button();
            this.checkBoxSelect = new System.Windows.Forms.CheckBox();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.labelSelected = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.checkBoxGroupByPrefix = new System.Windows.Forms.CheckBox();
            this.radioButtonSelectAll = new System.Windows.Forms.RadioButton();
            this.radioButtonUnselectAll = new System.Windows.Forms.RadioButton();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkedListBoxDevices
            // 
            this.checkedListBoxDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxDevices.CheckOnClick = true;
            this.checkedListBoxDevices.FormattingEnabled = true;
            this.checkedListBoxDevices.Location = new System.Drawing.Point(12, 112);
            this.checkedListBoxDevices.Name = "checkedListBoxDevices";
            this.checkedListBoxDevices.Size = new System.Drawing.Size(334, 304);
            this.checkedListBoxDevices.TabIndex = 4;
            this.checkedListBoxDevices.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxDevices_ItemCheck);
            // 
            // buttonEnableSelected
            // 
            this.buttonEnableSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnableSelected.Location = new System.Drawing.Point(361, 12);
            this.buttonEnableSelected.Name = "buttonEnableSelected";
            this.buttonEnableSelected.Size = new System.Drawing.Size(111, 29);
            this.buttonEnableSelected.TabIndex = 5;
            this.buttonEnableSelected.Tag = "&Enable Selected";
            this.buttonEnableSelected.Text = "&Enable Selected";
            this.buttonEnableSelected.UseVisualStyleBackColor = true;
            this.buttonEnableSelected.Click += new System.EventHandler(this.buttonEnableSelected_Click);
            // 
            // checkBoxSelect
            // 
            this.checkBoxSelect.AutoSize = true;
            this.checkBoxSelect.Checked = true;
            this.checkBoxSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSelect.Location = new System.Drawing.Point(12, 35);
            this.checkBoxSelect.Name = "checkBoxSelect";
            this.checkBoxSelect.Size = new System.Drawing.Size(109, 17);
            this.checkBoxSelect.TabIndex = 1;
            this.checkBoxSelect.Text = "&Select / Unselect";
            this.checkBoxSelect.UseVisualStyleBackColor = true;
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearch.Location = new System.Drawing.Point(120, 33);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(160, 20);
            this.textBoxSearch.TabIndex = 2;
            // 
            // buttonFind
            // 
            this.buttonFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFind.Location = new System.Drawing.Point(281, 32);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(65, 22);
            this.buttonFind.TabIndex = 3;
            this.buttonFind.Text = "&Find";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // labelSelected
            // 
            this.labelSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelected.AutoSize = true;
            this.labelSelected.Location = new System.Drawing.Point(352, 112);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(103, 13);
            this.labelSelected.TabIndex = 8;
            this.labelSelected.Tag = "Active Calcs: {0:N0}";
            this.labelSelected.Text = "Active Calcs: {0:N0}";
            this.labelSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTotal
            // 
            this.labelTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(358, 90);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(97, 13);
            this.labelTotal.TabIndex = 7;
            this.labelTotal.Tag = "Total Calcs: {0:N0}";
            this.labelTotal.Text = "Total Calcs: {0:N0}";
            this.labelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefresh.Location = new System.Drawing.Point(361, 48);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(111, 29);
            this.buttonRefresh.TabIndex = 6;
            this.buttonRefresh.Text = "&Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // checkBoxGroupByPrefix
            // 
            this.checkBoxGroupByPrefix.AutoSize = true;
            this.checkBoxGroupByPrefix.Checked = true;
            this.checkBoxGroupByPrefix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGroupByPrefix.Location = new System.Drawing.Point(12, 58);
            this.checkBoxGroupByPrefix.Name = "checkBoxGroupByPrefix";
            this.checkBoxGroupByPrefix.Size = new System.Drawing.Size(217, 17);
            this.checkBoxGroupByPrefix.TabIndex = 9;
            this.checkBoxGroupByPrefix.Text = "&Group by prefix (commonly device name)";
            this.checkBoxGroupByPrefix.UseVisualStyleBackColor = true;
            this.checkBoxGroupByPrefix.CheckedChanged += new System.EventHandler(this.checkBoxGroupByPrefix_CheckedChanged);
            // 
            // radioButtonSelectAll
            // 
            this.radioButtonSelectAll.AutoSize = true;
            this.radioButtonSelectAll.Location = new System.Drawing.Point(12, 11);
            this.radioButtonSelectAll.Name = "radioButtonSelectAll";
            this.radioButtonSelectAll.Size = new System.Drawing.Size(69, 17);
            this.radioButtonSelectAll.TabIndex = 10;
            this.radioButtonSelectAll.TabStop = true;
            this.radioButtonSelectAll.Text = "Select &All";
            this.radioButtonSelectAll.UseVisualStyleBackColor = true;
            this.radioButtonSelectAll.CheckedChanged += new System.EventHandler(this.SelectAllCheckedChanged);
            // 
            // radioButtonUnselectAll
            // 
            this.radioButtonUnselectAll.AutoSize = true;
            this.radioButtonUnselectAll.Location = new System.Drawing.Point(87, 11);
            this.radioButtonUnselectAll.Name = "radioButtonUnselectAll";
            this.radioButtonUnselectAll.Size = new System.Drawing.Size(81, 17);
            this.radioButtonUnselectAll.TabIndex = 11;
            this.radioButtonUnselectAll.TabStop = true;
            this.radioButtonUnselectAll.Text = "&Unselect All";
            this.radioButtonUnselectAll.UseVisualStyleBackColor = true;
            this.radioButtonUnselectAll.CheckedChanged += new System.EventHandler(this.SelectAllCheckedChanged);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.Location = new System.Drawing.Point(42, 81);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(304, 20);
            this.textBoxFilter.TabIndex = 12;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Fil&ter:";
            // 
            // BulkCalculationState
            // 
            this.AcceptButton = this.buttonFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 426);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.radioButtonUnselectAll);
            this.Controls.Add(this.radioButtonSelectAll);
            this.Controls.Add(this.checkBoxGroupByPrefix);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.checkBoxSelect);
            this.Controls.Add(this.buttonEnableSelected);
            this.Controls.Add(this.checkedListBoxDevices);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 465);
            this.Name = "BulkCalculationState";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bulk State Updater for Dynamic Calculations";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BulkCalculationState_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BulkCalculationState_FormClosed);
            this.Load += new System.EventHandler(this.BulkCalculationState_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxDevices;
        private System.Windows.Forms.Button buttonEnableSelected;
        private System.Windows.Forms.CheckBox checkBoxSelect;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Label labelSelected;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.RadioButton radioButtonSelectAll;
        private System.Windows.Forms.RadioButton radioButtonUnselectAll;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox checkBoxGroupByPrefix;
        public System.Windows.Forms.TextBox textBoxFilter;
    }
}

