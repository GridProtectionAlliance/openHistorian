//******************************************************************************************************
//  ArchiveConstants.cs - Gbtc
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
//  12/3/2011 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// Maintains some global constants for the archive file.
    /// </summary>
    public static class ArchiveConstants
    {
        /// <summary>
        /// The largest supported file system size
        /// </summary>
        public const long MaxFileSystemSize = (long)BlockSize * int.MaxValue;
        /// <summary>
        /// The number of bytes in a block. (the smallest unit of data space).
        /// </summary>
        public const int BlockSize = 4096;
        /// <summary>
        /// The number of addresses that can fit in each indirect block
        /// </summary>
        public const int AddressesPerBlock = IndexIndirectBlockDataLength / 4; //rounds down
        /// <summary>
        /// The number of addresses that can fit in a double indirect block
        /// </summary>
        public const long AddressesPerBlockSquare = (long)AddressesPerBlock * (long)AddressesPerBlock;
        /// <summary>
        /// The number of addresses that can fit in a triple indirect block
        /// </summary>
        public const long AddressesPerBlockCube = (long)AddressesPerBlock * (long)AddressesPerBlock * (long)AddressesPerBlock;
        /// <summary>
        /// The number of bytes in the footer of the file allocation table block
        /// </summary>
        public const int FileAllocationTableFooterLength = 21;
        /// <summary>
        /// The number of data bytes available for a file allocation table block
        /// </summary>
        public const int FileAllocationTableDataLength = BlockSize - FileAllocationTableFooterLength;
        /// <summary>
        /// The number of bytes in the footer of the indirect block
        /// </summary>
        public const int IndexIndirectBlockFooterLength = 22;
        /// <summary>
        /// The number of data bytes available for a indirect block
        /// </summary>
        public const int IndexIndirectBlockDataLength = BlockSize - IndexIndirectBlockFooterLength;
        /// <summary>
        /// The number of bytes in the footer of a data block
        /// </summary>
        public const int DataBlockFooterLength = 21;
        /// <summary>
        /// The number of data bytes available in a data block
        /// </summary>
        public const int DataBlockDataLength = BlockSize - DataBlockFooterLength;

        const long TmpFirstDoubleIndirectBlockIndex = FirstSingleIndirectBlockIndex + (long)AddressesPerBlock;
        const long TmpFirstTripleIndirectIndex = FirstDoubleIndirectBlockIndex + (long)AddressesPerBlock * (long)AddressesPerBlock;
        const long TmpFirstQuadrupleIndirectBlockIndex = FirstTripleIndirectIndex + (long)AddressesPerBlock * (long)AddressesPerBlock * (long)AddressesPerBlock;

        /// <summary>
        /// The index of the first block in the single indirect blocks
        /// </summary>
        public const int FirstSingleIndirectBlockIndex = 1;
        /// <summary>
        /// The index of the first block in the double indirect blocks
        /// </summary>
        public const int FirstDoubleIndirectBlockIndex = (int)(TmpFirstDoubleIndirectBlockIndex > int.MaxValue ? int.MaxValue : TmpFirstDoubleIndirectBlockIndex);
        /// <summary>
        /// The index of the first block in the triple indirect blocks
        /// </summary>
        public const int FirstTripleIndirectIndex = (int)(TmpFirstTripleIndirectIndex > int.MaxValue ? int.MaxValue : TmpFirstTripleIndirectIndex);
        /// <summary>
        /// The index of the first block in the quadruple indirect blocks
        /// </summary>
        public const int FirstQuadrupleIndirectBlockIndex = (int)(TmpFirstQuadrupleIndirectBlockIndex > int.MaxValue ? int.MaxValue : TmpFirstQuadrupleIndirectBlockIndex);
    }
}
