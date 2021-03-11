//******************************************************************************************************
//  WinApi.cs - Gbtc
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
//  03/16/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  11/03/2016 - J. Ritchie Carroll
//       Added Mono compatible implementation.       
//
//******************************************************************************************************

#if !MONO
using System.Runtime.InteropServices;
using System.Security;
#endif
using Microsoft.Win32.SafeHandles;

namespace GSF
{
    /// <summary>
    /// Provides necessary Windows API functions.
    /// </summary>
    public static unsafe class WinApi
    {
#if MONO
        /// <summary>
        /// Flushes the buffers of a specified file and causes all buffered data to be written to a file.
        /// </summary>
        /// <param name="handle"></param>
        /// <remarks>Since the flush of a file stream does not actually work, this finishes the flush to the disk file system.
        /// Which still could cache the results, but this is about the best we can do for a flush right now.</remarks>
        public static void FlushFileBuffers(SafeFileHandle handle)
        {            
        }

        /// <summary>
        /// Copies data from one memory location to another. This function does a check
        /// to see if the data bytes overlaps and guarantees that the bytes are copied in 
        /// such a way to preserve the move.
        /// </summary>
        /// <param name="destination">a pointer to the destination</param>
        /// <param name="source">a pointer to the source</param>
        /// <param name="count">the number of bytes to move</param>
        public static void MoveMemory(byte* destination, byte* source, int count)
        {
            for (int i = 0; i < count; i++)
                *destination++ = *source++;
        }
#else
        /// <summary>
        /// Flushes the buffers of a specified file and causes all buffered data to be written to a file.
        /// </summary>
        /// <param name="handle"></param>
        /// <remarks>Since the flush of a file stream does not actually work, this finishes the flush to the disk file system.
        /// Which still could cache the results, but this is about the best we can do for a flush right now.</remarks>
        [DllImport("KERNEL32", SetLastError = true)]
        public static extern void FlushFileBuffers(SafeFileHandle handle);

        /// <summary>
        /// Copies data from one memory location to another. This function does a check
        /// to see if the data bytes overlaps and guarantees that the bytes are copied in 
        /// such a way to preserve the move.
        /// </summary>
        /// <param name="destination">a pointer to the destination</param>
        /// <param name="source">a pointer to the source</param>
        /// <param name="count">the number of bytes to move</param>
        /// <remarks>By setting the SuppressUnmanagedCodeSecurityAttribute will decrease the pinvoke overhead by about 2x.</remarks>
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false), SuppressUnmanagedCodeSecurity]
        public static extern void MoveMemory(byte* destination, byte* source, int count);
#endif
    }
}