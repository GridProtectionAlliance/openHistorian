using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Historian;
using System.Diagnostics;

namespace FileArchitecture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Historian2 Hist;
        //int PointCount;
        private void button1_Click(object sender, EventArgs e)
        {
        //    PointCount = 0;
        //    Hist = new Historian2();
        //    Stopwatch SW = new Stopwatch();
        //    HistorianReader H = new HistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
        //    H.NewPoint+=new HistorianReader.NewPointEventHandler(H_NewPoint);
        //    SW.Start();
        //    H.Read();
        //    int Size;
        //    Hist.ArchiveLiveData(out Size);
        //    SW.Stop();
        //    MessageBox.Show(SW.ElapsedMilliseconds.ToString() + " " + (PointCount/SW.Elapsed.TotalSeconds).ToString() + " Size:" + (Size/1024.0/1024.0).ToString());

        }
        //private void H_NewPoint(Points pt)
        //{
        //    PointCount++;
        //    Hist.QueuePointToArchive(pt);
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder SB = new StringBuilder();
            
            SB.AppendLine(new TimeSpan(1L << 8).TotalMilliseconds.ToString() + "ms");
            SB.AppendLine(new TimeSpan(1L << 16).TotalMilliseconds.ToString() + "ms");
            SB.AppendLine(new TimeSpan(1L << 24).TotalSeconds.ToString() + "s");
            SB.AppendLine(new TimeSpan(1L << 32).TotalMinutes.ToString() + "min");
            SB.AppendLine(new TimeSpan(1L << 40).TotalHours.ToString() + "hr");
            SB.AppendLine(new TimeSpan(1L << 48).TotalDays.ToString() + "days");
            SB.AppendLine(new TimeSpan(1L << 56).TotalDays.ToString() + "days");
            MessageBox.Show(SB.ToString());
        }


    }
}
