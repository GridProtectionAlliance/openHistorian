//******************************************************************************************************
//  ResourceQueueCollection.cs - Gbtc
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
//  9/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace GSF.Collections
{
    /// <summary>
    /// Provides a thread safe collection of many different resources of the same type.
    /// </summary>
    /// <typeparam name="TKey">An ICompariable type that is used to distinquish different resource queues.</typeparam>
    /// <typeparam name="TResource">They type of the resource queue.</typeparam>
    public class ResourceQueueCollection<TKey, TResource>
        where TResource : class
    {
        private readonly SortedList<TKey, ResourceQueue<TResource>> m_list;
        private readonly Func<TKey, Func<TResource>> m_instanceObject;
        private readonly Func<TKey, int> m_maximumCount;
        private readonly Func<TKey, int> m_initialCount;

        /// <summary>
        /// Creates a new ResourceQueueCollection.
        /// </summary>
        /// <param name="instance">A function to pass to the ResourceQueue for a given TCompare </param>
        /// <param name="initialCount">The initial size of each resource queue</param>
        /// <param name="maximumCount">The maximum size of each resource queue</param>
        public ResourceQueueCollection(Func<TResource> instance, int initialCount, int maximumCount)
            : this(x => instance, x => initialCount, x => maximumCount)
        {
        }

        /// <summary>
        /// Creates a new ResourceQueueCollection.
        /// </summary>
        /// <param name="instance">A function that will return the function to pass to the ResourceQueue for a given TCompare </param>
        /// <param name="initialCount">The initial size of each resource queue</param>
        /// <param name="maximumCount">The maximum size of each resource queue</param>
        public ResourceQueueCollection(Func<TKey, TResource> instance, int initialCount, int maximumCount)
            : this(key => () => instance(key), x => initialCount, x => maximumCount)
        {

        }

        /// <summary>
        /// Creates a new ResourceQueueCollection.
        /// </summary>
        /// <param name="instance">A function that will return the function to pass to the ResourceQueue for a given TCompare </param>
        /// <param name="initialCount">The initial size of each resource queue</param>
        /// <param name="maximumCount">The maximum size of each resource queue</param>
        public ResourceQueueCollection(Func<TKey, Func<TResource>> instance, int initialCount, int maximumCount)
            : this(instance, x => initialCount, x => maximumCount)
        {
        }

        /// <summary>
        /// Creates a new ResourceQueueCollection.
        /// </summary>
        /// <param name="instance">A function that will return the function to pass to the ResourceQueue for a given TCompare </param>
        /// <param name="initialCount">The initial size of each resource queue</param>
        /// <param name="maximumCount">The maximum size of each resource queue</param>
        public ResourceQueueCollection(Func<TKey, Func<TResource>> instance, Func<TKey, int> initialCount, Func<TKey, int> maximumCount)
        {
            m_instanceObject = instance;
            m_initialCount = initialCount;
            m_maximumCount = maximumCount;
            m_list = new SortedList<TKey, ResourceQueue<TResource>>();
        }

        /// <summary>
        /// Gets the resource queue for a key of this.
        /// </summary>
        /// <param name="key">The key identifying the resource queue to pull from</param>
        /// <returns></returns>
        public ResourceQueue<TResource> this[TKey key] => GetResourceQueue(key);

        /// <summary>
        /// Gets the resource queue for a key of this.
        /// </summary>
        /// <param name="key">The key identifying the resource queue to pull from</param>
        /// <returns></returns>
        public ResourceQueue<TResource> GetResourceQueue(TKey key)
        {
            ResourceQueue<TResource> resourceQueue;
            lock (m_list)
            {
                if (!m_list.TryGetValue(key, out resourceQueue))
                {
                    resourceQueue = new ResourceQueue<TResource>(m_instanceObject(key), m_initialCount(key), m_maximumCount(key));
                    m_list.Add(key, resourceQueue);
                }
            }
            return resourceQueue;
        }
    }
}