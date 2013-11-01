//******************************************************************************************************
//  CreateValueMethodBase.cs - Gbtc
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
//  4/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// Used to create a <see cref="T:openHistorian.Collections.Generic.TreeValueMethodsBase`1"/>
    /// </summary>
    public abstract class CreateValueMethodBase
    {
        /// <summary>
        /// The type that can be created by this class.
        /// </summary>
        public abstract Type GenericType
        {
            get;
        }

        /// <summary>
        /// The Guid uniquely defining this type. 
        /// It is important to uniquely tie 1 type to 1 guid.
        /// </summary>
        public abstract Guid GenericTypeGuid
        {
            get;
        }

        /// <summary>
        /// Creates a <see cref="T:openHistorian.Collections.Generic.TreeValueMethodsBase`1"/>
        /// </summary>
        /// <typeparam name="TValue">This type must be exactly the same as <see cref="GenericType"/></typeparam>
        /// <returns></returns>
        public TreeValueMethodsBase<TValue> Create<TValue>()
            where TValue : class, new()
        {
            return As<TValue>().Create();
        }

        /// <summary>
        /// Type casts up.
        /// </summary>
        /// <typeparam name="TValue">This type must be exactly the same as <see cref="GenericType"/></typeparam>
        /// <returns></returns>
        public CreateValueMethodBase<TValue> As<TValue>()
            where TValue : class, new()
        {
            if (typeof(TValue) != GenericType)
                throw new InvalidCastException("Type specified must be the same as GenericType");
            return ((TreeValueMethodsBase<TValue>)this);
        }
    }

    public abstract class CreateValueMethodBase<TValue>
        : CreateValueMethodBase
        where TValue : class, new()
    {
        /// <summary>
        /// The type that can be created by this class.
        /// It is important to uniquely tie 1 type to 1 guid. 
        /// </summary>
        public override Type GenericType
        {
            get
            {
                return typeof(TValue);
            }
        }

        /// <summary>
        /// Creates a <see cref="T:openHistorian.Collections.Generic.TreeValueMethodsBase`1"/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public abstract TreeValueMethodsBase<TValue> Create();
    }
}