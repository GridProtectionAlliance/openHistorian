//******************************************************************************************************
//  ImmutableObjectBase`1.cs - Gbtc
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

using System.Data;
using System.Runtime.CompilerServices;

namespace GSF.Immutable
{
    /// <summary>
    /// Represents an object that can be configured as read only and thus made immutable.  
    /// The origional contents of this class will not be editable once <see cref="IsReadOnly"/> is set to true.
    /// In order to modify the contest of this object, a clone of the object must be created with <see cref="CloneEditable"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// For a classes that implement this, all setters should call <see cref="TestForEditable"/> before 
    /// setting the value. 
    /// </remarks>
    public abstract class ImmutableObjectBase<T> 
        : IImmutableObject<T>
        where T : ImmutableObjectBase<T>
    {
        private bool m_isReadOnly;

        /// <summary>
        /// Gets/Sets if this class is immutable and thus read only. Once
        /// setting to readonly, the class becomes immutable.
        /// </summary>
        public bool IsReadOnly
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

        /// <summary>
        /// Test if the class has been marked as readonly. Throws an exception if editing cannot occur.
        /// </summary>
        protected void TestForEditable()
        {
            if (m_isReadOnly)
                ThrowReadOnly(); 
        }

        void ThrowReadOnly()
        {
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

        /// <summary>
        /// Makes a readonly clone of this object. Returns the same object if it is already marked as readonly.
        /// </summary>
        /// <returns></returns>
        object IImmutableObject.CloneReadonly()
        {
            return CloneReadonly();
        }

        /// <summary>
        /// Makes a clone of this object and allows it to be edited.
        /// </summary>
        /// <returns></returns>
        object IImmutableObject.CloneEditable()
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

            T copy = CloneEditable();
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