//******************************************************************************************************
//  Library.cs - Gbtc
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
//  05/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GSF.Diagnostics;
using GSF.Snap.Definitions;
using GSF.Snap.Encoding;
using GSF.Snap.Filters;
using GSF.Snap.Streaming;
using GSF.Snap.Tree;

namespace GSF.Snap
{
    /// <summary>
    /// A library of <see cref="SnapTypeBase"/> types. This
    /// library will dynamically register types via reflection if possible.
    /// </summary>
    public static class Library
    {
        private static readonly LogType Log = Logger.LookupType(typeof(Library));

        /// <summary>
        /// Gets all of the encoding data.
        /// </summary>
        public static readonly EncodingLibrary Encodings;
        /// <summary>
        /// Gets all of the filters.
        /// </summary>
        public static readonly FilterLibrary Filters;

        private static readonly object SyncRoot;
        private static readonly Dictionary<Guid, Type> TypeLookup;
        private static readonly Dictionary<Type, Guid> RegisteredType;

        /// <summary>
        /// The assembly must reference one of these assembly names in order to be scanned for matching types.
        /// </summary>
        private static readonly HashSet<string> FilterAssemblyNames;
        private static readonly HashSet<Assembly> LoadedAssemblies;

        private static readonly Dictionary<Tuple<Type, Type>, object> KeyValueMethodsList;

