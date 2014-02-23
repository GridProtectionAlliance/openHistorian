//******************************************************************************************************
//  EncodingMethodsLibrary.cs - Gbtc
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
using System.Collections;
using System.Collections.Generic;

namespace GSF.SortedTreeStore.Encoding
{
    /// <summary>
    /// Contains all of the fundamental encoding methods. Types implementing <see cref="ISupportsCustomEncoding"/>
    /// will automatically register when passed to one of the child methods. 
    /// </summary>
    public static class EncodingMethodsLibrary
    {
        private static readonly object SyncRoot;

        private static readonly HashSet<Type> RegisteredTypes;
        private static readonly Dictionary<Guid, CreateSingleValueBase> SingleEncoding;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateSingleValueBase> TypedSingleEncoding;

        private static readonly Dictionary<Guid, CreateCombinedValuesBase> CombinedEncoding;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateCombinedValuesBase> KeyTypedCombinedEncoding;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateCombinedValuesBase> ValueTypedCombinedEncoding;
        private static readonly Dictionary<Tuple<Guid, Type, Type>, CreateCombinedValuesBase> KeyValueTypedCombinedEncoding;

        private static readonly Dictionary<Tuple<Guid, Guid>, CreateDualSingleValueBase> DualSingleEncoding;
        private static readonly Dictionary<Tuple<Guid, Guid, Type>, CreateDualSingleValueBase> KeyTypedDualSingleEncoding;
        private static readonly Dictionary<Tuple<Guid, Guid, Type>, CreateDualSingleValueBase> ValueTypedDualSingleEncoding;
        private static readonly Dictionary<Tuple<Guid, Guid, Type, Type>, CreateDualSingleValueBase> KeyValueTypedDualSingleEncoding;

        static EncodingMethodsLibrary()
        {
            SyncRoot = new object();
            RegisteredTypes = new HashSet<Type>();
            SingleEncoding = new Dictionary<Guid, CreateSingleValueBase>();
            TypedSingleEncoding = new Dictionary<Tuple<Guid, Type>, CreateSingleValueBase>();
            CombinedEncoding = new Dictionary<Guid, CreateCombinedValuesBase>();
            KeyTypedCombinedEncoding = new Dictionary<Tuple<Guid, Type>, CreateCombinedValuesBase>();
            ValueTypedCombinedEncoding = new Dictionary<Tuple<Guid, Type>, CreateCombinedValuesBase>();
            KeyValueTypedCombinedEncoding = new Dictionary<Tuple<Guid, Type, Type>, CreateCombinedValuesBase>();
            DualSingleEncoding = new Dictionary<Tuple<Guid, Guid>, CreateDualSingleValueBase>();
            KeyTypedDualSingleEncoding = new Dictionary<Tuple<Guid, Guid, Type>, CreateDualSingleValueBase>();
            ValueTypedDualSingleEncoding = new Dictionary<Tuple<Guid, Guid, Type>, CreateDualSingleValueBase>();
            KeyValueTypedDualSingleEncoding = new Dictionary<Tuple<Guid, Guid, Type, Type>, CreateDualSingleValueBase>();

            Register(new CreateFixedSizeSingleEncoding());
            Register(new CreateFixedSizeCombinedEncoding());
            Register(new CreateFixedSizeDualSingleEncoding());
        }

        public static void Register(ISupportsCustomEncoding type)
        {
            lock (SyncRoot)
            {
                if (RegisteredTypes.Add(type.GetType()))
                {
                    IEnumerable encodingMethods = type.GetEncodingMethods();
                    if (encodingMethods == null)
                        return;

                    foreach (var method in encodingMethods)
                    {
                        var single = method as CreateSingleValueBase;
                        var dual = method as CreateDualSingleValueBase;
                        var combined = method as CreateCombinedValuesBase;

                        if (single != null)
                            Register(single);
                        else if (dual != null)
                            Register(dual);
                        else if (combined != null)
                            Register(combined);
                    }
                }
            }
        }

        public static void Register(CreateSingleValueBase encoding)
        {
            lock (SyncRoot)
            {
                if (encoding.TypeIfNotGeneric == null)
                {
                    SingleEncoding.Add(encoding.Method, encoding);
                }
                else
                {
                    TypedSingleEncoding.Add(Tuple.Create(encoding.Method, encoding.TypeIfNotGeneric), encoding);
                }
            }
        }

