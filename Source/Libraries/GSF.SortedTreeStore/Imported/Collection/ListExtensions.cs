//******************************************************************************************************
//  ListExtensions.cs - Gbtc
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

namespace GSF.Collections.Imported
{
    /// <summary>
    /// Extensions for <see cref="List{T}"/>
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Iterates through each item in the list. Allowing items to be removed from the list.
        /// </summary>
        /// <param name="list">the list to iterate though</param>
        /// <param name="shouldRemove">the function to call to determine 
        /// if the items should be removed from the list. </param>
        /// <remarks>
        /// In order to minimize the overhead of a removal. Any item removed with be replaced with
        /// the last item in the list. Therefore Sequence will not be preserved using this method.
        /// </remarks>
        public static void RemoveWhere<T>(this List<T> list, Func<T, bool> shouldRemove)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (shouldRemove == null)
                throw new ArgumentNullException("shouldRemove");

            for (int x = 0; x < list.Count; x++)
            {
                if (shouldRemove(list[x]))
                {
                    if (list.Count > 1 && x != list.Count - 1)
                        list[x] = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                    x--;
                }
            }
        }
    }
}
