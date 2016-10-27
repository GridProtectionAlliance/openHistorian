//******************************************************************************************************
//  MethodBaseExtensions.cs - Gbtc
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
using System.Reflection;
using System.Text;

namespace GSF.Reflection
{
    /// <summary>
    /// Extensions for <see cref="MethodBase"/>.
    /// </summary>
    public static class MethodBaseExtensions
    {
        /// <summary>
        /// Gets the full class name of the provided type. Trimming generic parameters.
        /// Example: System.Collections.Generic.List`1+Enumberable
        /// Returns String.Empty if none.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFriendlyClassName(this Type type)
        {
            if (type != null)
            {
                var name = type.FullName;
                if (name != null)
                {
                    int length = name.Length;
                    int indexOfBracket = name.IndexOf('[');
                    int indexOfComma = name.IndexOf(',');
                    if (indexOfBracket >= 0)
                        length = Math.Min(indexOfBracket, length);
                    if (indexOfComma >= 0)
                        length = Math.Min(indexOfComma, length);
                    name = name.Substring(0, length).Trim();
                    return name;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the full class name of the provided type. Trimming generic parameters.
        /// Example: System.Collections.Generic.List`1+Enumerable
        /// Returns String.Empty if none.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetFriendlyClassName(this MethodBase method)
        {
            if (method != null)
            {
                return method.DeclaringType.GetFriendlyClassName();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets a friendly name for the method. Example:
        /// MethodName&lt;TKey,TValue&gt;(String key, String value)
        /// Returns String.Empty if none.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetFriendlyMethodName(this MethodBase method)
        {
            if (method == null)
                return string.Empty;

            bool appendComma;
            var sb = new StringBuilder();
            sb.Append(method.Name);
            if (method is MethodInfo && method.IsGenericMethod)
            {
                appendComma = false;
                sb.Append("<");
                foreach (var arg in method.GetGenericArguments())
                {
                    if (appendComma)
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        appendComma = true;
                    }
                    sb.Append(arg.Name);
                }
                sb.Append(">");
            }
            appendComma = false;
            sb.Append("(");
            foreach (var param in method.GetParameters())
            {
                if (appendComma)
                {
                    sb.Append(", ");
                }
                else
                {
                    appendComma = true;
                }
                sb.Append(param.ParameterType.Name);
                sb.Append(" ");
                sb.Append(param.Name);
            }
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Gets a friendly name for the method including class name. Example:
        /// Namespace.Class`1+Subclass`2.MethodName&lt;TKey,TValue&gt;(String key, String value)
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetFriendlyMethodNameWithClass(this MethodBase method)
        {
            string className = method.GetFriendlyClassName();
            string methodName = method.GetFriendlyMethodName();
            if (className.Length == 0)
                return methodName;
            return className + "." + methodName;
        }
    }
}
