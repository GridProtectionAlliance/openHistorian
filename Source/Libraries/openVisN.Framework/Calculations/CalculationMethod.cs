//******************************************************************************************************
//  CalculationMethod.cs - Gbtc
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Data.Query;

namespace openVisN.Calculations
{
    public class CalculationMethod
    {
        public static CalculationMethod Empty
        {
            get;
            private set;
        }

        static CalculationMethod()
        {
            Empty = new CalculationMethod();
        }

        protected MetadataBase[] Dependencies;

        protected CalculationMethod(params MetadataBase[] dependencies)
        {
            Dependencies = dependencies;
            foreach (MetadataBase point in dependencies)
                if (point==null)
                    throw new NullReferenceException();
        }

        public virtual void Calculate(IDictionary<Guid, SignalDataBase> signals)
        {
            
        }

        public void AddDependentPoints(HashSet<MetadataBase> dependencies)
        {
            foreach (MetadataBase point in Dependencies)
            {
                dependencies.Add(point);
                point.Calculations.AddDependentPoints(dependencies);
            }
        }
    }
}