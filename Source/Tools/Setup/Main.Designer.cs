namespace Setup
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageInstallOptions = new System.Windows.Forms.TabPage();
            this.groupBoxInstallationOptions = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonUninstall = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.labelInstallationOptions = new System.Windows.Forms.Label();
            this.labelNotes = new System.Windows.Forms.Label();
            this.tabPageReleaseNotes = new System.Windows.Forms.TabPage();
            this.richTextBoxReleaseNotes = new System.Windows.Forms.RichTextBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.tabControlMain.SuspendLayout();
            this.tabPageInstallOptions.SuspendLayout();
            this.groupBoxInstallationOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPageReleaseNotes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageInstallOptions);
            this.tabControlMain.Controls.Add(this.tabPageReleaseNotes);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControlMain.Location = new System.Drawing.Point(0, 71);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(501, 309);
            this.tabControlMain.TabIndex = 1;
            this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
            // 
            // tabPageInstallOptions
            // 
            this.tabPageInstallOptions.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageInstallOptions.BackgroundImage")));
            this.tabPageInstallOptions.Controls.Add(this.groupBoxInstallationOptions);
            this.tabPageInstallOptions.Controls.Add(this.labelNotes);
            this.tabPageInstallOptions.Location = new System.Drawing.Point(4, 25);
            this.tabPageInstallOptions.Name = "tabPageInstallOptions";
            this.tabPageInstallOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInstallOptions.Size = new System.Drawing.Size(493, 280);
            this.tabPageInstallOptions.TabIndex = 0;
            this.tabPageInstallOptions.Text = "Installation";
            this.tabPageInstallOptions.UseVisualStyleBackColor = true;
            // 
            // groupBoxInstallationOptions
            // 
            this.groupBoxInstallationOptions.Controls.Add(this.pictureBox1);
            this.groupBoxInstallationOptions.Controls.Add(this.buttonUninstall);
            this.groupBoxInstallationOptions.Controls.Add(this.buttonCancel);
            this.groupBoxInstallationOptions.Controls.Add(this.buttonInstall);
            this.groupBoxInstallationOptions.Controls.Add(this.labelInstallationOptions);
            this.groupBoxInstallationOptions.Location = new System.Drawing.Point(4, 7);
            this.groupBoxInstallationOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxInstallationOptions.Name = "groupBoxInstallationOptions";
            this.groupBoxInstallationOptions.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxInstallationOptions.Size = new System.Drawing.Size(483, 114);
            this.groupBoxInstallationOptions.TabIndex = 2;
            this.groupBoxInstallationOptions.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Setup.Properties.Resources.GearGraphic;
            this.pictureBox1.Location = new System.Drawing.Point(11, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // buttonUninstall
            // 
            this.buttonUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUninstall.Location = new System.Drawing.Point(368, 45);
            this.buttonUninstall.Name = "buttonUninstall";
            this.buttonUninstall.Size = new System.Drawing.Size(100, 31);
            this.buttonUninstall.TabIndex = 4;
            this.buttonUninstall.Text = "&Uninstall";
            this.buttonUninstall.UseVisualStyleBackColor = true;
            this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(368, 77);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 31);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonInstall
            // 
            this.buttonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInstall.Location = new System.Drawing.Point(368, 13);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(100, 31);
            this.buttonInstall.TabIndex = 3;
            this.buttonInstall.Text = "&Install";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // labelInstallationOptions
            // 
            this.labelInstallationOptions.Location = new System.Drawing.Point(74, 26);
            this.labelInstallationOptions.Name = "labelInstallationOptions";
            this.labelInstallationOptions.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelInstallationOptions.Size = new System.Drawing.Size(297, 66);
            this.labelInstallationOptions.TabIndex = 7;
            this.labelInstallationOptions.Text = "This setup utility will install the openHistorian 2.0 and/or related tools. Note " +
    "that this installation requires .NET 4.5.  The openHistorian is only available a" +
    "s a 64-bit installation.";
            this.labelInstallationOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelNotes
            // 
            this.labelNotes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelNotes.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotes.Location = new System.Drawing.Point(3, 125);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Padding = new System.Windows.Forms.Padding(10, 0, 5, 0);
            this.labelNotes.Size = new System.Drawing.Size(487, 152);
            this.labelNotes.TabIndex = 3;
            this.labelNotes.Text = resources.GetString("labelNotes.Text");
            this.labelNotes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageReleaseNotes
            // 
            this.tabPageReleaseNotes.Controls.Add(this.richTextBoxReleaseNotes);
            this.tabPageReleaseNotes.Location = new System.Drawing.Point(4, 25);
            this.tabPageReleaseNotes.Name = "tabPageReleaseNotes";
            this.tabPageReleaseNotes.Size = new System.Drawing.Size(493, 280);
            this.tabPageReleaseNotes.TabIndex = 2;
            this.tabPageReleaseNotes.Text = "Release Notes";
            this.tabPageReleaseNotes.ToolTipText = "Click here to see notes about this version of the product release.";
            this.tabPageReleaseNotes.UseVisualStyleBackColor = true;
            // 
            // richTextBoxReleaseNotes
            // 
            this.richTextBoxReleaseNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxReleaseNotes.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxReleaseNotes.Name = "richTextBoxReleaseNotes";
            this.richTextBoxReleaseNotes.Size = new System.Drawing.Size(493, 280);
            this.richTextBoxReleaseNotes.TabIndex = 0;
            this.richTextBoxReleaseNotes.Text = "";
            this.richTextBoxReleaseNotes.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxReleaseNotes_LinkClicked);
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(375, 75);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(125, 13);
            this.labelVersion.TabIndex = 2;
            this.labelVersion.Text = "Version: {0}.{1}.{2}.{3}";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxLogo.Image = global::Setup.Properties.Resources.WelcomeScreen;
            this.pictureBoxLogo.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(501, 72);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxLogo.TabIndex = 1;
            this.pictureBoxLogo.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(501, 380);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.pictureBoxLogo);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "openHistorian Setup";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageInstallOptions.ResumeLayout(false);
            this.groupBoxInstallationOptions.ResumeLayout(false);
            this.groupBoxInstallationOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPageReleaseNotes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageInstallOptions;
        private System.Windows.Forms.GroupBox groupBoxInstallationOptions;
        private System.Windows.Forms.Button buttonUninstall;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TabPage tabPageReleaseNotes;
        private System.Windows.Forms.RichTextBox richTextBoxReleaseNotes;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelInstallationOptions;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

