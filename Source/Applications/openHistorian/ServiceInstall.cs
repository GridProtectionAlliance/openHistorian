using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace openHistorian
{
    [RunInstaller(true)]
    public partial class ServiceInstall : System.Configuration.Install.Installer
    {
        public ServiceInstall()
        {
            InitializeComponent();
        }
    }
}
