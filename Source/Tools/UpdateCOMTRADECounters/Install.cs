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
                buttonInstall.Visible = false;
                buttonInstallNoAdmin.Visible = true;
            }
            else
            {
                radioButtonInstallAllUsers.Enabled = true;
                radioButtonInstallAllUsers.Checked = true;
                radioButtonInstallOnlyMe.Checked = false;
                buttonInstallNoAdmin.Visible = false;
                buttonInstall.Visible = true;
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            string currentExeFilePath = Process.GetCurrentProcess().MainModule?.FileName ?? Program.CurrentAssembly.Location;
            
            ProcessStartInfo startInfo = new(currentExeFilePath)
            {
                Arguments = radioButtonInstallOnlyMe.Checked ?
                    "-Install -ForceUpdate" :
                    "-Install -AllUsers -ForceUpdate",
                Verb = checkBoxNoAdminRights.Checked ? "" : "runas"
            };
            
            using Process process = Process.Start(startInfo);
            
            if (process is null)
            {
                MessageBox.Show($"Failed to install {Program.FriendlyName}, could not start \"{currentExeFilePath}\".", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                process.WaitForExit();
                m_installed = true;
                MessageBox.Show($"Successfully installed {Program.FriendlyName}.", "Installation Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Close();
        }
    }
}
