//******************************************************************************************************
//  SupportsReadonlyBase.cs - Gbtc
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
using System.Data;
using System.Linq;
using System.Text;

namespace openHistorian.Collections
{
    /// <summary>
    /// Represents an object that can be configured as read only and thus made immutable.  
    /// The origional contents of this class will not be editable once <see cref="IsReadOnly"/> is set to true.
    /// In order to modify the contest of this object, a clone of the object must be created with <see cref="CloneEditable"/>.
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
                    SetMembersAsReadOnly();
                }
            }
        }

        protected void TestForEditable()
        {
            if (IsReadOnly)
                throw new ReadOnlyException("Object has been set as read only");
        }

        /// <summary>
        /// Requests that member fields be set to readonly. 
        /// </summary>
        protected abstract void SetMembersAsReadOnly();
        
        /// <summary>
        /// Request that member fields be cloned and marked as editable.
        /// </summary>
        protected abstract void CloneMembersAsEditable();

        /// <summary>
        /// Creates a clone of this class that is editable.
        /// A clone is always created, even if this class is already editable.
        /// </summary>
        /// <returns></returns>
        public virtual T CloneEditable()
        {
            T initializer = (T)MemberwiseClone();
            initializer.m_isReadOnly = false;
            initializer.CloneMembersAsEditable();
            return initializer;
        }

        object ISupportsReadonly.CloneReadonly()
        {
            return CloneReadonly();
        }

        object ISupportsReadonly.CloneEditable()
        {
            return CloneEditable();
        }

        /// <summary>
        /// Makes a readonly clone of the object.
        /// If the class is currently marked as readonly, the current instance is returned.
        /// </summary>
        /// <returns></returns>
        public virtual T CloneReadonly()
        {
            if (IsReadOnly)
                return (T)this;
            
            var copy = CloneEditable();
            copy.IsReadOnly = true;
            return copy;
        }

        /// <summary>
        /// Returns a clone of this class.
        /// If the class is marked as readonly, it returns the current instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            if (IsReadOnly)
                return this;
            else
                return CloneEditable();
        }
    }
}
