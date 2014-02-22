//******************************************************************************************************
//  CreateHistorianCompressionDelta.cs - Gbtc
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
//  7/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using openHistorian.Collections;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    /// <summary>
    /// Used to generically create a fixed size node.
    /// </summary>
    public class CreateHistorianCompressionDelta
        : CreateTreeNodeBase
    {
        static CreateHistorianCompressionDelta()
        {
            //Gaurenteed to execute only once.
            try
            {
                TreeNodeInitializer.Register(new CreateHistorianCompressionDelta());
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Registers this implementation with the appropriate class.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void Register()
        {
            //Do Nothing. The static constructor will be called.
        }

        /// <summary>
        /// Creates Class
        /// </summary>
        public CreateHistorianCompressionDelta()
            : base(typeof(HistorianKey), typeof(HistorianValue), TypeGuid)
        {

        }

        // {F9B08E1E-2D3E-466A-A186-453064588087}
        /// <summary>
        /// A unique identifier for this compression method.
        /// </summary>
        public readonly static Guid TypeGuid = new Guid(0xf9b08e1e, 0x2d3e, 0x466a, 0xa1, 0x86, 0x45, 0x30, 0x64, 0x58, 0x80, 0x87);

        /// <summary>
        /// Creates a TreeNodeBase
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="level"></param>
        /// <returns></returns>
        public override SortedTreeNodeBase<TKey, TValue> Create<TKey, TValue>(byte level)
        {
            return (SortedTreeNodeBase<TKey, TValue>)(object)new HistorianCompressionDelta(level);
        }


    }
}