namespace openHistorianServiceHost
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
            this.BtnStartStream = new System.Windows.Forms.Button();
            this.btnOpenClient = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnStartStream
            // 
            this.BtnStartStream.Location = new System.Drawing.Point(197, 12);
            this.BtnStartStream.Name = "BtnStartStream";
            this.BtnStartStream.Size = new System.Drawing.Size(75, 23);
            this.BtnStartStream.TabIndex = 0;
            this.BtnStartStream.Text = "Stream Data";
            this.BtnStartStream.UseVisualStyleBackColor = true;
            this.BtnStartStream.Click += new System.EventHandler(this.BtnStartStream_Click);
            // 
            // btnOpenClient
            // 
            this.btnOpenClient.Location = new System.Drawing.Point(197, 81);
            this.btnOpenClient.Name = "btnOpenClient";
            this.btnOpenClient.Size = new System.Drawing.Size(75, 23);
            this.btnOpenClient.TabIndex = 1;
            this.btnOpenClient.Text = "Open Client";
            this.btnOpenClient.UseVisualStyleBackColor = true;
            this.btnOpenClient.Click += new System.EventHandler(this.btnOpenClient_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnOpenClient);
            this.Controls.Add(this.BtnStartStream);
            this.Name = "FrmMain";
            this.Text = "FrmMain";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnStartStream;
        private System.Windows.Forms.Button btnOpenClient;
    }
}