        static Library()
        {
            try
            {
                FilterAssemblyNames = new HashSet<string>();
                LoadedAssemblies = new HashSet<Assembly>();
                Encodings = new EncodingLibrary();
                Filters = new FilterLibrary();
                SyncRoot = new object();
                TypeLookup = new Dictionary<Guid, Type>();
                RegisteredType = new Dictionary<Type, Guid>();
                KeyValueMethodsList = new Dictionary<Tuple<Type, Type>, object>();

                FilterAssemblyNames.Add(typeof(IndividualEncodingDefinitionBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(PairEncodingDefinitionBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(MatchFilterDefinitionBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(SeekFilterDefinitionBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(SnapTypeBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(KeyValueMethods).Assembly.GetName().Name);

                ReloadNewAssemblies();
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;
            }
            catch (Exception ex)
            {
                Log.Publish(VerboseLevel.Fatal, "Static Constructor Error", null, null, ex);
            }
        }

        private static void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            lock (SyncRoot)
            {
                Log.Publish(VerboseLevel.DebugNormal, "Reloading Assembly", args.LoadedAssembly.FullName);
                ReloadNewAssemblies();
            }
        }

        /// <summary>
        /// Will attempt to reload any type that 
        /// inherits from <see cref="SnapTypeBase"/> in
        /// any new assemblies.
        /// </summary>
        private static void ReloadNewAssemblies()
        {
            var typeCreateSingleValueEncodingBase = typeof(IndividualEncodingDefinitionBase);
            var typeCreateDoubleValueEncodingBase = typeof(PairEncodingDefinitionBase);
            var typeCreateFilterBase = typeof(MatchFilterDefinitionBase);
            var typeCreateSeekFilterBase = typeof(SeekFilterDefinitionBase);
            var typeSnapTypeBase = typeof(SnapTypeBase);
            var typeKeyValueMethods = typeof(KeyValueMethods);

            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (!LoadedAssemblies.Contains(assembly))
                    {
                        LoadedAssemblies.Add(assembly);

                        if (FilterAssemblyNames.Contains(assembly.GetName().Name) || assembly.GetReferencedAssemblies().Any(x => FilterAssemblyNames.Contains(x.Name)))
                        {
                            Log.Publish(VerboseLevel.DebugNormal, "Loading Assembly", assembly.GetName().Name);

                            var modules = assembly.GetModules(false);
                            foreach (var module in modules)
                            {
                                try
                                {
                                    Type[] types;
                                    try
                                    {
                                        types = module.GetTypes();
                                    }
                                    catch (ReflectionTypeLoadException ex)
                                    {
                                        Log.Publish(VerboseLevel.DebugHigh, "Reflection Load Error Occurred",
                                            assembly.GetName().Name, ex.ToString() + Environment.NewLine +
                                            String.Join(Environment.NewLine, ex.LoaderExceptions.Select(x => x.ToString())));
                                        types = ex.Types;
                                    }
                                    foreach (var assemblyType in types)
                                    {
                                        try
                                        {
                                            if ((object)assemblyType != null && !assemblyType.IsAbstract && !assemblyType.ContainsGenericParameters)
                                            {
                                                if (typeCreateSingleValueEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Log.Publish(VerboseLevel.DebugNormal, "Loading Individual Encoding Method", assemblyType.AssemblyQualifiedName);
                                                    Encodings.Register((IndividualEncodingDefinitionBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateDoubleValueEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Log.Publish(VerboseLevel.DebugNormal, "Loading Pair Encoding Method", assemblyType.AssemblyQualifiedName);
                                                    Encodings.Register((PairEncodingDefinitionBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateFilterBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Log.Publish(VerboseLevel.DebugNormal, "Loading Match Filter", assemblyType.AssemblyQualifiedName);
                                                    Filters.Register((MatchFilterDefinitionBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateSeekFilterBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Log.Publish(VerboseLevel.DebugNormal, "Loading Seek Filter", assemblyType.AssemblyQualifiedName);
                                                    Filters.Register((SeekFilterDefinitionBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeSnapTypeBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Log.Publish(VerboseLevel.DebugNormal, "Loading Snap Type", assemblyType.AssemblyQualifiedName);
                                                    Register((SnapTypeBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeKeyValueMethods.IsAssignableFrom(assemblyType))
                                                {
                                                    Log.Publish(VerboseLevel.DebugNormal, "Loading Key Value Methods", assemblyType.AssemblyQualifiedName);
                                                    var obj = (KeyValueMethods)Activator.CreateInstance(assemblyType);
                                                    var ttypes = Tuple.Create(obj.KeyType, obj.ValueType);
                                                    if (!KeyValueMethodsList.ContainsKey(ttypes))
                                                    {
                                                        KeyValueMethodsList.Add(ttypes, obj);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Publish(VerboseLevel.Fatal, "Static Constructor Error", null, null, ex);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Publish(VerboseLevel.Fatal, "Static Constructor Error", null, null, ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Publish(VerboseLevel.Fatal, "Static Constructor Error", null, null, ex);
            }
        }

        /// <summary>
        /// Gets the <see cref="SnapTypeBase"/> associated with the provided <see cref="id"/>.
        /// </summary>
        /// <param name="id">the ID to lookup</param>
        /// <returns></returns>
        public static Type GetSortedTreeType(Guid id)
        {
            lock (SyncRoot)
            {
                return TypeLookup[id];
            }
        }

        /// <summary>
        /// Gets a set of KeyValueMethods.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static KeyValueMethods<TKey, TValue> GetKeyValueMethods<TKey, TValue>()
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            var t = Tuple.Create(typeof(TKey), typeof(TValue));
            lock (SyncRoot)
            {
                object obj;
                if (KeyValueMethodsList.TryGetValue(t, out obj))
                {
                    return (KeyValueMethods<TKey, TValue>)obj;
                }
            }
            return new KeyValueMethods<TKey, TValue>();
        }

        /// <summary>
        /// Creates a stream encoding from the provided <see cref="encodingMethod"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="encodingMethod">the encoding method</param>
        /// <returns></returns>
        internal static StreamEncodingBase<TKey, TValue> CreateStreamEncoding<TKey, TValue>(EncodingDefinition encodingMethod)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            return new StreamEncodingGeneric<TKey, TValue>(encodingMethod);
        }

        internal static SortedTreeNodeBase<TKey, TValue> CreateTreeNode<TKey, TValue>(EncodingDefinition encodingMethod, byte level)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if ((object)encodingMethod == null)
                throw new ArgumentNullException("encodingMethod");

            if (encodingMethod.IsFixedSizeEncoding)
                return new FixedSizeNode<TKey, TValue>(level);

            return new GenericEncodedNode<TKey, TValue>(Encodings.GetEncodingMethod<TKey, TValue>(encodingMethod), level);
        }

        /// <summary>
        /// Registers the generic type with the SortedTreeStore.
        /// </summary>
        private static void Register(SnapTypeBase snapType)
        {
            Type type = snapType.GetType();
            Guid id = snapType.GenericTypeGuid;

            lock (SyncRoot)
            {
                Type existingType;
                Guid existingId;

                if (RegisteredType.TryGetValue(type, out existingId))
                {
                    if (existingId != id)
                        throw new Exception("Existing type does not match Guid: " + type.FullName + " ID: " + id.ToString());

                    //Type is already registered.
                    return;
                }

                if (TypeLookup.TryGetValue(id, out existingType))
                {
                    if (existingType != type)
                        throw new Exception("Existing type does not have a unique Guid. Type1:" + type.FullName + " Type2: " + existingType.FullName + " ID: " + id.ToString());

                    //Type is already registered.
                    return;
                }

                RegisteredType.Add(type, id);
                TypeLookup.Add(id, type);
            }
        }
    }
}
