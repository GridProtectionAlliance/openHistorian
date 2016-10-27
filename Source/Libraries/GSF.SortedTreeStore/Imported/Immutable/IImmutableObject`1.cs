//******************************************************************************************************
//  IImmutableObject`1.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Immutable
{
    /// <summary>
    /// Represents an object that can be configured as read only and thus made immutable.  
    /// The origional contents of this class will not be editable once <see cref="IImmutableObject.IsReadOnly"/> is set to true.
    /// In order to modify the contest of this object, a clone of the object must be created with <see cref="CloneEditable"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IImmutableObject<out T> 
        : IImmutableObject
    {
        /// <summary>
        /// Makes a clone of this object and allows it to be edited.
        /// </summary>
        /// <returns></returns>
        new T CloneEditable();

        /// <summary>
        /// Makes a readonly clone of this object. Returns the same object if it is already marked as readonly.
        /// </summary>
        /// <returns></returns>
        new T CloneReadonly();
    }
}