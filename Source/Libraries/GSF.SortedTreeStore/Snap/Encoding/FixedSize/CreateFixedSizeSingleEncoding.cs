//******************************************************************************************************
//  CreateFixedSizeSingleEncoding.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  02/21/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.Snap.Definitions;

namespace GSF.Snap.Encoding
{
    /// <summary>
    /// Constructs a single encoding method for a fixed size encoding
    /// </summary>
    public class CreateFixedSizeSingleEncoding
        : IndividualEncodingBaseDefinition
    {
        // {1DEA326D-A63A-4F73-B51C-7B3125C6DA55}
        /// <summary>
        /// The guid that represents the encoding method of this class
        /// </summary>
        public static readonly Guid TypeGuid = new Guid(0x1dea326d, 0xa63a, 0x4f73, 0xb5, 0x1c, 0x7b, 0x31, 0x25, 0xc6, 0xda, 0x55);

        /// <summary>
        /// The type supported by the encoded method. Can be null if the encoding is not type specific.
        /// </summary>
        public override Type TypeIfNotGeneric
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The encoding method as specified by a <see cref="Guid"/>.
        /// </summary>
        public override Guid Method
        {
            get
            {
                return TypeGuid;
            }
        }

        /// <summary>
        /// Constructs a new class based on this encoding method. 
        /// </summary>
        /// <typeparam name="T">The type of this base class</typeparam>
        /// <returns>
        /// The encoding method
        /// </returns>
        public override IndividualEncodingBase<T> Create<T>()
        {
            return new FixedSizeSingleEncoding<T>();
        }
    }
}