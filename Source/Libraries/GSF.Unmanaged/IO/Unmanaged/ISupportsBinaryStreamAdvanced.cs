//******************************************************************************************************
//  ISupportsBinaryStreamAdvanced.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  6/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;

namespace GSF.IO.Unmanaged
{
    public class StreamBlockEventArgs
        : EventArgs
    {
        public long Position { get; private set; }
        public IntPtr Data { get; private set; }
        public int Length { get; private set; }
        public StreamBlockEventArgs(long position, IntPtr data, int length)
        {
            Position = position;
            Data = data;
            Length = length;
        }
    }
    /// <summary>
    /// A <see cref="ISupportsBinaryStream"/> that has many advance functions 
    /// that are needed for <see cref="DiskIo"/> to function properly.
    /// </summary>
    public interface ISupportsBinaryStreamAdvanced : ISupportsBinaryStream
    {
        /// <summary>
        /// This event occurs any time new data is added to the BinaryStream's 
        /// internal memory. It gives the consumer of this class an opportunity to 
        /// properly initialize the data before it is handed to an IoSession.
        /// </summary>
        event EventHandler<StreamBlockEventArgs> BlockLoadedFromDisk;
        
        /// <summary>
        /// This event occurs right before something is committed to the disk. 
        /// This gives the opportunity to finalize the data, such as updating checksums.
        /// After the block has been successfully written <see cref="BlockLoadedFromDisk"/>
        /// is called if the block is to remain in memory.
        /// </summary>
        event EventHandler<StreamBlockEventArgs> BlockAboutToBeWrittenToDisk;
        
        /// <summary>
        /// Gets the unit size of an individual block
        /// </summary>
        int BlockSize { get; }
       
        /// <summary>
        /// Gets the length of the current stream.
        /// </summary>
        long Length { get; }
        
        /// <summary>
        /// Sets the size of the stream.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        long SetLength(long length);

        /// <summary>
        /// Writes all current data to the disk subsystem.
        /// </summary>
        void Flush();

        /// <summary>
        /// Equivalent to SetLength but my not change the size of the file.
        /// </summary>
        /// <param name="position">The start of the position that will be invalidated</param>
        void TrimEditsAfterPosition(long position);

    }
}
