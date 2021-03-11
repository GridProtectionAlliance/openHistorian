//******************************************************************************************************
//  SubFileStream_SimplifiedIoSession.cs - Gbtc
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
//  10/15/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.IO.FileStructure.Media;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure
{
    public partial class SubFileStream
    {
        /// <summary>
        /// An IoSession for the sub file stream.
        /// </summary>
        private unsafe class SimplifiedIoSession
            : BinaryStreamIoSessionBase
        {
            #region [ Members ]

            private readonly SubFileStream m_stream;

            private bool m_disposed;

            private readonly int m_blockDataLength;

            private DiskIoSession m_dataIoSession;


            #endregion

            #region [ Constructors ]

            public SimplifiedIoSession(SubFileStream stream)
            {
                m_stream = stream;
                m_dataIoSession = stream.m_dataReader.CreateDiskIoSession(stream.m_fileHeaderBlock, stream.m_subFile);
                m_blockDataLength = m_stream.m_blockSize - FileStructureConstants.BlockFooterLength;
            }

            #endregion

            #region [ Properties ]


            #endregion

            #region [ Methods ]

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="IoSession"/> object and optionally releases the managed resources.
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
                            if (m_dataIoSession != null)
                            {
                                m_dataIoSession.Dispose();
                                m_dataIoSession = null;
                            }

                            // This will be done only when the object is disposed by calling Dispose().
                        }
                    }
                    finally
                    {
                        m_dataIoSession = null;
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
                if (IsDisposed || m_dataIoSession.IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_dataIoSession.Clear();
            }

            public override void GetBlock(BlockArguments args)
            {
                int blockDataLength = m_blockDataLength;
                long pos = args.Position;
                if (IsDisposed || m_dataIoSession.IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (pos < 0)
                    throw new ArgumentOutOfRangeException("position", "cannot be negative");
                if (pos >= blockDataLength * (uint.MaxValue - 1))
                    throw new ArgumentOutOfRangeException("position", "position reaches past the end of the file.");

                uint physicalBlockIndex;
                uint indexPosition;

                if (pos <= uint.MaxValue) //64-bit divide is 2 times slower
                    indexPosition = (uint)pos / (uint)blockDataLength;
                else
                    indexPosition = (uint)((ulong)pos / (ulong)blockDataLength); //64-bit signed divide is twice as slow as 64-bit unsigned.

                args.FirstPosition = indexPosition * blockDataLength;
                args.Length = blockDataLength;

                if (args.IsWriting)
                {
                    throw new Exception("File is read only");
                }
                else
                {
                    //Reading
                    if (indexPosition >= m_stream.m_subFile.DataBlockCount)
                        throw new ArgumentOutOfRangeException("position", "position reaches past the end of the file.");
                    physicalBlockIndex = m_stream.m_subFile.DirectBlock + indexPosition;

                    m_dataIoSession.Read(physicalBlockIndex, BlockType.DataBlock, indexPosition);
                    args.FirstPointer = (IntPtr)m_dataIoSession.Pointer;
                    args.SupportsWriting = false;
                }
            }

            #endregion
        }
    }
}