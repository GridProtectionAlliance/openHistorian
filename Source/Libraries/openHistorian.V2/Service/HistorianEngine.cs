//******************************************************************************************************
//  HistorianEngine.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.Service.Instance;

namespace openHistorian.V2.Service
{
    /// <summary>
    /// The main engine of the openHistorian. Instance this class to host a historian.
    /// </summary>
    public class HistorianEngine
    {
        SortedList<string, Engine> m_instances;

        SortedList<Guid, Engine> m_homeInstances;

        public HistorianEngine()
        {
            m_instances = new SortedList<string, Engine>();
            m_homeInstances = new SortedList<Guid, Engine>();
        }
        
        public Engine LookupHomeInstance(Guid pointId)
        {
            return null;
        }
        
        public void CreateInstance(string name)
        {

        }
        
        public Engine LoadInstance(string name)
        {
            return null;
        }

        public bool InstanceExists(string name)
        {
            return true;
        }

        public List<string> GetInstanceNames()
        {
            return new List<string>(m_instances.Keys);
        }

    }
}
