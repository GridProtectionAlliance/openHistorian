#if !DEBUG
#define RunAsService
#endif

#if RunAsService
    using System.ServiceProcess;
#else
using System.Windows.Forms;
#endif

namespace openHistorian
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceHost host = new ServiceHost();

#if RunAsService
            // Run as Windows Service.
            ServiceBase.Run(new ServiceBase[] { host });
#else
            // Run as Windows Application.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DebugHost(host));
#endif
        }
    }
}