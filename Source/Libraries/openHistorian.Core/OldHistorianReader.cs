using System;
using System.Collections.Generic;
using GSF.Snap.Collection;
using openHistorian.Snap;

namespace openHistorian
{
    public class OldHistorianReader
    {
        public string File;

        public OldHistorianReader(string File)
        {
            this.File = File;
        }

        public unsafe void Read(Func<Points, bool> callback, out SortedPointBuffer<HistorianKey, HistorianValue> pointBuffer)
        {
            //using (MemoryStream FS = new MemoryStream(System.IO.File.ReadAllBytes(File)))
            using (System.IO.FileStream FS = new System.IO.FileStream(File, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 8192, System.IO.FileOptions.SequentialScan))
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

                pointBuffer = new SortedPointBuffer<HistorianKey, HistorianValue>(PointsArchived, true);

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
                                P.Flags = S & 0x1F;

                                if (!callback(P))
                                    break;
                            }
                        }
                        //FS.Position = NextPos;
                        NextPos += DataBlockSize * 1024;
                    }
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
            public int Flags;
        }

        class TimeTag
        {
            static DateTime Jan11995 = DateTime.Parse("01/01/1995");
            public static System.DateTime Convert(double D)
            {
                return Jan11995.AddSeconds(D);
            }
            public static System.DateTime Convert(long D)
            {
                return Jan11995.AddTicks(D * 0x2710L);
            }
        }

        #region [ Old Code ]

        //public static unsafe UInt32 ToUInt(float value)
        //{
        //    return *(UInt32*)&value;
        //}
        //public static unsafe float ToSingle(UInt32 value)
        //{
        //    return *(float*)&value;
        //}

        //public delegate void NewPointEventHandler(Points pt);

        //public event NewPointEventHandler NewPoint;

        //public unsafe void Read()
        //{
        //    //using (MemoryStream FS = new MemoryStream(System.IO.File.ReadAllBytes(File)))
        //    using (FileStream fs = new FileStream(File, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, FileOptions.SequentialScan))
        //    {
        //        int footerPos = (int)fs.Length - 32;
        //        fs.Position = footerPos;
        //        BinaryReader RD = new BinaryReader(fs);

        //        DateTime startTime = TimeTag.Convert(RD.ReadDouble());
        //        DateTime endTime = TimeTag.Convert(RD.ReadDouble());
        //        int pointsReceived = RD.ReadInt32();
        //        int pointsArchived = RD.ReadInt32();
        //        int dataBlockSize = RD.ReadInt32();
        //        int dataBlockCount = RD.ReadInt32();

        //        int fatPos = footerPos - 10 - 12 * dataBlockCount;
        //        fs.Position = fatPos;

        //        List<Blocks> blocks = new List<Blocks>(dataBlockCount);
        //        byte[] header = RD.ReadBytes(10);

        //        Blocks b = default(Blocks);
        //        for (int x = 1; x <= dataBlockCount; x++)
        //        {
        //            b.BlockID = RD.ReadInt32();
        //            b.Time = TimeTag.Convert(RD.ReadDouble());
        //            blocks.Add(b);
        //        }

        //        fs.Position = 0;
        //        Points p = default(Points);

        //        byte[] buffer = new byte[dataBlockSize * 1024];

        //        fixed (byte* lp = buffer)
        //        {
        //            foreach (Blocks bk in blocks)
        //            {
        //                fs.Read(buffer, 0, dataBlockSize * 1024);
        //                int pos = 0;
        //                while (pos < dataBlockSize * 1024 - 9)
        //                {

        //                    int i = *(int*)(lp + pos);
        //                    short s = *(short*)(lp + pos + 4);
        //                    float v = *(float*)(lp + pos + 6);
        //                    pos += 10;

        //                    long timeDiff = i * 1000L + (s >> 5);
        //                    if (timeDiff != 0)
        //                    {
        //                        p.Time = TimeTag.Convert(timeDiff);
        //                        p.Value = v;
        //                        p.PointID = bk.BlockID;

        //                        //if (NewPoint != null)
        //                        //    NewPoint(p);
        //                    }
        //                }
        //                //FS.Position = NextPos;
        //            }
        //            //return;
        //        }
        //    }
        //}

        //public unsafe void Read(Action<Points> callback)
        //{
        //    //using (MemoryStream FS = new MemoryStream(System.IO.File.ReadAllBytes(File)))
        //    using (FileStream fs = new FileStream(File, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, FileOptions.SequentialScan))
        //    {
        //        int footerPos = (int)fs.Length - 32;
        //        fs.Position = footerPos;
        //        BinaryReader RD = new BinaryReader(fs);

        //        DateTime startTime = TimeTag.Convert(RD.ReadDouble());
        //        DateTime endTime = TimeTag.Convert(RD.ReadDouble());
        //        int pointsReceived = RD.ReadInt32();
        //        int pointsArchived = RD.ReadInt32();
        //        int dataBlockSize = RD.ReadInt32();
        //        int dataBlockCount = RD.ReadInt32();

        //        int FATPos = footerPos - 10 - 12 * dataBlockCount;
        //        fs.Position = FATPos;

        //        List<Blocks> Blocks = new List<Blocks>(dataBlockCount);
        //        byte[] Header = RD.ReadBytes(10);

        //        Blocks B = default(Blocks);
        //        for (int x = 1; x <= dataBlockCount; x++)
        //        {
        //            B.BlockID = RD.ReadInt32();
        //            B.Time = TimeTag.Convert(RD.ReadDouble());
        //            Blocks.Add(B);
        //        }

        //        fs.Position = 0;
        //        Points P = default(Points);
        //        int NextPos = dataBlockSize * 1024;

        //        byte[] Buffer = new byte[dataBlockSize * 1024];

        //        fixed (byte* lp = Buffer)
        //        {
        //            foreach (Blocks BK in Blocks)
        //            {
        //                fs.Read(Buffer, 0, dataBlockSize * 1024);
        //                int pos = 0;
        //                while (pos < dataBlockSize * 1024 - 9)
        //                {

        //                    int I = *(int*)(lp + pos);
        //                    short S = *(short*)(lp + pos + 4);
        //                    float V = *(float*)(lp + pos + 6);
        //                    pos += 10;

        //                    long TimeDiff = I * 1000L + (S >> 5);
        //                    if (TimeDiff != 0)
        //                    {
        //                        P.Time = TimeTag.Convert(TimeDiff);
        //                        P.Value = V;
        //                        P.PointID = BK.BlockID;
        //                        P.Flags = S & 0x1F;
        //                        callback(P);
        //                    }
        //                }
        //                //FS.Position = NextPos;
        //                NextPos += dataBlockSize * 1024;
        //            }
        //            return;
        //        }
        //    }
        //}

        #endregion
    }
}