using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace openHistorian.V2
{
    public class HistorianReader
    {
        public delegate void NewPointEventHandler(Points pt);

        public event NewPointEventHandler NewPoint;

        public string File;
        public HistorianReader(string File)
        {
            this.File = File;
        }
        public unsafe void Read()
        {
            using (MemoryStream FS = new MemoryStream(System.IO.File.ReadAllBytes(File)))
            //using (System.IO.FileStream FS = new System.IO.FileStream(File, System.IO.FileMode.Open, System.IO.FileAccess.Read,System.IO.FileShare.Read,8192,System.IO.FileOptions.SequentialScan))
            {
                int FooterPOS = (int)FS.Length - 32;
                FS.Position = FooterPOS;
                System.IO.BinaryReader RD = new System.IO.BinaryReader(FS);

                DateTime StartTime = TimeTag.Convert(RD.ReadDouble());
                DateTime EndTime = TimeTag.Convert(RD.ReadDouble());
                int PointsReceived = RD.ReadInt32();
                int PointsArchived = RD.ReadInt32();
                int DataBlockSize = RD.ReadInt32();
                int DataBlockCount = RD.ReadInt32();

                int FATPos = FooterPOS - 10 - 12 * DataBlockCount;
                FS.Position = FATPos;

                List<Blocks> Blocks = new List<Blocks>(DataBlockCount);
                byte[] Header = RD.ReadBytes(10);

                Blocks B = default(Blocks);
                for (int x = 1; x <= DataBlockCount; x++)
                {
                    B.BlockID = RD.ReadInt32();
                    B.Time = TimeTag.Convert(RD.ReadDouble());
                    Blocks.Add(B);
                }

                FS.Position = 0;
                Points P = default(Points);
                int NextPos = DataBlockSize * 1024;

                byte[] Buffer = new byte[DataBlockSize * 1024];

                fixed (byte* lp = Buffer)
                {
                    foreach (Blocks BK in Blocks)
                    {
                        FS.Read(Buffer, 0, DataBlockSize * 1024);
                        int pos = 0;
                        while (pos < DataBlockSize * 1024 - 9)
                        {

                            int I = *(int*)(lp + pos);
                            short S = *(short*)(lp + pos + 4);
                            float V = *(float*)(lp + pos + 6);
                            pos += 10;

                            long TimeDiff = I * 1000L + (S >> 5);
                            if (TimeDiff != 0)
                            {
                                P.Time = TimeTag.Convert(TimeDiff);
                                P.Value = V;
                                P.PointID = BK.BlockID;
                                if (NewPoint != null)
                                    NewPoint(P);
                            }
                        }
                        //FS.Position = NextPos;
                        NextPos += DataBlockSize * 1024;
                    }
                    return;
                }
            }
        }

        public struct Blocks
        {
            public int BlockID;
            public System.DateTime Time;
        }

        public struct Points
        {
            public int PointID;
            public System.DateTime Time;
            public float Value;
            public byte flags;
        }
    }
}