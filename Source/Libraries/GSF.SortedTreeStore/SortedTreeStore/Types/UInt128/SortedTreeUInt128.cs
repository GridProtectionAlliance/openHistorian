//******************************************************************************************************
//  SortedTreeUInt128.cs - Gbtc
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
using System.Collections;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Types
{
    public class SortedTreeUInt128
        : ISortedTreeKey<SortedTreeUInt128>, ISortedTreeValue<SortedTreeUInt128>
    {
        public ulong Value1;
        public ulong Value2;
        public SortedTreeKeyMethodsBase<SortedTreeUInt128> CreateKeyMethods()
        {
            return new SortedTreeKeyMethodsUInt128();
        }

        public SortedTreeMethodsBase<SortedTreeUInt128> CreateValueMethods()
        {
            return new SortedTreeValueMethodsUInt128();
        }

        public IEnumerable GetEncodingMethods()
        {
            return null;
        }

        public SortedTreeMethodsBase<SortedTreeUInt128> CreateBasicMethods()
        {
            return CreateValueMethods();
        }
    }

}