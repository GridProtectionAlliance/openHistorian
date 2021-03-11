//******************************************************************************************************
//  GrafanaPhasorController.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  10/27/2017 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using openHistorian.Adapters.Model;
using openHistorian.Model;
using GrafanaAdapters;
using CancellationToken = System.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana "phasor" based data source,
    /// accessible from Grafana data source as http://localhost:8180/api/grafanaphasor
    /// </summary>
    public class GrafanaPhasorController : GrafanaController
    {
        #region [ Members ]

        /// <summary>
        /// Defines a Grafana query request target.
        /// </summary>
        public class PhasorQueryRequest
        {
            /// <summary>
            /// Panel ID of request.
            /// </summary>
            public int panelId { get; set; }

            /// <summary>
            /// Request range.
            /// </summary>
            public Range range { get; set; }

            /// <summary>
            /// Relative request range.
            /// </summary>
            public RangeRaw rangeRaw { get; set; }

            /// <summary>
            /// Request interval.
            /// </summary>
            public string interval { get; set; }

            /// <summary>
            /// Request targets.
            /// </summary>
            public List<PhasorTarget> targets { get; set; }

            /// <summary>
            /// Request format (typically json).
            /// </summary>
            public string format { get; set; }

            /// <summary>
            /// Maximum data points to return.
            /// </summary>
            public int maxDataPoints { get; set; }

            /// <summary>
            /// Defines options current in play for data source that may affect query.
            /// </summary>
            public dynamic options { get; set; }

            /// <summary>
            /// casting from PhasorQueryRequest to QueryRequest.
            /// </summary>
            public static explicit operator QueryRequest(PhasorQueryRequest request)
            {
                QueryRequest nr = new QueryRequest();
                nr.panelId = request.panelId;
                nr.range = request.range;
                nr.rangeRaw = request.rangeRaw;
                nr.interval = request.interval;
                nr.format = request.format;
                nr.maxDataPoints = request.maxDataPoints;
                nr.targets = new List<Target>();
                foreach (PhasorTarget pqr in request.targets)
                {
                    Target target = new Target();
                    target.refId = pqr.refId;
                    target.target = pqr.target;
                    nr.targets.Add(target);
                }

                return nr;
            }
        }

        /// <summary>
        /// Defines a Grafana query request target.
        /// </summary>
        public class PhasorTarget
        {
            /// <summary>
            /// Reference ID.
            /// </summary>
            public string refId { get; set; }

            /// <summary>
            /// Target point/tag name.
            /// </summary>
            public string target { get; set; }

            /// <summary>
            /// Reference point/tag name.
            /// </summary>
            public string referencephasor { get; set; }

        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public override Task<string[]> Search(Target request, CancellationToken cancellationToken)
        {
            string target = string.IsNullOrEmpty(request.target) ? string.Empty : request.target;

            return Task.Factory.StartNew(() =>
            {
                return DataSource?.Metadata.Tables["ActivePhasors"].Select($"Instance LIKE '{DataSource?.InstanceName}' AND PhasorTag LIKE '%{target}%'").Take(DataSource.MaximumSearchTargetsPerRequest).Select(row => $"{row["PhasorTag"]}").ToArray();
            },
            cancellationToken);
        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public override Task<string[]> SearchFields(Target request, CancellationToken cancellationToken)
        {
            request.target = "ActivePhasors";
            return DataSource?.SearchFields(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a table.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public override Task<string[]> SearchFilters(Target request, CancellationToken cancellationToken)
        {
            request.target = "ActivePhasors";
            return DataSource?.SearchFilters(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public override Task<string[]> SearchOrderBys(Target request, CancellationToken cancellationToken)
        {
            request.target = "ActivePhasors";
            return DataSource?.SearchOrderBys(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public Task<IEnumerable<Tuple<string, string, string>>> SearchPhasors(Target request)
        {
            string target = request.target == "select metric" ? "" : request.target;

            if (DataSource is null)
                return Task.FromResult(Enumerable.Empty<Tuple<string, string, string>>());

            return Task.Factory.StartNew(() =>
            {
                return DataSource.Metadata.Tables["ActivePhasors"].Select($"Label LIKE '%{target}%'").Take(DataSource.MaximumSearchTargetsPerRequest).Select(row => Tuple.Create(row["Label"].ToString(), row["MagPointTag"].ToString(), row["AnglePointTag"].ToString()));
            });
        }

        /// <summary>
        /// Queries data source returning data as Grafana time-series data set.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public Task<List<TimeSeriesPhasorValues>> QueryPhasors(PhasorQueryRequest request, CancellationToken cancellationToken)
        {
            if (DataSource is null) return Task.FromResult(new List<TimeSeriesPhasorValues>());

            return Task.Factory.StartNew(() =>
            {
                List<DataSourcePhasorValueGroup> valueGroups = new List<DataSourcePhasorValueGroup>();
                // Query any remaining targets
                foreach (PhasorTarget phasorTarget in request.targets)
                {
                    // Split remaining targets on semi-colon, this way even multiple filter expressions can be used as inputs to functions
                    string[] allTargets = phasorTarget.target.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    // Target set now contains both original expressions and newly parsed individual point tags - to create final point list we
                    // are only interested in the point tags, provided either by direct user entry or derived by parsing filter expressions
                    string refAngle = DataSource.Metadata.Tables["ActivePhasors"].Select($"PhasorTag = '{phasorTarget.referencephasor ?? ""}'").FirstOrDefault()?["AnglePointTag"].ToString() ?? "";

                    List<string> newMags = new List<string>();
                    List<string> newAngles = new List<string>();
                    List<string> newPowers = new List<string>();
                    foreach (string target in allTargets)
                    {
                        if (string.IsNullOrEmpty(target)) continue;

                        DataRow row = DataSource?.Metadata.Tables["ActivePhasors"].Select($"PhasorTag = '{target}'").First();
                        DataRow toRow = DataSource?.Metadata.Tables["ActivePhasors"].Select($"ID = {(string.IsNullOrEmpty(row["DestinationPhasorID"].ToString()) ? "-1" : row["DestinationPhasorID"].ToString()) }").FirstOrDefault();
                        DataSourcePhasorValueGroup value = new DataSourcePhasorValueGroup();
                        value.Target = target;
                        value.PhasorID = int.Parse(row["ID"].ToString());
                        value.MagnitudeTarget = row["MagPointTag"].ToString();
                        value.AngleTarget = row["AnglePointTag"].ToString();
                        value.PowerTarget = row["PowerPointTag"].ToString();
                        value.Latitude = row["Latitude"].ToString();
                        value.Longitude = row["Longitude"].ToString();
                        value.ToLatitude = row["ToLatitude"].ToString();
                        value.ToLongitude = row["ToLongitude"].ToString();
                        value.DestinationPhasorLabel = toRow?["PhasorTag"].ToString() ?? "";
                        value.ToAngleTarget = toRow?["AnglePointTag"].ToString() ?? "";
                        value.ReferenceAngle = refAngle;
                        valueGroups.Add(value);

                        newMags.Add(value.MagnitudeTarget);
                        newAngles.Add(value.AngleTarget);
                        if (!newPowers.Exists(x => x == value.PowerTarget))
                            newPowers.Add(value.PowerTarget);
                    }

                    newAngles.Add(refAngle);

                    phasorTarget.target = string.Join(";", newMags) + ";" + string.Join(";", newAngles) + ";" + string.Join(";", newPowers);
                }

                List<TimeSeriesValues> queryValues = DataSource?.Query((QueryRequest)request, cancellationToken).Result;

                List<TimeSeriesPhasorValues> result = valueGroups.Select(valueGroup => new TimeSeriesPhasorValues { target = valueGroup.Target, pointtag = valueGroup.Target, latitude = valueGroup.Latitude, longitude = valueGroup.Longitude, tolatitude = valueGroup.ToLatitude, tolongitude = valueGroup.ToLongitude, todevicepointtag = valueGroup.DestinationPhasorLabel, powerpointtag = valueGroup.PowerTarget, anglepointtag = valueGroup.AngleTarget, toanglepointtag = valueGroup.ToAngleTarget, magpointtag = valueGroup.MagnitudeTarget }).ToList();

                foreach (DataSourcePhasorValueGroup group in valueGroups)
                {
                    List<double[]> refAngles = queryValues.FirstOrDefault(x => x.rootTarget == group.ReferenceAngle)?.datapoints;
                    TimeSeriesPhasorValues r = result.First(x => x.target == group.Target);
                    r.magdatapoints = queryValues.FirstOrDefault(x => x.rootTarget == group.MagnitudeTarget)?.datapoints ?? new List<double[]>();
                    r.powerdatapoints = queryValues.FirstOrDefault(x => x.rootTarget == group.PowerTarget)?.datapoints ?? new List<double[]>();

                    if (refAngles != null)
                        r.angledatapoints = Difference(queryValues.First(x => x.rootTarget == group.AngleTarget).datapoints, refAngles);
                    else
                        r.angledatapoints = queryValues.First(x => x.rootTarget == group.AngleTarget).datapoints;

                    r.magvalue = (double)((r.magdatapoints?.LastOrDefault()?[0] ?? 0) / r.magdatapoints?.Select(x => x[0]).DefaultIfEmpty(1).Average());
                    r.powervalue = r.powerdatapoints?.LastOrDefault()?[0] ?? 0;
                    r.anglevalue = r.angledatapoints?.LastOrDefault()?[0] ?? 0;
                    r.maxanglevalue = r.angledatapoints?.Select(x => x[0]).DefaultIfEmpty(0).Max() ?? 0;
                    r.minanglevalue = r.angledatapoints?.Select(x => x[0]).DefaultIfEmpty(0).Min() ?? 0;
                }


                return result;

            }, cancellationToken);
        }

        private List<double[]> Difference(List<double[]> a, List<double[]> b)
        {
            if (a.Count != b.Count)
                return a;

            for (int i = 0; i < a.Count; ++i)
            {
                a[i][0] = Get360Angle(b[i][0]) - Get360Angle(a[i][0]);
            }
            return a;
        }

        private double Get360Angle(double angle)
        {
            return angle < 0 ? 360 + angle : angle;
        }

        #endregion
    }
}
