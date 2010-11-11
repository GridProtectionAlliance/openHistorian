//******************************************************************************************************
//  Main.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  -----------------------------------------------------------------------------------------------------
//  03/05/2010 - Pinal C. Patel
//       Generated original version of source code.
//  03/17/2010 - Pinal C. Patel
//       Updated to include File and Serial output options and option to format plain text output.
//  03/19/2010 - Pinal C. Patel
//       Modified to work with a live archive.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TVA.Communication;
using TVA.Configuration;
using TimeSeriesArchiver;
using TimeSeriesArchiver.Files;
using TimeSeriesArchiver.Packets;
using TVA.IO;
using TVA.Reflection;
using TVA.Windows.Forms;

namespace HistorianPlaybackUtility
{
    public partial class Main : Form
    {
        #region [ Members ]

        // Nested Types
        private class Metadata
        {
            public string Instance;
            public int PointID;
            public string PointName;
            public string PointDescription;

            public Metadata(MetadataRecord metadata)
            {
                Instance = metadata.PlantCode;
                PointID = metadata.HistorianID;
                PointName = metadata.Name;
                PointDescription = metadata.Description;
            }

            public override string ToString()
            {
                return string.Format("({0}:{1}) {2} | {3}", Instance, PointID, PointName, PointDescription);
            }
        }

        // Constants
        private const string ArchiveFileName = "{0}{1}_archive.d";
        private const string MetadataFileName = "{0}{1}_dbase.dat";
        private const string StateFileName = "{0}{1}_startup.dat";
        private const string IntercomFileName = "{0}scratch.dat";
        private const string WatermarkText = "Enter search phrase";

        // Fields
        private bool m_watermarkEnabled;
        private List<Thread> m_activeThreads;
        private ArchiveFile m_archiveFile;
        private IClient m_transmitClient;
        private System.Timers.Timer m_rollverWatcher;
        private ManualResetEvent m_rolloverWaitHandle;

        #endregion

        #region [ Constructors ]

        public Main()
        {
            InitializeComponent();

            // Initialize UI.
            EnableWatermark();
            StartTimeInput.Value = DateTime.UtcNow.AddMinutes(-5);
            EndTimeInput.Value = DateTime.UtcNow;

            // Add version number to title
            this.Text = string.Format(this.Text, AssemblyInfo.EntryAssembly.Version.ToString(3));

            foreach (string port in SerialPort.GetPortNames())
            {
                SerialPortInput.Items.Add(port);
            }

            if (SerialPortInput.Items.Count > 0)
            {
                SerialPortInput.SelectedIndex = 0;
                SerialBaudRateInput.SelectedIndex = 4;
                SerialParityInput.SelectedIndex = 0;
                SerialStopBitsInput.SelectedIndex = 1;
            }
            else
            {
                // No serial ports where found on this machineso the option for serial output will be removed
                OutputCannelTabs.TabPages.Remove(SerialSettingsTab);
            }

             // Initialize member variables.
            m_activeThreads = new List<Thread>();
            m_archiveFile = new ArchiveFile();
            m_archiveFile.StateFile = new StateFile();
            m_archiveFile.StateFile.FileAccessMode = FileAccess.Read;
            m_archiveFile.IntercomFile = new IntercomFile();
            m_archiveFile.IntercomFile.FileAccessMode = FileAccess.Read;
            m_archiveFile.MetadataFile = new MetadataFile();
            m_archiveFile.MetadataFile.FileAccessMode = FileAccess.Read;
            m_archiveFile.FileAccessMode = FileAccess.Read;
            m_archiveFile.HistoricFileListBuildStart += ArchiveFile_HistoricFileListBuildStart;
            m_archiveFile.HistoricFileListBuildComplete += ArchiveFile_HistoricFileListBuildComplete;
            m_rollverWatcher = new System.Timers.Timer();
            m_rollverWatcher.Interval = 1000;
            m_rollverWatcher.Elapsed += RollverWatcher_Elapsed;
            m_rollverWatcher.Start();
            m_rolloverWaitHandle = new ManualResetEvent(true);
        }

        #endregion

        #region [ Methods ]

        private void EnableWatermark()
        {
            if (SearchPhraseInput.Text == "")
            {
                SearchPhraseInput.Text = WatermarkText;
                SearchPhraseInput.ForeColor = SystemColors.GrayText;
                m_watermarkEnabled = true;
            }
        }

        private void DisableWatermark()
        {
            if (SearchPhraseInput.Text == WatermarkText)
            {
                SearchPhraseInput.Text = "";
                SearchPhraseInput.ForeColor = SystemColors.WindowText;
                m_watermarkEnabled = false;
            }
        }

