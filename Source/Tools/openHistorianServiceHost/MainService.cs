using System.ServiceProcess;

namespace openHistorianServiceHost
{
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //args = args;
        }

        protected override void OnStop()
        {
        }
    }
}