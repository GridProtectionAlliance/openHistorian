//******************************************************************************************************
//  AddSynchrophasorDeviceDev.AliasedVoltage.js - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  06/04/2025 - J. Ritchie Carroll
//       Generated original version of source code as part of "AddSynchrophasorDeviceDev.cshtml".
//  08/31/2026 - J. Ritchie Carroll
//       Extracted from "AddSynchrophasorDeviceDev.cshtml" and parked pending an opt-in design.
//
//******************************************************************************************************
//
//  PARKED FEATURE - NOT ACTIVE
//  ----------------------------------------------------------------------------------------------------
//  This file is not loaded by anything. It is source control only: it is intentionally not listed as a
//  <Content Include> item in "openHistorian.csproj", so it is never copied to an install and is never
//  served by the web host. It is parked because the feature is invasive and not yet optional - see
//  "Why it is parked" below. Do not paste it back without addressing that.
//
//  WHAT IT DID
//  ----------------------------------------------------------------------------------------------------
//  For every current phasor mapped to a voltage, it republished that voltage's magnitude under a second
//  point tag named after the current. This is an alias of an existing measurement, not a new
//  calculation - the dynamic calculator adapter it created was a pure pass-through, equation "value".
//
//  The intent: when several lines or circuits on one device share a single bus voltage, every line's
//  current maps to the same voltage phasor. Consumers that want a voltage tag per line would otherwise
//  have to follow the voltage / current association themselves. This materialized a per-line copy named
//  for that line's current so a display, export or per-circuit analytic could pick up a tag such as
//  "<device>-VBUS_IA:CALC-AV1" alongside the IA tags. Magnitude only - no angle was aliased.
//
//  GATING - all three had to hold, per current phasor:
//
//      1) the current had an associated voltage,
//      2) current.Label() !== voltage.Label(), case-insensitive, i.e., no alias was created when the
//         naming convention already tied the two together,
//      3) the associated voltage had a saved "vphmMeasurement".
//
//  WHAT IT CREATED - "index" counted qualifying currents, starting at 1:
//
//      Measurement    {ACRONYM}-AV-{CALC suffix}{index}                e.g., SUB1-AV-CV1
//      Point tag      {ACRONYM}-{VLABEL}_{ILABEL}:CALC-AV{index}       e.g., SUB1-VBUS_IA:CALC-AV1
//      Description    {ACRONYM} {vlabel}-{ilabel} Calculated Value: Aliased Voltage {index}
//      Adapter        {clean point tag}-CALC, DynamicCalculator.DynamicCalculator
//
//  WHY IT IS PARKED
//  ----------------------------------------------------------------------------------------------------
//  It is unconditional. It ran as the first step of "saveTemplateCalculations", before tag templates
//  were even loaded, and was not driven by the selected tag template at all. Every device saved through
//  the page with differing-label voltage / current pairs got these tags regardless of which template the
//  user picked - unlike everything else in that function, which is template driven. There is no way to
//  decline it.
//
//  The index is positional. The "count" variable increments over qualifying currents in phasor
//  definition order, so if the qualifying set changes between saves - a current disabled, a mapping
//  cleared, phasors re-sorted - then "-AV-{suffix}{n}" can silently re-point at a different line, and
//  existing archived history then belongs to the wrong circuit.
//
//  It ignores Enabled(). A current excluded from the update through the include checkbox still produced
//  an alias, because the loop never checks "phasorDefinition.Enabled()".
//
//  BEFORE REINSTATING
//  ----------------------------------------------------------------------------------------------------
//      - Make it opt-in, e.g., a tag template type, a connection string setting or a screen check box,
//        so that it is not forced on every device.
//      - Key the output on something stable, e.g., the phasor SourceIndex or the current label, instead
//        of a positional counter, so that tags do not migrate between lines across saves.
//      - Honor Enabled() for both the current and its associated voltage.
//
//  REINTEGRATION - two edits to "AddSynchrophasorDeviceDev.cshtml", both in the calculation save path:
//  ----------------------------------------------------------------------------------------------------
//  1) Paste the function below back in, immediately before "function saveCustomActionAdapter". The
//     original eight space script block indentation is preserved, so it pastes in as-is.
//
//  2) In "saveTemplateCalculations", chain it back into the promise. Change:
//
//         return resolveSourceDeviceVoltageMeasurements(cell).then(function() {
//             return phasorHub.loadTemplate(templateType).then(function (tagTemplates) {
//
//     back to:
//
//         return resolveSourceDeviceVoltageMeasurements(cell).then(function () {
//             return saveAliasedVoltageMagnitudes(cell, calcSignalType);
//         })
//         .then(function() {
//             return phasorHub.loadTemplate(templateType).then(function (tagTemplates) {
//
//     and restore the trailing failure message on that chain from "Failed to resolve associated voltage
//     measurements: " back to "Failed to save aliased voltage calculations: ".
//
//  The code below is the current revision, not the original 06/04/2025 text: it was updated along with
//  the rest of the page to resolve its associated voltage through "resolveAssociatedVoltage", so it
//  already handles an associated voltage that lives on another device.
//
//  HISTORY
//  ----------------------------------------------------------------------------------------------------
//  Added in commit 1c9b718e1c, "Added initial aliased voltage calcs". Only ever present in the
//  openHistorian dev page - it was never promoted into the GSF source of record,
//  "PhasorWebUI/Views/AddSynchrophasorDevice.cshtml", which is why that view and the dev page had
//  diverged. Extracted here when the dev page voltage / current mapping work was ported back to GSF so
//  that the two files differ only by the embedded resource header and footer.
//
//******************************************************************************************************

        function saveAliasedVoltageMagnitudes(cell, calcSignalType) {
            return initializeDynamicCalculation(cell).then(function () {
                function addDynamicCalculatorAdapter(inputMeasurement, outputMeasurement) {
                    const connectionString = new Dictionary();
                    const variableList = new Dictionary();
                    const equation = "value";

                    variableList.set("value", inputMeasurement.SignalID);

                    connectionString.set("variableList", variableList.joinKeyValuePairs());
                    connectionString.set("expressionText", equation);
                    connectionString.set("framesPerSecond", viewModel.configFrame().FrameRate);
                    connectionString.set("lagTime", "@DefaultCalculationLagTime");
                    connectionString.set("leadTime", "@DefaultCalculationLeadTime");
                    connectionString.set("outputMeasurements", outputMeasurement.SignalID);
                    connectionString.set("useLatestValues", "false");

                    return phasorHub.newCustomActionAdapter().then(function (adapter) {
                        adapter.NodeID = nodeID;
                        adapter.AdapterName = getCleanAcronym(outputMeasurement.PointTag) + "-CALC";
                        adapter.AssemblyName = "DynamicCalculator.dll";
                        adapter.TypeName = "DynamicCalculator.DynamicCalculator";
                        adapter.ConnectionString = connectionString.joinKeyValuePairs();
                        adapter.LoadOrder = 100;
                        adapter.Enabled = true;

                        return phasorHub.addNewOrUpdateCustomActionAdapter(adapter).then(function () {
                            return serviceHub.sendCommand("initialize " + adapter.AdapterName);
                        })
                        .fail(function (error) {
                            showErrorMessage("Save failed for new custom action adapter: " + error, null, true);
                        });
                    })
                    .fail(function (error) {
                        showErrorMessage("Failed to create new custom action adapter record: " + error, null, true);
                    });
                }

                const promises = [];
                let count = 0;

                for (let i = 0; i < cell.PhasorDefinitions.length; i++) {
                    const phasorDefinition = cell.PhasorDefinitions[i];

                    if (phasorDefinition.PhasorType.toLowerCase() === "current") {
                        const current = phasorDefinition;
                        const voltage = resolveAssociatedVoltage(cell, phasorDefinition);

                        if (voltage && current.Label().toLowerCase() !== voltage.Label().toLowerCase() && voltage.hasOwnProperty("vphmMeasurement")) {
                            count++;
                            const index = count;
                            const orgSignalReference = String.format("{0}-{1}-{2}{3}", cell.OriginalAcronym(), "AV", calcSignalType.Suffix, index.toString());
                            const signalReference = String.format("{0}-{1}-{2}{3}", cell.IDLabel(), "AV", calcSignalType.Suffix, index.toString());

                            // Query existing measurement record for specified signal reference - function will create a new blank measurement record if one does not exist
                            promises.push(phasorHub.queryMeasurement(orgSignalReference).then(function (measurement) {
                                const voltageLabel = voltage.Label().replaceAll(" ", "_").toUpperCase();
                                const currentLabel = current.Label().replaceAll(" ", "_").toUpperCase();
                                measurement.DeviceID = cell.ID;
                                measurement.HistorianID = viewModel.historianID();
                                measurement.PointTag = getCleanPointTag(String.format("{0}-{1}_{2}:CALC-AV{3}", cell.IDLabel(), voltageLabel, currentLabel, index));
                                measurement.Description = String.format("{0} {1}-{2} Calculated Value: Aliased Voltage {3}", cell.IDLabel(), voltage.Label(), current.Label(), index);
                                measurement.SignalReference = signalReference;
                                measurement.SignalTypeID = calcSignalType.ID;
                                measurement.Internal = true;
                                measurement.Enabled = true;

                                return phasorHub.addNewOrUpdateMeasurement(measurement).then(function () {
                                    return phasorHub.queryMeasurement(signalReference).then(function (updatedMeasurement) {
                                        return addDynamicCalculatorAdapter(voltage.vphmMeasurement, updatedMeasurement);
                                    })
                                    .fail(function (error) {
                                        showErrorMessage("Failed to lookup measurement " + signalReference + ": " + error, null, true);
                                    });
                                })
                                .fail(function (error) {
                                    showErrorMessage("Save failed for new measurement " + signalReference + ": " + error, null, true);
                                });
                            })
                            .fail(function (error) {
                                showErrorMessage("Failed to lookup or create measurement " + signalReference + ": " + error, null, true);
                            }));
                        }
                    }
                }

                return $(promises).whenAll();
            });
        }
