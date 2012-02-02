namespace openHistorianService
{
    partial class ServiceInstall
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
            this.m_serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.m_serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // m_serviceProcessInstaller
            // 
            this.m_serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.m_serviceProcessInstaller.Password = null;
            this.m_serviceProcessInstaller.Username = null;
            // 
            // m_serviceInstaller
            // 
            this.m_serviceInstaller.Description = "Receives, analyzes, archives and publishes time-series data.";
            this.m_serviceInstaller.DisplayName = "openHistorian";
            this.m_serviceInstaller.ServiceName = "openHistorianService";
            // 
            // ServiceInstall
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.m_serviceProcessInstaller,
            this.m_serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller m_serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller m_serviceInstaller;
    }
}