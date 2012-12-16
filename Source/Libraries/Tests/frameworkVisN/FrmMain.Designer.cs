namespace frameworkVisN
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
            this.BtnGo = new System.Windows.Forms.Button();
            this.ChkSelectSignalGroups = new openVisN.Components.SignalGroupSelectionCheckedListBox();
            this.SuspendLayout();
            // 
            // BtnGo
            // 
            this.BtnGo.Location = new System.Drawing.Point(226, 415);
            this.BtnGo.Name = "BtnGo";
            this.BtnGo.Size = new System.Drawing.Size(75, 23);
            this.BtnGo.TabIndex = 1;
            this.BtnGo.Text = "Go";
            this.BtnGo.UseVisualStyleBackColor = true;
            this.BtnGo.Click += new System.EventHandler(this.BtnGo_Click);
            // 
            // ChkSelectSignalGroups
            // 
            this.ChkSelectSignalGroups.Location = new System.Drawing.Point(12, 12);
            this.ChkSelectSignalGroups.Name = "ChkSelectSignalGroups";
            this.ChkSelectSignalGroups.Size = new System.Drawing.Size(289, 397);
            this.ChkSelectSignalGroups.TabIndex = 0;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 488);
            this.Controls.Add(this.BtnGo);
            this.Controls.Add(this.ChkSelectSignalGroups);
            this.Name = "FrmMain";
            this.Text = "openVisN Framework";
            this.ResumeLayout(false);

        }

        #endregion

        private openVisN.Components.SignalGroupSelectionCheckedListBox ChkSelectSignalGroups;
        private System.Windows.Forms.Button BtnGo;
    }
}

