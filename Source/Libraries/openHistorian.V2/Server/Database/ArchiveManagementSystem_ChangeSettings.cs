//******************************************************************************************************
//  ArchiveManagementSystem.cs - Gbtc
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

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public partial class ArchiveDatabaseEngine
    {
        public class ChangeSettings : IDisposable
        {
            bool m_disposed;
            ArchiveDatabaseEngine m_system;
            
            public ChangeSettings(ArchiveDatabaseEngine system)
            {
                m_system = system;
            }

            public void Commit()
            {
                if (m_disposed) 
                    throw new ObjectDisposedException(GetType().FullName);


                m_disposed = true;
            }

            public void Rollback()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);


                m_disposed = true;
            }

            public void Dispose()
            {
                if (!m_disposed)
                {
                    Rollback();
                }
            }
        }



    }
}
