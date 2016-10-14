namespace openVisN.Components
{
    partial class SignalGroupSelectionCheckedListBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkAllSignals = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // chkAllSignals
            // 
            this.chkAllSignals.CheckOnClick = true;
            this.chkAllSignals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAllSignals.FormattingEnabled = true;
            this.chkAllSignals.Location = new System.Drawing.Point(0, 0);
            this.chkAllSignals.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkAllSignals.Name = "chkAllSignals";
            this.chkAllSignals.Size = new System.Drawing.Size(456, 471);
            this.chkAllSignals.TabIndex = 0;
            this.chkAllSignals.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkAllSignals_ItemCheck);
            // 
            // SignalGroupSelectionCheckedListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkAllSignals);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SignalGroupSelectionCheckedListBox";
            this.Size = new System.Drawing.Size(456, 471);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chkAllSignals;
    }
}
