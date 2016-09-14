//******************************************************************************************************
//  IGrafanaDataSource.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  09/12/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines a Grafana time-series value.
    /// </summary>
    public class TimeSeriesValues
    {
        /// <summary>
        /// Defines a Grafana time-series value point source.
        /// </summary>
        public string target;

        /// <summary>
        /// Defines a Grafana time-series value data.
        /// </summary>
        /// <remarks>
        /// "datapoints":[
        ///       [622,1450754160000],
        ///       [365,1450754220000]
        /// ]
        /// </remarks>
        public List<double[]> datapoints;
    }

    /// <summary>
    /// Defines a Grafana query range.
    /// </summary>
    public class Range
    {
        /// <summary>
        /// From time for range.
        /// </summary>
        public string from { get; set; }

        // To time for range.
        public string to { get; set; }
    }

    /// <summary>
    /// Defines a Grafana relative query range.
    /// </summary>
    public class RangeRaw
    {
        /// <summary>
        /// Relative from time for raw range.
        /// </summary>
        public string from { get; set; }

        /// <summary>
        /// Relative to time for raw range.
        /// </summary>
        public string to { get; set; }
    }

    /// <summary>
    /// Defines a Grafana query request target.
    /// </summary>
    public class Target
    {
        /// <summary>
        /// Reference ID.
        /// </summary>
        public string refId { get; set; }

        /// <summary>
        /// Target point/tag name.
        /// </summary>
        public string target { get; set; }
    }

    /// <summary>
    /// Defines a Grafana query request.
    /// </summary>
    public class QueryRequest
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
        public List<Target> targets { get; set; }

        /// <summary>
        /// Request format (typically json).
        /// </summary>
        public string format { get; set; }

        /// <summary>
        /// Maximum data points to return.
        /// </summary>
        public int maxDataPoints { get; set; }
    }

    /// <summary>
    /// Defines a Grafana annotation.
    /// </summary>
    public class Annotation
    {
        /// <summary>
        /// Annotation name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Annotation data source.
        /// </summary>
        public string datasource { get; set; }

        /// <summary>
        /// Annotation enabled flag.
        /// </summary>
        public bool enable { get; set; }

        /// <summary>
        /// Annotation icon color.
        /// </summary>
        public string iconColor { get; set; }

        /// <summary>
        /// Annotation query.
        /// </summary>
        public string query { get; set; }
    }

    /// <summary>
    /// Defines a Grafana annotation request.
    /// </summary>
    public class AnnotationRequest
    {
        /// <summary>
        /// Annotation request details.
        /// </summary>
        public Annotation annotation { get; set; }

        /// <summary>
        /// Request range.
        /// </summary>
        public Range range { get; set; }

        /// <summary>
        /// Relative request range.
        /// </summary>
        public RangeRaw rangeRaw { get; set; }
    }

    /// <summary>
    /// Defines a Grafana annotation response.
    /// </summary>
    public class AnnotationResponse
    {
        /// <summary>
        /// Annotation in-response-to request.
        /// </summary>
        public Annotation annotation { get; set; }

        /// <summary>
        /// Annotation title.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Annotation time.
        /// </summary>
        public double time { get; set; }

        /// <summary>
        /// Annotation text.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Annotation tags.
        /// </summary>
        public string tags { get; set; }
    }

    /// <summary>
    /// Defines needed API calls for a Grafana data source.
    /// </summary>
    [ServiceContract]
    public interface IGrafanaDataSource
    {
        /// <summary>
        /// Validates that openHistorian Grafana data source is responding as expected.
        /// </summary>
        [OperationContract, WebInvoke(UriTemplate = "/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void TestDataSource();

        /// <summary>
        /// Queries openHistorian as a Grafana data source.
        /// </summary>
        /// <param name="request">Query request.</param>
        [OperationContract, WebInvoke(UriTemplate = "/query", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<List<TimeSeriesValues>> Query(QueryRequest request);

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        [OperationContract, WebInvoke(UriTemplate = "/search", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] Search(Target request);

        /// <summary>
        /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="request">Annotation request.</param>
        [OperationContract, WebInvoke(UriTemplate = "/annotations", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<AnnotationResponse> Annotations(AnnotationRequest request);
    }
}
