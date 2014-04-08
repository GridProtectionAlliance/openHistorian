//******************************************************************************************************
//  FileFlags.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  4/2/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

namespace GSF.SortedTreeStore.Storage
{
    /// <summary>
    /// A set of flags that are in the archive files.
    /// </summary>
    public static class FileFlags
    {
        // {4558F270-3F85-456C-8824-7805FF03B384}
        /// <summary>
        /// Indicates that the file is in Stage 0. These files are in memory only and not compressed.
        /// These files may be created multiple times per second, but typically are incrementally added to 10-100 times per second.
        /// </summary>
        public static readonly Guid Stage0 = new Guid(0x4558f270, 0x3f85, 0x456c, 0x88, 0x24, 0x78, 0x05, 0xff, 0x03, 0xb3, 0x84);

        // {AF019EB6-1BA7-40C4-AB2E-74C0C8619F75}
        /// <summary>
        /// Indicates that the file is in Stage 1. These files usually have been written to the disk, and they are compressed.
        /// These files are created every few seconds
        /// </summary>
        public static readonly Guid Stage1 = new Guid(0xaf019eb6, 0x1ba7, 0x40c4, 0xab, 0x2e, 0x74, 0xc0, 0xc8, 0x61, 0x9f, 0x75);

        // {C0C1465A-4149-414E-ADF8-AEAD3254187C}
        /// <summary>
        /// Indicates that the file is in Stage 2. 
        /// These files are created every few minutes
        /// </summary>
        public static readonly Guid Stage2 = new Guid(0xc0c1465a, 0x4149, 0x414e, 0xad, 0xf8, 0xae, 0xad, 0x32, 0x54, 0x18, 0x7c);

        // {1B192006-C3E8-48D8-8F20-1AEB45577DDD}
        /// <summary>
        /// Indicates that the file is in Stage 3. 
        /// These files are created one or two per hour and generally have a desired maximum size.
        /// </summary>
        public static readonly Guid Stage3 = new Guid(0x1b192006, 0xc3e8, 0x48d8, 0x8f, 0x20, 0x1a, 0xeb, 0x45, 0x57, 0x7d, 0xdd);

        // {4AF94839-E4FA-4CBD-8B41-E8457274A594}
        /// <summary>
        /// Indicates that the user requested this file to be rolled over, and therefore it should not be automatically recombined.
        /// </summary>
        public static readonly Guid ManualRollover = new Guid(0x4af94839, 0xe4fa, 0x4cbd, 0x8b, 0x41, 0xe8, 0x45, 0x72, 0x74, 0xa5, 0x94);


    }
}
