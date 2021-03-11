//******************************************************************************************************
//  AtomicInt64.cs - Gbtc
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
//  02/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Since 64 bit reads/asignments are not atomic on a 32-bit process, this class
    /// wrapps the <see cref="Interlocked"/> class to if using a 32-bit process to ensure
    /// atomic reads/writes.
    /// </summary>
    public class AtomicInt64
    {
        //Note: This is a class and not a struct to prevent users from copying the struct value
        //      which would result in a non-atomic clone of the struct.  

        private long m_value;

        /// <summary>
        /// Creates a new AtomicInt64
        /// </summary>
        /// <param name="value"></param>
        public AtomicInt64(long value = 0)
        {
            m_value = value;
        }

        /// <summary>
        /// Gets/Sets the value
        /// </summary>
        public long Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (Environment.Is64BitProcess)
                    return m_value;
                return Interlocked.Read(ref m_value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (Environment.Is64BitProcess)
                    m_value = value;
                else
                    Interlocked.Exchange(ref m_value, value);
            }
        }

        /// <summary>
        /// Converts to a long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(AtomicInt64 value)
        {
            return value.Value;
        }

    }
}
