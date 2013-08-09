//ToDo:  Support memory pinning in future releases of the OpenHistorian 2.0
//ToDo:  Support large memory pages in future releases
//There is a potential that there might be a bug in this code.

////******************************************************************************************************
////  WinApi.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  3/16/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Runtime.InteropServices;
//using Microsoft.Win32.SafeHandles;

//namespace openHistorian.UnmanagedMemory
//{
//    /// <summary>
//    /// This class allows allocating large memory pages.  In order to do this,
//    /// the user may have to enable the locking memory permission in the
//    /// local security policy.  If they fail to do this, small pages will be used
//    /// instead.  
//    /// Local Security Policy -> Security Settings -> Local Policies -> User Rights Assignment
//    /// -> Lock pages in memory.  Add the user that this application will run under.
//    /// </summary>
//    static class WinApi
//    {
//        [DllImport("KERNEL32", SetLastError = true)]
//        public static extern void FlushFileBuffers(SafeFileHandle handle);

//        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
//        public static extern void MoveMemory(IntPtr dest, IntPtr src, int size);

//        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
//        unsafe public static extern void MoveMemory(byte* dest, byte* src, int size);

//        public static bool CanAllocateLargePage { get; private set; }

//        static WinApi()
//        {
//            try
//            {
//                SetPrivilegeLockMemoryPrivilege();
//                CanAllocateLargePage = true;
//            }
//            catch (Exception)
//            {
//                CanAllocateLargePage = false;
//            }
//        }

//        [DllImport("kernel32.dll", SetLastError = true)]
//        public static extern UInt32 GetLargePageMinimum();

//        #region [ Allocate Memory ]

//        [Flags()]
//        enum AllocationType : uint
//        {
//            Commit = 0x1000,
//            Reserve = 0x2000,
//            Reset = 0x80000,
//            LargePages = 0x20000000,
//            Physical = 0x400000,
//            TopDown = 0x100000,
//            WriteWatch = 0x200000
//        }

//        [Flags()]
//        enum MemoryProtection : uint
//        {
//            Execute = 0x10,
//            ExecuteRead = 0x20,
//            ExecuteReadwrite = 0x40,
//            ExecuteWritecopy = 0x80,
//            Noaccess = 0x01,
//            Readonly = 0x02,
//            Readwrite = 0x04,
//            Writecopy = 0x08,
//            GuardModifierflag = 0x100,
//            NocacheModifierflag = 0x200,
//            WritecombineModifierflag = 0x400
//        }

//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

//        public static IntPtr VirtualAlloc(uint size, bool useLargePagesIfSupported)
//        {
//            if (useLargePagesIfSupported && CanAllocateLargePage)
//            {
//                IntPtr returnValue = VirtualAlloc(IntPtr.Zero, (UIntPtr)size, AllocationType.Commit | AllocationType.Reserve | AllocationType.LargePages, MemoryProtection.Readwrite);
//                if (returnValue == IntPtr.Zero)
//                {
//                    int error = Marshal.GetLastWin32Error();
//                    if (error == 1450)
//                    {
//                        return VirtualAlloc(size, false);
//                    }
//                    throw new Exception("error allocating large block: " + error);
//                }
//                return returnValue;
//            }
//            else
//            {
//                IntPtr returnValue = VirtualAlloc(IntPtr.Zero, (UIntPtr)size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.Readwrite | MemoryProtection.NocacheModifierflag);
//                if (returnValue == IntPtr.Zero)
//                {
//                    int error = Marshal.GetLastWin32Error();
//                    throw new Exception("error allocating large block: " + error);
//                }
//                return returnValue;
//            }
//        }


//        #endregion

//        #region [ Free memory ]

//        [Flags()]
//        enum FreeType : uint
//        {
//            Decommit = 0x4000,
//            Release = 0x8000,
//        }

//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern bool VirtualFree(IntPtr lpAddress, UIntPtr dwSize, FreeType dwFreeType);

//        public static void VirtualFree(IntPtr ptr)
//        {
//            bool success = VirtualFree(ptr, UIntPtr.Zero, FreeType.Release);
//            if (success == false)
//            {
//                int error = Marshal.GetLastWin32Error();
//                throw new Exception("error allocating large block: " + error);
//            }
//        }

//        #endregion

//        #region [ Adjust Security Policy To Allow SeLockMemoryPrivilege ]

//        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
//        internal static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges, ref TokenPrivileges newState, int bufferLength, IntPtr Null1, IntPtr Null2);

//        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
//        internal static extern IntPtr GetCurrentProcess();

//        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
//        internal static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, ref IntPtr tokenHandle);

//        [DllImport("advapi32.dll", SetLastError = true)]
//        internal static extern bool LookupPrivilegeValue(string systemName, string name, ref long luid);

//        [DllImport("kernel32.dll", SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        static extern bool CloseHandle(IntPtr hObject);

//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
//        internal struct TokenPrivileges
//        {
//            public int Count;
//            public long Luid;
//            public int Attr;
//        }

//        const int SePrivilegeEnabled = 0x00000002;
//        const int TokenQuery = 0x00000008;
//        const int TokenAdjustPrivileges = 0x00000020;
//        const string SeLockMemoryPrivilege = "SeLockMemoryPrivilege";

//        static void SetPrivilegeLockMemoryPrivilege()
//        {
//            IntPtr tokenHandle = IntPtr.Zero;
//            try
//            {
//                TokenPrivileges tokenPrivileges;
//                IntPtr currentProcessHandle = GetCurrentProcess();

//                //Opens a process token with AdjuctPrivileges and TokenQuery rights
//                bool success = OpenProcessToken(currentProcessHandle, TokenAdjustPrivileges | TokenQuery, ref tokenHandle);
//                if (success == false)
//                {
//                    int error = Marshal.GetLastWin32Error();
//                    throw new Exception("error allocating large block: " + error);
//                }

//                tokenPrivileges.Count = 1;
//                tokenPrivileges.Luid = 0;
//                tokenPrivileges.Attr = SePrivilegeEnabled;

//                success = LookupPrivilegeValue(null, SeLockMemoryPrivilege, ref tokenPrivileges.Luid);
//                if (success == false)
//                {
//                    int error = Marshal.GetLastWin32Error();
//                    throw new Exception("error allocating large block: " + error);
//                }

//                success = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);

//                int lastError = Marshal.GetLastWin32Error();
//                if (success == false || lastError != 0)
//                {
//                    throw new Exception("error allocating large block: " + lastError);
//                }
//            }
//            finally
//            {
//                bool success = CloseHandle(tokenHandle);
//                if (success == false)
//                {
//                    int error = Marshal.GetLastWin32Error();
//                    throw new Exception("error allocating large block: " + error);
//                }
//            }

//        }

//        #endregion

//    }
//}

