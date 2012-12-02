//******************************************************************************************************
//  ITreeScanner256.cs - Gbtc
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
//  6/23/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace openHistorian.V2.Collections.KeyValue
{
    /// <summary>
    /// Assists in the parsing of data from a <see cref="SortedTree256Base"/>.
    /// </summary>
    public interface ITreeScanner256
    {
        /// <summary>
        /// Advances the tree to the next available entry.
        /// </summary>
        /// <param name="key1">an output parameter to store the first key</param>
        /// <param name="key2">an output parameter to store the second key</param>
        /// <param name="value1">an output parameter to store the first value</param>
        /// <param name="value2">an output parameter to store the second value</param>
        /// <returns>
        /// Returns true if the next value was found. Returns false if the end of the tree has been encountered.
        /// </returns>
        bool GetNextKey(out ulong key1, out ulong key2, out ulong value1, out ulong value2);
        
        /// <summary>
        /// Moves the current position to the location where the provided key should be located. 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <remarks>
        /// If the key does not exist in the database, the location will be at the next point in the list. 
        /// To seek to the beginning of the tree. Seek to 0,0. 
        /// </remarks>
        void SeekToKey(ulong key1, ulong key2);
    }
}
