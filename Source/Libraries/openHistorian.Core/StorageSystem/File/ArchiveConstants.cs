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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.File
{
    public class ArchiveConstants
    {
        public const long MaxFileSystemSize = (long)BlockSize * UInt32.MaxValue;
        public const int BlockSize = 4096;
        public const int AddressesPerBlock = IndexIndirectBlockDataLength / 4; //rounds down
        public const long AddressesPerBlockSquare = (long)AddressesPerBlock * (long)AddressesPerBlock;
        public const long AddressesPerBlockCube = (long)AddressesPerBlock * (long)AddressesPerBlock * (long)AddressesPerBlock;
        public const int FileAllocationTableFooterLength = 21;
        public const int FileAllocationTableDataLength = BlockSize-FileAllocationTableFooterLength;
        public const int IndexIndirectBlockFooterLength = 22;
        public const int IndexIndirectBlockDataLength = BlockSize-IndexIndirectBlockFooterLength;
        public const int DataBlockFooterLength = 21;
        public const int DataBlockDataLength = BlockSize-DataBlockFooterLength;
        
        const long _LastSingleIndirectBlockIndex = LastDirectBlockIndex + (long)AddressesPerBlock;
        const long _LastDoubleIndirectBlockIndex = LastSingleIndirectBlockIndex + (long)AddressesPerBlock * (long)AddressesPerBlock;
        const long _LastTripleIndirectBlockIndex = LastDoubleIndirectBlockIndex + (long)AddressesPerBlock * (long)AddressesPerBlock * (long)AddressesPerBlock;
        const long _LastQuadrupleIndirectBlockIndex = LastTripleIndirectBlockIndex + (long)AddressesPerBlock * (long)AddressesPerBlock * (long)AddressesPerBlock * (long)AddressesPerBlock;

        public const uint LastDirectBlockIndex = 1;
        public const uint LastSingleIndirectBlockIndex = (uint)(_LastSingleIndirectBlockIndex > uint.MaxValue ? uint.MaxValue : _LastSingleIndirectBlockIndex);
        public const uint LastDoubleIndirectBlockIndex = (uint)(_LastDoubleIndirectBlockIndex > uint.MaxValue ? uint.MaxValue : _LastDoubleIndirectBlockIndex);
        public const uint LastTripleIndirectBlockIndex = (uint)(_LastTripleIndirectBlockIndex > uint.MaxValue ? uint.MaxValue : _LastTripleIndirectBlockIndex);
        public const uint LastQuadrupleIndirectBlockIndex = (uint)(_LastQuadrupleIndirectBlockIndex > uint.MaxValue ? uint.MaxValue : _LastQuadrupleIndirectBlockIndex);

        public static void Test()
        {

        }
             
    }
}
