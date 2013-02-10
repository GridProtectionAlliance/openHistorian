//******************************************************************************************************
//  IBinaryStreamIoSession.cs - Gbtc
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
//  4/26/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Implementing this interface allows a binary stream to be attached to a buffer.
    /// </summary>
    public interface IBinaryStreamIoSession : IDisposable
    {
        /// <summary>
        /// Gets a block for the following Io session.
        /// </summary>
        /// <param name="position">the block returned must contain this position</param>
        /// <param name="isWriting">indicates if the stream plans to write to this block</param>
        /// <param name="firstPointer">the pointer for the first byte of the block</param>
        /// <param name="firstPosition">the position that corresponds to <see cref="firstPointer"/></param>
        /// <param name="length">the length of the block</param>
        /// <param name="supportsWriting">notifies the calling class if this block supports 
        /// writing without requiring this function to be called again if <see cref="isWriting"/> was set to false.</param>
        void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting);
        /// <summary>
        /// Sets the current usage of the <see cref="IBinaryStreamIoSession"/> to null.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets if the object has been disposed
        /// </summary>
        bool IsDisposed { get; }
    }

    public static class BinaryStreamIoSessionExtensionMethods
    {
        /// <summary>
        /// Reads from the underlying stream the requested set of data. 
        /// This function is more user friendly than calling GetBlock().
        /// </summary>
        /// <param name="session">The session to do the reading on</param>
        /// <param name="position">the starting position of the read</param>
        /// <param name="pointer">an output pointer to <see cref="position"/>.</param>
        /// <param name="validLength">the number of bytes that are valid after this position.</param>
        /// <returns></returns>
        public static void ReadBlock(this IBinaryStreamIoSession session, long position, out IntPtr pointer, out int validLength)
        {
            long firstPosition;
            bool supportsWriting;
            session.GetBlock(position, false, out pointer, out firstPosition, out validLength, out supportsWriting);
            int seekDistance = (int)(position - firstPosition);
            validLength -= seekDistance;
            pointer += seekDistance;
        }
   }
}
