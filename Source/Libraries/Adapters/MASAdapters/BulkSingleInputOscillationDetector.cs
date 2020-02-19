//******************************************************************************************************
//  BulkSingleInputOscillationDetector.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/13/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using PowerCalculations;
using static MAS.OscillationDetector;

namespace MAS
{
    /// <summary>
    /// Represents an adapter that manages bulk detection of oscillations based on individual inputs.
    /// </summary>
    [Description("MAS Oscillation Detector [Bulk Single Input]: Manages bulk detection of oscillations based on individual inputs.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class BulkSingleInputOscillationDetector : IndependentActionAdapterManagerBase<SingleInputOscillationDetector>
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the default strategy used to adjust voltage values for based on the nature of the voltage measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines default strategy used to adjust voltage values for based on the nature of the voltage measurements.")]
        [DefaultValue(typeof(VoltageAdjustmentStrategy), SingleInputOscillationDetector.DefaultAdjustmentStrategy)]
        public VoltageAdjustmentStrategy AdjustmentStrategy { get; set; } = (VoltageAdjustmentStrategy)Enum.Parse(typeof(VoltageAdjustmentStrategy), SingleInputOscillationDetector.DefaultAdjustmentStrategy);

        /// <summary>
        /// Gets number of input measurement required by each adapter.
        /// </summary>
        public override int InputsPerAdapter => 1;

        /// <summary>
        /// Gets output measurement names.
        /// </summary>
        public override ReadOnlyCollection<string> OutputNames => Array.AsReadOnly(Outputs.Select(output => $"{output}").ToArray());

        /// <summary>
        /// Gets or sets output measurements that the <see cref="AdapterBase"/> will produce, if any.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)] // Hiding parameter from manager - outputs managed automatically
        public override IMeasurement[] OutputMeasurements
        {
            get => base.OutputMeasurements;
            set => base.OutputMeasurements = value;
        }

        /// <summary>
        /// Returns the detailed status of the <see cref="BulkSingleInputOscillationDetector"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat("        Voltage Adjustment: {0}", AdjustmentStrategy);
                status.AppendLine();
                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes the <see cref="BulkSingleInputOscillationDetector" />.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            InitializeChildAdapterManagement();
        }

        #endregion
    }
}