        public static void Register(CreateCombinedValuesBase encoding)
        {
            lock (SyncRoot)
            {
                if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric == null)
                {
                    CombinedEncoding.Add(encoding.Method, encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric != null && encoding.ValueTypeIfNotGeneric == null)
                {
                    KeyTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.KeyTypeIfNotGeneric), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric != null)
                {
                    ValueTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.ValueTypeIfNotGeneric),
                                                   encoding);
                }
                else
                {
                    KeyValueTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.KeyTypeIfNotGeneric, encoding.ValueTypeIfNotGeneric), encoding);
                }
            }
        }

        public static void Register(CreateDualSingleValueBase encoding)
        {
            lock (SyncRoot)
            {
                if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric == null)
                {
                    DualSingleEncoding.Add(Tuple.Create(encoding.KeyMethod, encoding.ValueMethod), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric != null && encoding.ValueTypeIfNotGeneric == null)
                {
                    KeyTypedDualSingleEncoding.Add(
                        Tuple.Create(encoding.KeyMethod, encoding.ValueMethod, encoding.KeyTypeIfNotGeneric), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric != null)
                {
                    ValueTypedDualSingleEncoding.Add(Tuple.Create(encoding.KeyMethod, encoding.ValueMethod, encoding.ValueTypeIfNotGeneric), encoding);
                }
                else
                {
                    KeyValueTypedDualSingleEncoding.Add(Tuple.Create(encoding.KeyMethod, encoding.ValueMethod, encoding.KeyTypeIfNotGeneric, encoding.ValueTypeIfNotGeneric), encoding);
                }
            }
        }

        public static SingleValueEncodingBase<T> GetEncodingMethod<T>(Guid compressionMethod)
            where T : class, ISupportsCustomEncoding, ISortedTreeType<T>, new()
        {
            Type valueType = typeof(T);
            CreateSingleValueBase customEncoding;

            lock (SyncRoot)
            {
                if (!RegisteredTypes.Contains(valueType))
                {
                    Register(new T());
                }
                if (TypedSingleEncoding.TryGetValue(Tuple.Create(compressionMethod, valueType), out customEncoding)
                    || SingleEncoding.TryGetValue(compressionMethod, out customEncoding))
                {
                    return customEncoding.Create<T>();
                }
            }
            throw new Exception("Type is not registered");
        }

        public static DoubleValueEncodingBase<TKey, TValue> GetEncodingMethod<TKey, TValue>(Guid compressionMethod)
            where TKey : class, ISupportsCustomEncoding, ISortedTreeType<TKey>, new()
            where TValue : class, ISupportsCustomEncoding, ISortedTreeType<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateCombinedValuesBase customEncoding;

            lock (SyncRoot)
            {
                if (!RegisteredTypes.Contains(keyType))
                {
                    Register(new TKey());
                }
                if (!RegisteredTypes.Contains(valueType))
                {
                    Register(new TValue());
                }

                if (KeyValueTypedCombinedEncoding.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out customEncoding)
                    || KeyTypedCombinedEncoding.TryGetValue(Tuple.Create(compressionMethod, keyType), out customEncoding)
                    || ValueTypedCombinedEncoding.TryGetValue(Tuple.Create(compressionMethod, valueType), out customEncoding)
                    || CombinedEncoding.TryGetValue(compressionMethod, out customEncoding))
                {
                    return customEncoding.Create<TKey, TValue>();
                }
            }
            throw new Exception("Type is not registered");

        }

        public static DoubleValueEncodingBase<TKey, TValue> GetEncodingMethod<TKey, TValue>(Guid keyEncodingMethod, Guid valueEncodingMethod)
            where TKey : class, ISupportsCustomEncoding, ISortedTreeType<TKey>, new()
            where TValue : class, ISupportsCustomEncoding, ISortedTreeType<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateDualSingleValueBase customEncoding;

            lock (SyncRoot)
            {
                if (!RegisteredTypes.Contains(keyType))
                {
                    Register(new TKey());
                }
                if (!RegisteredTypes.Contains(valueType))
                {
                    Register(new TValue());
                }

                if (KeyValueTypedDualSingleEncoding.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod, keyType, valueType), out customEncoding)
                    || KeyTypedDualSingleEncoding.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod, keyType), out customEncoding)
                    || ValueTypedDualSingleEncoding.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod, valueType), out customEncoding)
                    || DualSingleEncoding.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod), out customEncoding))
                {
                    return customEncoding.Create<TKey, TValue>();
                }
            }
            return new DoubleValueEncodingSet<TKey, TValue>(GetEncodingMethod<TKey>(keyEncodingMethod), GetEncodingMethod<TValue>(valueEncodingMethod));
        }
    }
}