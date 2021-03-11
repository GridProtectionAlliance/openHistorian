//******************************************************************************************************
//  SimplifiedFileWriter.cs - Gbtc
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
//  10/16/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;
using GSF.Snap.Storage;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// Assists in the writing of a simplified file. This file can only be appended to 
    /// and it must be sequentially written.
    /// </summary>
    public class SimplifiedFileWriter
        : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(SimplifiedFileWriter), MessageClass.Component);

        private bool m_disposed;

        private readonly FileHeaderBlock m_fileHeaderBlock;

        private SimplifiedSubFileStream m_subFileStream;

        public FileStream m_stream;

        private readonly string m_pendingFileName;
        private readonly string m_completeFileName;

        /// <summary>
        /// Creates a simplified file writer.
        /// </summary>
        /// <param name="pendingFileName"></param>
        /// <param name="completeFileName"></param>
        /// <param name="blockSize"></param>
        /// <param name="flags"></param>
        public SimplifiedFileWriter(string pendingFileName, string completeFileName, int blockSize, params Guid[] flags)
        {
            m_pendingFileName = pendingFileName;
            m_completeFileName = completeFileName;
            m_fileHeaderBlock = FileHeaderBlock.CreateNewSimplified(blockSize, flags).CloneEditable();
            m_fileHeaderBlock.ArchiveType = SortedTreeFile.FileType;
            m_stream = new FileStream(pendingFileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
        }

#if DEBUG
        ~SimplifiedFileWriter()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
        }
#endif

        public Guid ArchiveId => m_fileHeaderBlock.ArchiveId;

        private void CloseCurrentFile()
        {
            if (m_subFileStream is null)
                return;
            if (!m_subFileStream.IsDisposed)
                throw new Exception("The previous file must be disposed before completing this action");
            m_subFileStream = null;
        }

        /// <summary>
        /// Creates and Opens a new file on the current file system.
        /// </summary>
        /// <returns></returns>
        public ISupportsBinaryStream CreateFile(SubFileName fileName)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            CloseCurrentFile();

            SubFileHeader subFile = m_fileHeaderBlock.CreateNewFile(fileName);
            subFile.DirectBlock = m_fileHeaderBlock.LastAllocatedBlock + 1;
            m_subFileStream = new SimplifiedSubFileStream(m_stream, subFile, m_fileHeaderBlock);
            return m_subFileStream;
        }

        /// <summary>
        /// Commits the changes to the disk.
        /// </summary>
        public void Commit()
        {
            if (m_disposed)
                return;
            CloseCurrentFile();
            m_stream.Position = 0;
            m_stream.Write(m_fileHeaderBlock.GetBytes());
            m_stream.Flush(true);
            WinApi.FlushFileBuffers(m_stream.SafeFileHandle);
            m_stream.Dispose();
            m_stream = null;
            File.Move(m_pendingFileName, m_completeFileName);
            m_disposed = true;
            Dispose();
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="SimplifiedFileWriter"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SimplifiedFileWriter"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        if (m_subFileStream != null)
                        {
                            m_subFileStream.Dispose();
                            m_subFileStream = null;
                        }
                        if (m_stream != null)
                        {
                            m_stream.Dispose();
                        }
                        File.Delete(m_pendingFileName);
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }
    }
}
