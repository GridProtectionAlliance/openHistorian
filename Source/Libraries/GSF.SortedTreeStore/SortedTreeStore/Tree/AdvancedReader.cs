//******************************************************************************************************
//  AdvancedReader.cs - Gbtc
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
//  8/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Engine;
using GSF.SortedTreeStore.Filters;

namespace GSF.SortedTreeStore.Tree
{
    unsafe public class AdvanceReaderBase<TKey, TValue>
    {
        public byte* WritePointer;
        public byte* ReadPointer;

        public const int Quit = 1;
        public const int ContinueReading = 2;
        public const int RollbackPoint = 3;
    }

    unsafe public class AdvancedReader<TKey, TValue>
        : AdvanceReaderBase<TKey, TValue>
        where TKey : EngineKeyBase<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
      
        public int PointSize;
        public bool IsFull;

        /// <summary>
        /// Set this value to decide when to stop.
        /// </summary>
        public ulong StopOnTimestamp;

        public KeyMatchFilterBase<TKey> KeyFilter;

        public bool ReadTooFar;

        public int NextAction()
        {
            if (*(ulong*)WritePointer >= StopOnTimestamp)
                return RollbackPoint;

            if (!KeyFilter.Contains(WritePointer)) //If I should filter the point
            {
                WritePointer += PointSize;
            }

            if (IsFull)
                return Quit;

            return ContinueReading;
        }

        public bool FilterPoint()
        {
            return false;
        }


    }
}
