//******************************************************************************************************
//  BinaryStream.cs - Gbtc
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
//  4/6/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.IO.Unmanaged
{
    public unsafe class BinaryStream
        : BinaryStreamPointerBase
    {
        #region [ Members ]

        private readonly BlockArguments m_args;

        /// <summary>
        /// Determines if this class owns the underlying stream, thus when Dispose() is called
        /// the dispose of the underlying stream will also be called.
        /// </summary>
        private readonly bool m_leaveOpen;

        private bool m_disposed;

        private BinaryStreamIoSessionBase m_mainIoSession;

        private BinaryStreamIoSessionBase m_secondaryIoSession;

        private ISupportsBinaryStream m_stream;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is in memory only.
        /// </summary>
        public BinaryStream()
            : this(new MemoryPoolStream(), false)
        {
            if (!BitConverter.IsLittleEndian)
                throw new Exception("Only designed to run on a little endian processor.");
        }

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is in memory only.
        /// </summary>
        public BinaryStream(MemoryPool pool)
            : this(new MemoryPoolStream(pool), false)
        {
            if (!BitConverter.IsLittleEndian)
                throw new Exception("Only designed to run on a little endian processor.");
        }

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is in memory only.
        /// </summary>
        /// <param name="allocatesOwnMemory">true to allowcate its own memory rather than using the <see cref="MemoryPool"/>.</param>
        public BinaryStream(bool allocatesOwnMemory)
            : this(CreatePool(allocatesOwnMemory), false)
        {
            if (!BitConverter.IsLittleEndian)
                throw new Exception("Only designed to run on a little endian processor.");
        }

        private static ISupportsBinaryStream CreatePool(bool allocatesOwnMemory)
        {
            if (allocatesOwnMemory)
                return new UnmanagedMemoryStream();
            else
                return new MemoryPoolStream();
        }

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is at position 0 of the provided stream.
        /// </summary>
        /// <param name="stream">The base stream to use.</param>
        /// <param name="leaveOpen">Determines if the underlying stream will automatically be 
        /// disposed of when this class has it's dispose method called.</param>
        public BinaryStream(ISupportsBinaryStream stream, bool leaveOpen = true)
        {
            m_args = new BlockArguments();
            m_leaveOpen = leaveOpen;
            m_stream = stream;
            FirstPosition = 0;
            Current = null;
            First = null;
            LastRead = null;
            LastWrite = null;
            if (stream.RemainingSupportedIoSessions < 1)
                throw new Exception("Stream has run out of read sessions");
            m_mainIoSession = stream.CreateIoSession();
            //if (stream.RemainingSupportedIoSessions >= 1)
            //    m_secondaryIoSession = stream.CreateIoSession();
        }

        ~BinaryStream()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region [ Properties ]

        public ISupportsBinaryStream BaseStream => m_stream;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// When accessing the underlying stream, a lock is placed on the data. Calling this method clears that lock.
        /// </summary>
        public void ClearLocks()
        {
            FirstPosition = Position;
            LastPosition = FirstPosition;
            Current = null;
            First = null;
            LastRead = null;
            LastWrite = null;

            if (m_mainIoSession != null)
                m_mainIoSession.Clear();
            if (m_secondaryIoSession != null)
                m_secondaryIoSession.Clear();
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        public override void UpdateLocalBuffer(bool isWriting)
        {
            //If the block block is already looked up, skip this step.
            if (isWriting && LastWrite - Current > 0 || !isWriting && LastRead - Current > 0) //RemainingWriteLength == (m_lastWrite - m_current) //RemainingReadLength = (m_lastRead - m_current)
                return;

            long position = FirstPosition + (Current - First);
            m_args.Position = position;
            m_args.IsWriting = isWriting;
            PointerVersion++;
            m_mainIoSession.GetBlock(m_args);
            FirstPosition = m_args.FirstPosition;
            First = (byte*)m_args.FirstPointer;
            LastRead = First + m_args.Length;
            Current = First + (position - FirstPosition);
            LastPosition = FirstPosition + m_args.Length;

            if (m_args.SupportsWriting)
                LastWrite = LastRead;
            else
                LastWrite = First;
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (m_mainIoSession != null)
                        m_mainIoSession.Dispose();
                    if (m_secondaryIoSession != null)
                        m_secondaryIoSession.Dispose();
                    if (!m_leaveOpen && m_stream != null)
                        m_stream.Dispose();
                }
                finally
                {
                    FirstPosition = 0;
                    LastPosition = 0;
                    Current = null;
                    First = null;
                    LastRead = null;
                    LastWrite = null;
                    m_mainIoSession = null;
                    m_secondaryIoSession = null;
                    m_stream = null;
                    m_disposed = true;
                }
            }
            base.Dispose(disposing);
        }



        #endregion
    }
}