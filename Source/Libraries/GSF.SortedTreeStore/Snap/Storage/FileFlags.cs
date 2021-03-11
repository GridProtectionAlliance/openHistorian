//******************************************************************************************************
//  FileFlags.cs - Gbtc
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
//  04/02/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

namespace GSF.Snap.Storage
{
    /// <summary>
    /// A set of flags that are in the archive files.
    /// </summary>
    public static class FileFlags
    {
        /// <summary>
        /// Gets the flag associated with the supplied stage
        /// </summary>
        /// <param name="stageNumber">The stage number, must be between 0 and 9 inclusive.</param>
        /// <returns></returns>
        public static Guid GetStage(int stageNumber)
        {
            switch (stageNumber)
            {
                case 0:
                    return Stage0;
                case 1:
                    return Stage1;
                case 2:
                    return Stage2;
                case 3:
                    return Stage3;
                case 4:
                    return Stage4;
                case 5:
                    return Stage5;
                case 6:
                    return Stage6;
                case 7:
                    return Stage7;
                case 8:
                    return Stage8;
                case 9:
                    return Stage9;
            }
            throw new ArgumentOutOfRangeException("stageNumber", "Must be between 0 and 9");
        }

        // {4558F270-3F85-456C-8824-7805FF03B384}
        /// <summary>
        /// Indicates that the file is in Stage 0. These files are in memory only and not compressed.
        /// These files may be created multiple times per second, but typically are incrementally added to 10-100 times per second.
        /// </summary>
        public static readonly Guid Stage0 = new Guid(0x4558f270, 0x3f85, 0x456c, 0x88, 0x24, 0x78, 0x05, 0xff, 0x03, 0xb3, 0x84);

        // {AF019EB6-1BA7-40C4-AB2E-74C0C8619F75}
        /// <summary>
        /// Indicates that the file is in Stage 1. These files usually have been written to the disk, and they are compressed.
        /// </summary>
        public static readonly Guid Stage1 = new Guid(0xaf019eb6, 0x1ba7, 0x40c4, 0xab, 0x2e, 0x74, 0xc0, 0xc8, 0x61, 0x9f, 0x75);

        // {C0C1465A-4149-414E-ADF8-AEAD3254187C}
        /// <summary>
        /// Indicates that the file is in Stage 2. 
        /// </summary>
        public static readonly Guid Stage2 = new Guid(0xc0c1465a, 0x4149, 0x414e, 0xad, 0xf8, 0xae, 0xad, 0x32, 0x54, 0x18, 0x7c);

        // {1B192006-C3E8-48D8-8F20-1AEB45577DDD}
        /// <summary>
        /// Indicates that the file is in Stage 3. 
        /// </summary>
        public static readonly Guid Stage3 = new Guid(0x1b192006, 0xc3e8, 0x48d8, 0x8f, 0x20, 0x1a, 0xeb, 0x45, 0x57, 0x7d, 0xdd);

        // {8E325F41-0DB9-4E7C-9209-6DACF40D7C89}
        /// <summary>
        /// Indicates that the file is in Stage 4. 
        /// </summary>
        public static readonly Guid Stage4 = new Guid(0x8e325f41, 0x0db9, 0x4e7c, 0x92, 0x09, 0x6d, 0xac, 0xf4, 0x0d, 0x7c, 0x89);

        // {21C048D7-D51F-4372-B90B-AC859B0A406E}
        /// <summary>
        /// Indicates that the file is in Stage 5. 
        /// </summary>
        public static readonly Guid Stage5 = new Guid(0x21c048d7, 0xd51f, 0x4372, 0xb9, 0x0b, 0xac, 0x85, 0x9b, 0x0a, 0x40, 0x6e);

        // {23C433B7-90E9-48FB-8616-0392CAA528D9}
        /// <summary>
        /// Indicates that the file is in Stage 6. 
        /// </summary>
        public static readonly Guid Stage6 = new Guid(0x23c433b7, 0x90e9, 0x48fb, 0x86, 0x16, 0x03, 0x92, 0xca, 0xa5, 0x28, 0xd9);

        // {B2580D7C-102A-4B52-BC43-8E7078521AEA}
        /// <summary>
        /// Indicates that the file is in Stage 7. 
        /// </summary>
        public static readonly Guid Stage7 = new Guid(0xb2580d7c, 0x102a, 0x4b52, 0xbc, 0x43, 0x8e, 0x70, 0x78, 0x52, 0x1a, 0xea);

        // {69BE577B-C044-49C2-85B3-8F8FAD763803}
        /// <summary>
        /// Indicates that the file is in Stage 8. 
        /// </summary>
        public static readonly Guid Stage8 = new Guid(0x69be577b, 0xc044, 0x49c2, 0x85, 0xb3, 0x8f, 0x8f, 0xad, 0x76, 0x38, 0x03);

        /// <summary>
        /// Indicates that the file is in Stage 9.
        /// </summary>
        public static readonly Guid Stage9 = new Guid(0xb5a3fbc4, 0x285a, 0x4559, 0xa3, 0xea, 0x09, 0x40, 0x21, 0x96, 0x77, 0xf8);

        // {4AF94839-E4FA-4CBD-8B41-E8457274A594}
        /// <summary>
        /// Indicates that the user requested this file to be rolled over, and therefore it should not be automatically recombined.
        /// </summary>
        public static readonly Guid ManualRollover = new Guid(0x4af94839, 0xe4fa, 0x4cbd, 0x8b, 0x41, 0xe8, 0x45, 0x72, 0x74, 0xa5, 0x94);

        // {D4626375-3E2F-4A62-BC12-65BB45E4A7B6}
        /// <summary>
        /// Indicates that this is an intermediate file that can still be automatically rolled over.
        /// </summary>
        public static readonly Guid IntermediateFile = new Guid(0xd4626375, 0x3e2f, 0x4a62, 0xbc, 0x12, 0x65, 0xbb, 0x45, 0xe4, 0xa7, 0xb6);

    }
}
