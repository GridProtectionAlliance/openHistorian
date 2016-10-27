//******************************************************************************************************
//  LoggerInternal.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Collections;
using GSF.Collections.Imported;
using GSF.Threading;

namespace GSF.Diagnostics
{
    /// <summary>
    /// The fundamental functionality of <see cref="Logger"/>.
    /// </summary>
    internal class LoggerInternal
        : IDisposable
    {
        /// <summary>
        /// The common synchronization object to make sure external calls are properly synchronized.
        /// </summary>
        private readonly object m_syncRoot;

        private readonly Dictionary<Type, LogPublisherInternal> m_typeIndexCache;
        private readonly SingleOccurrenceAction m_disposingAction;
        private readonly List<LogPublisherInternal> m_allPublishers;
        private readonly List<WeakReference> m_subscribers;

        /// <summary>
        /// Creates a <see cref="LoggerInternal"/>.
        /// </summary>
        public LoggerInternal()
        {
            m_disposingAction = new SingleOccurrenceAction();
            m_syncRoot = new object();
            m_typeIndexCache = new Dictionary<Type, LogPublisherInternal>();
            m_allPublishers = new List<LogPublisherInternal>();
            m_subscribers = new List<WeakReference>();
        }

        /// <summary>
        /// Creates a <see cref="LogSubscriber"/> that can subscribe to log messages.
        /// </summary>
        /// <returns></returns>
        public LogSubscriberInternal CreateSubscriber()
        {
            bool success;
            using (m_disposingAction.TryBlockAction(out success))
            {
                if (success)
                {
                    var s = new LogSubscriberInternal(ClearSubscriptionCache);
                    lock (m_syncRoot)
                    {
                        m_subscribers.Add(new WeakReference(s));
                    }
                    return s;
                }
                else
                {
                    return LogSubscriberInternal.DisposedSubscriber;
                }
            }
        }

        /// <summary>
        /// Handles the routing of messages through the logging system.
        /// </summary>
        private void ClearSubscriptionCache()
        {
            m_disposingAction.TryBlockAction(() =>
            {
                lock (m_syncRoot)
                {
                    foreach (var pub in m_allPublishers)
                    {
                        pub.ClearSubscriptionCache();
                    }
                }
            });
        }

        /// <summary>
        /// Handles the routing of messages through the logging system.
        /// </summary>
        /// <param name="message">the message to route</param>
        /// <param name="publisher">the publisher that is originating this message.</param>
        public void OnNewMessage(LogMessage message, LogPublisherInternal publisher)
        {
            m_disposingAction.TryBlockAction(() =>
            {
                List<LogSubscriberInternal> lst;
                lock (m_syncRoot)
                {
                    lst = GetAllSubscribersSync();
                }
                foreach (var sub in lst)
                {
                    sub.RaiseLogMessages(message, publisher);
                }
            });
        }

        /// <summary>
        /// Gets a strong reference of all subscribers. 
        /// Be sure that this list is not kept long term as it will inhibit garbage collection.
        /// </summary>
        /// <returns></returns>
        private List<LogSubscriberInternal> GetAllSubscribersSync()
        {
            var lst = new List<LogSubscriberInternal>(m_subscribers.Count);
            m_subscribers.RemoveWhere(x =>
            {
                LogSubscriberInternal subscriber = (LogSubscriberInternal)x.Target;
                if (subscriber != null)
                {
                    lst.Add(subscriber);
                    return false;
                }
                return true;
            });

            return lst;
        }

        /// <summary>
        /// Adds the supplied publisher to the valid publishers
        /// </summary>
        /// <param name="publisher">the publisher to add</param>
        private void AddPublisher(LogPublisherInternal publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException(nameof(publisher));

            bool success;
            using (m_disposingAction.TryBlockAction(out success))
            {
                if (success)
                {
                    lock (m_syncRoot)
                    {
                        m_allPublishers.Add(publisher);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a type topic on a specified type.
        /// </summary>
        /// <param name="type">the type to create the topic from</param>
        /// <returns></returns>
        public LogPublisherInternal CreateType(Type type)
        {
            if ((object)type == null)
                throw new ArgumentNullException(nameof(type));

            LogPublisherInternal item;
            lock (m_typeIndexCache)
            {
                if (!m_typeIndexCache.TryGetValue(type, out item))
                {

                    item = new LogPublisherInternal(this, type);
                    m_typeIndexCache.Add(type, item);
                    AddPublisher(item);
                }
            }

            return item;
        }

        /// <summary>
        /// Gracefully terminate all message routing. Function blocks until all termination is successful.
        /// </summary>
        public void Dispose()
        {
            //Stops all of the message routing of this class.
            m_disposingAction.ExecuteAndWait(() =>
            {
                //A single occurrence action will only block the ExecuteAndWait() method. Therefore
                //A lock on the m_globalSyncRoot can be safely acquired without worrying about a deadlock.
                lock (m_syncRoot)
                {
                    m_allPublishers.Clear();
                    m_subscribers.RemoveWhere(x =>
                    {
                        LogSubscriberInternal subscriber = (LogSubscriberInternal)x.Target;
                        if (subscriber != null)
                        {
                            subscriber.Dispose();
                            return false;
                        }
                        return true;
                    });
                    m_subscribers.Clear();
                }
            });
        }

        public MessageAttributeFilter GetSubscription(LogPublisherInternal publisher)
        {
            MessageAttributeFilter verbose = new MessageAttributeFilter();
            m_disposingAction.TryBlockAction(() =>
            {
                List<LogSubscriberInternal> lst;
                lock (m_syncRoot)
                {
                    lst = GetAllSubscribersSync();
                }
                foreach (var sub in lst)
                {
                    verbose.Append(sub.GetSubscription(publisher));
                }
            });
            return verbose;
        }
    }
}
