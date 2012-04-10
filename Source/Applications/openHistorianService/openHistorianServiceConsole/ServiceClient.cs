using System;
using System.ComponentModel;
using System.Text;
using TVA.Console;
using TVA.Reflection;
using TVA.ServiceProcess;

namespace openHistorianServiceConsole
{
    public partial class ServiceClient : Component
    {
        #region [ Members ]

        // Fields
        private bool m_telnetActive;
        private ConsoleColor m_originalBgColor;
        private ConsoleColor m_originalFgColor;

        #endregion

        #region [ Constructors ]

        public ServiceClient()
            : base()
        {
            InitializeComponent();

            // Save the color scheme.
            m_originalBgColor = Console.BackgroundColor;
            m_originalFgColor = Console.ForegroundColor;

            // Register event handlers.
            m_clientHelper.AuthenticationFailure += ClientHelper_AuthenticationFailure;
            m_clientHelper.ReceivedServiceUpdate += ClientHelper_ReceivedServiceUpdate;
            m_clientHelper.ReceivedServiceResponse += ClientHelper_ReceivedServiceResponse;
            m_clientHelper.TelnetSessionEstablished += ClientHelper_TelnetSessionEstablished;
            m_clientHelper.TelnetSessionTerminated += ClientHelper_TelnetSessionTerminated;
        }

        #endregion

        #region [ Methods ]

        public void Start(string[] args)
        {
            string userInput = null;
            Arguments arguments = new Arguments(string.Join(" ", args));

            if (arguments.Exists("server"))
            {
                // Override default settings with user provided input. 
                m_clientHelper.PersistSettings = false;
                m_remotingClient.PersistSettings = false;
                if (arguments.Exists("server"))
                    m_remotingClient.ConnectionString = string.Format("Server={0}", arguments["server"]);
            }

            // Connect to service and send commands. 
            m_clientHelper.Connect();
            while (m_clientHelper.Enabled &&
                   string.Compare(userInput, "Exit", true) != 0)
            {
                // Wait for a command from the user. 
                userInput = Console.ReadLine();
                // Write a blank line to the console.
                Console.WriteLine();

                if (!string.IsNullOrEmpty(userInput))
                {
                    // The user typed in a command and didn't just hit <ENTER>. 
                    switch (userInput.ToUpper())
                    {
                        case "CLS":
                            // User wants to clear the console window. 
                            Console.Clear();
                            break;
                        case "EXIT":
                            // User wants to exit the telnet session with the service. 
                            if (m_telnetActive)
                            {
                                userInput = string.Empty;
                                m_clientHelper.SendRequest("Telnet -disconnect");
                            }
                            break;
                        default:
                            // User wants to send a request to the service. 
                            m_clientHelper.SendRequest(userInput);
                            if (string.Compare(userInput, "Help", true) == 0)
                                DisplayHelp();

                            break;
                    }
                }
            }
        }

        private void DisplayHelp()
        {
            StringBuilder help = new StringBuilder();

            help.AppendFormat("Commands supported by {0}:", AssemblyInfo.EntryAssembly.Name);
            help.AppendLine();
            help.AppendLine();
            help.Append("Command".PadRight(20));
            help.Append(" ");
            help.Append("Description".PadRight(55));
            help.AppendLine();
            help.Append(new string('-', 20));
            help.Append(" ");
            help.Append(new string('-', 55));
            help.AppendLine();
            help.Append("Cls".PadRight(20));
            help.Append(" ");
            help.Append("Clears this console screen".PadRight(55));
            help.AppendLine();
            help.Append("Exit".PadRight(20));
            help.Append(" ");
            help.Append("Exits this console screen".PadRight(55));
            help.AppendLine();
            help.AppendLine();
            help.AppendLine();

            Console.Write(help.ToString());
        }

        private void ClientHelper_AuthenticationFailure(object sender, CancelEventArgs e)
        {
            // Prompt for credentials.
            StringBuilder prompt = new StringBuilder();
            prompt.AppendLine();
            prompt.AppendLine();
            prompt.Append("Connection to the service was rejected due to authentication failure. \r\n");
            prompt.Append("Enter the credentials to be used for authentication with the service.");
            prompt.AppendLine();
            prompt.AppendLine();
            Console.Write(prompt.ToString());

            // Capture the username.
            Console.Write("Enter username: ");
            m_clientHelper.Username = Console.ReadLine();

            // Capture the password.
            ConsoleKeyInfo key;
            Console.Write("Enter password: ");
            while ((key = Console.ReadKey(true)).KeyChar != '\r')
            {
                m_clientHelper.Password += key.KeyChar;
            }

            // Re-attempt connection with new credentials.
            e.Cancel = false;
            Console.WriteLine();
            Console.WriteLine();
        }

        private void ClientHelper_ReceivedServiceUpdate(object sender, EventArgs<UpdateType, string> e)
        {
            // Output status updates from the service to the console window.
            switch (e.Argument1)
            {
                case UpdateType.Alarm:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case UpdateType.Information:
                    Console.ForegroundColor = m_originalFgColor;
                    break;
                case UpdateType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }
            Console.Write(e.Argument2);
            Console.ForegroundColor = m_originalFgColor;
        }

        private void ClientHelper_ReceivedServiceResponse(object sender, EventArgs<ServiceResponse> e)
        {
            // TODO: Handle custom service responses here.
            Console.Write(string.Format("Received custom response \"{0}\" from remote service.\r\n\r\n", e.Argument.Type));
        }

        private void ClientHelper_TelnetSessionEstablished(object sender, EventArgs e)
        {
            // Change the console color scheme to indicate active telnet session.
            m_telnetActive = true;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        private void ClientHelper_TelnetSessionTerminated(object sender, EventArgs e)
        {
            // Revert to original color scheme to indicate end of telnet session.
            m_telnetActive = false;
            Console.BackgroundColor = m_originalBgColor;
            Console.ForegroundColor = m_originalFgColor;
            Console.Clear();
        }

        #endregion
    }
}
