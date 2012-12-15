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
            this.chkAllSignals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAllSignals.FormattingEnabled = true;
            this.chkAllSignals.Location = new System.Drawing.Point(0, 0);
            this.chkAllSignals.Name = "chkAllSignals";
            this.chkAllSignals.Size = new System.Drawing.Size(304, 306);
            this.chkAllSignals.TabIndex = 0;
            // 
            // SignalGroupSelectionCheckedListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkAllSignals);
            this.Name = "SignalGroupSelectionCheckedListBox";
            this.Size = new System.Drawing.Size(304, 306);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chkAllSignals;
    }
}
