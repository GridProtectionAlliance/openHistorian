//******************************************************************************************************
//  ReadonlyList.cs - Gbtc
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
//  7/27/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Collections
{
    /// <summary>
    /// Represents an object that can be configured as read only and thus made immutable.  
    /// The origional contents of this class will not be editable once <see cref="IsReadOnly"/> is set to true.
    /// In order to modify the contest of this object, a clone of the object must be created with <see cref="EditableClone"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SupportsReadonlyBase<T> : ISupportsReadonly<T>
        where T : SupportsReadonlyBase<T>
    {
        bool m_isReadOnly;

        public virtual bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
            set
            {
                if (value ^ m_isReadOnly) //if values are different
                {
                    if (m_isReadOnly)
                        throw new ReadOnlyException("Object has been set as read only and cannot be reversed");
                    m_isReadOnly = true;
                    SetInternalMembersAsReadOnly();
                }
            }
        }

        protected void TestForEditable()
        {
            if (IsReadOnly)
                throw new ReadOnlyException("Object has been set as read only");
        }

        protected abstract void SetInternalMembersAsReadOnly();
        protected abstract void SetInternalMembersAsEditable();

        public virtual T EditableClone()
        {
            T initializer = (T)MemberwiseClone();
            initializer.m_isReadOnly = false;
            initializer.SetInternalMembersAsEditable();
            return initializer;
        }

        public virtual T ReadonlyClone()
        {
            var copy = EditableClone();
            copy.IsReadOnly = true;
            return copy;
        }
    }
}
