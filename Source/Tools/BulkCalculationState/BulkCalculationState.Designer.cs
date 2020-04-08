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
            this.checkedListBoxDevices = new System.Windows.Forms.CheckedListBox();
            this.checkBoxSelectAll = new System.Windows.Forms.CheckBox();
            this.buttonEnableSelected = new System.Windows.Forms.Button();
            this.checkBoxSelect = new System.Windows.Forms.CheckBox();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.labelSelected = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkedListBoxDevices
            // 
            this.checkedListBoxDevices.CheckOnClick = true;
            this.checkedListBoxDevices.FormattingEnabled = true;
            this.checkedListBoxDevices.Location = new System.Drawing.Point(12, 67);
            this.checkedListBoxDevices.Name = "checkedListBoxDevices";
            this.checkedListBoxDevices.Size = new System.Drawing.Size(329, 364);
            this.checkedListBoxDevices.TabIndex = 4;
            this.checkedListBoxDevices.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxDevices_ItemCheck);
            // 
            // checkBoxSelectAll
            // 
            this.checkBoxSelectAll.AutoSize = true;
            this.checkBoxSelectAll.Checked = true;
            this.checkBoxSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSelectAll.Location = new System.Drawing.Point(15, 11);
            this.checkBoxSelectAll.Name = "checkBoxSelectAll";
            this.checkBoxSelectAll.Size = new System.Drawing.Size(123, 17);
            this.checkBoxSelectAll.TabIndex = 0;
            this.checkBoxSelectAll.Text = "Select / Unselect &All";
            this.checkBoxSelectAll.UseVisualStyleBackColor = true;
            this.checkBoxSelectAll.CheckedChanged += new System.EventHandler(this.checkBoxSelectAll_CheckedChanged);
            // 
            // buttonEnableSelected
            // 
            this.buttonEnableSelected.Location = new System.Drawing.Point(378, 12);
            this.buttonEnableSelected.Name = "buttonEnableSelected";
            this.buttonEnableSelected.Size = new System.Drawing.Size(111, 29);
            this.buttonEnableSelected.TabIndex = 5;
            this.buttonEnableSelected.Text = "&Enable Selected";
            this.buttonEnableSelected.UseVisualStyleBackColor = true;
            this.buttonEnableSelected.Click += new System.EventHandler(this.buttonEnableSelected_Click);
            // 
            // checkBoxSelect
            // 
            this.checkBoxSelect.AutoSize = true;
            this.checkBoxSelect.Checked = true;
            this.checkBoxSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSelect.Location = new System.Drawing.Point(15, 35);
            this.checkBoxSelect.Name = "checkBoxSelect";
            this.checkBoxSelect.Size = new System.Drawing.Size(109, 17);
            this.checkBoxSelect.TabIndex = 1;
            this.checkBoxSelect.Text = "&Select / Unselect";
            this.checkBoxSelect.UseVisualStyleBackColor = true;
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(130, 33);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(150, 20);
            this.textBoxSearch.TabIndex = 2;
            // 
            // buttonFind
            // 
            this.buttonFind.Location = new System.Drawing.Point(276, 31);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(65, 23);
            this.buttonFind.TabIndex = 3;
            this.buttonFind.Text = "&Find";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // labelSelected
            // 
            this.labelSelected.AutoSize = true;
            this.labelSelected.Location = new System.Drawing.Point(403, 76);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(86, 13);
            this.labelSelected.TabIndex = 6;
            this.labelSelected.Tag = "Selected: {0:N0}";
            this.labelSelected.Text = "Selected: {0:N0}";
            this.labelSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(421, 53);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(68, 13);
            this.labelTotal.TabIndex = 7;
            this.labelTotal.Tag = "Total: {0:N0}";
            this.labelTotal.Text = "Total: {0:N0}";
            this.labelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BulkCalculationState
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 443);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.checkBoxSelect);
            this.Controls.Add(this.buttonEnableSelected);
            this.Controls.Add(this.checkBoxSelectAll);
            this.Controls.Add(this.checkedListBoxDevices);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "BulkCalculationState";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bulk Calculation State";
            this.Load += new System.EventHandler(this.BulkCalculationState_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxDevices;
        private System.Windows.Forms.CheckBox checkBoxSelectAll;
        private System.Windows.Forms.Button buttonEnableSelected;
        private System.Windows.Forms.CheckBox checkBoxSelect;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Label labelSelected;
        private System.Windows.Forms.Label labelTotal;
    }
}

