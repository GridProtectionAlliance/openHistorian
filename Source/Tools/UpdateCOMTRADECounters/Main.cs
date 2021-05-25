//******************************************************************************************************
//  Main.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/16/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF;
using GSF.COMTRADE;
using GSF.Console;
using GSF.Threading;

#pragma warning disable IDE1006 // Naming Styles

// ReSharper disable LocalizableElement
namespace UpdateCOMTRADECounters
{
    public partial class Main : Form
    {
        private readonly DelayedSynchronizedOperation m_showToolTip;
        private Control m_focusTarget;
        private string m_callback;

        public Main()
        {
            InitializeComponent();

            // Tool tip is being shown any time window is moved, however, this event is called very rapidly,
            // so we used a delayed synchronized operation to prevent updating tool tip too frequently
            m_showToolTip = new DelayedSynchronizedOperation(() =>
            {
                BeginInvoke(new Action(() =>
                {
                    if (!Visible)
                        return;

                    toolTip.Show(toolTip.GetToolTip(buttonOpenSourceCFFLocation), buttonOpenSourceCFFLocation, -20, 20, 10000);
                    m_focusTarget?.Focus();
                }));
            })
            {
                Delay = 10
            };
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (!TryParseCommandLine() && !TryParseClipboard())
                m_focusTarget = maskedTextBoxEndSampleCount;
        }

        private void buttonOpenSourceCFFLocation_Click(object sender, EventArgs e)
        {
            hideToolTip(sender, e);
            getCounters(out long endSampleCount, out long binaryByteCount);

            if (endSampleCount == 0L)
            {
                MessageBox.Show(this, "Cannot update counters: end sample count is zero", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            openFileDialogCFF.InitialDirectory = ShellHelpers.GetDownloadsFolder();
            openFileDialogCFF.FileName = string.Empty;

            if (openFileDialogCFF.ShowDialog(this) != DialogResult.OK)
                return;

            string sourceCFF = textBoxSourceCFF.Text = openFileDialogCFF.FileName;
            
            textBoxSourceCFF.Refresh();
            Application.DoEvents();

            if (!File.Exists(sourceCFF))
            {
                MessageBox.Show(this, $"Cannot update counters: selected CFF \"{sourceCFF}\" does not exist", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Application.UseWaitCursor = true;
                Schema schema = new(sourceCFF);
                
                if (!schema.IsCombinedFileFormat)
                {
                    MessageBox.Show(this, $"Cannot update counters: selected COMTRADE file \"{sourceCFF}\" is not combined format file (CFF)", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (schema.FileType != FileType.Ascii && binaryByteCount == 0L)
                {
                    MessageBox.Show(this, "Cannot update counters: CFF file type is binary and binary count is zero", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (FileStream stream = File.Open(sourceCFF, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    Writer.UpdateCFFEndSample(stream, endSampleCount);

                    if (binaryByteCount > 0L && schema.FileType != FileType.Ascii)
                        Writer.UpdateCFFStreamBinaryByteCount(stream, binaryByteCount);
                }

                Hide();
                Application.DoEvents();

                Task<WebResponse> responseTask = null;

                if (!string.IsNullOrEmpty(m_callback))
                {
                    try
                    {
                        responseTask = WebRequest.Create(Uri.UnescapeDataString(m_callback)).GetResponseAsync();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                MessageBox.Show(this, $"COMTRADE counters for \"{sourceCFF}\" successfully updated with end sample count of {endSampleCount:N0}{(binaryByteCount > 0L ? $" and binary byte count of {binaryByteCount:N0}" : "")}", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                responseTask?.Wait(5000);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed while updating counters: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        private void getCounters(out long endSampleCount, out long binaryByteCount)
        {
            long.TryParse(maskedTextBoxEndSampleCount.Text, out endSampleCount);
            long.TryParse(maskedTextBoxBinaryByteCount.Text, out binaryByteCount);
        }

        private void showToolTip(object sender, EventArgs e) =>
            m_showToolTip.TryRunOnce();

        private void hideToolTip(object sender, EventArgs e) =>
            toolTip.Hide(buttonOpenSourceCFFLocation);

        private void textBox_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)delegate
            {
                switch (sender)
                {
                    case TextBox textBox:
                        textBox.SelectAll();
                        break;
                    case MaskedTextBox maskedTextBox:
                        maskedTextBox.SelectAll();
                        break;
                }
            });
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e) => 
            Process.Start("https://github.com/GridProtectionAlliance/openHistorian");

        // Parsing command line is reliable
        private bool TryParseCommandLine()
        {
            string[] args = Arguments.ToArgs(Environment.CommandLine);
            return args.Length > 1 && TryParseURL(args[1], true);
        }

        // Fall back on parsing from clipboard, this can be disabled by browser/administrator so it is less reliable,
        // however, this option can help when URI scheme is not registered, i.e., user downloads app directly
        private bool TryParseClipboard() => 
            Clipboard.ContainsText() && TryParseURL(Clipboard.GetText(), false);

        private bool TryParseURL(string url, bool showError)
        {
            try
            {
                string query = new Uri(url).Query;

                while (query.Length > 1 && query[0] == '?')
                    query = query.Length == 1 ? string.Empty : query.Substring(1);

                if (string.IsNullOrEmpty(query))
                    return false;

                Dictionary<string, string> parameters = query.ParseKeyValuePairs('&');

                if (parameters.TryGetValue("targetExportName", out string value))
                    textBoxSourceCFF.Text = value;

                if (parameters.TryGetValue("endSampleCount", out value) && long.TryParse(value, out long endSampleCount))
                {
                    maskedTextBoxEndSampleCount.Text = endSampleCount.ToString();
                    m_focusTarget = maskedTextBoxBinaryByteCount;
                }

                if (parameters.TryGetValue("binaryByteCount", out value) && long.TryParse(value, out long binaryByteCount))
                {
                    maskedTextBoxBinaryByteCount.Text = binaryByteCount.ToString();
                    m_focusTarget = textBoxSourceCFF;
                }

                if (m_focusTarget is null)
                    m_focusTarget = maskedTextBoxEndSampleCount;

                parameters.TryGetValue("callback", out m_callback);

                return true;
            }
            catch (Exception ex)
            {
                if (showError)
                    MessageBox.Show($"Failed to parse command line \"{Environment.CommandLine}\": {ex.Message}", "Command Line Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }
    }
}
