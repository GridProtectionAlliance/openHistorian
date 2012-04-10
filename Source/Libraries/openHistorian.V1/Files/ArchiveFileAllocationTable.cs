//******************************************************************************************************
//  ArchiveFileAllocationTable.cs - Gbtc
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
//  02/18/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  01/23/2008 - Pinal C. Patel
//       Added thread safety to all FindDataBlock() methods.
//       Recoded RequestDataBlock() method to include the logic to use previously used partially filled 
//       data blocks first.
//  03/31/2008 - Pinal C. Patel
//       Removed intervaled persisting of FAT since FAT is persisted when new block is requested.
//       Recoded RequestDataBlock() method to speed up the block request process based on the block index 
//       suggestion provided from the state information of the point.
//  07/14/2008 - Pinal C. Patel
//       Added overload to GetDataBlock() method that takes a block index.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/23/2009 - Pinal C. Patel
//       Edited code comments.
//  12/03/2009 - Pinal C. Patel
//       Modified FindDataBlocks() to accurately find matching data blocks even when the specified 
//       search timespan is a sub-second range.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TVA;
using TVA.Interop;
using TVA.Parsing;

namespace openHistorian.V1.Files
{
    /// <summary>
    /// Represents the File Allocation Table of an <see cref="ArchiveFile"/>.
    /// </summary>
    /// <seealso cref="ArchiveFile"/>.
    /// <seealso cref="ArchiveDataBlock"/>
    /// <seealso cref="ArchiveDataBlockPointer"/>
    public class ArchiveFileAllocationTable : ISupportBinaryImage
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// Defines the fixed table region of the <see cref="ArchiveFileAllocationTable"/>.
        /// </summary>
        private class FixedTableRegion : ISupportBinaryImage
        {
            #region [ Members ]

            // Constants

            /// <summary>
            /// Specifies the number of bytes in the binary image of the <see cref="FixedTableRegion"/>.
            /// </summary>
            public const int FixedLength = 32;

            // Fields
            private ArchiveFileAllocationTable m_parent;

            #endregion

            #region [ Constructors ]

            /// <summary>
            /// Creates a new instance of the <see cref="FixedTableRegion"/>.
            /// </summary>
            /// <param name="parent">Reference to parent <see cref="ArchiveFileAllocationTable"/>.</param>
            public FixedTableRegion(ArchiveFileAllocationTable parent)
            {
                m_parent = parent;
            }

            #endregion

            #region [ Properties ]

