//******************************************************************************************************
//  CompressionSetting.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  07/26/2017 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using GSF.Data;
using GSF.TimeSeries.UI;
using GSF.TimeSeries.UI.DataModels;

namespace openHistorian.UI.DataModels
{
    public class CompressionSetting : DataModelBase
    {
        #region [ Members ]

        // Fields
        private int m_pointID;
        private ulong m_compressionMinTime;
        private ulong m_compressionMaxTime;
        private double m_compressionLimit;
        private Measurement m_referencedMeasurement;

        #endregion

        #region [ Constructors ]

        public CompressionSetting()
        {
            IsNew = true;
        }

        #endregion

        #region [ Properties ]
        
        public int PointID
        {
            get => m_pointID;
            set
            {
                m_pointID = value;
                m_referencedMeasurement = null;
                OnPropertyChanged(nameof(PointID));
                OnPropertyChanged(nameof(PointTag));
            }
        }

        public ulong CompressionMinTime
        {
            get => m_compressionMinTime;
            set
            {
                m_compressionMinTime = value;
                OnPropertyChanged(nameof(CompressionMinTime));
            }
        }

        public ulong CompressionMaxTime
        {
            get => m_compressionMaxTime;
            set
            {
                m_compressionMaxTime = value;
                OnPropertyChanged(nameof(CompressionMaxTime));
            }
        }

        public double CompressionLimit
        {
            get => m_compressionLimit;
            set
            {
                m_compressionLimit = value;
                OnPropertyChanged(nameof(CompressionLimit));
            }
        }

        public string PointTag => ReferencedMeasurement?.PointTag;

        public bool IsNew
        {
            get;
            set;
        }

        private Measurement ReferencedMeasurement => m_referencedMeasurement ?? GetReferencedMeasurement();

    #endregion

        #region [ Methods ]

        private Measurement GetReferencedMeasurement()
        {
            return Measurement.GetMeasurement(null, $"WHERE PointID = {m_pointID}");
        }

        #endregion

        #region [ Operators ]

        #endregion

        #region [ Static ]

        // Static Methods

        /// <summary>
        /// Loads <see cref="CompressionSetting"/> IDs as an <see cref="IList{T}"/>.
        /// </summary>        
        /// <param name="database"><see cref="AdoDataConnection"/> to connection to database.</param>
        /// <param name="sortMember">The field to sort by.</param>
        /// <param name="sortDirection"><c>ASC</c> or <c>DESC</c> for ascending or descending respectively.</param>
        /// <returns>Collection of <see cref="Int32"/>.</returns>
        public static IList<int> LoadKeys(AdoDataConnection database, string sortMember = "", string sortDirection = "")
        {
            bool createdConnection = false;

            try
            {
                createdConnection = CreateConnection(ref database);

                IList<int> compressionSettingList = new List<int>();
                string sortClause = string.Empty;
                DataTable compressionSettingTable;

                if (!string.IsNullOrEmpty(sortMember))
                    sortClause = string.Format("ORDER BY {0} {1}", sortMember, sortDirection);

                compressionSettingTable = database.RetrieveData($"SELECT PointID FROM CompressionSetting {sortClause}");

                foreach (DataRow row in compressionSettingTable.Rows)
                    compressionSettingList.Add(row.ConvertField<int>("PointID"));

                return compressionSettingList;
            }
            finally
            {
                if (createdConnection && database != null)
                    database.Dispose();
            }
        }

