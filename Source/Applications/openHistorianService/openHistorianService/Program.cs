#if DEBUG
#define RunAsApp
#endif

#if RunAsApp
using System.Windows.Forms;
#else
using System.ServiceProcess;
#endif

namespace openHistorianService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if RunAsApp
            // Run as Windows Application.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DebugHost());
#else
            // Run as Windows Service.
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ServiceHost() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
