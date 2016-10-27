//******************************************************************************************************
//  ImmutableObjectAutoBase`1.cs - Gbtc
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
using System.Collections.Generic;
using System.Reflection;

namespace GSF.Immutable
{
    /// <summary>
    /// Represents an object that can be configured as read only and thus made immutable.  
    /// This class will automatically clone any field that implements <see cref="IImmutableObject"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ImmutableObjectAutoBase<T> 
        : ImmutableObjectBase<T>
        where T : ImmutableObjectAutoBase<T>
    {
// ReSharper disable once StaticFieldInGenericType
        private static List<FieldInfo> s_readonlyFields;

        static ImmutableObjectAutoBase()
        {
            s_readonlyFields = new List<FieldInfo>();
            Type newType = typeof(IImmutableObject);
            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ))
            {
                if (newType.IsAssignableFrom(field.FieldType))
                    s_readonlyFields.Add(field);
            }
        }

        protected override void SetMembersAsReadOnly()
        {
            foreach (FieldInfo field in s_readonlyFields)
            {
                IImmutableObject value = (IImmutableObject)field.GetValue(this);
                value.IsReadOnly = true;
            }
        }

        protected override void CloneMembersAsEditable()
        {
            foreach (FieldInfo field in s_readonlyFields)
            {
                IImmutableObject value = (IImmutableObject)field.GetValue(this);
                field.SetValue(this, value.CloneEditable());
            }
        }
    }
}