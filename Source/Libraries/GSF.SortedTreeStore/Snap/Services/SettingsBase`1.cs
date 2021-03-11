//******************************************************************************************************
//  SettingsBase`1.cs - Gbtc
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
//  10/08/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System.IO;
using GSF.Immutable;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Core functionality for any setting of <see cref="SnapServer"/> or any child setting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SettingsBase<T>
        : ImmutableObjectAutoBase<T>
        where T : SettingsBase<T>
    {
        /// <summary>
        /// Saves the setting to the supplied stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public abstract void Save(Stream stream);

        /// <summary>
        /// Loads the settings from the supplied stream
        /// </summary>
        /// <param name="stream">the stream to load from</param>
        public abstract void Load(Stream stream);

        /// <summary>
        /// Validates the settings before they are loaded by the main class.
        /// </summary>
        public abstract void Validate();

    }
}
