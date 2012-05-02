//******************************************************************************************************
//  IBPlusTreeType.cs - Gbtc
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
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.V2.IO;

namespace openHistorian.V2.Collections
{
    public interface IBPlusTreeType<T>
    {
        int SizeOf { get; }
        void LoadValue(IBinaryStream stream);
        void SaveValue(IBinaryStream stream);
       
        /// <summary>
        /// Return 0 if equal. 
        /// Return -1 if instance is before stream
        /// Return 1 if instance is after stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        int CompareToStream(IBinaryStream stream);
        int CompareTo(T key);
    }
}
