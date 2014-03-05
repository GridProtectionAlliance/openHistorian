//******************************************************************************************************
//  EncodingLibrary.cs - Gbtc
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
//  2/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Encoding
{
    /// <summary>
    /// Contains all of the fundamental encoding methods. Types implementing <see cref="SortedTreeTypeBase{T}"/>
    /// will automatically register when passed to one of the child methods. 
    /// </summary>
    public static class EncodingLibrary
    {
        private static readonly SingleEncodingDictionary<CreateSingleValueEncodingBase> SingleEncoding;
        private static readonly DualEncodingDictionary<CreateDoubleValueEncodingBase> DoubleEncoding;

        static EncodingLibrary()
        {
            SingleEncoding = new SingleEncodingDictionary<CreateSingleValueEncodingBase>();
            DoubleEncoding = new DualEncodingDictionary<CreateDoubleValueEncodingBase>();

            SingleEncoding.Register(new CreateFixedSizeSingleEncoding());
            DoubleEncoding.Register(new CreateFixedSizeCombinedEncoding());
            DoubleEncoding.Register(new CreateFixedSizeDualSingleEncoding());
        }

        /// <summary>
        /// Gets the single encoding method if it exists in the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encodingMethod"></param>
        /// <returns></returns>
        public static SingleValueEncodingBase<T> GetEncodingMethod<T>(Guid encodingMethod)
            where T : SortedTreeTypeBase<T>, new()
        {
            CreateSingleValueEncodingBase encoding;

            if (SingleEncoding.TryGetEncodingMethod<T>(encodingMethod, out encoding))
                return encoding.Create<T>();
            
            throw new Exception("Type is not registered");
        }

        /// <summary>
        /// Gets the Double encoding method
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="encodingMethod"></param>
        /// <returns></returns>
        public static DoubleValueEncodingBase<TKey, TValue> GetEncodingMethod<TKey, TValue>(EncodingDefinition encodingMethod)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            CreateDoubleValueEncodingBase encoding;

            if (DoubleEncoding.TryGetEncodingMethod<TKey, TValue>(encodingMethod, out encoding))
                return encoding.Create<TKey, TValue>();

            if (encodingMethod.IsKeyValueEncoded)
                throw new Exception("Type is not registered");

            return new DoubleValueEncodingSet<TKey, TValue>(encodingMethod);
        }
    }
}