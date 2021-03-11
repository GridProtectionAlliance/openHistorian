//******************************************************************************************************
//  InsertStreamHelper`2.cs - Gbtc
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
//  04/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace GSF.Snap.Tree
{

    public class InsertStreamHelper<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        public bool IsStillSequential;
        public bool IsValid;
        /// <summary>
        /// Determines if Key1 and Value1 are the current keys.
        /// Otherwise Key1 and Value2 are.
        /// </summary>
        public bool IsKVP1;
        public TKey Key1;
        public TKey Key2;
        public TValue Value1;
        public TValue Value2;

        public TreeStream<TKey, TValue> Stream;

        public TKey Key => IsKVP1 ? Key1 : Key2;

        public TValue Value => IsKVP1 ? Value1 : Value2;

        public TKey PrevKey => IsKVP1 ? Key2 : Key1;

        public TValue PrevValue => IsKVP1 ? Value2 : Value1;

        public InsertStreamHelper(TreeStream<TKey, TValue> stream)
        {
            Stream = stream;
            Key1 = new TKey();
            Key2 = new TKey();
            Value1 = new TValue();
            Value2 = new TValue();
            IsKVP1 = false;
            IsStillSequential = true;

            if (IsKVP1)
            {
                IsValid = Stream.Read(Key2, Value2);
                IsKVP1 = false;
            }
            else
            {
                IsValid = Stream.Read(Key1, Value1);
                IsKVP1 = true;
            }
        }

        public void Next()
        {
            if (IsKVP1)
            {
                IsValid = Stream.Read(Key2, Value2);
                IsStillSequential = Key1.IsLessThan( Key2);
                IsKVP1 = false;
            }
            else
            {
                IsValid = Stream.Read(Key1, Value1);
                IsStillSequential = Key2.IsLessThan( Key1);
                IsKVP1 = true;
            }
        }
        public void NextDoNotCheckSequential()
        {
            if (IsKVP1)
            {
                IsValid = Stream.Read(Key2, Value2);
                IsStillSequential = false;
                IsKVP1 = false;
            }
            else
            {
                IsValid = Stream.Read(Key1, Value1);
                IsStillSequential = false;
                IsKVP1 = true;
            }
        }

    }
}
