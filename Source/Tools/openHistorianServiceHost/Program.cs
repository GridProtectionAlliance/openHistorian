using System;
using System.Windows.Forms;

namespace openHistorianServiceHost
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
#if !debug
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MainService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}