//******************************************************************************************************
//  SimplifiedSubFileStreamIoSession.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  10/17/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using GSF.IO.FileStructure.Media;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// An IoSession for a Simplified Sub File Stream
    /// </summary>
    internal unsafe class SimplifiedSubFileStreamIoSession
        : BinaryStreamIoSessionBase
    {
        public static int ReadBlockCount;
        public static int WriteBlockCount;

        #region [ Members ]
        private readonly FileStream m_stream;
        private bool m_disposed;

        private readonly byte[] m_buffer;
        private readonly Memory m_memory;
        private long m_currentPhysicalBlock;
        private uint m_currentBlockIndex;
        private readonly int m_blockSize;
        private readonly FileHeaderBlock m_header;
        private readonly SubFileHeader m_subFile;
        private readonly int m_blockDataLength;

        #endregion

        #region [ Constructors ]

        public SimplifiedSubFileStreamIoSession(FileStream stream, SubFileHeader subFile, FileHeaderBlock header)
        {
            if (stream is null)
                throw new ArgumentNullException("stream");
            if (subFile is null)
                throw new ArgumentNullException("subFile");
            if (header is null)
                throw new ArgumentNullException("header");
            if (subFile.DirectBlock == 0)
                throw new Exception("Must assign subFile.DirectBlock");

            m_stream = stream;
            m_header = header;
            m_blockSize = header.BlockSize;
            m_subFile = subFile;
            m_memory = new Memory(m_blockSize);
            m_buffer = new byte[m_blockSize];
            m_currentPhysicalBlock = -1;
            m_blockDataLength = m_blockSize - FileStructureConstants.BlockFooterLength;
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SimplifiedSubFileStreamIoSession"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        Flush();
                        m_memory.Dispose();
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Sets the current usage of the <see cref="BinaryStreamIoSessionBase"/> to null.
        /// </summary>
        public override void Clear()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            Flush();
        }

        public override void GetBlock(BlockArguments args)
        {
            if (args is null)
                throw new ArgumentNullException("args");

            long pos = args.Position;
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (pos < 0)
                throw new ArgumentOutOfRangeException("position", "cannot be negative");
            if (pos >= m_blockDataLength * (uint.MaxValue - 1))
                throw new ArgumentOutOfRangeException("position", "position reaches past the end of the file.");

            uint physicalBlockIndex;
            uint indexPosition;

            if (pos <= uint.MaxValue) //64-bit divide is 2 times slower
                indexPosition = (uint)pos / (uint)m_blockDataLength;
            else
                indexPosition = (uint)((ulong)pos / (ulong)m_blockDataLength); //64-bit signed divide is twice as slow as 64-bit unsigned.

            args.FirstPosition = indexPosition * m_blockDataLength;
            args.Length = m_blockDataLength;

            physicalBlockIndex = m_subFile.DirectBlock + indexPosition;

            Read(physicalBlockIndex, indexPosition);

            args.FirstPointer = m_memory.Address;
            args.SupportsWriting = true;
        }

        private void Flush()
        {
            if (m_currentPhysicalBlock < 0)
                return;
            WriteBlockCount++;

            byte* data = (byte*)m_memory.Address + m_blockSize - 32;

            data[0] = (byte)BlockType.DataBlock;
            data[1] = 0;
            *(ushort*)(data + 2) = m_subFile.FileIdNumber;
            *(uint*)(data + 4) = m_currentBlockIndex;
            *(uint*)(data + 8) = m_header.SnapshotSequenceNumber;
            data[32 - 4] = Footer.ChecksumMustBeRecomputed;

            Footer.ComputeChecksumAndClearFooter(m_memory.Address, m_blockSize, m_blockSize);
            Marshal.Copy(m_memory.Address, m_buffer, 0, m_blockSize);
            m_stream.Position = m_currentPhysicalBlock * m_blockSize;
            m_stream.Write(m_buffer, 0, m_blockSize);
            m_currentPhysicalBlock = -1;
        }

        private void Read(uint physicalBlockIndex, uint blockIndex)
        {
            if (m_currentPhysicalBlock == physicalBlockIndex)
                return;

            Flush();

            if (blockIndex >= m_subFile.DataBlockCount)
            {
                uint additionalBlocks = blockIndex - m_subFile.DataBlockCount + 1;
                Memory.Clear(m_memory.Address, m_blockSize);

                m_header.AllocateFreeBlocks(additionalBlocks);
                m_subFile.DataBlockCount += additionalBlocks;
            }
            else
            {
                ReadBlockCount++;
                m_stream.Position = physicalBlockIndex * m_blockSize;
                int bytesRead = m_stream.Read(m_buffer, 0, m_blockSize);
                if (bytesRead < m_buffer.Length)
                    Array.Clear(m_buffer, bytesRead, m_buffer.Length - bytesRead);
                Marshal.Copy(m_buffer, 0, m_memory.Address, m_buffer.Length);
            }

            //I don't need to verify any of the footer data since I've written it all correctly.
            m_currentPhysicalBlock = physicalBlockIndex;
            m_currentBlockIndex = blockIndex;
        }


        #endregion
    }
}