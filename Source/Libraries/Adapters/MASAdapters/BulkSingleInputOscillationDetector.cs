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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GSF;
using GSF.Diagnostics;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units.EE;
using static MAS.OscillationDetector;
using MeasurementRecord = MAS.Model.Measurement;

namespace MAS
{
    /// <summary>
    /// Represents an adapter that manages bulk detection of oscillations based on individual inputs.
    /// </summary>
    [Description("MAS Oscillation Detector [Bulk Single Input]: Manages bulk detection of oscillations based on individual inputs.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class BulkSingleInputOscillationDetector : MultiActionAdapterCollectionBase
    {
        #region [ Members ]

        // Fields
        private ShortSynchronizedOperation m_manageChildAdapters;
        private uint m_adapterID;

        #endregion

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
        /// Gets or sets template for output measurement point tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement point tag names, typically an expression like \"" + DefaultPointTagTemplate + "\".")]
        [DefaultValue(DefaultPointTagTemplate)]
        public string PointTagTemplate { get; set; } = DefaultPointTagTemplate;

        /// <summary>
        /// Gets or sets template for local signal reference measurement name for source historian point.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement signal reference names, typically an expression like \"" + DefaultSignalReferenceTemplate + "\".")]
        [DefaultValue(DefaultSignalReferenceTemplate)]
        public string SignalReferenceTemplate { get; set; } = DefaultSignalReferenceTemplate;

        /// <summary>
        /// Gets or sets signal type for output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the signal type for output measurements.")]
        [DefaultValue(typeof(SignalType), DefaultSignalType)]
        public SignalType SignalType { get; set; } = (SignalType)Enum.Parse(typeof(SignalType), DefaultSignalType);

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
                status.AppendFormat("        Point Tag Template: {0}", PointTagTemplate);
                status.AppendLine();
                status.AppendFormat(" Signal Reference Template: {0}", SignalReferenceTemplate);
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

            // Define a synchronized operation to manage bulk collection of child adapters
            m_manageChildAdapters = new ShortSynchronizedOperation(ManageChildAdapters, ex => OnProcessException(MessageLevel.Warning, ex));

            // Kick off initial child adapter management operations
            m_manageChildAdapters.RunOnceAsync();
        }

        /// <summary>
        /// Base class calls this method when data source has changed. Data source updates indicate a change
        /// in system configuration, so we update the child adapters, adding or removing as needed.
        /// </summary>
        protected override void DataSourceChanged() => m_manageChildAdapters?.RunOnceAsync();

        private void ManageChildAdapters()
        {
            HashSet<string> activeAdapterNames = new HashSet<string>(StringComparer.Ordinal);

            // Create settings dictionary for connection string to use with primary child adapters
            Dictionary<string, string> settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Band1TriggerThreshold)] = $"{Band1TriggerThreshold}", 
                [nameof(Band2TriggerThreshold)] = $"{Band2TriggerThreshold}", 
                [nameof(Band3TriggerThreshold)] = $"{Band3TriggerThreshold}", 
                [nameof(Band4TriggerThreshold)] = $"{Band4TriggerThreshold}"
            };

            // Create a child adapter for every input value provided to the parent bulk collection-based adapter
            foreach (MeasurementKey key in InputMeasurementKeys)
            {
                string inputPointTag = LookupPointTag(key);
                string adapterName = $"{nameof(SingleInputOscillationDetector).ToUpper()}!{inputPointTag}";

                // Track active adapter names so that adapters that no longer have sources can be removed
                activeAdapterNames.Add(adapterName);

                // See if child adapter already exists
                if (FindAdapter(adapterName) != null)
                    continue;

                // Setup new child adapter
                string[] outputs = new string[Outputs.Length];

                // Setup output measurements for child adapter
                foreach (Output output in Outputs)
                {
                    string outputID = $"{output.ToString().ToUpper()}-{inputPointTag}";
                    string outputPointTag = string.Format(PointTagTemplate, outputID);
                    string signalReference = string.Format(SignalReferenceTemplate, outputID);

                    // Get output measurement record, creating a new one if needed
                    MeasurementRecord measurement = GetMeasurementRecord(outputPointTag, signalReference, SignalType);
                    
                    outputs[(int)output] = measurement.SignalID.ToString();
                }

                // Add inputs and outputs to connection string settings for child adapter
                settings[nameof(InputMeasurementKeys)] = inputPointTag;
                settings[nameof(OutputMeasurements)] = string.Join(";", outputs);

                // Add new adapter to parent bulk adapter collection, this will auto-initialize child adapter
                Add(new SingleInputOscillationDetector
                {
                    Name = adapterName,
                    ID = m_adapterID++,
                    ConnectionString = settings.JoinKeyValuePairs(),
                    DataSource = DataSource
                });
            }

            // Check for adapters that are no longer referenced and need to be removed
            List<IActionAdapter> adaptersToRemove = new List<IActionAdapter>();

            foreach (IActionAdapter adapter in this)
            {
                if (!activeAdapterNames.Contains(adapter.Name))
                    adaptersToRemove.Add(adapter);
            }

            foreach (IActionAdapter adapter in adaptersToRemove)
                Remove(adapter);
        }

        #endregion
    }
}
