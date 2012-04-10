//******************************************************************************************************
//  IntercomRecord.cs - Gbtc
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
//  03/09/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using TVA;
using TVA.Parsing;

namespace openHistorian.V1.Files
{
    /// <summary>
    /// Represents a record in the <see cref="IntercomFile"/> that contains runtime information of a historian.
    /// </summary>
    /// <seealso cref="IntercomFile"/>
    public class IntercomRecord : ISupportBinaryImage
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4          0-3        Int32      DataBlocksUsed                                                *
        // * 4          4-7        Boolean    RolloverInProggress                                           *
        // * 8          8-15       Double     LatestDataTime                                                *
        // * 4          16-19      Int32      LatestDataID                                                  *
        // * 160        20-179     Double(20) SourceLatestDataTime                                                     *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="IntercomRecord"/>.
        /// </summary>
        public const int FixedLength = 180;

        // Fields
        private int m_recordID;
        private int m_dataBlocksUsed;
        private bool m_rolloverInProgress;
        private TimeTag m_latestDataTime;
        private int m_latestDataID;
        private List<TimeTag> m_sourceLatestDataTime;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="IntercomRecord"/> class.
        /// </summary>
        /// <param name="recordID">ID of the <see cref="IntercomRecord"/>.</param>
        public IntercomRecord(int recordID)
        {
            m_recordID = recordID;
            m_latestDataTime = TimeTag.MinValue;
            m_sourceLatestDataTime = new List<TimeTag>();

            for (int i = 0; i < 20; i++)
            {
                m_sourceLatestDataTime.Add(TimeTag.MinValue);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntercomRecord"/> class.
        /// </summary>
        /// <param name="recordID">ID of the <see cref="IntercomRecord"/>.</param>
        /// <param name="buffer">Binary image to be used for initializing <see cref="IntercomRecord"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public IntercomRecord(int recordID, byte[] buffer, int startIndex, int length)
            : this(recordID)
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the number of allocated <see cref="ArchiveDataBlock"/>s in the active <see cref="ArchiveFile"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or zero.</exception>
        public int DataBlocksUsed
        {
            get
            {
                lock (this)
                {
                    return m_dataBlocksUsed;
                }
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be positive or zero");

                lock (this)
                {
                    m_dataBlocksUsed = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the active <see cref="ArchiveFile"/> if being rolled-over.
        /// </summary>
        public bool RolloverInProgress
        {
            get
            {
                lock (this)
                {
                    return m_rolloverInProgress;
                }
            }
            set
            {
                lock (this)
                {
                    m_rolloverInProgress = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of latest <see cref="ArchiveDataPoint"/> received by the active <see cref="ArchiveFile"/>.
        /// </summary>
        public TimeTag LatestDataTime
        {
            get
            {
                lock (this)
                {
                    return m_latestDataTime;
                }
            }
            set
            {
                lock (this)
                {
                    m_latestDataTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the historian identifier of latest <see cref="ArchiveDataPoint"/> received by the active <see cref="ArchiveFile"/>.
        /// </summary>
        public int LatestDataID
        {
            get
            {
                lock (this)
                {
                    return m_latestDataID;
                }
            }
            set
            {
                lock (this)
                {
                    m_latestDataID = value;
                }
            }
        }

        /// <summary>
        /// Gets a list of <see cref="TimeTag"/>s of the latest <see cref="ArchiveDataPoint"/> received from each of the <see cref="MetadataRecord.SourceID"/>s.
        /// </summary>
        public IList<TimeTag> SourceLatestDataTime
        {
            get
            {
                lock (this)
                {
                    return m_sourceLatestDataTime.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="IntercomRecord"/>.
        /// </summary>
        public int BinaryLength
        {
            get
            {
                return FixedLength;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="IntercomRecord"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="IntercomRecord"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="IntercomRecord"/>.</returns>
        public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= FixedLength)
            {
                // Binary image has sufficient data.
                DataBlocksUsed = EndianOrder.LittleEndian.ToInt32(buffer, startIndex);
                RolloverInProgress = EndianOrder.LittleEndian.ToBoolean(buffer, startIndex + 4);
                LatestDataTime = new TimeTag(EndianOrder.LittleEndian.ToDouble(buffer, startIndex + 8));
                LatestDataID = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 16);

                for (int i = 0; i < m_sourceLatestDataTime.Count; i++)
                {
                    m_sourceLatestDataTime[i] = new TimeTag(EndianOrder.LittleEndian.ToDouble(buffer, startIndex + 20 + (i * 8)));
                }

                return FixedLength;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="IntercomRecord"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        public virtual int GenerateBinaryImage(byte[] buffer, int startIndex)
        {
            int length = BinaryLength;

            buffer.ValidateParameters(startIndex, length);

            lock (this)
            {
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_dataBlocksUsed), 0, buffer, startIndex, 4);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(Convert.ToInt32(m_rolloverInProgress)), 0, buffer, startIndex + 4, 4);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_latestDataTime.Value), 0, buffer, startIndex + 8, 8);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_latestDataID), 0, buffer, startIndex + 16, 4);

                for (int i = 0; i < m_sourceLatestDataTime.Count; i++)
                {
                    Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_sourceLatestDataTime[i].Value), 0, buffer, startIndex + 20 + (i * 8), 8);
                }
            }

            return length;
        }

        #endregion
    }
}
