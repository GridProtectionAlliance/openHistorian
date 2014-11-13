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
using GSF.Snap.Tree;

namespace GSF.Snap
{
    /// <summary>
    /// A library of <see cref="SnapTypeBase"/> types. This
    /// library will dynamically register types via reflection if possible.
    /// </summary>
    public static class Library
    {
        /// <summary>
        /// Gets all of the streaming like encoding.
        /// </summary>
        public static readonly StreamEncoding Streaming;
        /// <summary>
        /// Gets all of the encoding data.
        /// </summary>
        public static readonly EncodingLibrary Encodings;
        /// <summary>
        /// Gets all of the filters.
        /// </summary>
        public static readonly FilterLibrary Filters;

        /// <summary>
        /// Contains user definable tree nodes.
        /// </summary>
        public static readonly SortedTreeNodeInitializer SortedTreeNodes;

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
                Streaming = new StreamEncoding();
                Encodings = new EncodingLibrary();
                Filters = new FilterLibrary();
                SortedTreeNodes = new SortedTreeNodeInitializer();
                SyncRoot = new object();
                TypeLookup = new Dictionary<Guid, Type>();
                RegisteredType = new Dictionary<Type, Guid>();
                KeyValueMethodsList = new Dictionary<Tuple<Type, Type>, object>();

                FilterAssemblyNames.Add(typeof(StreamEncodingBaseDefinition).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(IndividualEncodingBaseDefinition).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(CombinedEncodingBaseDefinition).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(MatchFilterBaseDefinition).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(SeekFilterBaseDefinition).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(SnapTypeBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(KeyValueMethods).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(SortedTreeNodeBaseDefinition).Assembly.GetName().Name);

                ReloadNewAssemblies();
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;
            }
            catch (Exception ex)
            {
                Logger.RootSource.Publish(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
            }
        }

        private static void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            lock (SyncRoot)
            {
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
            var typeCreateStreamEncodingBase = typeof(StreamEncodingBaseDefinition);
            var typeCreateSingleValueEncodingBase = typeof(IndividualEncodingBaseDefinition);
            var typeCreateDoubleValueEncodingBase = typeof(CombinedEncodingBaseDefinition);
            var typeCreateFilterBase = typeof(MatchFilterBaseDefinition);
            var typeCreateSeekFilterBase = typeof(SeekFilterBaseDefinition);
            var typeSnapTypeBase = typeof(SnapTypeBase);
            var typeKeyValueMethods = typeof(KeyValueMethods);
            var typeSortedTreeNodeBaseDefinition = typeof(SortedTreeNodeBaseDefinition);

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
                                        types = ex.Types;
                                    }
                                    foreach (var assemblyType in types)
                                    {
                                        try
                                        {
                                            if ((object)assemblyType != null && !assemblyType.IsAbstract && !assemblyType.ContainsGenericParameters)
                                            {
                                                if (typeCreateStreamEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Streaming.Register((StreamEncodingBaseDefinition)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateSingleValueEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Encodings.Register((IndividualEncodingBaseDefinition)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateDoubleValueEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Encodings.Register((CombinedEncodingBaseDefinition)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateFilterBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Filters.Register((MatchFilterBaseDefinition)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateSeekFilterBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Filters.Register((SeekFilterBaseDefinition)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeSortedTreeNodeBaseDefinition.IsAssignableFrom(assemblyType))
                                                {
                                                    SortedTreeNodes.Register((SortedTreeNodeBaseDefinition)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeSnapTypeBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Register((SnapTypeBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeKeyValueMethods.IsAssignableFrom(assemblyType))
                                                {
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
                                            Logger.RootSource.Publish(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.RootSource.Publish(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.RootSource.Publish(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
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
        /// Registeres the generic type with the SortedTreeStore.
        /// </summary>
        private static void Register(SnapTypeBase snapType)
        {
            Type type = snapType.GetType();
            Guid id = snapType.GenericTypeGuid;

            lock (SyncRoot)
            {
                Type existingType;
                Guid existingID;

                if (RegisteredType.TryGetValue(type, out existingID))
                {
                    if (existingID != id)
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
