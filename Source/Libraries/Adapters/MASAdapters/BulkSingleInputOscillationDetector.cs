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
using GSF.TimeSeries.Adapters;
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
        /// Gets or sets the triggering threshold for band 1 oscillation energy applied to each adapter.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 1 oscillation energy applied to each adapter.")]
        [DefaultValue(DefaultBand1TriggerThreshold)]
        public double Band1TriggerThreshold { get; set; } = DefaultBand1TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 2 oscillation energy applied to each adapter.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 2 oscillation energy applied to each adapter.")]
        [DefaultValue(DefaultBand2TriggerThreshold)]
        public double Band2TriggerThreshold { get; set; } = DefaultBand2TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 3 oscillation energy applied to each adapter.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 3 oscillation energy applied to each adapter.")]
        [DefaultValue(DefaultBand3TriggerThreshold)]
        public double Band3TriggerThreshold { get; set; } = DefaultBand3TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 4 oscillation energy applied to each adapter.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 4 oscillation energy applied to each adapter.")]
        [DefaultValue(DefaultBand4TriggerThreshold)]
        public double Band4TriggerThreshold { get; set; } = DefaultBand4TriggerThreshold;

        /// <summary>
        /// Gets output measurement names.
        /// </summary>
        public override ReadOnlyCollection<string> OutputNames => Array.AsReadOnly(Outputs.Select(output => $"{output}").ToArray());

        /// <summary>
        /// Returns the detailed status of the <see cref="BulkSingleInputOscillationDetector"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat("  Band 1 Trigger Threshold: {0:N3}", Band1TriggerThreshold);
                status.AppendLine();
                status.AppendFormat("  Band 2 Trigger Threshold: {0:N3}", Band2TriggerThreshold);
                status.AppendLine();
                status.AppendFormat("  Band 3 Trigger Threshold: {0:N3}", Band3TriggerThreshold);
                status.AppendLine();
                status.AppendFormat("  Band 4 Trigger Threshold: {0:N3}", Band4TriggerThreshold);
                status.AppendLine();
                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion
    }
}
