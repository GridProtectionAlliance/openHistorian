//******************************************************************************************************
//  HistorianServer_DatabaseConfig.cs - Gbtc
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
//  10/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace openHistorian.V2.Local
{
    public partial class HistorianServer
    {
        private class DatabaseConfig : IDatabaseConfig
        {
            public DatabaseConfig()
            {
                m_paths = new PathList();
            }

            PathList m_paths;

            public WriterOptions? Writer { get; set; }

            public bool IsOnline { get; set; }

            IPathList IDatabaseConfig.Paths
            {
                get
                {
                    return m_paths;
                }
            }

            public PathList Paths { get; set; }

            public DatabaseConfig Clone()
            {
                return (DatabaseConfig)MemberwiseClone();
            }
        }

        class PathList : IPathList
        {
            public IEnumerable<string> GetPaths()
            {
                return new string[] { };
            }
            public IEnumerable<string> GetSavePaths()
            {
                return new string[] { };
            }
            public void AddPath(string path, bool allowWritingToPath)
            {
                throw new NotImplementedException();
            }
            public void DropPath(string path, float waitTimeSeconds)
            {
                throw new NotImplementedException();
            }
            public void DropSavePath(string path, bool terminateActiveFiles)
            {
                throw new NotImplementedException();
            }
        }
    }
}
