//******************************************************************************************************
//  CreateDoubleValueBase.cs - Gbtc
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
//  2/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace GSF.SortedTreeStore
{
    /// <summary>
    /// The base class for all create type classes that involve a double value.
    /// </summary>
    public abstract class CreateDoubleValueBase
    {
        /// <summary>
        /// The key type supported by the encoded method. Can be null if the encoding is not type specific.
        /// </summary>
        public abstract Type KeyTypeIfNotGeneric { get; }

        /// <summary>
        /// The value type supported by the encoded method. Can be null if the encoding is not type specific.
        /// </summary>
        public abstract Type ValueTypeIfNotGeneric { get; }

        /// <summary>
        /// The encoding method that defines this class.
        /// </summary>
        public abstract EncodingDefinition Method { get; }
    }
}
