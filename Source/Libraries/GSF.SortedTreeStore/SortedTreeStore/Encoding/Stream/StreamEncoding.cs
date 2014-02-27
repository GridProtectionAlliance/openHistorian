//******************************************************************************************************
//  StreamEncoding.cs - Gbtc
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
//******************************************************************************************************

using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Encoding
{
    //public delegate SortedTreeKeyMethodsBase CreateKeyMethod();

    //public delegate SortedTreeValueMethodsBase CreateValueMethod();

    public static class StreamEncoding
    {

        private static readonly DualEncodingDictionary<CreateStreamEncodingBase> DoubleEncoding;

        //static Dictionary<Type, SortedTreeValueMethodsBase> s_valueMethods;

        static StreamEncoding()
        {
            DoubleEncoding = new DualEncodingDictionary<CreateStreamEncodingBase>();

        }
        
        public static void Register(CreateStreamEncodingBase encoding)
        {
            DoubleEncoding.Register(encoding);
        }
        
        internal static StreamEncodingBase<TKey, TValue> CreateStreamEncoding<TKey, TValue>(EncodingDefinition encodingMethod)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            CreateStreamEncodingBase encoding;

            if (DoubleEncoding.TryGetEncodingMethod<TKey, TValue>(encodingMethod, out encoding))
                return encoding.Create<TKey, TValue>();
            
            return new GenericStreamEncoding<TKey, TValue>(encodingMethod);
        }
       
    }
}