namespace UpdateCOMTRADECounters
{
    partial class Install
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Install));
            this.groupBoxInstallTarget = new System.Windows.Forms.GroupBox();
            this.radioButtonInstallOnlyMe = new System.Windows.Forms.RadioButton();
            this.radioButtonInstallAllUsers = new System.Windows.Forms.RadioButton();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelNotes = new System.Windows.Forms.Label();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.buttonInstallNoAdmin = new System.Windows.Forms.Button();
            this.checkBoxNoAdminRights = new System.Windows.Forms.CheckBox();
            this.groupBoxInstallTarget.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxInstallTarget
            // 
            this.groupBoxInstallTarget.Controls.Add(this.radioButtonInstallOnlyMe);
            this.groupBoxInstallTarget.Controls.Add(this.radioButtonInstallAllUsers);
            this.groupBoxInstallTarget.Location = new System.Drawing.Point(210, 157);
            this.groupBoxInstallTarget.Name = "groupBoxInstallTarget";
            this.groupBoxInstallTarget.Size = new System.Drawing.Size(261, 91);
            this.groupBoxInstallTarget.TabIndex = 0;
            this.groupBoxInstallTarget.TabStop = false;
            this.groupBoxInstallTarget.Text = "Installation Target";
            // 
            // radioButtonInstallOnlyMe
            // 
            this.radioButtonInstallOnlyMe.AutoSize = true;
            this.radioButtonInstallOnlyMe.Location = new System.Drawing.Point(21, 52);
            this.radioButtonInstallOnlyMe.Name = "radioButtonInstallOnlyMe";
            this.radioButtonInstallOnlyMe.Size = new System.Drawing.Size(78, 17);
            this.radioButtonInstallOnlyMe.TabIndex = 1;
            this.radioButtonInstallOnlyMe.Text = "Only for me";
            this.radioButtonInstallOnlyMe.UseVisualStyleBackColor = true;
            // 
            // radioButtonInstallAllUsers
            // 
            this.radioButtonInstallAllUsers.AutoSize = true;
            this.radioButtonInstallAllUsers.Checked = true;
            this.radioButtonInstallAllUsers.Location = new System.Drawing.Point(21, 29);
            this.radioButtonInstallAllUsers.Name = "radioButtonInstallAllUsers";
            this.radioButtonInstallAllUsers.Size = new System.Drawing.Size(222, 17);
            this.radioButtonInstallAllUsers.TabIndex = 0;
            this.radioButtonInstallAllUsers.TabStop = true;
            this.radioButtonInstallAllUsers.Text = "Anyone who uses this computer (all users)";
            this.radioButtonInstallAllUsers.UseVisualStyleBackColor = true;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.BackgroundImage")));
            this.pictureBoxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxLogo.Location = new System.Drawing.Point(91, 252);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(64, 53);
            this.pictureBoxLogo.TabIndex = 10;
            this.pictureBoxLogo.TabStop = false;
            // 
            // pictureBoxBackground
            // 
            this.pictureBoxBackground.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxBackground.BackgroundImage")));
            this.pictureBoxBackground.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxBackground.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxBackground.Name = "pictureBoxBackground";
            this.pictureBoxBackground.Size = new System.Drawing.Size(164, 365);
            this.pictureBoxBackground.TabIndex = 11;
            this.pictureBoxBackground.TabStop = false;
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Verdana", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(164, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(361, 86);
            this.labelTitle.TabIndex = 12;
            this.labelTitle.Text = "COMTRADE CFF Post Download\r\nCount Updater Tool Setup";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelNotes
            // 
            this.labelNotes.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotes.Location = new System.Drawing.Point(164, 86);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(361, 57);
            this.labelNotes.TabIndex = 13;
            this.labelNotes.Text = "Installer registers count update tool with URI protocol:\r\ncomtrade-update-counter" +
    "://open";
            this.labelNotes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonInstall
            // 
            this.buttonInstall.FlatAppearance.BorderSize = 2;
            this.buttonInstall.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInstall.Image = ((System.Drawing.Image)(resources.GetObject("buttonInstall.Image")));
            this.buttonInstall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonInstall.Location = new System.Drawing.Point(277, 266);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(124, 39);
            this.buttonInstall.TabIndex = 15;
            this.buttonInstall.Text = "    Install";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // buttonInstallNoAdmin
            // 
            this.buttonInstallNoAdmin.FlatAppearance.BorderSize = 2;
            this.buttonInstallNoAdmin.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInstallNoAdmin.Location = new System.Drawing.Point(277, 266);
            this.buttonInstallNoAdmin.Name = "buttonInstallNoAdmin";
            this.buttonInstallNoAdmin.Size = new System.Drawing.Size(124, 39);
            this.buttonInstallNoAdmin.TabIndex = 16;
            this.buttonInstallNoAdmin.Text = "Install";
            this.buttonInstallNoAdmin.UseVisualStyleBackColor = true;
            this.buttonInstallNoAdmin.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // checkBoxNoAdminRights
            // 
            this.checkBoxNoAdminRights.AutoSize = true;
            this.checkBoxNoAdminRights.Location = new System.Drawing.Point(267, 323);
            this.checkBoxNoAdminRights.Name = "checkBoxNoAdminRights";
            this.checkBoxNoAdminRights.Size = new System.Drawing.Size(148, 17);
            this.checkBoxNoAdminRights.TabIndex = 17;
            this.checkBoxNoAdminRights.Text = "I do not have admin rights";
            this.checkBoxNoAdminRights.UseVisualStyleBackColor = true;
            this.checkBoxNoAdminRights.CheckedChanged += new System.EventHandler(this.checkBoxNoAdminRights_CheckedChanged);
            // 
            // Install
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 365);
            this.Controls.Add(this.checkBoxNoAdminRights);
            this.Controls.Add(this.buttonInstall);
            this.Controls.Add(this.labelNotes);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.pictureBoxBackground);
            this.Controls.Add(this.groupBoxInstallTarget);
            this.Controls.Add(this.buttonInstallNoAdmin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Install";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setup - {0}";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Install_FormClosed);
            this.Load += new System.EventHandler(this.Install_Load);
            this.groupBoxInstallTarget.ResumeLayout(false);
            this.groupBoxInstallTarget.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxInstallTarget;
        private System.Windows.Forms.RadioButton radioButtonInstallOnlyMe;
        private System.Windows.Forms.RadioButton radioButtonInstallAllUsers;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.PictureBox pictureBoxBackground;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Button buttonInstallNoAdmin;
        private System.Windows.Forms.CheckBox checkBoxNoAdminRights;
    }
}