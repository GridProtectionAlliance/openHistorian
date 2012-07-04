namespace Historian2Demo
{
    partial class Form1
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
            this.BtnStartEngine = new System.Windows.Forms.Button();
            this.BtnAttachFile = new System.Windows.Forms.Button();
            this.BtnCreatePoints = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnStartEngine
            // 
            this.BtnStartEngine.Location = new System.Drawing.Point(12, 12);
            this.BtnStartEngine.Name = "BtnStartEngine";
            this.BtnStartEngine.Size = new System.Drawing.Size(92, 23);
            this.BtnStartEngine.TabIndex = 0;
            this.BtnStartEngine.Text = "Start Engine";
            this.BtnStartEngine.UseVisualStyleBackColor = true;
            this.BtnStartEngine.Click += new System.EventHandler(this.BtnStartEngine_Click);
            // 
            // BtnAttachFile
            // 
            this.BtnAttachFile.Location = new System.Drawing.Point(12, 41);
            this.BtnAttachFile.Name = "BtnAttachFile";
            this.BtnAttachFile.Size = new System.Drawing.Size(121, 23);
            this.BtnAttachFile.TabIndex = 1;
            this.BtnAttachFile.Text = "Attach WAV File";
            this.BtnAttachFile.UseVisualStyleBackColor = true;
            // 
            // BtnCreatePoints
            // 
            this.BtnCreatePoints.Location = new System.Drawing.Point(264, 12);
            this.BtnCreatePoints.Name = "BtnCreatePoints";
            this.BtnCreatePoints.Size = new System.Drawing.Size(104, 23);
            this.BtnCreatePoints.TabIndex = 2;
            this.BtnCreatePoints.Text = "Create Points";
            this.BtnCreatePoints.UseVisualStyleBackColor = true;
            this.BtnCreatePoints.Click += new System.EventHandler(this.BtnCreatePoints_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 426);
            this.Controls.Add(this.BtnCreatePoints);
            this.Controls.Add(this.BtnAttachFile);
            this.Controls.Add(this.BtnStartEngine);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnStartEngine;
        private System.Windows.Forms.Button BtnAttachFile;
        private System.Windows.Forms.Button BtnCreatePoints;
    }
}

