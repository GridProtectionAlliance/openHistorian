using TimeSeriesFramework;
namespace openHistorian
{
    public partial class DebugHost : DebugHostBase
    {
        public DebugHost(ServiceHost host)
        {
            this.ServiceHost = host;
        }

        protected override string ServiceClientName
        {
            get
            {
                return "openHistorianConsole.exe";
            }
        }
    }
}
