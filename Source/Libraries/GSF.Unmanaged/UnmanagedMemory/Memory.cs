//******************************************************************************************************
//  Memory.cs - Gbtc
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
//  3/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  6/8/2012 - Steven E. Chisholm
//       Removed large page support and simplified unused and untested procedures for initial release     
//
//******************************************************************************************************

using System;
using System.Runtime.InteropServices;

namespace openHistorian.UnmanagedMemory
{

    /// <summary>
    /// This class is used to allocate and free unmanaged memory.  To release memory allocated throught this class,
    /// call the Dispose method of the return value.
    /// </summary>
    /// <remarks>
    /// .NET does not respond well when managing tens of GBs of ram.  If a very large buffer pool must be created,
    /// it would be good to allocate that buffer pool in unmanaged memory.
    /// </remarks>
    // ToDo: Consider adding support for managed memory allocation. By allocating a Byte[] and using GCHandle.Alloc() to pin the object.
    // ToDo: Support Large block allocations via the OS.
    public sealed class Memory : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// The pointer to the first byte of this unmanaged memory.
        /// </summary>
        public IntPtr Address { get; private set; }
        /// <summary>
        /// The number of bytes in this allocation.
        /// </summary>
        public int Size { get; private set; }
        bool m_disposed;
        //bool m_isLargePage;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Allocates unmanaged memory. The block is uninitialized.
        /// </summary>
        /// <param name="requestedSize">The desired number of bytes to allocate. 
        /// Be sure to check the actual size in the return class. </param>
        /// <returns>The allocated memory.</returns>
        public Memory(int requestedSize)
        {
            if (requestedSize <= 0)
                throw new ArgumentOutOfRangeException("requestedSize", "must be greater than zero");
            //if (UseLargePages && (requestedSize & (s_largePageMinimumSize - 1)) == 0)
            //{
            //    Address = WinApi.LargePageSupport.VirtualAllocLargePages((uint)requestedSize);
            //    if (Address != IntPtr.Zero)
            //    {
            //        Size = requestedSize;
            //        m_isLargePage = true;
            //        return;
            //    }
            //}

            Address = Marshal.AllocHGlobal(requestedSize);
            Size = requestedSize;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="Memory"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~Memory()
        {
            Dispose(false);
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the allocated memory back to the OS.
        /// Same thing as calling Dispose().
        /// </summary>
        public void Release()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="Memory"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Memory"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (Address != IntPtr.Zero)
                    {
                        //if (m_isLargePage)
                        //    WinApi.LargePageSupport.VirtualFree(Address);
                        //else
                            Marshal.FreeHGlobal(Address);
                    }
                }
                finally
                {
                    Size = 0;
                    Address = IntPtr.Zero;
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion

        #region [ Static ]

        #region [ Fields ]

        //static bool s_useLargePageSizes;
        //static uint s_largePageMinimumSize;
        //public static bool UseLargePages
        //{
        //    get
        //    {
        //        return s_useLargePageSizes;
        //    }
        //    set
        //    {
        //        s_useLargePageSizes = value && WinApi.LargePageSupport.CanAllocateLargePage;
        //    }
        //}

        #endregion

        #region [ Constructor ]
        //static Memory()
        //{
        //    //if (WinApi.LargePageSupport.CanAllocateLargePage)
        //    //    s_largePageMinimumSize = WinApi.LargePageSupport.GetLargePageMinimum();
        //    //else
        //        s_largePageMinimumSize = 0;
        //}
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Does a safe copy of data from one location to another. 
        /// A safe copy allows for the source and destination to overlap.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="count"></param>
        public static unsafe void Copy(byte* src, byte* dest, int count)
        {
            WinApi.MoveMemory(dest, src, count);
        }

        /// <summary>
        /// Sets the data in this buffer to all zeroes
        /// </summary>
        /// <param name="pointer">the starting position.</param>
        /// <param name="length">the number of bytes to clear.</param>
        public static unsafe void Clear(byte* pointer, int length)
        {
            int i;
            for (i = 0; i < length - 8; i += 8)
            {
                *(long*)(pointer + i) = 0;
            }
            for (; i < length; i++)
            {
                pointer[i] = 0;
            }
        }

        #endregion

        #endregion

    }
}