        private void Process(object state)
        {
            object[] info = (object[])state;
            string ids = (string)info[0];
            DateTime startTime = (DateTime)info[1];
            DateTime endTime = (DateTime)info[2];
            bool repeatTransmit = (bool)info[3];
            string dataFormat = (string)info[4];
            int sampleRate = (int)info[5];

            int sleepTime = 0;
            if (sampleRate > 0)
                sleepTime = 1000 / sampleRate;

            try
            {
                lock (m_activeThreads)
                {
                    m_activeThreads.Add(Thread.CurrentThread);
                }

                ShowUpdateMessage("Started processing point {0} on thread {1}...", ids, Thread.CurrentThread.ManagedThreadId);
                while (true)
                {
                    foreach (string id in ids.Split(','))
                    {
                        IEnumerable<IDataPoint> data = m_archiveFile.ReadData(int.Parse(id), startTime, endTime);
                        ShowUpdateMessage("Processing measurements for point {0}...", id);
                        int count = 0;
                        byte[] buffer = null;
                        if (string.IsNullOrEmpty(dataFormat))
                        {
                            // Output in binary format.
                            foreach (IDataPoint sample in data)
                            {
                                m_transmitClient.SendAsync(new PacketType1(sample).BinaryImage, 0, PacketType1.ByteCount);
                                count++;
                                if (!m_rolloverWaitHandle.WaitOne(0))
                                    break;                  // Abort for rollover.
                                Thread.Sleep(sleepTime);    // Sleep for throttling.
                            }
                        }
                        else
                        {
                            // Output in plain-text format.
                            foreach (IDataPoint sample in data)
                            {
                                buffer = Encoding.ASCII.GetBytes(string.Format(dataFormat, sample, sample, sample, sample));
                                m_transmitClient.SendAsync(buffer, 0, buffer.Length);
                                count++;
                                if (!m_rolloverWaitHandle.WaitOne(0))
                                    break;                  // Abort for rollover.
                                Thread.Sleep(sleepTime);    // Sleep for throttling.
                            }
                        }
                        ShowUpdateMessage("Processed {0} measurements for point {1}.", count, id);
                    }

                    if (!repeatTransmit)
                        break;
                }
                ShowUpdateMessage("Completed processing point {0} on thread {1}.", ids, Thread.CurrentThread.ManagedThreadId);
            }
            catch (ThreadAbortException)
            {
                ShowUpdateMessage("Aborted processing point {0} on thread {1}.", ids, Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Error processing point {0} on thread {1} - {2}", ids, Thread.CurrentThread.ManagedThreadId, ex.Message);
            }
            finally
            {
                lock (m_activeThreads)
                {
                    m_activeThreads.Remove(Thread.CurrentThread);
                    if (m_activeThreads.Count == 0)
                        this.BeginInvoke((ThreadStart)delegate()
                        {
                            StopProcessing.Visible = false;
                            StartProcessing.Visible = true;
                            SplitContainerTop.Enabled = true;
                        });
                }
            }
        }

        private void ShowUpdateMessage(string message, params object[] args)
        {
            this.BeginInvoke((ThreadStart)delegate()
            {
                MessagesOutput.AppendText(string.Format("[{0}] ", DateTime.Now.ToString()));
                MessagesOutput.AppendText(string.Format(message, args));
                MessagesOutput.AppendText("\r\n");
                Application.DoEvents();
            });
        }

        #region [ Handlers ]

        private void Main_Load(object sender, EventArgs e)
        {
            this.RestoreLayout();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_archiveFile.StateFile != null)
                m_archiveFile.StateFile.Dispose();

            if (m_archiveFile.IntercomFile != null)
                m_archiveFile.IntercomFile.Dispose();

            if (m_archiveFile.MetadataFile != null)
                m_archiveFile.MetadataFile.Dispose();

            if (m_archiveFile != null)
                m_archiveFile.Dispose();

            if (m_transmitClient != null)
                m_transmitClient.Dispose();

            if (m_rollverWatcher != null)
                m_rollverWatcher.Dispose();

            if (m_rolloverWaitHandle != null)
                m_rolloverWaitHandle.Close();

            this.SaveLayout();
        }

        private void ArchiveLocationBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Prompt user for primary archive location.
            if (FolderBrowser.ShowDialog(this) == DialogResult.OK)
                ArchiveLocationInput.Text = FolderBrowser.SelectedPath;
        }

