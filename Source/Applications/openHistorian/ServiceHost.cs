using TimeSeriesFramework;
using TVA;
using TVA.Configuration;

namespace openHistorian
{
    public class ServiceHost : ServiceHostBase
    {
        public ServiceHost() :base()
        {
            InitializeComponent();
        }

        protected override void ServiceStartingHandler(object sender, EventArgs<string[]> e)
        {
            // Handle base class service starting procedures
            base.ServiceStartingHandler(sender, e);

            // Make sure openPDC specific default service settings exist
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];

            systemSettings.Add("CompanyName", "Grid Protection Alliance", "The name of the company who owns this instance of the openHistorian.");
            systemSettings.Add("CompanyAcronym", "GPA", "The acronym representing the company who owns this instance of the openHistorian.");
        }

        private void InitializeComponent()
        {
            // 
            // ServiceHost
            // 
            this.ServiceName = "openHistorian";
        }
    }
}
