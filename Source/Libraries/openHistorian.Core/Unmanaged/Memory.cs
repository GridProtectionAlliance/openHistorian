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
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.Core.Unmanaged
{

    /// <summary>
    /// This class is used to allocate and free unmanaged memory.  To release memory allocated throught this class,
    /// call the Dispose method of the return value.
    /// </summary>
    /// <remarks>
    /// .NET does not respond well when managing tens of GBs of ram.  If a very large buffer pool must be created,
    /// it would be good to allocate that buffer pool in unmanaged memory.
    /// </remarks>
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
        public uint Size { get; private set; }

        #endregion

        #region [ Constructors ]

        Memory(IntPtr address, uint size)
        {
            Address = address;
            Size = size;
        }
        ~Memory()
        {
            if (Address != IntPtr.Zero)
            {
                Release(Address);
                Address = IntPtr.Zero;
            }
        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the allocated memory back to the OS.
        /// </summary>
        public void Dispose()
        {
            if (Address != IntPtr.Zero)
            {
                Release(Address);
                Size = 0;
                Address = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }
        }
        /// <summary>
        /// Releases the allocated memory back to the OS.
        /// Same thing as calling Dispose().
        /// </summary>
        public void Release()
        {
            Dispose();
        }


        #endregion

        #region [ Static ]

        #region [ Fields ]

        static bool s_supportsLargePageSizes;
        static uint s_largePageMinimumSize;

        #endregion

        #region [ Constructor ]
        static Memory()
        {
            s_supportsLargePageSizes = WinApi.CanAllocateLargePage;
            if (s_supportsLargePageSizes)
                s_largePageMinimumSize = WinApi.GetLargePageMinimum();
            else
                s_largePageMinimumSize = 0;
        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Allocates unmanaged memory.
        /// </summary>
        /// <param name="requestedSize">The desired number of bytes to allocate. 
        /// Be sure to check the actual size in the return class. 
        /// When using large pages, the actual size may vary.</param>
        /// <param name="useLargePagesIfSupported">Determines if large pages 
        /// should be used if they are supported by the OS.</param>
        /// <returns>The allocated memory.</returns>
        public static Memory Allocate(uint requestedSize, bool useLargePagesIfSupported)
        {
            if (useLargePagesIfSupported && s_supportsLargePageSizes)
            {
                uint roundDownAmount = requestedSize % s_largePageMinimumSize;

                if (roundDownAmount > 0)
                    requestedSize = requestedSize + s_largePageMinimumSize - roundDownAmount;

                return new Memory(WinApi.VirtualAlloc(requestedSize, true), requestedSize);
            }
            else
            {
                return new Memory(WinApi.VirtualAlloc(requestedSize, false), requestedSize);
            }
        }
        /// <summary>
        /// Releases the memory back to the OS
        /// </summary>
        /// <param name="pointer"></param>
        static void Release(IntPtr pointer)
        {
            WinApi.VirtualFree(pointer);
        }


        public static unsafe void Copy(byte* src, byte* dest, int count)
        {
            WinApi.MoveMemory(dest, src, count);
            return;
            if (Math.Abs((long)src-(long)dest)<count)
            {
                
            }

            int block;

            block = count >> 3;

            long* pDest = (long*)dest;
            long* pSrc = (long*)src;

            for (int i = 0; i < block; i++)
            {
                *pDest = *pSrc; 
                pDest++; 
                pSrc++;
            }
            dest = (byte*)pDest;
            src = (byte*)pSrc;
            count = count - (block << 3);

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    *dest = *src; dest++; src++;
                }
            }
        }
    

        #endregion

        #endregion


    }
}
