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
using System.Reflection;
using System.Runtime.CompilerServices;
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
        public static readonly StreamEncoding Streaming;
        public static readonly EncodingLibrary Encodings;
        public static readonly FilterLibrary Filters;

        private static readonly object SyncRoot;
        private static readonly Dictionary<Guid, Type> TypeLookup;
        private static readonly Dictionary<Type, Guid> RegisteredType;

        static Library()
        {
            try
            {
                Streaming = new StreamEncoding();
                Encodings = new EncodingLibrary();
                Filters = new FilterLibrary();
                SyncRoot = new object();
                TypeLookup = new Dictionary<Guid, Type>();
                RegisteredType = new Dictionary<Type, Guid>();

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
            var sortedTreeType = typeof(SortedTreeTypeBase);
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
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
                                    if (!assemblyType.IsAbstract && sortedTreeType.IsAssignableFrom(assemblyType))
                                    {
                                        Register(assemblyType);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Default.UniversalReporter.LogMessage(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Default.UniversalReporter.LogMessage(VerboseLevel.Fatal, "Static Constructor Error", typeof(Library).ToString(), null, ex);
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
        /// Registers the provided type with the SortedTreeStore. 
        /// If they type does not inherit from <see cref="SortedTreeTypeBase"/> or
        /// does not have a default initializer, then this method will fail.
        /// </summary>
        /// <param name="type">the type to register</param>
        public static void Register(Type type)
        {
            Type library = typeof(Library);
            MethodInfo method = library.GetMethod("InternalRegister", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo reflectionMethod = method.MakeGenericMethod(type);
            reflectionMethod.Invoke(null, null);
        }


        //Called by Register(Type type) via reflection.
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static void InternalRegister<T>()
            where T : SortedTreeTypeBase, new()
        {
            Register<T>();
        }

        /// <summary>
        /// Registeres the generic type with the SortedTreeStore.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
            where T : SortedTreeTypeBase, new()
        {
            T obj = new T();
            Type type = typeof(T);
            Guid id = obj.GenericTypeGuid;

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

            Encodings.Register<T>();
            Filters.Register<T>();
            Streaming.Register<T>();
        }


    }
}
