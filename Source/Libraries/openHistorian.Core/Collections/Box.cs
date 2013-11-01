//******************************************************************************************************
//  Box.cs - Gbtc
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Collections
{
    /// <summary>
    /// Wraps an item in a class. Similiar to a <see cref="Tuple"/> except allows assignment of the <see cref="Value"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Box<T>
        where T : new()
    {
        /// <summary>
        /// Creates a new item and initializes it with it's default constructor.
        /// </summary>
        public Box()
            : this(new T())
        {
        }

        /// <summary>
        /// Creates a new item using the provided class.
        /// </summary>
        /// <param name="value"></param>
        public Box(T value)
        {
            Value = value;
        }
        /// <summary>
        /// The value wrapped in this class.
        /// </summary>
        public T Value;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        
        //public static implicit operator T(Box<T> value)
        //{
        //    return value.Value;
        //}
    }
}