
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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace openHistorian.V2.UnmanagedMemory
{
    /// <summary>
    /// This class allows allocating large memory pages.  In order to do this,
    /// the user may have to enable the locking memory permission in the
    /// local security policy.  If they fail to do this, small pages will be used
    /// instead.  
    /// Local Security Policy -> Security Settings -> Local Policies -> User Rights Assignment
    /// -> Lock pages in memory.  Add the user that this application will run under.
    /// </summary>
    static class WinApi
    {
        [DllImport("KERNEL32", SetLastError = true)]
        public static extern void FlushFileBuffers(SafeFileHandle handle);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void MoveMemory(IntPtr dest, IntPtr src, int size);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        unsafe public static extern void MoveMemory(byte* dest, byte* src, int size);
    }
}
