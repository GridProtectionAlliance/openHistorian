//******************************************************************************************************
//  SortedTreeKeyMethodsUInt32.cs - Gbtc
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
//  04/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace GSF.Snap.Types
{
    public class SnapCustomMethodsUInt32
        : SnapTypeCustomMethods<SnapUInt32>
    {

        public override unsafe int BinarySearch(byte* pointer, SnapUInt32 key2, int recordCount, int keyValueSize)
        {
            int lastFoundIndex = LastFoundIndex;
            uint key = key2.Value;

            //shortcut for sequentially adding. 
            if (lastFoundIndex == recordCount - 1)
            {
                if (key > *(uint*)(pointer + keyValueSize * lastFoundIndex)) //Key > CompareKey
                {
                    LastFoundIndex++;
                    return ~recordCount;
                }
            }
            //Shortcut for sequentially getting  
            else if (lastFoundIndex < recordCount)
            {
                if (key == *(uint*)(pointer + keyValueSize * (lastFoundIndex + 1)))
                {
                    LastFoundIndex++;
                    return lastFoundIndex + 1;
                }
            }

            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = recordCount - 1;
            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);

                uint compareKey = *(uint*)(pointer + keyValueSize * currentTestIndex);

                if (key == compareKey) //Are Equal
                {
                    LastFoundIndex = currentTestIndex;
                    return currentTestIndex;
                }
                if (key > compareKey) //Key > CompareKey
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            LastFoundIndex = searchLowerBoundsIndex;
            return ~searchLowerBoundsIndex;
        }
    }
}