//******************************************************************************************************
//  CustomFileStream.cs - Gbtc
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
//  09/25/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GSF.Collections;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;
using GSF.Threading;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// A functional wrapper around a <see cref="FileStream"/>
    /// specific to how the <see cref="TransactionalFileStructure"/> uses the <see cref="FileStream"/>.
    /// </summary>
    internal sealed class CustomFileStream
        : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(CustomFileStream), MessageClass.Component);


        /// <summary>
        /// Needed since this class computes footer checksums.
        /// </summary>
        private bool m_disposed;
        private string m_fileName;
        private bool m_isReadOnly;
        private bool m_isSharingEnabled;
        private readonly int m_ioSize;
        private readonly int m_fileStructureBlockSize;
        private FileStream m_stream;
        private int m_streamUsers;
        private readonly ResourceQueue<byte[]> m_bufferQueue;

        /// <summary>
        /// Lock this first. Allows the <see cref="m_stream"/> item to be replaced in 
        /// a synchronized fashion. 
        /// </summary>
        private readonly ReaderWriterLockEasy m_isUsingStream = new ReaderWriterLockEasy();
        /// <summary>
        /// Needed to properly synchronize Read/Write operations.
        /// </summary>
        private readonly object m_syncRoot;
        private readonly AtomicInt64 m_length = new AtomicInt64();

        /// <summary>
        /// Creates a new CustomFileStream
        /// </summary>
        /// <param name="stream">The filestream to use as the base stream</param>
        /// <param name="ioSize">The size of a buffer pool entry</param>
        /// <param name="fileStructureBlockSize">The size of an individual block</param>
        /// <param name="fileName">The filename</param>
        /// <param name="isReadOnly">If the file is read only</param>
        /// <param name="isSharingEnabled">if the file is exclusively opened</param>
        private CustomFileStream(int ioSize, int fileStructureBlockSize, string fileName, bool isReadOnly, bool isSharingEnabled)
        {
            if (ioSize < 4096)
                throw new ArgumentOutOfRangeException("ioSize", "Cannot be less than 4096");
            if (fileStructureBlockSize > ioSize)
                throw new ArgumentOutOfRangeException("fileStructureBlockSize", "Must not be greater than BufferPoolSize");
            if (!BitMath.IsPowerOfTwo(ioSize))
                throw new ArgumentException("Must be a power of 2", "ioSize");
            if (!BitMath.IsPowerOfTwo(fileStructureBlockSize))
                throw new ArgumentException("Must be a power of 2", "fileStructureBlockSize");

            m_ioSize = ioSize;
            m_fileName = fileName;
            m_isReadOnly = isReadOnly;
            m_isSharingEnabled = isSharingEnabled;
            m_fileStructureBlockSize = fileStructureBlockSize;
            m_bufferQueue = ResourceList.GetResourceQueue(ioSize);
            m_syncRoot = new object();

            FileInfo fileInfo = new FileInfo(fileName);
            m_length.Value = fileInfo.Length;
        }

#if DEBUG
        ~CustomFileStream()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
        }
