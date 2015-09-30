namespace LogFileViewer
{
    partial class FrmShowError
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
            this.TxtMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TxtMessage
            // 
            this.TxtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtMessage.Location = new System.Drawing.Point(0, 0);
            this.TxtMessage.Multiline = true;
            this.TxtMessage.Name = "TxtMessage";
            this.TxtMessage.Size = new System.Drawing.Size(549, 408);
            this.TxtMessage.TabIndex = 0;
            // 
            // FrmShowError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 408);
            this.Controls.Add(this.TxtMessage);
            this.Name = "FrmShowError";
            this.Text = "FrmShowError";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox TxtMessage;

    }
}