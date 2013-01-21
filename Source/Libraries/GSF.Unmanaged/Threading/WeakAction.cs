//******************************************************************************************************
//  WeakAction.cs - Gbtc
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
//  1/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakAction : WeakReference
    {
        MethodInfo m_method;

        public WeakAction(Action target)
            : base((target == null) ? null : target.Target)
        {
            if (target != null)
                m_method = target.Method;
        }
        public bool TryInvoke()
        {
            object target = base.Target;
            if (target == null)
                return false;
            m_method.Invoke(target, null);
            return true;
        }
    }

    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakAction<T> : WeakReference
    {
        MethodInfo m_method;
        public WeakAction(Action<T> target)
            : base((target == null) ? null : target.Target)
        {
            if (target != null)
                m_method = target.Method;
        }
        public bool TryInvoke(T obj)
        {
            object target = base.Target;
            if (target == null)
                return false;
            m_method.Invoke(target, new object[] { obj });
            return true;
        }
    }

    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakEventHandler<T> : WeakReference
        where T : EventArgs
    {
        MethodInfo m_method;
        public WeakEventHandler(EventHandler<T> target)
            : base((target == null) ? null : target.Target)
        {
            if (target != null)
                m_method = target.Method;
        }
        public bool TryInvoke(object sender, T e)
        {
            object target = base.Target;
            if (target == null)
                return false;
            m_method.Invoke(target, new object[] { sender, e });
            return true;
        }
    }
}
