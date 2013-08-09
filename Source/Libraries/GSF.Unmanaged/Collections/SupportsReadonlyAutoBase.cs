//******************************************************************************************************
//  SupportsReadonlyAutoBase.cs - Gbtc
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
//  7/27/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;

namespace GSF.Collections
{
    /// <summary>
    /// Represents an object that can be configured as read only and thus made immutable.  
    /// This class will automatically clone any field that implements <see cref="T:GSF.Collections.ISupportsReadonly`1"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SupportsReadonlyAutoBase<T> : SupportsReadonlyBase<T>
        where T : SupportsReadonlyAutoBase<T>
    {
        private static List<FieldInfo> s_readonlyFields;

        static SupportsReadonlyAutoBase()
        {
            s_readonlyFields = new List<FieldInfo>();

            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Type type = field.FieldType;
                Type newType = typeof(ISupportsReadonly<>).MakeGenericType(field.FieldType);
                if (newType.IsAssignableFrom(field.FieldType))
                    s_readonlyFields.Add(field);
            }
        }

        protected override sealed void SetMembersAsReadOnly()
        {
            foreach (FieldInfo field in s_readonlyFields)
            {
                ISupportsReadonly value = (ISupportsReadonly)field.GetValue(this);
                value.IsReadOnly = true;
            }
        }

        protected override sealed void CloneMembersAsEditable()
        {
            foreach (FieldInfo field in s_readonlyFields)
            {
                ISupportsReadonly value = (ISupportsReadonly)field.GetValue(this);
                field.SetValue(this, value.CloneEditable());
            }
        }
    }
}