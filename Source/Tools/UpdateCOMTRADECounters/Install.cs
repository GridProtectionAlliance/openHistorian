using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace UpdateCOMTRADECounters
{
    public partial class Install : Form
    {
        private bool m_installed;

        public Install()
        {
            InitializeComponent();
        }

        private void Install_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.FriendlyName);
        }

        private void Install_FormClosed(object sender, FormClosedEventArgs e)
        {
            // If install form cancelled with out installing, close application
            if (!m_installed)
                Environment.Exit(0);
        }

        private void checkBoxNoAdminRights_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNoAdminRights.Checked)
            {
                radioButtonInstallAllUsers.Checked = false;
                radioButtonInstallAllUsers.Enabled = false;
                radioButtonInstallOnlyMe.Checked = true;
                checkBoxAllowAutoLaunch.Checked = false;
                checkBoxAllowCfgDownload.Checked = false;
                groupBoxBrowserPolcies.Enabled = false;
                buttonInstall.Visible = false;
                buttonInstallNoAdmin.Visible = true;
            }
            else
            {
                radioButtonInstallAllUsers.Enabled = true;
                radioButtonInstallAllUsers.Checked = true;
                radioButtonInstallOnlyMe.Checked = false;
                groupBoxBrowserPolcies.Enabled = true;
                checkBoxAllowAutoLaunch.Checked = true;
                checkBoxAllowCfgDownload.Checked = true;
                buttonInstallNoAdmin.Visible = false;
                buttonInstall.Visible = true;
            }
        }

        private void checkBoxBrowserPolicy_CheckedChanged(object sender, EventArgs e)
        {
            if (!groupBoxBrowserPolcies.Enabled)
                return;

            if (radioButtonInstallOnlyMe.Checked && !checkBoxAllowAutoLaunch.Checked && !checkBoxAllowCfgDownload.Checked)
            {
                buttonInstall.Visible = false;
                buttonInstallNoAdmin.Visible = true;
            }
            else
            {
                buttonInstallNoAdmin.Visible = false;
                buttonInstall.Visible = true;
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            string currentExeFilePath = Process.GetCurrentProcess().MainModule?.FileName ?? Program.CurrentAssembly.Location;
            
            ProcessStartInfo startInfo = new(currentExeFilePath)
            {
                Arguments = "-Install -ForceUpdate" +
                    (radioButtonInstallOnlyMe.Checked ? "" : " -AllUsers") +
                    (checkBoxAllowAutoLaunch.Checked ? "" : " -NoAutoLaunch") +
                    (checkBoxAllowCfgDownload.Checked ? "" : " -NoCfgExemption"),
                Verb = buttonInstallNoAdmin.Visible ? "" : "runas" // Launch as admin
            };

            try
            {
                using Process process = Process.Start(startInfo);
            
                if (process is null)
                {
                    MessageBox.Show(this, $"Failed to install {Program.FriendlyName}, could not start \"{currentExeFilePath}\".", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    process.WaitForExit();
                    m_installed = true;
                    MessageBox.Show(this, $"Successfully installed {Program.FriendlyName}.", "Installation Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to install {Program.FriendlyName}: {ex.Message}", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
