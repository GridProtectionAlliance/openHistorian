//******************************************************************************************************
//  RegisterSortedTreeTypes.cs - Gbtc
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
//  4/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

//******************************************************************************************************
// This is where types for the SortedTree should be placed for registering. This code is called in the 
// SortedTree static class and therefore are automatically registered before any SortedTrees are created.
// If they are not here, you will need to be sure to register them before before instancing a SortedTree
// that utilizes them or an exception will be generated. 
//******************************************************************************************************

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// Automatically called by <see cref="SortedTree"/> static constructor. 
    /// </summary>
    internal static class RegisterSortedTreeTypes
    {
        private static bool s_keysHasBeenCalled = false;
        private static bool s_valuesHasBeenCalled = false;
        private static bool s_treeNodeHasBeenCalled = false;

        internal static void RegisterKeyTypes()
        {
            if (s_keysHasBeenCalled)
                return;
            s_keysHasBeenCalled = true;

            SortedTree.Register(new KeyMethodsUInt32());
            SortedTree.Register(new KeyMethodsUInt128());
            SortedTree.Register(new KeyMethodsHistorianKey());
        }

        internal static void RegisterValueTypes()
        {
            if (s_valuesHasBeenCalled)
                return;
            s_valuesHasBeenCalled = true;

            SortedTree.Register(new ValueMethodsUInt32());
            SortedTree.Register(new ValueMethodsUInt128());
            SortedTree.Register(new ValueMethodsHistorianValue());
        }

        internal static void RegisterTreeNodeType()
        {
            if (s_treeNodeHasBeenCalled)
                return;
            s_treeNodeHasBeenCalled = true;

            SortedTree.Register(new CreateFixedSizeNode());
            SortedTree.Register(new CreateZeroNode());
            SortedTree.Register(new CreateHistorianCompressionDelta());
            SortedTree.Register(new CreateHistorianCompressionTs());
            //SortedTree.Register(new CreateFixedSizeNodeUint128Uint128());
            //SortedTree.Register(new CreateFixedSizeNodeUintUint());
        }
    }
}