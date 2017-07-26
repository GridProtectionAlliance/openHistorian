using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF;
using GSF.IO;
using GSF.IO.FileStructure;
using GSF.IO.FileStructure.Media;

namespace Archive_Integrity_Checker
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private volatile bool m_quit;

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            m_quit = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Result\tName");
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Archive Files|*.d2";
                dlg.Multiselect = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in dlg.FileNames)
                    {
                        sb.AppendLine($"{TestFile(file)}\t{file}");
                        txtResults.Text = sb.ToString();
                        if (m_quit)
                            return;
                    }
                }

            }
        }

        private unsafe string TestFile(string fileName)
        {
            ShortTime lastReport = ShortTime.Now;
            int errorBlocks = 0;
            uint firstBlockWithError = uint.MaxValue;
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int blockSize = FileHeaderBlock.SearchForBlockSize(stream);
                    stream.Position = 0;
                    byte[] data = new byte[blockSize];
                    stream.ReadAll(data, 0, blockSize);
                    var header = FileHeaderBlock.Open(data);

                    fixed (byte* lp = data)
                    {
                        stream.Position = header.HeaderBlockCount * blockSize;

                        for (uint x = header.HeaderBlockCount; x <= header.LastAllocatedBlock; x++)
                        {
                            long checksum1;
                            int checksum2;

                            stream.ReadAll(data, 0, blockSize);

                            Footer.ComputeChecksum((IntPtr)lp, out checksum1, out checksum2, blockSize - 16);

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
                                lblProgress.Text = (stream.Position / (double)stream.Length).ToString("0.0%");
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
    }
}
