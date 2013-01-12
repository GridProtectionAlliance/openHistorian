////******************************************************************************************************
////  XmlConfig.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
////  7/24/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;

//namespace openHistorian.Local
//{

//    /// <summary>
//    /// Contains the openHistorian config settings. 
//    /// </summary>
//    public class XmlConfig
//    {
//        class DataSource : ConfigNode.DataSourceBase
//        {
//            XmlConfig m_config;
//            public DataSource(XmlConfig config)
//            {
//                m_config = config;
//            }


//            public override ConfigNode Get(int id)
//            {
//                if (id < 0)
//                    return m_config.m_root;
//                return m_config.m_rows[id];
//            }

//            public override void Add(ConfigNode node)
//            {
//                m_config.m_rows.Add(node.Id, node);
//            }

//            public override int NextIndex
//            {
//                get
//                {
//                    return m_config.m_nextNode;
//                }
//                set
//                {
//                    m_config.m_nextNode = value;

//                }
//            }

//            public override IEnumerable<ConfigNode> GetAllNodes()
//            {
//                return m_config.m_rows.Values;
//            }

//            public override void Remove(int id)
//            {
//                m_config.m_rows.Remove(id);
//            }

//            public override bool Contains(int id)
//            {
//                return m_config.m_rows.ContainsKey(id);
//            }
//        }

//        public int m_nextNode;
//        public SortedList<int, ConfigNode> m_rows;
//        public ConfigNode m_root;

//        public XmlConfig()
//        {
//            m_rows = new SortedList<int, ConfigNode>();
//            m_root = new ConfigNode(new DataSource(this));
//            m_nextNode = 1;
//        }
//        public XmlConfig(Stream stream)
//            : this()
//        {
//            DataTable table = new DataTable("Historian_2.0_Kernel_Config");
//            table.ReadXml(stream);
//            var ndata = new DataSource(this);
//            foreach (DataRow row in table.Rows)
//            {
//                ConfigNode n = new ConfigNode(ndata, (int)(row["ID"]), (int)row["ParentID"], (string)row["Field"], (string)row["Value"]);
//                m_nextNode = Math.Max(m_nextNode, n.Id);
//                m_rows.Add(n.Id, n);
//            }
//            m_nextNode++;

//        }

//        public ConfigNode RootNode
//        {
//            get
//            {
//                return m_root;
//            }
//        }

//        public void Save(Stream stream)
//        {
//            DataTable table = new DataTable("Historian_2.0_Kernel_Config");
//            table.Columns.Add("ID", typeof(int));
//            table.Columns.Add("ParentID", typeof(int));
//            table.Columns.Add("Field", typeof(string));
//            table.Columns.Add("Value", typeof(string));
//            //System.Text.StringBuilder sb = new StringBuilder();
//            foreach (var node in m_rows.Values)
//            {
//                //sb.AppendLine(node.Id.ToString() + '\t' + node.ParentId + '\t' + node.Field + '\t' + node.Value);
//                table.Rows.Add(node.Id, node.ParentId, node.Field, node.Value);
//            }
//            //System.Windows.Forms.Clipboard.SetText(sb.ToString()); 

//            table.WriteXml(stream, XmlWriteMode.WriteSchema);
//        }



//    }
//}
