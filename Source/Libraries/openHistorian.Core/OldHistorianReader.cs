//******************************************************************************************************
//  OldHistorianReader.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  11/24/2014 - J. Ritchie Carroll
//       Updated to include applying quality flags and removed unused code.
//  11/28/2014 - J. Ritchie Carroll
//       Refactored, commented and cleaned-up class.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GSF;

namespace openHistorian
{
    /// <summary>
    /// Version 1.0 openHistorian optimized file reader.
    /// </summary>
    public class OldHistorianReader : IDisposable
    {
        #region [ Members ]

        // Nested Types        
        private class TimeTag
        {
            static DateTime Jan11995 = DateTime.Parse("01/01/1995");

            public static DateTime Convert(double timestamp)
            {
                return Jan11995.AddSeconds(timestamp);
            }

            public static DateTime Convert(long timestamp)
            {
                return Jan11995.AddTicks(timestamp * 0x2710L);
            }
        }

        private struct DataBlock
        {
            public int BlockID;
            public DateTime Timestamp;
        }

        /// <summary>
        /// openHistorian 1.0 Data Point.
        /// </summary>
        public struct DataPoint
        {
            public int PointID;
            public DateTime Timestamp;
            public float Value;
            public int Flags;
        }

        // Fields
        private FileStream m_fileStream;
        private DateTime m_startTime;
        private DateTime m_endTime;
        private int m_pointsReceived;
        private int m_pointsArchived;
        private int m_dataBlockSize;
        private int m_dataBlockCount;
        private List<DataBlock> m_dataBlocks;
        private byte[] m_buffer;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="OldHistorianReader"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~OldHistorianReader()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets start time of the data in archive as serialized in the header data.
        /// </summary>
        private DateTime StartTime => m_startTime;

        /// <summary>
        /// Gets the end time of the data in the archive as serialized in the header data.
        /// </summary>
        private DateTime EndTime => m_endTime;

        /// <summary>
        /// Gets points received by archive as serialized in header data.
        /// </summary>
        public int PointsReceived => m_pointsReceived;

        /// <summary>
        /// Gets points received by archive as serialized in header data.
        /// </summary>
        public int PointsArchived => m_pointsArchived;

        /// <summary>
        /// Gets data-block size as serialized in header data.
        /// </summary>
        public int DataBlockSize => m_dataBlockSize;

        /// <summary>
        /// Gets data-block count as serialized in header data.
        /// </summary>
        public int DataBlockCount => m_dataBlockCount;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="OldHistorianReader"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="OldHistorianReader"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (m_fileStream != null)
                        {
                            m_fileStream.Dispose();
                            m_fileStream = null;
                        }
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Opens the historian archive file.
        /// </summary>
        /// <param name="fileName">File name of historian archive to open.</param>
        public void Open(string fileName)
        {
            m_fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, FileOptions.SequentialScan);

            int footerPosition = (int)m_fileStream.Length - 32;
            m_fileStream.Position = footerPosition;

            using (BinaryReader reader = new BinaryReader(m_fileStream, Encoding.Default, true))
            {
                m_startTime = TimeTag.Convert(reader.ReadDouble());
                m_endTime = TimeTag.Convert(reader.ReadDouble());
                m_pointsReceived = reader.ReadInt32();
                m_pointsArchived = reader.ReadInt32();
                m_dataBlockSize = reader.ReadInt32();
                m_dataBlockCount = reader.ReadInt32();

                int fatPosition = footerPosition - 10 - 12 * m_dataBlockCount;
                m_fileStream.Position = fatPosition;

                m_dataBlocks = new List<DataBlock>(m_dataBlockCount);

                // Scan through header bytes
                reader.ReadBytes(10);

                DataBlock block = default;

                for (int x = 1; x <= m_dataBlockCount; x++)
                {
                    block.BlockID = reader.ReadInt32();
                    block.Timestamp = TimeTag.Convert(reader.ReadDouble());
                    m_dataBlocks.Add(block);
                }

                m_fileStream.Position = 0;
                m_buffer = new byte[m_dataBlockSize * 1024];
            }
        }

        /// <summary>
        /// Reads points from openHistorian 1.0 archive file in native order.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataPoint> Read()
        {
            DataPoint point = default;

            foreach (DataBlock block in m_dataBlocks)
            {
                m_fileStream.Read(m_buffer, 0, m_dataBlockSize * 1024);

                int position = 0;

                while (position < m_dataBlockSize * 1024 - 9)
                {
                    int baseTime = LittleEndian.ToInt32(m_buffer, position);
                    short flags = LittleEndian.ToInt16(m_buffer, position + 4);
                    float value = LittleEndian.ToSingle(m_buffer, position + 6);

                    position += 10;

                    long fullTimestamp = baseTime * 1000L + (flags >> 5);

                    if (fullTimestamp != 0)
                    {
                        point.Timestamp = TimeTag.Convert(fullTimestamp);
                        point.Value = value;
                        point.PointID = block.BlockID;
                        point.Flags = flags & 0x1F;

                        yield return point;
                    }
                }
            }
        }

        #endregion

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