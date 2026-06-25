//******************************************************************************************************
//  ArchiveDirectoryFillMethod.cs - Gbtc
//
//  Copyright © 2026, Grid Protection Alliance.  All Rights Reserved.
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
//  06/23/2026 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

namespace GSF.Snap.Services
{
    /// <summary>
    /// Specifies how a write directory is selected from the set of configured archive directories when more than one
    /// is available.
    /// </summary>
    public enum ArchiveDirectoryFillMethod
    {
        /// <summary>
        /// Fill each configured write path, in order, advancing to the next only when the current path's drive lacks
        /// sufficient free space. This is the default and matches the historical archive write behavior.
        /// </summary>
        Sequential,

        /// <summary>
        /// Distribute new files across the configured write paths in round-robin order, skipping any path whose drive
        /// lacks sufficient free space.
        /// </summary>
        RoundRobin
    }
}
