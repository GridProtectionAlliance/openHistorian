////******************************************************************************************************
////  IConfigNode.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  10/19/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Local
//{
//    public class ConfigNode
//    {
//        public abstract class DataSourceBase
//        {

//            public abstract IEnumerable<ConfigNode> GetAllNodes();
//            public abstract void Remove(int id);
//            public abstract bool Contains(int id);
//            public abstract ConfigNode Get(int id);
//            public abstract void Add(ConfigNode node);
//            public abstract int NextIndex { get; set; }

//            public ConfigNode[] GetChildren(ConfigNode node)
//            {
//                List<ConfigNode> list = new List<ConfigNode>();
//                foreach (var n in GetAllNodes())
//                {
//                    if (n.ParentId == node.Id)
//                    {
//                        list.Add(n);
//                    }
//                }
//                return list.ToArray();
//            }

//            public ConfigNode[] GetChildren(ConfigNode node, string field)
//            {
//                List<ConfigNode> list = new List<ConfigNode>();
//                foreach (var n in GetAllNodes())
//                {
//                    if (n.ParentId == node.Id && string.Compare(n.Field, field, true) == 0)
//                    {
//                        list.Add(n);
//                    }
//                }
//                return list.ToArray();
//            }

//            public ConfigNode GetChild(ConfigNode node, string field)
//            {
//                foreach (var n in GetAllNodes())
//                {
//                    if (n.ParentId == node.Id && string.Compare(n.Field, field, true) == 0)
//                    {
//                        return n;
//                    }
//                }
//                return null;
//            }

//        }

//        DataSourceBase m_dataSource;
//        int m_id;
//        int m_parentId;
//        string m_field;
//        string m_value;
//        bool m_deleted;
//        bool m_deleting;

//        public ConfigNode(DataSourceBase dataSource)
//        {
//            m_dataSource = dataSource;
//            m_id = -1;
//            m_parentId = -1;
//            m_field = "Root";
//            m_value = string.Empty;
//        }

//        public ConfigNode(DataSourceBase dataSource, int id, int parentId, string field, string value, bool parentIdUnchecked = false)
//        {
//            m_dataSource = dataSource;
//            if (id < 0)
//                throw new ArgumentOutOfRangeException("id", "Must be >= 0");
//            if (field == null || field.Trim().Length == 0)
//                throw new ArgumentNullException("field");
//            if (!parentIdUnchecked && parentId >= 0 && !m_dataSource.Contains(parentId))
//                throw new ArgumentException("Foreign key does not exist", "parentId");
//            if (value == null)
//                value = string.Empty;

//            m_id = id;
//            m_parentId = parentId;
//            m_field = field;
//            m_value = value;
//        }

//        /// <summary>
//        /// Gets the parent <see cref="ConfigNode"/>. Returns itself if it is root.
//        /// </summary>
//        public ConfigNode Parent
//        {
//            get
//            {
//                if (m_deleted)
//                    throw new ObjectDisposedException(GetType().FullName);
//                return m_dataSource.Get(m_parentId);
//            }
//        }

//        /// <summary>
//        /// Gets the Id value associated with this <see cref="ConfigNode"/>.
//        /// </summary>
//        public int Id
//        {
//            get
//            {
//                if (m_deleted)
//                    throw new ObjectDisposedException(GetType().FullName);
//                return m_id;
//            }
//        }

//        /// <summary>
//        /// Gets the Id value of its parent. 
//        /// -1 if the parent is root or -1 if this instance is root
//        /// </summary>
//        public int ParentId
//        {
//            get
//            {
//                if (m_deleted)
//                    throw new ObjectDisposedException(GetType().FullName);
//                return m_parentId;
//            }
//        }

//        /// <summary>
//        /// Same as <see cref="GetChild"/>.
//        /// </summary>
//        /// <param name="field">the field to search for</param>
//        /// <returns>
//        /// The first child that uses the provided field. null if none exist.
//        /// </returns>
//        public ConfigNode this[string field]
//        {
//            get
//            {
//                return GetChild(field);
//            }
//        }

//        /// <summary>
//        /// Same as <see cref="GetChildString"/>.
//        /// </summary>
//        /// <param name="field">the field to search for</param>
//        /// <param name="defaultValue">The value to use if the child does not exist.</param>
//        /// <returns></returns>
//        public string this[string field, string defaultValue]
//        {
//            get
//            {
//                return GetChildString(field, defaultValue);
//            }
//        }

