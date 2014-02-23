//******************************************************************************************************
//  KeyValueStreamCompression.cs - Gbtc
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

using System;
using System.Collections.Generic;
using System.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Net.Initialization
{
    //public delegate SortedTreeKeyMethodsBase CreateKeyMethod();

    //public delegate SortedTreeValueMethodsBase CreateValueMethod();

    public static class KeyValueStreamCompression
    {
        private static readonly object s_syncRoot;

        private static readonly Dictionary<Tuple<Guid, Type, Type>, CreateKeyValueStreamCompressionBase> s_compressedStreamKeyValue;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateKeyValueStreamCompressionBase> s_compressedStreamKey;
        private static readonly Dictionary<Guid, CreateKeyValueStreamCompressionBase> s_compressedStream;

        //static Dictionary<Type, SortedTreeValueMethodsBase> s_valueMethods;

        static KeyValueStreamCompression()
        {
            s_syncRoot = new object();
            s_compressedStreamKeyValue = new Dictionary<Tuple<Guid, Type, Type>, CreateKeyValueStreamCompressionBase>();
            s_compressedStreamKey = new Dictionary<Tuple<Guid, Type>, CreateKeyValueStreamCompressionBase>();
            s_compressedStream = new Dictionary<Guid, CreateKeyValueStreamCompressionBase>();

            RegisterKeyValueStreamCompressionTypes.RegisterKeyValueStreamTypes();
        }

        public static void Register(CreateKeyValueStreamCompressionBase streamCompression)
        {
            lock (s_syncRoot)
            {
                if (streamCompression.KeyTypeIfFixed == null && streamCompression.ValueTypeIfFixed == null)
                {
                    s_compressedStream.Add(streamCompression.GetTypeGuid, streamCompression);
                }
                else if (streamCompression.KeyTypeIfFixed != null && streamCompression.ValueTypeIfFixed == null)
                {
                    s_compressedStreamKey.Add(Tuple.Create(streamCompression.GetTypeGuid, streamCompression.KeyTypeIfFixed), streamCompression);
                }
                else if (streamCompression.KeyTypeIfFixed != null && streamCompression.ValueTypeIfFixed != null)
                {
                    s_compressedStreamKeyValue.Add(Tuple.Create(streamCompression.GetTypeGuid, streamCompression.KeyTypeIfFixed, streamCompression.ValueTypeIfFixed), streamCompression);
                }
                else
                {
                    throw new InvalidDataException("Type is not supported");
                }
            }
        }
        
        public static KeyValueStreamCompressionBase<TKey, TValue> CreateKeyValueStreamCompression<TKey, TValue>(Guid compressionMethod)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateKeyValueStreamCompressionBase createStreamCompression;
            lock (s_syncRoot)
            {
                if (!s_compressedStreamKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out createStreamCompression))
                    if (!s_compressedStreamKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out createStreamCompression))
                        if (!s_compressedStream.TryGetValue(compressionMethod, out createStreamCompression))
                        {
                            //new TKey().RegisterCustomKeyImplementations();
                            //new TValue().RegisterCustomValueImplementations();

                            if (!s_compressedStreamKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out createStreamCompression))
                                if (!s_compressedStreamKey.TryGetValue(Tuple.Create(compressionMethod, keyType),out createStreamCompression))
                                    if (!s_compressedStream.TryGetValue(compressionMethod, out createStreamCompression))
                                        throw new Exception("Type is not registered");
                        }
            }

            return createStreamCompression.Create<TKey,TValue>();
        }
    }
}