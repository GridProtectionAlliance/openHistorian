//******************************************************************************************************
//  TypeDouble.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  12/15/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;

namespace openHistorian.Data.Types
{
    /// <summary>
    /// Method for converting data to and from a <see cref="Double"/>.
    /// </summary>
    public unsafe class TypeDouble
        : TypeBase
    {
        public static readonly TypeDouble Instance = new TypeDouble();
        
        /// <summary>
        /// Must use the static instance
        /// </summary>
        TypeDouble()
        {
        }

        protected override ulong ToRaw(IConvertible value)
        {
            double tmp = value.ToDouble(null);
            return *(ulong*)&tmp;
        }
        protected override IConvertible GetValue(ulong value)
        {
            return *(double*)&value;
        }
    }
}
