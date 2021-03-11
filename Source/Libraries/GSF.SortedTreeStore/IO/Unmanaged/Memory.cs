//******************************************************************************************************
//  Memory.cs - Gbtc
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
//  03/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  06/08/2012 - Steven E. Chisholm
//       Removed large page support and simplified unused and untested procedures for initial release     
//
//******************************************************************************************************

using System;
using System.Runtime.InteropServices;
using System.Threading;
using GSF.Diagnostics;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// This class is used to allocate and free unmanaged memory.  To release memory allocated throught this class,
    /// call the Dispose method of the return value.
    /// </summary>
    /// <remarks>
    /// .NET does not respond well when managing tens of GBs of ram.  If a very large buffer pool must be created,
    /// it would be good to allocate that buffer pool in unmanaged memory.
    /// </remarks>
    public sealed class Memory
        : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(Memory), MessageClass.Component);

        #region [ Members ]

        private IntPtr m_address;
        private int m_size;

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

            m_address = Marshal.AllocHGlobal(requestedSize);
            m_size = requestedSize;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="Memory"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~Memory()
        {
            Dispose();
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The pointer to the first byte of this unmanaged memory. 
        /// Equals <see cref="IntPtr.Zero"/> if memory has been released.
        /// </summary>
        public IntPtr Address => m_address;

        /// <summary>
        /// The number of bytes in this allocation.
        /// </summary>
        public int Size => m_size;

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
            m_size = 0;
            IntPtr value = Interlocked.Exchange(ref m_address, IntPtr.Zero);
            if (value != IntPtr.Zero)
            {
                try
                {
                    Marshal.FreeHGlobal(value);
                }
                catch (Exception ex)
                {
                    Log.Publish(MessageLevel.Error, "Unexpected Exception while releasing unmanaged memory", null, null, ex);
                }
                finally
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        #region [ Static ]

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
        /// Does a safe copy of data from one location to another. 
        /// A safe copy allows for the source and destination to overlap.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="count"></param>
        public static unsafe void Copy(IntPtr src, IntPtr dest, int count)
        {
            WinApi.MoveMemory((byte*)dest, (byte*)src, count);
        }

        /// <summary>
        /// Sets the data in this buffer to all zeroes
        /// </summary>
        /// <param name="pointer">the starting position.</param>
        /// <param name="length">the number of bytes to clear.</param>
        public static unsafe void Clear(byte* pointer, int length)
        {
            int i;
            for (i = 0; i <= length - 8; i += 8)
            {
                *(long*)(pointer + i) = 0;
            }
            for (; i < length; i++)
            {
                pointer[i] = 0;
            }
        }

        /// <summary>
        /// Sets the data in this buffer to all zeroes
        /// </summary>
        /// <param name="pointer">the starting position.</param>
        /// <param name="length">the number of bytes to clear.</param>
        public static unsafe void Clear(IntPtr pointer, int length)
        {
            Clear((byte*)pointer, length);
        }

        #endregion

        #endregion
    }
}