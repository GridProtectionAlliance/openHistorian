using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GSF;
using GSF.IO;
using GSF.IO.FileStructure;
using GSF.IO.FileStructure.Media;

namespace ArchiveIntegrityChecker
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private volatile bool m_quit;
        private bool m_shouldQuickScan;
        private double m_mbToScan;
        private int m_filesScanned;
        private int m_filesToScan;

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            m_filesScanned = 0;
            m_shouldQuickScan = chkQuickScan.Checked;
            if (m_shouldQuickScan)
            {
                if (!double.TryParse(txtMBToScan.Text, out m_mbToScan))
                {
                    MessageBox.Show("Enter a valid decimal for MB To Scan");
                    return;
                }
            }
            m_quit = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Result\tName");
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Archive Files|*.d2";
                dlg.Multiselect = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_filesToScan = dlg.FileNames.Length;
                    foreach (string file in dlg.FileNames)
                    {
                        sb.AppendLine($"{TestFile(file)}\t{file}");
                        txtResults.Text = sb.ToString();
                        if (m_quit)
                            return;
                        lblProgress.Text = $"Completed {m_filesScanned} of {m_filesToScan} files. Current File: 100%";
                        Application.DoEvents();
                        m_filesScanned++;
                    }
                }
            }

            lblProgress.Text = "Done";
        }

        private unsafe string TestFile(string fileName)
        {
            ShortTime lastReport = ShortTime.Now;
            int errorBlocks = 0;
            uint firstBlockWithError = uint.MaxValue;
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int blockSize = FileHeaderBlock.SearchForBlockSize(stream);
                    stream.Position = 0;
                    byte[] data = new byte[blockSize];
                    stream.ReadAll(data, 0, blockSize);
                    FileHeaderBlock header = FileHeaderBlock.Open(data);

                    fixed (byte* lp = data)
                    {
                        stream.Position = header.HeaderBlockCount * blockSize;

                        uint startingBlock = header.HeaderBlockCount;
                        if (m_shouldQuickScan)
                        {
                            uint blocksToScan = (uint)Math.Ceiling(m_mbToScan * 1024 * 1024 / blockSize);
                            if (blocksToScan < header.LastAllocatedBlock)
                            {
                                startingBlock = Math.Max(header.HeaderBlockCount, header.LastAllocatedBlock - blocksToScan);
                            }
                        }

                        for (uint x = startingBlock; x <= header.LastAllocatedBlock; x++)
                        {
                            stream.ReadAll(data, 0, blockSize);

                            Footer.ComputeChecksum((IntPtr)lp, out long checksum1, out int checksum2, blockSize - 16);

                            long checksumInData1 = *(long*)(lp + blockSize - 16);
                            int checksumInData2 = *(int*)(lp + blockSize - 8);
                            if (!(checksum1 == checksumInData1 && checksum2 == checksumInData2))
                            {
                                firstBlockWithError = Math.Min(firstBlockWithError, x);
                                errorBlocks++;
                            }
                            if (m_quit)
                            {
                                if (errorBlocks == 0)
                                {
                                    return "Quit Early - No Errors Found";
                                }
                                return $"Quit Early - Blocks With Errors {errorBlocks} Starting At {firstBlockWithError}";
                            }
                            if (lastReport.ElapsedSeconds() > .25)
                            {
                                lastReport = ShortTime.Now;
                                lblProgress.Text = $"Completed {m_filesScanned} of {m_filesToScan} files. Current File: {(stream.Position / (double)stream.Length).ToString("0.0%")}";
                                Application.DoEvents();
                            }
                        }
                    }
                }

                if (errorBlocks == 0)
                {
                    return "No Errors Found";
                }
                return $"Blocks With Errors {errorBlocks} Starting At {firstBlockWithError}";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return "Error";
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_quit = true;
        }

        private void chkQuickScan_CheckedChanged(object sender, EventArgs e)
        {
            lblMBToScan.Visible = chkQuickScan.Checked;
            txtMBToScan.Visible = chkQuickScan.Checked;
        }

        private void txtResults_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                if (sender != null)
                    ((TextBox)sender).SelectAll();
                e.Handled = true;
            }
        }
    }
}
