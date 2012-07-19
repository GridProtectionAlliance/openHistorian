namespace Historian2Demo
{
    partial class FrmServerSettings
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
            this.LstDatabases = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LstAddDatabases = new System.Windows.Forms.Button();
            this.BtnRemoveDatabases = new System.Windows.Forms.Button();
            this.BtnCreateDatabase = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // LstDatabases
            // 
            this.LstDatabases.FormattingEnabled = true;
            this.LstDatabases.Location = new System.Drawing.Point(6, 69);
            this.LstDatabases.Name = "LstDatabases";
            this.LstDatabases.Size = new System.Drawing.Size(237, 446);
            this.LstDatabases.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Databases:";
            // 
            // LstAddDatabases
            // 
            this.LstAddDatabases.Location = new System.Drawing.Point(86, 521);
            this.LstAddDatabases.Name = "LstAddDatabases";
            this.LstAddDatabases.Size = new System.Drawing.Size(75, 23);
            this.LstAddDatabases.TabIndex = 2;
            this.LstAddDatabases.Text = "Add";
            this.LstAddDatabases.UseVisualStyleBackColor = true;
            // 
            // BtnRemoveDatabases
            // 
            this.BtnRemoveDatabases.Location = new System.Drawing.Point(5, 521);
            this.BtnRemoveDatabases.Name = "BtnRemoveDatabases";
            this.BtnRemoveDatabases.Size = new System.Drawing.Size(75, 23);
            this.BtnRemoveDatabases.TabIndex = 3;
            this.BtnRemoveDatabases.Text = "Remove";
            this.BtnRemoveDatabases.UseVisualStyleBackColor = true;
            // 
            // BtnCreateDatabase
            // 
            this.BtnCreateDatabase.Location = new System.Drawing.Point(167, 521);
            this.BtnCreateDatabase.Name = "BtnCreateDatabase";
            this.BtnCreateDatabase.Size = new System.Drawing.Size(75, 23);
            this.BtnCreateDatabase.TabIndex = 2;
            this.BtnCreateDatabase.Text = "Create";
            this.BtnCreateDatabase.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.BtnRemoveDatabases);
            this.groupBox1.Controls.Add(this.BtnCreateDatabase);
            this.groupBox1.Controls.Add(this.LstAddDatabases);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LstDatabases);
            this.groupBox1.Location = new System.Drawing.Point(7, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(794, 625);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Instance";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(249, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(539, 475);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Database Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Database Path";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(109, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(416, 20);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(109, 51);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(416, 20);
            this.textBox2.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(6, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(533, 392);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Internal Settings";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(103, 19);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(416, 20);
            this.textBox3.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Database Name";
            // 
            // FrmServerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 660);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmServerSettings";
            this.Text = "openHistorian: Server Instance";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox LstDatabases;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button LstAddDatabases;
        private System.Windows.Forms.Button BtnRemoveDatabases;
        private System.Windows.Forms.Button BtnCreateDatabase;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
    }
}