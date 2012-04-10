//******************************************************************************************************
//  PositionData.cs - Gbtc
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
//  1/1/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************


namespace openHistorian.V2.StorageSystem.File
{
    public struct PositionData
    {
        public uint PhysicalBlockIndex;
        public long VirtualPosition;
        public long Length;
        /// <summary>
        /// Determines if the position is contained within this translation.
        /// </summary>
        /// <param name="virtualPos"></param>
        /// <returns></returns>
        public bool Containts(long virtualPos)
        {
            return (virtualPos >= VirtualPosition) && (virtualPos < VirtualPosition + Length);
        }
        /// <summary>
        /// Determines how many bytes there are until this translation is no longer valid.
        /// </summary>
        /// <param name="virtualPos"></param>
        /// <returns></returns>
        public long ValidLength(long virtualPos)
        {
            return Length - (virtualPos - VirtualPosition);
        }
        public int Offset(long virtualPos)
        {
            return (int)(virtualPos - VirtualPosition);
        }
    }
}
