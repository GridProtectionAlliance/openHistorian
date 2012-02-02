
namespace openHistorianServiceConsole
{
    class Program
    {
        static ServiceClient m_serviceClient;

        static void Main(string[] args)
        {
            // Enable console events.
            TVA.Console.Events.ConsoleClosing += OnConsoleClosing;
            TVA.Console.Events.EnableRaisingEvents();

            // Start the client component.
            m_serviceClient = new ServiceClient();
            m_serviceClient.Start(args);
            m_serviceClient.Dispose();
        }

        static void OnConsoleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Dispose the client component.
            m_serviceClient.Dispose();
        }
    }
}