//        /// <summary>
//        /// Gets the field.
//        /// </summary>
//        public string Field
//        {
//            get
//            {
//                if (m_deleted)
//                    throw new ObjectDisposedException(GetType().FullName);
//                return m_field;
//            }
//        }

//        /// <summary>
//        /// Gets the value.
//        /// </summary>
//        public string Value
//        {
//            get
//            {
//                if (m_deleted)
//                    throw new ObjectDisposedException(GetType().FullName);
//                return m_value;
//            }
//        }

//        /// <summary>
//        /// Gets if this <see cref="ConfigNode"/> is the root node.
//        /// </summary>
//        public bool IsRoot
//        {
//            get
//            {
//                return m_id < 0;
//            }
//        }

//        /// <summary>
//        /// Deletes this <see cref="ConfigNode"/> and all children recursively.
//        /// After returning, this class can no longer be used, unless it is the root config.
//        /// </summary>
//        public void Delete()
//        {
//            if (m_deleting) //Prevents an endless loop if this ever happends: Parent1 -> Child1 -> Child2 -> Parent1
//                return;
//            m_deleting = true;
//            foreach (var child in GetChildren())
//            {
//                child.Delete();
//            }
//            if (!IsRoot) //Root cannot be deleted.
//            {
//                m_deleted = true;
//                m_dataSource.Remove(m_id);
//            }
//        }

//        /// <summary>
//        /// Determines if the <see cref="ConfigNode"/> has a child with the following field name
//        /// </summary>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public bool Contains(string field)
//        {
//            return GetChild(field) != null;
//        }

//        /// <summary>
//        /// Gets the number of children with the provided field name.
//        /// </summary>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public int Count(string field)
//        {
//            return GetChildren(field).Length;
//        }

//        /// <summary>
//        /// Adds the following Field/Value to the current nodes children.
//        /// </summary>
//        /// <param name="field"></param>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public ConfigNode Add(string field, string value = "")
//        {
//            if (m_deleted)
//                throw new ObjectDisposedException(GetType().FullName);
//            var node = new ConfigNode(m_dataSource, m_dataSource.NextIndex, Id, field, value);
//            m_dataSource.Add(node);
//            m_dataSource.NextIndex++;
//            return node;
//        }

//        /// <summary>
//        /// Returns the first child that has the following field. Returns null if no children exist.
//        /// </summary>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public ConfigNode GetChild(string field)
//        {
//            if (m_deleted)
//                throw new ObjectDisposedException(GetType().FullName);
//            return m_dataSource.GetChild(this, field);
//        }

//        /// <summary>
//        /// Gets the first child entry, substituting the default value in if the child does not exist.
//        /// </summary>
//        /// <param name="field"></param>
//        /// <param name="defaultValue">the value to substitute in if the field does not exist</param>
//        /// <returns></returns>
//        public string GetChildString(string field, string defaultValue = null)
//        {
//            if (m_deleted)
//                throw new ObjectDisposedException(GetType().FullName);
//            var c = m_dataSource.GetChild(this, field);
//            if (c == null)
//                return defaultValue;
//            return c.Value;
//        }

//        /// <summary>
//        /// Returns all of the children that match the given field.
//        /// </summary>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public ConfigNode[] GetChildren(string field)
//        {
//            if (m_deleted)
//                throw new ObjectDisposedException(GetType().FullName);
//            return m_dataSource.GetChildren(this, field);
//        }

//        /// <summary>
//        /// Returns all children of the parent.
//        /// </summary>
//        /// <returns></returns>
//        public ConfigNode[] GetChildren()
//        {
//            if (m_deleted)
//                throw new ObjectDisposedException(GetType().FullName);
//            return m_dataSource.GetChildren(this);
//        }


//        public int? GetValueInt(string field)
//        {
//            string c = GetChild(field);
//            if (c == null)
//            {
//                return null;
//            }
//            else
//            {
//                return int.Parse(c);
//            }
//        }

//        public TimeSpan? GetValueTimeSpan(string field, long ticksPerUnit)
//        {
//            string c = GetChild(field);
//            if (c == null)
//            {
//                return null;
//            }
//            else
//            {
//                return new TimeSpan((long)(ticksPerUnit * double.Parse(c)));
//            }
//        }

//        public long? GetValueLong(string field, long scaling)
//        {
//            string c = GetChild(field);
//            if (c == null)
//            {
//                return null;
//            }
//            else
//            {
//                return (long)(scaling * double.Parse(c));
//            }
//        }

//        public static implicit operator string(ConfigNode value)
//        {
//            if (value == null)
//                return null;
//            return value.Value;
//        }

//    }
//}