#endif

        #region [ Properties ]

        /// <summary>
        /// Gets if the file was opened read only.
        /// </summary>
        public bool IsReadOnly => m_isReadOnly;

        /// <summary>
        /// Gets if the file was opened allowing shared read access.
        /// </summary>
        public bool IsSharingEnabled => m_isSharingEnabled;

        /// <summary>
        /// Gets the name of the file
        /// </summary>
        public string FileName => m_fileName;

        /// <summary>
        /// Gets the number of bytes in a file structure block.
        /// </summary>
        public int FileStructureBlockSize => m_fileStructureBlockSize;

        /// <summary>
        /// Gets the number of bytes in each IO operation.
        /// </summary>
        public int IOSize => m_ioSize;

        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        public long Length => m_length;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens the underlying file stream
        /// </summary>
        public void Open()
        {
            using (m_isUsingStream.EnterWriteLock())
            {
                if (m_streamUsers == 0)
                    m_stream = new FileStream(m_fileName, FileMode.Open, m_isReadOnly ? FileAccess.Read : FileAccess.ReadWrite, m_isSharingEnabled ? FileShare.Read : FileShare.None, 2048, true);

                m_streamUsers++;
            }
        }

        /// <summary>
        /// Closes the underlying file stream
        /// </summary>
        public void Close()
        {
            using (m_isUsingStream.EnterWriteLock())
            {
                m_streamUsers--;

                if (m_streamUsers == 0)
                {
                    m_stream?.Dispose();
                    m_stream = null;
                }
            }
        }

        /// <summary>
        /// Reads data from the disk
        /// </summary>
        /// <param name="position">The starting position</param>
        /// <param name="buffer">the byte buffer of data to read</param>
        /// <param name="length">the number of bytes to read</param>
        /// <returns>the number of bytes read</returns>
        public int ReadRaw(long position, byte[] buffer, int length)
        {
            bool needsOpen = m_stream is null;
            try
            {
                if (needsOpen)
                    Open();

                int totalLengthRead = 0;
                int len = 0;
                while (length > 0)
                {
                    using (m_isUsingStream.EnterReadLock())
                    {
                        Task<int> results;
                        lock (m_syncRoot)
                        {
                            m_stream.Position = position;
                            results = m_stream.ReadAsync(buffer, 0, length);
                        }
                        len = results.Result;
                    }
                    totalLengthRead += len;
                    if (len == length)
                        return totalLengthRead;
                    if (len == 0 && position >= m_length)
                        return totalLengthRead; //End of the stream has occurred
                    if (len != 0)
                    {
                        position += len;
                        length -= len; //Keep Reading
                    }
                    else
                    {
                        Log.Publish(MessageLevel.Warning, "File Read Error", $"The OS has closed the following file {m_stream.Name}. Attempting to reopen.");
                        ReopenFile();
                    }
                }

                return length;
            }
            finally
            {
                if (needsOpen)
                    Close();
            }
        }

        /// <summary>
        /// Writes data to the disk
        /// </summary>
        /// <param name="position">The starting position</param>
        /// <param name="buffer">the byte buffer of data to write</param>
        /// <param name="length">the number of bytes to write</param>
        public void WriteRaw(long position, byte[] buffer, int length)
        {
            bool needsOpen = m_stream is null;
            try
            {
                if (needsOpen)
                    Open();

                using (m_isUsingStream.EnterReadLock())
                {
                    Task results;
                    lock (m_syncRoot)
                    {
                        m_stream.Position = position;
                        results = m_stream.WriteAsync(buffer, 0, length);
                    }
                    results.Wait();
                    m_length.Value = m_stream.Length;
                }
            }
            finally
            {
                if (needsOpen)
                    Close();
            }
        }

        /// <summary>
        /// Reads an entire page at the provided location. Also computes the checksum information.
        /// </summary>
        /// <param name="position">The stream position. May be any position inside the desired block</param>
        /// <param name="locationToCopyData">The place where to write the data to.</param>
        public void Read(long position, IntPtr locationToCopyData)
        {
            byte[] buffer = m_bufferQueue.Dequeue();
            int bytesRead = ReadRaw(position, buffer, buffer.Length);
            if (bytesRead < buffer.Length)
                Array.Clear(buffer, bytesRead, buffer.Length - bytesRead);

            Marshal.Copy(buffer, 0, locationToCopyData, buffer.Length);

            m_bufferQueue.Enqueue(buffer);

            Footer.WriteChecksumResultsToFooter(locationToCopyData, m_fileStructureBlockSize, buffer.Length);
        }


        /// <summary>
        /// Writes all of the dirty blocks passed onto the disk subsystem. Also computes the checksum for the data.
        /// </summary>
        /// <param name="currentEndOfCommitPosition">the last valid byte of the file system where this data will be appended to.</param>
        /// <param name="stream">the source of the data to dump to the disk</param>
        /// <param name="length">The number by bytes to write to the file system.</param>
        /// <param name="waitForWriteToDisk">True to wait for a complete commit to disk before returning from this function.</param>
        public void Write(long currentEndOfCommitPosition, MemoryPoolStreamCore stream, long length, bool waitForWriteToDisk)
        {
            bool needsOpen = m_stream is null;
            try
            {
                if (needsOpen)
                    Open();

                byte[] buffer = m_bufferQueue.Dequeue();
                long endPosition = currentEndOfCommitPosition + length;
                long currentPosition = currentEndOfCommitPosition;
                while (currentPosition < endPosition)
                {
                    stream.ReadBlock(currentPosition, out IntPtr ptr, out int streamLength);
                    int subLength = (int)Math.Min(streamLength, endPosition - currentPosition);
                    Footer.ComputeChecksumAndClearFooter(ptr, m_fileStructureBlockSize, subLength);
                    Marshal.Copy(ptr, buffer, 0, subLength);
                    WriteRaw(currentPosition, buffer, subLength);

                    currentPosition += subLength;
                }
                m_bufferQueue.Enqueue(buffer);

                if (waitForWriteToDisk)
                {
                    FlushFileBuffers();
                }
                else
                {
                    using (m_isUsingStream.EnterReadLock())
                    {
                        m_stream.Flush(false);
                    }
                }
            }
            finally
            {
                if (needsOpen)
                    Close();
            }
        }


        /// <summary>
        /// Flushes any temporary data to the disk. This also calls WindowsAPI function 
        /// to have the OS flush to the disk.
        /// </summary>
        public void FlushFileBuffers()
        {
            //.NET's stream.Flush(FlushToDisk:=true) actually doesn't do what it says. 
            //Therefore WinApi must be called.
            using (m_isUsingStream.EnterReadLock())
            {
                if (m_stream != null)
                {
                    m_stream.Flush(true);
                    WinApi.FlushFileBuffers(m_stream.SafeFileHandle);
                }
            }
        }

        /// <summary>
        /// Changes the extension of the current file.
        /// </summary>
        /// <param name="extension">the new extension</param>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeExtension(string extension, bool isReadOnly, bool isSharingEnabled)
        {
            using (m_isUsingStream.EnterWriteLock())
            {
                string oldFileName = m_fileName;
                string newFileName = Path.ChangeExtension(oldFileName, extension);
                if (File.Exists(newFileName))
                    throw new Exception("New file already exists with this extension");

                bool openStream = m_stream is null;
                m_stream?.Dispose();
                m_stream = null;
                File.Move(oldFileName, newFileName);
                if (openStream)
                    m_stream = new FileStream(newFileName, FileMode.Open, isReadOnly ? FileAccess.Read : FileAccess.ReadWrite, isSharingEnabled ? FileShare.Read : FileShare.None, 2048, true);
                m_fileName = newFileName;
                m_isSharingEnabled = isSharingEnabled;
                m_isReadOnly = isReadOnly;
            }
        }

        private void ReopenFile()
        {
            using (m_isUsingStream.EnterWriteLock())
            {
                string fileName = m_stream.Name;

                try
                {
                    m_stream.Dispose();
                    m_stream = null;
                }
                catch (Exception e)
                {
                    Log.Publish(MessageLevel.Info, "Error when disposing stream", null, null, e);
                }
                m_stream = new FileStream(fileName, FileMode.Open, m_isReadOnly ? FileAccess.Read : FileAccess.ReadWrite, m_isSharingEnabled ? FileShare.Read : FileShare.None, 2048, true);
            }
        }

        /// <summary>
        /// Reopens the file with different permissions.
        /// </summary>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeShareMode(bool isReadOnly, bool isSharingEnabled)
        {
            using (m_isUsingStream.EnterWriteLock())
            {
                if (m_stream != null)
                {
                    m_stream.Dispose();
                    m_stream = new FileStream(m_fileName, FileMode.Open, isReadOnly ? FileAccess.Read : FileAccess.ReadWrite, isSharingEnabled ? FileShare.Read : FileShare.None, 2048, true);
                }
                m_isSharingEnabled = isSharingEnabled;
                m_isReadOnly = isReadOnly;
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="CustomFileStream"/> object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (!m_disposed)
            {
                try
                {
                    using (m_isUsingStream.EnterWriteLock())
                    {
                        m_stream?.Dispose();
                        m_stream = null;
                    }
                }
                finally
                {
                    m_disposed = true; // Prevent duplicate dispose.
                }
            }
        }

        #endregion

        #region [ Static ]


        /// <summary>
        /// Creates a file with the supplied name
        /// </summary>
        /// <param name="fileName">the name of the file</param>
        /// <param name="ioBlockSize">the number of bytes to do all io with</param>
        /// <param name="fileStructureBlockSize">the number of bytes in the file structure so checksums can be properly computed.</param>
        /// <returns></returns>
        public static CustomFileStream CreateFile(string fileName, int ioBlockSize, int fileStructureBlockSize)
        {
            return new CustomFileStream(ioBlockSize, fileStructureBlockSize, fileName, false, false);
        }

        /// <summary>
        /// Opens a file
        /// </summary>
        /// <param name="fileName">the name of the file.</param>
        /// <param name="ioBlockSize">the number of bytes to do all of the io</param>
        /// <param name="fileStructureBlockSize">The number of bytes in the file structure</param>
        /// <param name="isReadOnly">if the file should be opened in read only</param>
        /// <param name="isSharingEnabled">if the file should be opened with read sharing permissions.</param>
        /// <returns></returns>
        public static CustomFileStream OpenFile(string fileName, int ioBlockSize, out int fileStructureBlockSize, bool isReadOnly, bool isSharingEnabled)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, isReadOnly ? FileAccess.Read : FileAccess.ReadWrite, isSharingEnabled ? FileShare.Read : FileShare.None, 2048, true))
            {
                fileStructureBlockSize = FileHeaderBlock.SearchForBlockSize(fileStream);
            }

            return new CustomFileStream(ioBlockSize, fileStructureBlockSize, fileName, isReadOnly, isSharingEnabled);
        }

        /// <summary>
        /// Queues byte[] blocks.
        /// </summary>
        private static readonly ResourceQueueCollection<int, byte[]> ResourceList;

        /// <summary>
        /// Creates a resource list that everyone shares.
        /// </summary>
        static CustomFileStream()
        {
            ResourceList = new ResourceQueueCollection<int, byte[]>(blockSize => () => new byte[blockSize], 10, 20);
        }

        #endregion
    }
}
