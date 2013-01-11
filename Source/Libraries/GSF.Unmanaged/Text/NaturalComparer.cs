//******************************************************************************************************
//  NaturalComparer.cs - Gbtc
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
//  1/11/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

// Origional Source From http://www.codeproject.com/Articles/22517/Natural-Sort-Comparer
// Licensed under The Code Project Open License (CPOL)

namespace GSF.Text
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class NaturalComparer : Comparer<string>, IDisposable
    {
        private Dictionary<string, string[]> table;

        public NaturalComparer()
        {
            table = new Dictionary<string, string[]>();
        }

        public void Dispose()
        {
            table.Clear();
            table = null;
        }

        public override int Compare(string x, string y)
        {
            if (x == y)
            {
                return 0;
            }
            string[] x1, y1;
            if (!table.TryGetValue(x, out x1))
            {
                x1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
                table.Add(x, x1);
            }
            if (!table.TryGetValue(y, out y1))
            {
                y1 = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
                table.Add(y, y1);
            }

            for (int i = 0; i < x1.Length && i < y1.Length; i++)
            {
                if (x1[i] != y1[i])
                {
                    return PartCompare(x1[i], y1[i]);
                }
            }
            if (y1.Length > x1.Length)
            {
                return 1;
            }
            else if (x1.Length > y1.Length)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private static int PartCompare(string left, string right)
        {
            int x, y;
            if (!int.TryParse(left, out x))
            {
                return left.CompareTo(right);
            }

            if (!int.TryParse(right, out y))
            {
                return left.CompareTo(right);
            }

            return x.CompareTo(y);
        }
    }
}
