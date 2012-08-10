//******************************************************************************************************
//  WinApi.cs - Gbtc
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
//  3/16/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace openHistorian.V2
{
    /// <summary>
    /// Provides necessary Windows API functions.
    /// </summary>
    static class WinApi
    {
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
        /// to see if the data bytes overlaps and gaurentees that the bytes are copied in 
        /// such a way to preserve the move.
        /// </summary>
        /// <param name="destination">a pointer to the destination</param>
        /// <param name="source">a pointer to the source</param>
        /// <param name="count">the number of bytes to move</param>
        /// <remarks>By setting the SuppressUnmanagedCodeSecurityAttribute will decrease the pinvoke overhead by about 2x.</remarks>
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false), SuppressUnmanagedCodeSecurityAttribute]
        unsafe public static extern void MoveMemory(byte* destination, byte* source, int count);

        //[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        //public static extern void MoveMemory(IntPtr dest, IntPtr src, int size);

        //[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        //public static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

        //[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        //unsafe public static extern IntPtr memcpy(byte* dest, byte* src, int count);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace(string lpRootPathName,
          out uint lpSectorsPerCluster,
          out uint lpBytesPerSector,
          out uint lpNumberOfFreeClusters,
          out uint lpTotalNumberOfClusters);


        /// <summary>
        /// Tries to get the free space values for a given path. This path can be a network share, a mount point.
        /// </summary>
        /// <param name="pathName">The path to the location</param>
        /// <param name="freeSpace">The number of user space bytes</param>
        /// <param name="totalSize">The total number of bytes on the drive.</param>
        /// <returns>True if successful, false if there was an error.</returns>
        public static bool GetAvailableFreeSpace(string pathName, out long freeSpace, out long totalSize)
        {
            try
            {
                var fullPath = Path.GetFullPath(pathName);

                ulong lpFreeBytesAvailable;
                ulong lpTotalNumberOfBytes;
                ulong lpTotalNumberOfFreeBytes;

                var success = GetDiskFreeSpaceEx(fullPath, out lpFreeBytesAvailable, out lpTotalNumberOfBytes,
                                                 out lpTotalNumberOfFreeBytes);

                freeSpace = (long)lpFreeBytesAvailable;
                totalSize = (long)lpTotalNumberOfBytes;

                return success;
            }
            catch (Exception)
            {
                freeSpace = (long)0;
                totalSize = (long)0;
                return false;
            }

        
        }


    }
}