            /// <summary>
            /// Gets the length of the <see cref="FixedTableRegion"/>.
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
            /// Initializes <see cref="FixedTableRegion"/> by parsing the specified <paramref name="buffer"/> containing a binary image.
            /// </summary>
            /// <param name="buffer">Buffer containing binary image to parse.</param>
            /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start parsing.</param>
            /// <param name="length">Valid number of bytes within <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
            /// <returns>The number of bytes used for initialization in the <paramref name="buffer"/> (i.e., the number of bytes parsed).</returns>
            /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0 -or- 
            /// <paramref name="startIndex"/> and <paramref name="length"/> will exceed <paramref name="buffer"/> length.
            /// </exception>
            public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
            {
                buffer.ValidateParameters(startIndex, length);

                double startTime = EndianOrder.LittleEndian.ToDouble(buffer, startIndex);
                double stopTime = EndianOrder.LittleEndian.ToDouble(buffer, startIndex + 8);

                // Validate file time tags
                if (startTime < TimeTag.MinValue.Value || startTime > TimeTag.MaxValue.Value)
                    startTime = TimeTag.MinValue.Value;

                if (stopTime < TimeTag.MinValue.Value || stopTime > TimeTag.MaxValue.Value)
                    stopTime = TimeTag.MinValue.Value;

                m_parent.FileStartTime = new TimeTag(startTime);
                m_parent.FileEndTime = new TimeTag(stopTime);
                m_parent.DataPointsReceived = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 16);
                m_parent.DataPointsArchived = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 20);
                m_parent.DataBlockSize = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 24);
                m_parent.DataBlockCount = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 28);

                return FixedLength;
            }

            /// <summary>
            /// Generates binary image of the <see cref="FixedTableRegion"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
            /// </summary>
            /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
            /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
            /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
            /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
            /// </exception>
            public int GenerateBinaryImage(byte[] buffer, int startIndex)
            {
                int length = BinaryLength;

                buffer.ValidateParameters(startIndex, length);

                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_parent.m_fileStartTime.Value), 0, buffer, startIndex, 8);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_parent.m_fileEndTime.Value), 0, buffer, startIndex + 8, 8);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_parent.m_dataPointsReceived), 0, buffer, startIndex + 16, 4);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_parent.m_dataPointsArchived), 0, buffer, startIndex + 20, 4);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_parent.m_dataBlockSize), 0, buffer, startIndex + 24, 4);
                Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_parent.m_dataBlockCount), 0, buffer, startIndex + 28, 4);

                return length;
            }

            #endregion
        }

        /// <summary>
        /// Defines the variable table region of the <see cref="ArchiveFileAllocationTable"/>.
        /// </summary>
        private class VariableTableRegion : ISupportBinaryImage
        {
            #region [ Members ]

            // Fields
            private ArchiveFileAllocationTable m_parent;

            #endregion

            #region [ Constructors ]

            /// <summary>
            /// Creates a new instance of the <see cref="VariableTableRegion"/>.
            /// </summary>
            /// <param name="parent">Reference to parent <see cref="ArchiveFileAllocationTable"/>.</param>
            public VariableTableRegion(ArchiveFileAllocationTable parent)
            {
                m_parent = parent;
            }

            #endregion

            #region [ Properties ]

            /// <summary>
            /// Gets the length of the <see cref="VariableTableRegion"/>.
            /// </summary>
            public int BinaryLength
            {
                get
                {
                    // We add the extra bytes for the array descriptor that are required for reading the file from a VB6 style application.
                    return (ArrayDescriptorLength + (m_parent.m_dataBlockCount * ArchiveDataBlockPointer.FixedLength));
                }
            }

            #endregion

            #region [ Methods ]

            /// <summary>
            /// Initializes <see cref="VariableTableRegion"/> by parsing the specified <paramref name="buffer"/> containing a binary image.
            /// </summary>
            /// <param name="buffer">Buffer containing binary image to parse.</param>
            /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start parsing.</param>
            /// <param name="length">Valid number of bytes within <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
            /// <returns>The number of bytes used for initialization in the <paramref name="buffer"/> (i.e., the number of bytes parsed).</returns>
            /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0 -or- 
            /// <paramref name="startIndex"/> and <paramref name="length"/> will exceed <paramref name="buffer"/> length.
            /// </exception>
            public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
            {
                buffer.ValidateParameters(startIndex, length);

                ArchiveDataBlockPointer blockPointer;

                // Set parsing cursor beyond old-style visual basic array descriptor
                int index = startIndex + ArrayDescriptorLength;

                for (int i = 0; i < m_parent.m_dataBlockCount; i++)
                {
                    blockPointer = new ArchiveDataBlockPointer(m_parent.m_parent, i);
                    index += blockPointer.ParseBinaryImage(buffer, index, length - (index - startIndex));
                    m_parent.m_dataBlockPointers.Add(blockPointer);
                }

                return (index - startIndex);
            }

            /// <summary>
            /// Generates binary image of the <see cref="VariableTableRegion"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
            /// </summary>
            /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
            /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
            /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
            /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
            /// </exception>
            public int GenerateBinaryImage(byte[] buffer, int startIndex)
            {
                int length = BinaryLength;

                buffer.ValidateParameters(startIndex, length);

                VBArrayDescriptor arrayDescriptor = VBArrayDescriptor.OneBasedOneDimensionalArray(m_parent.m_dataBlockCount);

                startIndex += arrayDescriptor.GenerateBinaryImage(buffer, startIndex);

                lock (m_parent.m_dataBlockPointers)
                {
                    for (int i = 0; i < m_parent.m_dataBlockPointers.Count; i++)
                    {
                        startIndex += m_parent.m_dataBlockPointers[i].GenerateBinaryImage(buffer, startIndex);
                    }
                }

                return length;
            }

            #endregion
        }

        // Constants
        private const int ArrayDescriptorLength = 10;

        // Fields
        private TimeTag m_fileStartTime;
        private TimeTag m_fileEndTime;
        private int m_dataPointsReceived;
        private int m_dataPointsArchived;
        private int m_dataBlockSize;
        private int m_dataBlockCount;
        private List<ArchiveDataBlockPointer> m_dataBlockPointers;
        private ArchiveFile m_parent;
        private FixedTableRegion m_fixedTableRegion;
        private VariableTableRegion m_variableTableRegion;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileAllocationTable"/> class.
        /// </summary>
        /// <param name="parent">An <see cref="ArchiveFile"/> object.</param>
        internal ArchiveFileAllocationTable(ArchiveFile parent)
        {
            m_parent = parent;
            m_dataBlockPointers = new List<ArchiveDataBlockPointer>();
            m_fixedTableRegion = new FixedTableRegion(this);
            m_variableTableRegion = new VariableTableRegion(this);

            if (m_parent.FileData.Length == 0)
            {
                // File is brand new.
                m_fileStartTime = TimeTag.MinValue;
                m_fileEndTime = TimeTag.MinValue;
                m_dataBlockSize = m_parent.DataBlockSize;
                m_dataBlockCount = ArchiveFile.MaximumDataBlocks(m_parent.FileSize, m_parent.DataBlockSize);

                for (int i = 0; i < m_dataBlockCount; i++)
                {
                    m_dataBlockPointers.Add(new ArchiveDataBlockPointer(m_parent, i));
                }
            }
            else
            {
                // Existing file, read table regions:

                // Seek to beginning of fixed table region
                m_parent.FileData.Seek(-m_fixedTableRegion.BinaryLength, SeekOrigin.End);

                // Parse fixed table region
                m_fixedTableRegion.ParseBinaryImageFromStream(m_parent.FileData);

                // Seek to beginning of variable table region (above fixed from bottom of file)
                m_parent.FileData.Seek(-(m_fixedTableRegion.BinaryLength + m_variableTableRegion.BinaryLength), SeekOrigin.End);

                // Parse variable table region
                m_variableTableRegion.ParseBinaryImageFromStream(m_parent.FileData);
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of the oldest <see cref="ArchiveDataBlock"/> in the <see cref="ArchiveFile"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 01/01/1995 and 01/19/2063.</exception>
        public TimeTag FileStartTime
        {
            get
            {
                return m_fileStartTime;
            }
            set
            {
                if (value < TimeTag.MinValue || value > TimeTag.MaxValue)
                    throw new ArgumentException("Value must between 01/01/1995 and 01/19/2063");

                m_fileStartTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of the newest <see cref="ArchiveDataBlock"/> in the <see cref="ArchiveFile"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 01/01/1995 and 01/19/2063.</exception>
        public TimeTag FileEndTime
        {
            get
            {
                return m_fileEndTime;
            }
            set
            {
                if (value < TimeTag.MinValue || value > TimeTag.MaxValue)
                    throw new ArgumentException("Value must between 01/01/1995 and 01/19/2063");

                m_fileEndTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the number <see cref="ArchiveDataPoint"/>s received by the <see cref="ArchiveFile"/> for archival.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or zero.</exception>
        public int DataPointsReceived
        {
            get
            {
                return m_dataPointsReceived;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be positive or zero");

                m_dataPointsReceived = value;
            }
        }

        /// <summary>
        /// Gets or sets the number <see cref="ArchiveDataPoint"/>s archived by the <see cref="ArchiveFile"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or zero.</exception>
        public int DataPointsArchived
        {
            get
            {
                return m_dataPointsArchived;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be positive or zero");

                m_dataPointsArchived = value;
            }
        }

        /// <summary>
        /// Gets the size (in KB) of a single <see cref="ArchiveDataBlock"/> in the <see cref="ArchiveFile"/>.
        /// </summary>
        public int DataBlockSize
        {
            get
            {
                return m_dataBlockSize;
            }
            private set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be positive");

                m_dataBlockSize = value;
            }
        }

        /// <summary>
        /// Gets the total number of <see cref="ArchiveDataBlock"/>s in the <see cref="ArchiveFile"/>.
        /// </summary>
        public int DataBlockCount
        {
            get
            {
                return m_dataBlockCount;
            }
            private set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be positive");

                m_dataBlockCount = value;
            }
        }

        /// <summary>
        /// Gets the number of used <see cref="ArchiveDataBlock"/>s in the <see cref="ArchiveFile"/>.
        /// </summary>
        public int DataBlocksUsed
        {
            get
            {
                return m_dataBlockCount - DataBlocksAvailable;
            }
        }

        /// <summary>
        /// Gets the number of unused <see cref="ArchiveDataBlock"/>s in the <see cref="ArchiveFile"/>.
        /// </summary>
        public int DataBlocksAvailable
        {
            get
            {
                ArchiveDataBlock unusedDataBlock = FindDataBlock(-1);

                if (unusedDataBlock != null)
                    return m_dataBlockCount - unusedDataBlock.Index;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="ArchiveDataBlockPointer"/>s to the <see cref="ArchiveDataBlock"/>s in the <see cref="ArchiveFile"/>.
        /// </summary>
        /// <remarks>
        /// WARNING: <see cref="DataBlockPointers"/> is not thread safe. Synchronized access is required.
        /// </remarks>
        public IList<ArchiveDataBlockPointer> DataBlockPointers
        {
            get
            {
                return m_dataBlockPointers.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="ArchiveFileAllocationTable"/>.
        /// </summary>
        public int BinaryLength
        {
            get
            {
                return m_variableTableRegion.BinaryLength + m_fixedTableRegion.BinaryLength;
            }
        }

        private long DataLength
        {
            get
            {
                return (m_dataBlockCount * (m_dataBlockSize * 1024));
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Generates binary image of the <see cref="ArchiveFileAllocationTable"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        public int GenerateBinaryImage(byte[] buffer, int startIndex)
        {
            int length = BinaryLength;

            buffer.ValidateParameters(startIndex, length);

            startIndex += m_variableTableRegion.GenerateBinaryImage(buffer, startIndex);
            startIndex += m_fixedTableRegion.GenerateBinaryImage(buffer, startIndex);

            return length;
        }

        // This method is never used since constructor parses the point allocation table
        int ISupportBinaryImage.ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Saves the <see cref="ArchiveFileAllocationTable"/> data to the <see cref="ArchiveFile"/>.
        /// </summary>
        public void Save()
        {
            // Leave space for data blocks.
            lock (m_parent.FileData)
            {
                m_parent.FileData.Seek(DataLength, SeekOrigin.Begin);
                this.CopyBinaryImageToStream(m_parent.FileData);

                if (!m_parent.CacheWrites)
                    m_parent.FileData.Flush();
            }
        }

        /// <summary>
        /// Extends the <see cref="ArchiveFile"/> by one <see cref="ArchiveDataBlock"/>.
        /// </summary>
        public void Extend()
        {
            Extend(1);
        }

        /// <summary>
        /// Extends the <see cref="ArchiveFile"/> by the specified number of <see cref="ArchiveDataBlock"/>s.
        /// </summary>
        /// <param name="dataBlocksToAdd">Number of <see cref="ArchiveDataBlock"/>s to add to the <see cref="ArchiveFile"/>.</param>
        public void Extend(int dataBlocksToAdd)
        {
            // Extend the FAT.
            lock (m_dataBlockPointers)
            {
                for (int i = 1; i <= dataBlocksToAdd; i++)
                {
                    m_dataBlockPointers.Add(new ArchiveDataBlockPointer(m_parent, m_dataBlockPointers.Count));

                }
                m_dataBlockCount = m_dataBlockPointers.Count;
            }
            Save();

            // Initialize newly added data blocks.
            ArchiveDataBlock dataBlock;

            for (int i = m_dataBlockCount - dataBlocksToAdd; i < m_dataBlockCount; i++)
            {
                dataBlock = new ArchiveDataBlock(m_parent, i, -1, true);
            }
        }

        /// <summary>
        /// Returns the first <see cref="ArchiveDataBlock"/> in the <see cref="ArchiveFile"/> for the specified <paramref name="historianID"/>.
        /// </summary>
        /// <param name="historianID">Historian identifier whose <see cref="ArchiveDataBlock"/> is to be retrieved.</param>
        /// <param name="preRead">true to pre-read data to locate write cursor.</param>
        /// <returns><see cref="ArchiveDataBlock"/> object if a match is found; otherwise null.</returns>
        public ArchiveDataBlock FindDataBlock(int historianID, bool preRead = true)
        {
            ArchiveDataBlockPointer pointer;

            lock (m_dataBlockPointers)
            {
                pointer = m_dataBlockPointers.FirstOrDefault(dataBlockPointer => dataBlockPointer.HistorianID == historianID);
            }

            if (pointer == null)
                return null;
            else
                return pointer.GetDataBlock(preRead);
        }

        /// <summary>
        /// Returns the last <see cref="ArchiveDataBlock"/> in the <see cref="ArchiveFile"/> for the specified <paramref name="historianID"/>.
        /// </summary>
        /// <param name="historianID">Historian identifier.</param>
        /// <param name="preRead">true to pre-read data to locate write cursor.</param>
        /// <returns><see cref="ArchiveDataBlock"/> object if a match is found; otherwise null.</returns>
        public ArchiveDataBlock FindLastDataBlock(int historianID, bool preRead = true)
        {
            ArchiveDataBlockPointer pointer;

            lock (m_dataBlockPointers)
            {
                pointer = m_dataBlockPointers.LastOrDefault(dataBlockPointer => dataBlockPointer.HistorianID == historianID);
            }

            if (pointer == null)
                return null;
            else
                return pointer.GetDataBlock(preRead);
        }

        /// <summary>
        /// Returns all <see cref="ArchiveDataBlock"/>s in the <see cref="ArchiveFile"/> for the specified <paramref name="historianID"/>.
        /// </summary>
        /// <param name="historianID">Historian identifier.</param>
        /// <param name="preRead">true to pre-read data to locate write cursor.</param>
        /// <returns>A collection of <see cref="ArchiveDataBlock"/>s.</returns>
        public List<ArchiveDataBlock> FindDataBlocks(int historianID, bool preRead = true)
        {
            return FindDataBlocks(historianID, TimeTag.MinValue, preRead);
        }

        /// <summary>
        /// Returns all <see cref="ArchiveDataBlock"/>s in the <see cref="ArchiveFile"/> for the specified <paramref name="historianID"/> with <see cref="ArchiveDataPoint"/> points later than the specified <paramref name="startTime"/>.
        /// </summary>
        /// <param name="historianID">Historian identifier.</param>
        /// <param name="startTime">Start <see cref="TimeTag"/>.</param>
        /// <param name="preRead">true to pre-read data to locate write cursor.</param>
        /// <returns>A collection of <see cref="ArchiveDataBlock"/>s.</returns>
        public List<ArchiveDataBlock> FindDataBlocks(int historianID, TimeTag startTime, bool preRead = true)
        {
            return FindDataBlocks(historianID, startTime, TimeTag.MaxValue, preRead);
        }

        /// <summary>
        /// Returns all <see cref="ArchiveDataBlock"/>s in the <see cref="ArchiveFile"/> for the specified <paramref name="historianID"/> with <see cref="ArchiveDataPoint"/>s between the specified <paramref name="startTime"/> and <paramref name="endTime"/>.
        /// </summary>
        /// <param name="historianID">Historian identifier.</param>
        /// <param name="startTime">Start <see cref="TimeTag"/>.</param>
        /// <param name="endTime">End <see cref="TimeTag"/>.</param>
        /// <param name="preRead">true to pre-read data to locate write cursor.</param>
        /// <returns>A collection of <see cref="ArchiveDataBlock"/>s.</returns>
        public List<ArchiveDataBlock> FindDataBlocks(int historianID, TimeTag startTime, TimeTag endTime, bool preRead = true)
        {
            if ((object)startTime == null)
                startTime = TimeTag.MaxValue;

            if ((object)endTime == null)
                endTime = TimeTag.MaxValue;

            List<ArchiveDataBlockPointer> blockPointers;

            lock (m_dataBlockPointers)
            {
                // Get all block pointers for given point ID over specified time range
                blockPointers = m_dataBlockPointers.FindAll(dataBlockPointer => dataBlockPointer.Matches(historianID, startTime, endTime));

                // Look for pointer to data block on the borders of the specified range which may contain data
                if (!(startTime == TimeTag.MinValue || endTime == TimeTag.MaxValue))
                {
                    // There are 2 different search criteria for this:
                    // 1) If matching data block pointers have been found, then find data block pointer before the first matching data block pointer.
                    //    or
                    // 2) Find the last data block pointer in the time range of TimeTag.MinValue to m_searchEndTime.

                    TimeTag searchEndTime = endTime;

                    if (blockPointers.Count > 0)
                        searchEndTime = new TimeTag(blockPointers.First().StartTime.Value - 1.0D);

                    ArchiveDataBlockPointer borderMatch = m_dataBlockPointers.LastOrDefault(dataBlockPointer => dataBlockPointer.Matches(historianID, TimeTag.MinValue, searchEndTime));
                    if (borderMatch != null)
                        blockPointers.Insert(0, borderMatch);
                }
            }

            // Return list of data blocks for given block pointers
            return blockPointers.Select(pointer => pointer.GetDataBlock(preRead)).ToList();
        }

        /// <summary>
        /// Returns an <see cref="ArchiveDataBlock"/> for writing <see cref="ArchiveDataPoint"/>s for the specified <paramref name="historianID"/>.
        /// </summary>
        /// <param name="historianID">Historian identifier for which the <see cref="ArchiveDataBlock"/> is being requested.</param>
        /// <param name="dataTime"><see cref="TimeTag"/> of the <see cref="ArchiveDataPoint"/> to be written to the <see cref="ArchiveDataBlock"/>.</param>
        /// <param name="blockIndex"><see cref="ArchiveDataBlock.Index"/> of the <see cref="ArchiveDataBlock"/> last used for writting <see cref="ArchiveDataPoint"/>s for the <paramref name="historianID"/>.</param>
        /// <returns><see cref="ArchiveDataBlock"/> object if available; otherwise null if all <see cref="ArchiveDataBlock"/>s have been allocated.</returns>
        internal ArchiveDataBlock RequestDataBlock(int historianID, TimeTag dataTime, int blockIndex)
        {
            ArchiveDataBlock dataBlock = null;
            ArchiveDataBlockPointer dataBlockPointer = null;
            if (blockIndex >= 0 && blockIndex < m_dataBlockCount)
            {
                // Valid data block index is specified, so retrieve the corresponding data block.
                lock (m_dataBlockPointers)
                {
                    dataBlockPointer = m_dataBlockPointers[blockIndex];
                }

                dataBlock = dataBlockPointer.DataBlock;
                if (!dataBlockPointer.IsAllocated && dataBlock.SlotsUsed > 0)
                {
                    // Clear existing data from the data block since it is unallocated.
                    dataBlock.Reset();
                }
                else if (dataBlockPointer.IsAllocated &&
                         (dataBlockPointer.HistorianID != historianID ||
                          (dataBlockPointer.HistorianID == historianID && dataBlock.SlotsAvailable == 0)))
                {
                    // Search for a new data block since the suggested data block cannot be used.
                    blockIndex = -1;
                }
            }

            if (blockIndex < 0)
            {
                // Negative data block index is specified indicating a search must be performed for a data block.
                dataBlock = FindLastDataBlock(historianID);
                if (dataBlock != null && dataBlock.SlotsAvailable == 0)
                {
                    // Previously used data block is full.
                    dataBlock = null;
                }

                if (dataBlock == null)
                {
                    // Look for the first unallocated data block.
                    dataBlock = FindDataBlock(-1);
                    if (dataBlock == null)
                    {
                        // Extend the file for historic writes only.
                        if (m_parent.FileType == ArchiveFileType.Historic)
                        {
                            Extend();
                            dataBlock = m_dataBlockPointers[m_dataBlockPointers.Count - 1].DataBlock;
                        }
                    }
                    else
                    {
                        // Reset the unallocated data block if there is data in it.
                        if (dataBlock.SlotsUsed > 0)
                        {
                            dataBlock.Reset();
                        }
                    }
                }

                // Get the pointer to the data block so that its information can be updated if necessary.
                if (dataBlock == null)
                {
                    dataBlockPointer = null;
                }
                else
                {
                    lock (m_dataBlockPointers)
                    {
                        dataBlockPointer = m_dataBlockPointers[dataBlock.Index];
                    }
                }
            }

            if (dataBlockPointer != null && !dataBlockPointer.IsAllocated)
            {
                // Mark the data block as allocated.
                dataBlockPointer.HistorianID = historianID;
                dataBlockPointer.StartTime = dataTime;

                // Set the file start time if not set.
                if (m_fileStartTime == TimeTag.MinValue)
                    m_fileStartTime = dataTime;

                // Persist data block information to disk.
                lock (m_parent.FileData)
                {
                    // We'll write information about the just allocated data block to the file.
                    m_parent.FileData.Seek(DataLength + ArrayDescriptorLength + (dataBlockPointer.Index * ArchiveDataBlockPointer.FixedLength), SeekOrigin.Begin);
                    dataBlockPointer.CopyBinaryImageToStream(m_parent.FileData);

                    // We'll also write the fixed part of the FAT data that resides at the end.
                    m_parent.FileData.Seek(-m_fixedTableRegion.BinaryLength, SeekOrigin.End);

                    // Copy generated binary image to stream
                    m_fixedTableRegion.CopyBinaryImageToStream(m_parent.FileData);

                    if (!m_parent.CacheWrites)
                        m_parent.FileData.Flush();
                }

                // Re-fetch the data block with updated information after allocation.
                dataBlock = dataBlockPointer.DataBlock;
            }

            return dataBlock;
        }

        #endregion
    }
}