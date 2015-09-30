//******************************************************************************************************
//  MySqlSetup.cs - Gbtc
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
//  02/23/2011 - Mehulbhai Thakkar
//       Added "Allow User Variables" setting so that session variables can be created without errors.
//  03/02/2011 - J. Ritchie Carroll
//       Added key value delimeters only between settings.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GSF;
using GSF.IO;
using Microsoft.Win32;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// This class is used to aid in the manipulation of a MySQL connection string as well as running the mysql.exe process.
    /// </summary>
    public class MySqlSetup
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// This event is triggered when error data is received while executing a SQL Script.
        /// </summary>
        public event DataReceivedEventHandler ErrorDataReceived;

        /// <summary>
        /// This event is triggered when output data is received while executing a SQL Script.
        /// </summary>
        public event DataReceivedEventHandler OutputDataReceived;

        // Fields

        private Dictionary<string, string> m_settings;
        private string m_mysqlExe;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="MySqlSetup"/> class.
        /// </summary>
        public MySqlSetup()
        {
            m_settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            m_settings["Allow User Variables"] = "true"; // This setting allows creation of user defined session variables.

            try
            {
                // Try to get path for mysql executable based on registered Windows service path, if this fails, fall back on just the executable name which will require a proper environmental path to run
                m_mysqlExe = FilePath.GetDirectoryName(Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\MySQL", "ImagePath", "mysql.exe").ToString().Split(new string[] {"\" "}, StringSplitOptions.RemoveEmptyEntries)[0].Replace("\"", "")) + "mysql.exe";

                if (!File.Exists(m_mysqlExe))
                    m_mysqlExe = "mysql.exe";
            }
            catch
            {
                m_mysqlExe = "mysql.exe";
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the path to the MySQL client executable.
        /// </summary>
        public string MysqlExe
        {
            get
            {
                return m_mysqlExe;
            }
            set
            {
                m_mysqlExe = value;
            }
        }

        /// <summary>
        /// Gets or sets the host name of the MySQL database.
        /// </summary>
        public string HostName
        {
            get
            {
                return m_settings["Server"];
            }
            set
            {
                m_settings["Server"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the MySQL database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                if (m_settings.ContainsKey("Database"))
                    return m_settings["Database"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Database");
                else
                    m_settings["Database"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the user name for the user whom has access to the database.
        /// </summary>
        public string UserName
        {
            get
            {
                if (m_settings.ContainsKey("Uid"))
                    return m_settings["Uid"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Uid");
                else
                    m_settings["Uid"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the password for the user whom has access to the database.
        /// </summary>
        public string Password
        {
            get
            {
                if (m_settings.ContainsKey("Pwd"))
                    return m_settings["Pwd"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Pwd");
                else
                    m_settings["Pwd"] = value;
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

                return builder.ToString();
            }
            set
            {
                m_settings = value.ParseKeyValuePairs();
            }
        }

        /// <summary>
        /// Converts the current settings to an OleDB connection string.
        /// </summary>
        public string OleDbConnectionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("Provider=MySQLProv");
                builder.Append("; location=");
                builder.Append(HostName.Replace("localhost", "MACHINE"));
                builder.Append("; Data Source=");
                builder.Append(DatabaseName);

                if (!string.IsNullOrEmpty(UserName))
                {
                    builder.Append("; User Id=");
                    builder.Append(UserName);
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    builder.Append("; Password=");
                    builder.Append(Password);
                }

                return builder.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Execute a SQL statement using the mysql.exe process.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool ExecuteStatement(string statement)
        {
            Process mySqlProcess = null;

            try
            {
                // Set up arguments for mysql.exe.
                StringBuilder args = new StringBuilder();

                args.Append("-h");
                args.Append(HostName);

                args.Append(" -D");
                args.Append(DatabaseName);

                if (!string.IsNullOrEmpty(UserName))
                {
                    args.Append(" -u");
                    args.Append(UserName);
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    args.Append(" -p");
                    args.Append(Password);
                }

                args.Append(" -e \"");
                args.Append(statement);
                args.Append('"');

                // Start mysql.exe.
                mySqlProcess = new Process();
                mySqlProcess.StartInfo.FileName = m_mysqlExe;
                mySqlProcess.StartInfo.Arguments = args.ToString();
                mySqlProcess.StartInfo.UseShellExecute = false;
                mySqlProcess.StartInfo.RedirectStandardError = true;
                mySqlProcess.ErrorDataReceived += mySqlProcess_ErrorDataReceived;
                mySqlProcess.StartInfo.RedirectStandardOutput = true;
                mySqlProcess.OutputDataReceived += mySqlProcess_OutputDataReceived;
                mySqlProcess.StartInfo.CreateNoWindow = true;
                mySqlProcess.Start();

                mySqlProcess.BeginErrorReadLine();
                mySqlProcess.BeginOutputReadLine();

                // Wait for mysql.exe to finish.
                mySqlProcess.WaitForExit();

                return mySqlProcess.ExitCode == 0;
            }
            finally
            {
                // Close the process.
                if (mySqlProcess != null)
                    mySqlProcess.Close();
            }
        }

        /// <summary>
        /// Executes a SQL Script using the mysql.exe process.
        /// </summary>
        /// <param name="scriptPath">The path of the script to be executed.</param>
        /// <returns>True if the script executes successfully. False otherwise.</returns>
        public bool ExecuteScript(string scriptPath)
        {
            Process mySqlProcess = null;
            StreamReader scriptStream = null;
            StreamWriter processInput = null;

            try
            {
                // Set up arguments for mysql.exe.
                StringBuilder args = new StringBuilder();

                args.Append("-h");
                args.Append(HostName);

                if (!string.IsNullOrEmpty(UserName))
                {
                    args.Append(" -u");
                    args.Append(UserName);
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    args.Append(" -p");
                    args.Append(Password);
                }

                // Start mysql.exe.
                mySqlProcess = new Process();
                mySqlProcess.StartInfo.FileName = m_mysqlExe;
                mySqlProcess.StartInfo.Arguments = args.ToString();
                mySqlProcess.StartInfo.UseShellExecute = false;
                mySqlProcess.StartInfo.RedirectStandardError = true;
                mySqlProcess.ErrorDataReceived += mySqlProcess_ErrorDataReceived;
                mySqlProcess.StartInfo.RedirectStandardInput = true;
                mySqlProcess.StartInfo.RedirectStandardOutput = true;
                mySqlProcess.OutputDataReceived += mySqlProcess_OutputDataReceived;
                mySqlProcess.StartInfo.CreateNoWindow = true;
                mySqlProcess.Start();

                mySqlProcess.BeginErrorReadLine();
                mySqlProcess.BeginOutputReadLine();

                // Send the script as standard input to mysql.exe.
                scriptStream = new StreamReader(new FileStream(scriptPath, FileMode.Open, FileAccess.Read));
                processInput = mySqlProcess.StandardInput;

                while (!scriptStream.EndOfStream)
                {
                    string line = scriptStream.ReadLine();

                    if (line.StartsWith("CREATE DATABASE") || line.StartsWith("USE"))
                        line = line.Replace("openHistorian", DatabaseName);

                    processInput.WriteLine(line);
                }

                // Wait for mysql.exe to finish.
                processInput.Close();
                mySqlProcess.WaitForExit();

                return mySqlProcess.ExitCode == 0;
            }
            finally
            {
                // Close streams and processes.
                if (scriptStream != null)
                    scriptStream.Close();

                if (processInput != null)
                    processInput.Close();

                if (mySqlProcess != null)
                    mySqlProcess.Close();
            }
        }

        private void mySqlProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (ErrorDataReceived != null)
                ErrorDataReceived(sender, e);
        }

        private void mySqlProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (OutputDataReceived != null)
                OutputDataReceived(sender, e);
        }

        #endregion
    }
}