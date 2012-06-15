//******************************************************************************************************
//  DiskIoEnhanced.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  ----------------------------------------------------------------------------------------------------
//  3/24/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.FileSystem
{
    internal class DiskIoEnhanced : IDisposable
    {
        ISupportsBinaryStreamSizing m_stream;

        public DiskIoEnhanced()
        {
            m_stream = new MemoryStream();
        }

        public MemoryUnit GetMemoryUnit()
        {
            return new MemoryUnit(this, m_stream.GetNextIoSession());
        }

        /// <summary>
        /// Resizes the file to the requested size
        /// </summary>
        /// <param name="requestedSize">The size to resize to</param>
        /// <returns>The actual size of the file after the resize</returns>
        long SetFileLength(long requestedSize)
        {
            return m_stream.SetLength(requestedSize);
            //return (int)(requestedSize / ArchiveConstants.BlockSize) * ArchiveConstants.BlockSize;
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsDisposed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the current size of the file.
        /// </summary>
        public long FileSize
        {
            get
            {
                return m_stream.Length;
            }
        }

        #region [ OldMethods ]

        /// <summary>
        /// This will resize the file to the provided size in bytes;
        /// If resizing smaller than the allocated space, this number is 
        /// increased to the allocated space.  
        /// If file size is not a multiple of the page size, it is rounded up to
        /// the nearest page boundry
        /// </summary>
        /// <param name="size">The number of bytes to make the file.</param>
        /// <param name="nextUnallocatedBlock">the next free block.  
        /// This value is used to ensure that the archive file is not 
        /// reduced beyond this limit causing data coruption</param>
        /// <returns>The size that the file is after this call</returns>
        /// <remarks>Passing 0 to this function will effectively trim out 
        /// all of the free space in this file.</remarks>
        public long SetFileLength(long size, int nextUnallocatedBlock)
        {
            if (nextUnallocatedBlock * ArchiveConstants.BlockSize > size)
            {
                //if shrinking beyond the allocated space, 
                //adjust the size exactly to the allocated space.
                size = nextUnallocatedBlock * ArchiveConstants.BlockSize;
            }
            else
            {
                long remainder = (size % ArchiveConstants.BlockSize);
                //if there will be a fragmented page remaining
                if (remainder != 0)
                {
                    //if the requested size is not a multiple of the page size
                    //round up to the nearest page
                    size = size + ArchiveConstants.BlockSize - remainder;
                }
            }
            return SetFileLength(size);
        }
        
        public void Dispose()
        {
            m_stream.Dispose();
        }

        #endregion



    }
}
