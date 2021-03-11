//******************************************************************************************************
//  WeakDelegateBase.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  1/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Reflection;

namespace GSF.Threading
{
    public abstract class WeakDelegateBase<T> : WeakReference
        where T : class
    {
        private readonly MethodInfo m_method;

        protected WeakDelegateBase(Delegate target)
            : base(target is null ? null : target.Target)
        {
            if (target != null)
                m_method = target.Method;
        }

        protected bool TryInvokeInternal(object[] parameters)
        {
            object target = base.Target;
            if (target is null)
                return false;
            m_method.Invoke(target, parameters);
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is null)
                return false;

            WeakDelegateBase<T> typeObject = obj as WeakDelegateBase<T>;
            if (typeObject != null)
                return Equals(typeObject);

            return false;
        }

        protected virtual bool Equals(WeakDelegateBase<T> obj)
        {
            if (obj is null)
                return false;

            return m_method == obj.m_method && ReferenceEquals(Target, obj.Target);
        }
    }
}