        private void ArchiveLocationInput_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ArchiveLocationInput.Text) && Directory.Exists(ArchiveLocationInput.Text))
            {
                // Specified directory is a valid one.
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    string[] matches = Directory.GetFiles(ArchiveLocationInput.Text, "*_archive.d");
                    if (matches.Length > 0)
                    {
                        // Capture the instance name.
                        string folder = FilePath.GetDirectoryName(matches[0]);
                        string instance = FilePath.GetFileName(matches[0]).Split('_')[0];

                        // Capture active archive.
                        m_archiveFile.FileName = matches[0];
                        m_archiveFile.StateFile.FileName = string.Format(StateFileName, folder, instance);
                        m_archiveFile.IntercomFile.FileName = string.Format(IntercomFileName, folder);
                        m_archiveFile.MetadataFile.FileName = string.Format(MetadataFileName, folder, instance);

                        // Open the active archive.
                        m_archiveFile.Open();

                        MetadataRecord definition;
                        List<string> previousSelection = new List<string>(ConfigurationFile.Current.Settings.General["Selection", true].ValueAs("").Split(','));
                        IDInput.Items.Clear();
                        for (int i = 1; i <= m_archiveFile.MetadataFile.RecordsOnDisk; i++)
                        {
                            definition = m_archiveFile.MetadataFile.Read(i);
                            if (definition.GeneralFlags.Enabled)
                            {
                                IDInput.Items.Add(new Metadata(definition));
                                if (previousSelection.Contains(definition.HistorianID.ToString()))
                                    IDInput.SetItemChecked(IDInput.Items.Count - 1, true);
                                Application.DoEvents();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowUpdateMessage("Error initializing application - {0}", ex.Message);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void SearchPhraseInput_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_watermarkEnabled)
                DisableWatermark();
        }

        private void SearchPhraseInput_Leave(object sender, EventArgs e)
        {
            if (!m_watermarkEnabled)
                EnableWatermark();
        }

        private void SearchPhraseInput_TextChanged(object sender, EventArgs e)
        {
            if (!m_watermarkEnabled)
                EnableWatermark();
        }

        private void SearchPhraseFind_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // Search for points matching the search criteria.
                int pointID = -1;
                Metadata definition = null;
                string searchPhrase = SearchPhraseInput.Text.ToLower();
                int.TryParse(searchPhrase, out pointID);

                this.Cursor = Cursors.WaitCursor;
                ShowUpdateMessage("Searching for points matching \"{0}\"...", searchPhrase);
                for (int i = 0; i < IDInput.Items.Count; i++)
                {
                    definition = (Metadata)IDInput.Items[i];
                    if (definition.PointID == pointID ||
                        definition.PointName.ToLower().Contains(searchPhrase) ||
                        definition.PointDescription.ToLower().Contains(searchPhrase))
                    {
                        IDInput.SetItemChecked(i, true);
                    }
                }
                ShowUpdateMessage("Found {0} point(s) matching \"{1}\".", IDInput.CheckedIndices.Count, searchPhrase);
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Error finding points - {0}", ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void SearchPhraseClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (int index in IDInput.CheckedIndices)
            {
                IDInput.SetItemChecked(index, false);
            }
        }

        private void FileNameBrowse_Click(object sender, EventArgs e)
        {
            if (SaveFile.ShowDialog(this) == DialogResult.OK)
                FileNameInput.Text = SaveFile.FileName;
        }

        private void StartProcessing_Click(object sender, EventArgs e)
        {
            // Validate selection.
            MessagesOutput.Clear();
            if (IDInput.CheckedIndices.Count == 0)
            {
                ShowUpdateMessage("No points selected for processing.");
                return;
            }

            // Capture selection.
            DateTime startTime = DateTime.Parse(StartTimeInput.Text);
            DateTime endTime = DateTime.Parse(EndTimeInput.Text);

            try
            {
                StartProcessing.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                // Dispose previously created client.
                if (m_transmitClient != null)
                    m_transmitClient.Dispose();

                // Create new client.
                ShowUpdateMessage("Initializing client...");
                List<object> state = new List<object>();
                state.Add(null);
                state.Add(startTime);
                state.Add(endTime);
                state.Add(RepeatDataProcessing.Checked);
                state.Add(OutputPlainTextDataFormat.Text);
                state.Add(int.Parse(ProcessDataAtIntervalSampleRate.Text));
                switch (OutputCannelTabs.SelectedIndex)
                {
                    case 0: // TCP
                        m_transmitClient = ClientBase.Create(string.Format("Protocol=TCP;Server={0}:{1}", TCPServerInput.Text, TCPPortInput.Text));
                        break;
                    case 1: // UDP
                        m_transmitClient = ClientBase.Create(string.Format("Protocol=UDP;Server={0}:{1};Port=-1", UDPServerInput.Text, UDPPortInput.Text));
                        break;
                    case 2: // File
                        m_transmitClient = ClientBase.Create(string.Format("Protocol=File;File={0}", FileNameInput.Text));
                        break;
                    case 3: // Serial
                        m_transmitClient = ClientBase.Create(string.Format("Protocol=Serial;Port={0};BaudRate={1};Parity={2};StopBits={3};DataBits={4};DtrEnable={5};RtsEnable={6}", SerialPortInput.Text, SerialBaudRateInput.Text, SerialParityInput.Text, SerialStopBitsInput.Text, SerialDataBitsInput.Text, SerialDtrEnable.Checked, SerialRtsEnable.Checked));
                        break;
                }
                m_transmitClient.Handshake = false;
                m_transmitClient.MaxConnectionAttempts = 10;
                ShowUpdateMessage("Client initialized!");

                // Connect the newly created client.
                ShowUpdateMessage("Connecting client...");
                m_transmitClient.Connect();
                if (m_transmitClient.CurrentState == ClientState.Connected)
                {
                    // Client connected successfully.
                    ShowUpdateMessage("Client connected!");

                    // Queue all selected points for processing.
                    string selection = "";
                    Metadata definition = null;
                    for (int i = 0; i < IDInput.CheckedItems.Count; i++)
                    {
                        definition = (Metadata)IDInput.CheckedItems[i];
                        if (ProcessDataInParallel.Checked)
                        {
                            ShowUpdateMessage("Queuing processing request for point {0}...", definition.PointID);
                            state[0] = definition.PointID.ToString();
                            ThreadPool.QueueUserWorkItem(Process, state.ToArray());
                        }
                        selection += definition.PointID + ",";   // Update information to be persisted.
                    }
                    selection = selection.TrimEnd(',');

                    if (!ProcessDataInParallel.Checked)
                    {
                        state[0] = selection;
                        ThreadPool.QueueUserWorkItem(Process, state.ToArray());
                    }

                    ConfigurationFile.Current.Settings.General["Selection", true].Value = selection.TrimEnd(',');
                    ConfigurationFile.Current.Save();

                    StopProcessing.Visible = true;
                    StartProcessing.Visible = false;
                    SplitContainerTop.Enabled = false;
                }
                else
                {
                    ShowUpdateMessage("Connection timeout.");
                }
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Error starting processing - {0}", ex.Message);
            }
            finally
            {
                StartProcessing.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void StopProcessing_Click(object sender, EventArgs e)
        {
            lock (m_activeThreads)
            {
                foreach (Thread activeThread in m_activeThreads)
                {
                    activeThread.Abort();
                }
            }
        }

        private void ProcessDataFullSpeed_CheckedChanged(object sender, EventArgs e)
        {
            ProcessDataAtIntervalSampleRate.Text = "0";
            ProcessDataAtIntervalSampleRate.Enabled = false;
        }

        private void ProcessDataAtInterval_CheckedChanged(object sender, EventArgs e)
        {
            ProcessDataAtIntervalSampleRate.Text = "30";
            ProcessDataAtIntervalSampleRate.Enabled = true;
        }

        private void OutputBinaryData_CheckedChanged(object sender, EventArgs e)
        {
            OutputPlainTextDataFormat.Text = "";
            OutputPlainTextDataFormat.Enabled = false;
        }

        private void OutputPlainTextData_CheckedChanged(object sender, EventArgs e)
        {
            OutputPlainTextDataFormat.Text = "{0:I},{1:T},{2:V},{3:Q}";
            OutputPlainTextDataFormat.Enabled = true;
        }

        private void ArchiveFile_HistoricFileListBuildStart(object sender, EventArgs e)
        {
            ShowUpdateMessage("Building list of historic archive files...");
        }

        private void ArchiveFile_HistoricFileListBuildComplete(object sender, EventArgs e)
        {
            ShowUpdateMessage("Completed building list of historic archive files.");
        }

        private void RollverWatcher_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_archiveFile.IntercomFile.IsOpen)
            {
                IntercomRecord record = m_archiveFile.IntercomFile.Read(1);
                // Pause processing.
                if (record.RolloverInProgress && m_archiveFile.IsOpen)
                {
                    m_rolloverWaitHandle.Reset();
                    m_archiveFile.Close();
                    ShowUpdateMessage("Archive rollover in progress...");
                }
                // Resume processing.
                if (!record.RolloverInProgress && !m_archiveFile.IsOpen)
                {
                    m_archiveFile.Open();
                    m_rolloverWaitHandle.Set();
                    ShowUpdateMessage("Archive rollover complete!");
                }
            }
        }
        
        #endregion

        #endregion
    }
}
