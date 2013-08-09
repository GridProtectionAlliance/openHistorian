////******************************************************************************************************
////  NullTreeScanner256.cs - Gbtc
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
////  12/1/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//namespace openHistorian.Collections
//{
//    /// <summary>
//    /// Represents an empty tree scanner. 
//    /// </summary>
//    /// <remarks>
//    /// This can be useful to return instead of null at times. Seeks will not throw exceptions and 
//    /// scans will yield no results.
//    /// To use this class. Call the static property <see cref="Instance"/>.
//    /// </remarks>
//    public class NullTreeScanner256 : ITreeScanner256
//    {
//        static NullTreeScanner256()
//        {
//            Instance = new NullTreeScanner256();
//        }

//        /// <summary>
//        /// Returns a static instance of this class
//        /// </summary>
//        public static ITreeScanner256 Instance { get; private set; }

//        /// <summary>
//        /// Advances the tree to the next available entry.
//        /// </summary>
//        /// <param name="timestamp">an output parameter to store the first key</param>
//        /// <param name="pointId">an output parameter to store the second key</param>
//        /// <param name="quality">an output parameter to store the first value</param>
//        /// <param name="value">an output parameter to store the second value</param>
//        /// <returns>
//        /// Returns true if the next value was found. Returns false if the end of the tree has been encountered.
//        /// </returns>
//        public bool Read(out ulong timestamp, out ulong pointId, out ulong quality, out ulong value)
//        {
//            timestamp = 0;
//            pointId = 0;
//            quality = 0;
//            value = 0;
//            return false;
//        }

//        /// <summary>
//        /// Moves the current position to the location where the provided key should be located. 
//        /// </summary>
//        /// <param name="key1"></param>
//        /// <param name="key2"></param>
//        /// <remarks>
//        /// If the key does not exist in the database, the location will be at the next point in the list. 
//        /// To seek to the beginning of the tree. Seek to 0,0. 
//        /// </remarks>
//        public void SeekToKey(ulong key1, ulong key2)
//        {
//        }
//    }
//}

