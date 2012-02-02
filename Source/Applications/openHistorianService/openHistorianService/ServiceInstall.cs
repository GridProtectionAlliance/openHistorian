using System.ComponentModel;
using System.Configuration.Install;


namespace openHistorianService
{
    [RunInstaller(true)]
    public partial class ServiceInstall : Installer
    {
        public ServiceInstall()
        {
            InitializeComponent();
        }
    }
}
