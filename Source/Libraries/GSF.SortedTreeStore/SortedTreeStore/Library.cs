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
//  5/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GSF.Diagnostics;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore
{
    /// <summary>
    /// A library of <see cref="SortedTreeTypeBase"/> types. This
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

        private static readonly object SyncRoot;
        private static readonly Dictionary<Guid, Type> TypeLookup;
        private static readonly Dictionary<Type, Guid> RegisteredType;

        private static readonly HashSet<string> FilterAssemblyNames;
        private static readonly HashSet<Assembly> LoadedAssemblies;

        static Library()
        {
            try
            {
                FilterAssemblyNames = new HashSet<string>();
                LoadedAssemblies = new HashSet<Assembly>();
                Streaming = new StreamEncoding();
                Encodings = new EncodingLibrary();
                Filters = new FilterLibrary();
                SyncRoot = new object();
                TypeLookup = new Dictionary<Guid, Type>();
                RegisteredType = new Dictionary<Type, Guid>();

                FilterAssemblyNames.Add(typeof(CreateStreamEncodingBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(CreateSingleValueEncodingBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(CreateDoubleValueEncodingBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(CreateFilterBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(CreateSeekFilterBase).Assembly.GetName().Name);
                FilterAssemblyNames.Add(typeof(SortedTreeTypeBase).Assembly.GetName().Name);

                ReloadNewAssemblies();
            }
            catch (Exception ex)
            {
                Logger.Default.UniversalReporter.LogMessage(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
            }
        }

        /// <summary>
        /// Will attempt to reload any type that 
        /// inherits from <see cref="SortedTreeTypeBase"/> in
        /// any new assemblies.
        /// </summary>
        static void ReloadNewAssemblies()
        {
            var typeCreateStreamEncodingBase = typeof(CreateStreamEncodingBase);
            var typeCreateSingleValueEncodingBase = typeof(CreateSingleValueEncodingBase);
            var typeCreateDoubleValueEncodingBase = typeof(CreateDoubleValueEncodingBase);
            var typeCreateFilterBase = typeof(CreateFilterBase);
            var typeCreateSeekFilterBase = typeof(CreateSeekFilterBase);
            var typeSortedTreeTypeBase = typeof(SortedTreeTypeBase);

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
                                    var types = module.GetTypes();
                                    foreach (var assemblyType in types)
                                    {
                                        try
                                        {
                                            if (!assemblyType.IsAbstract)
                                            {
                                                if (typeCreateStreamEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Streaming.Register((CreateStreamEncodingBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateSingleValueEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Encodings.Register((CreateSingleValueEncodingBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateDoubleValueEncodingBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Encodings.Register((CreateDoubleValueEncodingBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateFilterBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Filters.Register((CreateFilterBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeCreateSeekFilterBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Filters.Register((CreateSeekFilterBase)Activator.CreateInstance(assemblyType));
                                                }
                                                else if (typeSortedTreeTypeBase.IsAssignableFrom(assemblyType))
                                                {
                                                    Register((SortedTreeTypeBase)Activator.CreateInstance(assemblyType));
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Default.UniversalReporter.LogMessage(VerboseLevel.Fatal,
                                                "Static Constructor Error", typeof(Library).ToString(), null, ex);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Default.UniversalReporter.LogMessage(VerboseLevel.Fatal,
                                        "Static Constructor Error", typeof(Library).ToString(), null, ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Default.UniversalReporter.LogMessage(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
            }
        }

        /// <summary>
        /// Gets the <see cref="SortedTreeTypeBase"/> associated with the provided <see cref="id"/>.
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
        /// Registeres the generic type with the SortedTreeStore.
        /// </summary>
        private static void Register(SortedTreeTypeBase sortedTreeType)
        {
            Type type = sortedTreeType.GetType();
            Guid id = sortedTreeType.GenericTypeGuid;

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
