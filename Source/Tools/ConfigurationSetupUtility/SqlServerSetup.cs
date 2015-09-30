//******************************************************************************************************
//  SqlServerSetup.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  06/29/2010 - Stephen C. Wills
//       Generated original version of source code.
//  03/02/2011 - J. Ritchie Carroll
//       Added key value delimeters only between settings.
//       Added IntegratedSecurityConnectionString property.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GSF;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// This class is used to aid in the manipulation of a SQL Server connection string as well as running the sqlcmd.exe process.
    /// </summary>
    public class SqlServerSetup
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// This event is triggered when error data is received while running a SQL Script.
        /// </summary>
        public event DataReceivedEventHandler ErrorDataReceived;

        /// <summary>
        /// This event is triggered when output data is received while running a SQL Script.
        /// </summary>
        public event DataReceivedEventHandler OutputDataReceived;

        // Fields

        private Dictionary<string, string> m_settings;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="MySqlSetup"/> class.
        /// </summary>
        public SqlServerSetup()
        {
            m_settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            m_settings["pooling"] = "false";
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the host name of the MySQL database.
        /// </summary>
        public string HostName
        {
            get
            {
                return m_settings["Data Source"];
            }
            set
            {
                m_settings["Data Source"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the MySQL database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                if (m_settings.ContainsKey("Initial Catalog"))
                    return m_settings["Initial Catalog"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Initial Catalog");
                else
                    m_settings["Initial Catalog"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the user name for the user whom has access to the database.
        /// </summary>
        public string UserName
        {
            get
            {
                if (m_settings.ContainsKey("User ID"))
                    return m_settings["User ID"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("User ID");
                else
                    m_settings["User ID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the password for the user whom has access to the database.
        /// </summary>
        public string Password
        {
            get
            {
                if (m_settings.ContainsKey("Password"))
                    return m_settings["Password"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Password");
                else
                    m_settings["Password"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string used to access the database.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                foreach (string key in m_settings.Keys)
                {
                    if (builder.Length > 0)
                        builder.Append("; ");

                    builder.Append(key);
                    builder.Append('=');
                    builder.Append(m_settings[key]);
                }

                if (!m_settings.ContainsKey("User ID") && !m_settings.ContainsKey("Uid"))
                    builder.Append("; Integrated Security=SSPI");

                return builder.ToString();
            }
            set
            {
                m_settings = value.ParseKeyValuePairs();
            }
        }

        /// <summary>
        /// Gets connection string used to access the database with integrated security (i.e., Windows pass through authentication).
        /// </summary>
        public string IntegratedSecurityConnectionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                foreach (string key in m_settings.Keys)
                {
                    if (string.Compare(key, "User ID", true) != 0 && string.Compare(key, "Uid", true) != 0 && string.Compare(key, "Password", true) != 0 && string.Compare(key, "Pwd", true) != 0 && string.Compare(key, "pooling", true) != 0)
                    {
                        if (builder.Length > 0)
                            builder.Append("; ");

                        builder.Append(key);
                        builder.Append('=');
                        builder.Append(m_settings[key]);
                    }
                }

                builder.Append("; Integrated Security=SSPI");

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the connection string without "pooling=false" setting in it (this will be used to store into the config files).
        /// </summary>
        public string PooledConnectionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                foreach (string key in m_settings.Keys)
                {
                    if (string.Compare(key, "pooling", true) != 0)
                    {
                        if (builder.Length > 0)
                            builder.Append("; ");

                        builder.Append(key);
                        builder.Append('=');
                        builder.Append(m_settings[key]);
                    }
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Converts the current settings to an OleDB connection string.
        /// </summary>
        public string OleDbConnectionString
        {
            get
            {
                return "Provider=SQLOLEDB; " + ConnectionString;
            }
        }

        #endregion

        #region [ Methods ]

        public bool ExecuteStatement(string statement)
        {
            Process sqlCmdProcess = null;

            try
            {
                // Set up arguments for sqlcmd.exe.
                StringBuilder args = new StringBuilder();

                args.Append("-b -S ");
                args.Append(HostName);

                args.Append(" -d ");
                args.Append(DatabaseName);

                if (!string.IsNullOrEmpty(UserName))
                {
                    args.Append(" -U ");
                    args.Append(UserName);
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    args.Append(" -P ");
                    args.Append(Password);
                }

                args.Append(" -Q \"");
                args.Append(statement);
                args.Append('"');

                // Start sqlcmd.exe.
                sqlCmdProcess = new Process();
                sqlCmdProcess.StartInfo.FileName = "sqlcmd.exe";
                sqlCmdProcess.StartInfo.Arguments = args.ToString();
                sqlCmdProcess.StartInfo.UseShellExecute = false;
                sqlCmdProcess.StartInfo.RedirectStandardError = true;
                sqlCmdProcess.ErrorDataReceived += sqlCmdProcess_ErrorDataReceived;
                sqlCmdProcess.StartInfo.RedirectStandardOutput = true;
                sqlCmdProcess.OutputDataReceived += sqlCmdProcess_OutputDataReceived;
                sqlCmdProcess.StartInfo.CreateNoWindow = true;
                sqlCmdProcess.Start();

                sqlCmdProcess.BeginErrorReadLine();
                sqlCmdProcess.BeginOutputReadLine();

                sqlCmdProcess.WaitForExit();

                return sqlCmdProcess.ExitCode == 0;
            }
            finally
            {
                // Close the process.
                if (sqlCmdProcess != null)
                    sqlCmdProcess.Close();
            }
        }

        /// <summary>
        /// Executes a SQL script using the SQL Server database engine.
        /// </summary>
        /// <param name="scriptPath">The path to the SQL Server script to be executed.</param>
        /// <returns>True if the script executed successfully. False otherwise.</returns>
        public bool ExecuteScript(string scriptPath)
        {
            Process sqlCmdProcess = null;
            StreamReader scriptStream = null;
            StreamWriter copyStream = null;
            string copyPath = Path.GetTempFileName();

            try
            {
                // Set up arguments for sqlcmd.exe.
                StringBuilder args = new StringBuilder();

                args.Append("-b -S ");
                args.Append(HostName);

                if (!string.IsNullOrEmpty(UserName))
                {
                    args.Append(" -U ");
                    args.Append(UserName);
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    args.Append(" -P ");
                    args.Append(Password);
                }

                args.Append(" -i ");

                // Copy the script to a temporary file with the proper database name.
                scriptStream = new StreamReader(new FileStream(scriptPath, FileMode.Open, FileAccess.Read));
                copyStream = new StreamWriter(new FileStream(copyPath, FileMode.Create, FileAccess.Write));

                while (!scriptStream.EndOfStream)
                {
                    string line = scriptStream.ReadLine();

                    if (line.StartsWith("CREATE DATABASE") || line.StartsWith("ALTER DATABASE") || line.StartsWith("USE"))
                        line = line.Replace("openHistorian", DatabaseName);

                    copyStream.WriteLine(line);
                }

                copyStream.Close();

                // Start sqlcmd.exe.
                sqlCmdProcess = new Process();
                sqlCmdProcess.StartInfo.FileName = "sqlcmd.exe";
                sqlCmdProcess.StartInfo.Arguments = args.ToString() + '"' + copyPath + '"';
                sqlCmdProcess.StartInfo.UseShellExecute = false;
                sqlCmdProcess.StartInfo.RedirectStandardError = true;
                sqlCmdProcess.ErrorDataReceived += sqlCmdProcess_ErrorDataReceived;
                sqlCmdProcess.StartInfo.RedirectStandardOutput = true;
                sqlCmdProcess.OutputDataReceived += sqlCmdProcess_OutputDataReceived;
                sqlCmdProcess.StartInfo.CreateNoWindow = true;
                sqlCmdProcess.Start();

                sqlCmdProcess.BeginErrorReadLine();
                sqlCmdProcess.BeginOutputReadLine();

                sqlCmdProcess.WaitForExit();

                return sqlCmdProcess.ExitCode == 0;
            }
            finally
            {
                // Close streams and processes.
                if (scriptStream != null)
                    scriptStream.Close();

                if (copyStream != null)
                    copyStream.Close();

                if (sqlCmdProcess != null)
                    sqlCmdProcess.Close();

                // Delete the temporary file.
                if (File.Exists(copyPath))
                    File.Delete(copyPath);
            }
        }

        private void sqlCmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (ErrorDataReceived != null)
                ErrorDataReceived(sender, e);
        }

        private void sqlCmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (OutputDataReceived != null)
                OutputDataReceived(sender, e);
        }

        #endregion
    }
}