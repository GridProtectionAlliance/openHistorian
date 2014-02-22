//******************************************************************************************************
//  SortedTreeUInt32.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Types
{
    public class SortedTreeUInt32
        : ISortedTreeKey<SortedTreeUInt32>, ISortedTreeValue<SortedTreeUInt32>
    {
        public SortedTreeUInt32()
        {

        }
        public SortedTreeUInt32(uint value)
        {
            Value = value;
        }

        public uint Value;

        public SortedTreeKeyMethodsBase<SortedTreeUInt32> CreateKeyMethods()
        {
            return new SortedTreeKeyMethodsUInt32();
        }

        public SortedTreeValueMethodsBase<SortedTreeUInt32> CreateValueMethods()
        {
            return new SortedTreeValueMethodsUInt32();
        }

        void ISortedTreeValue<SortedTreeUInt32>.RegisterCustomValueImplementations()
        {
        }
        void ISortedTreeKey<SortedTreeUInt32>.RegisterCustomKeyImplementations()
        {
        }
    }
  
}