        /// <summary>
        /// Loads <see cref="CompressionSetting"/> information as an <see cref="ObservableCollection{T}"/> style list.
        /// </summary>        
        /// <param name="database"><see cref="AdoDataConnection"/> to connection to database.</param>
        /// <param name="keys">Keys of the compression settings to be loaded from the database.</param>
        /// <returns>Collection of <see cref="CompressionSetting"/>.</returns>
        public static ObservableCollection<CompressionSetting> Load(AdoDataConnection database, IList<int> keys)
        {
            bool createdConnection = false;

            try
            {
                createdConnection = CreateConnection(ref database);

                string commaSeparatedKeys;

                CompressionSetting[] compressionSettingList = null;
                DataTable compressionSettingTable;
                int pointID;

                if (keys != null && keys.Count > 0)
                {
                    commaSeparatedKeys = keys.Select(key => "" + key.ToString() + "").Aggregate((str1, str2) => str1 + "," + str2);

                    compressionSettingTable = database.RetrieveData(
                        $"SELECT PointID, CompressionMinTime, CompressionMaxTime, CompressionLimit " +
                        $"FROM CompressionSetting WHERE PointID IN ({commaSeparatedKeys})");

                    compressionSettingList = new CompressionSetting[compressionSettingTable.Rows.Count];

                    foreach (DataRow row in compressionSettingTable.Rows)
                    {
                        pointID = row.ConvertField<int>(nameof(PointID));

                        compressionSettingList[keys.IndexOf(pointID)] = new CompressionSetting()
                        {
                            PointID = pointID,
                            CompressionMinTime = row.ConvertField<ulong>(nameof(CompressionMinTime)),
                            CompressionMaxTime = row.ConvertField<ulong>(nameof(CompressionMaxTime)),
                            CompressionLimit = row.ConvertField<double>(nameof(CompressionLimit)),
                            IsNew = false
                        };
                    }
                }

                return new ObservableCollection<CompressionSetting>(compressionSettingList ?? new CompressionSetting[0]);
            }
            finally
            {
                if (createdConnection && database != null)
                    database.Dispose();
            }
        }

        /// <summary>
        /// Saves <see cref="CompressionSetting"/> information to database.
        /// </summary>
        /// <param name="database"><see cref="AdoDataConnection"/> to connection to database.</param>
        /// <param name="compressionSetting">Information about <see cref="CompressionSetting"/>.</param>        
        /// <returns>String, for display use, indicating success.</returns>
        public static string Save(AdoDataConnection database, CompressionSetting compressionSetting)
        {
            bool createdConnection = false;

            try
            {
                createdConnection = CreateConnection(ref database);

                string successMessage = "CompressionSetting information saved successfully";

                if (compressionSetting.IsNew)
                {
                    database.ExecuteNonQuery(
                        $"INSERT INTO CompressionSetting ({nameof(PointID)}, {nameof(CompressionMinTime)}, {nameof(CompressionMaxTime)}, {nameof(CompressionLimit)}) " +
                        $"VALUES({{0}}, {{1}}, {{2}}, {{3}})", compressionSetting.PointID, compressionSetting.CompressionMinTime, compressionSetting.CompressionMaxTime, compressionSetting.CompressionLimit);

                    compressionSetting.IsNew = false;
                }
                else
                {
                    database.ExecuteNonQuery(
                        $"UPDATE CompressionSetting SET " +
                        $"    {nameof(CompressionMinTime)} = {{0}}, " +
                        $"    {nameof(CompressionMaxTime)} = {{1}}, " +
                        $"    {nameof(CompressionLimit)} = {{2}} " +
                        $"WHERE {nameof(PointID)} = {{3}}",
                        compressionSetting.CompressionMinTime, compressionSetting.CompressionMaxTime, compressionSetting.CompressionLimit, compressionSetting.PointID);
                }

                try
                {
                    CommonFunctions.SendCommandToService("ReloadConfig");
                }
                catch (Exception ex)
                {
                    CommonFunctions.LogException(database, "CompressionSetting Save", ex);
                }

                return successMessage;
            }
            finally
            {
                if (createdConnection && database != null)
                    database.Dispose();
            }
        }

        /// <summary>
        /// Deletes specified <see cref="CompressionSetting"/> record from database.
        /// </summary>
        /// <param name="database"><see cref="AdoDataConnection"/> to connection to database.</param>
        /// <param name="pointID">ID of the record to be deleted.</param>
        /// <returns>String, for display use, indicating success.</returns>
        public static string Delete(AdoDataConnection database, int pointID)
        {
            bool createdConnection = false;

            try
            {
                createdConnection = CreateConnection(ref database);

                // Setup current user context for any delete triggers
                CommonFunctions.SetCurrentUserContext(database);
                database.ExecuteNonQuery("DELETE FROM CompressionSetting WHERE PointID = {0}", pointID);
                CommonFunctions.SendCommandToService("ReloadConfig");

                return "CompressionSetting deleted successfully";
            }
            finally
            {
                if (createdConnection && database != null)
                    database.Dispose();
            }
        }

        #endregion
    }
}
