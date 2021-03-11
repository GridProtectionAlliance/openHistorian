//******************************************************************************************************
//  GetDataReaderMethods.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GSF.Snap.Services;
using openHistorian.Snap;

namespace openHistorian.Data.Query
{
    public interface IDelinearlizedSignals2
    {
        IList<string> ColumnHeaders
        {
            get;
        }

        KeyValuePair<object, IList<ISignalWithType>> ColumnGroups
        {
            get;
        }
    }

    public interface ISignalWithName2 : ISignalWithType
    {
        string TagName
        {
            get;
        }
    }

    public class TableDefinition
    {
        bool m_finishedColumns;
        string m_timestampColumn;
        string m_identifierColumn;
        Type m_identifierType;

        readonly DataTable m_table;
        internal List<KeyValuePair<object, int?[]>> m_signalGroups;
        readonly List<string> m_customColumns;

        public TableDefinition()
        {
            m_customColumns = new List<string>();
            m_finishedColumns = true;
            m_table = new DataTable();
            m_signalGroups = new List<KeyValuePair<object, int?[]>>();
        }

        public void AddTimestampColumn(string name)
        {
            if (m_finishedColumns)
                throw new Exception("Cannot continue to add columns once groups have been added.");
            if (m_timestampColumn is null)
                throw new Exception("Duplicate calls to AddTimestampColumn");


            m_timestampColumn = name;
            m_table.Columns.Add(name, typeof(DateTime));
        }
        public void AddColumnGroupIdentifier(string name, Type type)
        {
            if (m_finishedColumns)
                throw new Exception("Cannot continue to add columns once groups have been added.");
            if (m_identifierColumn is null)
                throw new Exception("Duplicate calls to AddColumnGroupIdentifier");

            m_identifierColumn = name;
            m_identifierType = type;
            m_table.Columns.Add(name, type);
        }
        public void AddColumn(string name, Type type)
        {
            if (m_finishedColumns)
                throw new Exception("Cannot continue to add columns once groups have been added.");
            m_table.Columns.Add(name, type);
            m_customColumns.Add(name);
        }

        public void AddGroups(object identifier, params int?[] pointId)
        {
            if (!m_finishedColumns)
            {
                if (m_timestampColumn is null)
                    throw new Exception("Must first call AddTimestampColumn");
                if (m_identifierColumn is null)
                    throw new Exception("Must first call AddColumnGroupIdentifier");
                if (m_customColumns.Count == 0)
                    throw new Exception("Must have custom columns");
            }
            if (identifier.GetType() != m_identifierType)
                throw new Exception("Type Mismatch: identifier is not of the correct type");

            if (pointId.Length != m_customColumns.Count)
                throw new Exception("The number of points must equal the number of custom columns.");
            m_finishedColumns = true;

        }
    }

    public class HistorianDataPointReader
        : IDataReader
    {
        TableDefinition m_tableDefinition;
        SortedList<DateTime, FrameData> m_results;
        int m_currentFrame;

        public HistorianDataPointReader(ClientDatabaseBase<HistorianKey, HistorianValue> database, DateTime start, DateTime stop, TableDefinition tableDefinition)
        {
            HashSet<ulong> allPoints = new HashSet<ulong>();
            m_tableDefinition = tableDefinition;

            foreach (KeyValuePair<object, int?[]> signal in tableDefinition.m_signalGroups)
            {
                foreach (int? point in signal.Value)
                {
                    if (point.HasValue)
                        allPoints.Add((ulong)point.Value);
                }
            }
            //m_results = database.GetFrames((ulong)start.Ticks, (ulong)stop.Ticks, allPoints);
            m_currentFrame = -1;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Object"/> which will contain the field value upon return.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current record.
        /// </summary>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object"/> in the array.
        /// </returns>
        /// <param name="values">An array of <see cref="T:System.Object"/> to copy the attribute fields into. </param><filterpriority>2</filterpriority>
        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <returns>
        /// The index of the named field.
        /// </returns>
        /// <param name="name">The name of the field to find. </param><filterpriority>2</filterpriority>
        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <returns>
        /// The value of the column.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <returns>
        /// The actual number of bytes read.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><param name="fieldOffset">The index within the field from which to start the read operation. </param><param name="buffer">The buffer into which to read the stream of bytes. </param><param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation. </param><param name="length">The number of bytes to read. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <returns>
        /// The actual number of characters read.
        /// </returns>
        /// <param name="i">The zero-based column ordinal. </param><param name="fieldoffset">The index within the row from which to start the read operation. </param><param name="buffer">The buffer into which to read the stream of bytes. </param><param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation. </param><param name="length">The number of bytes to read. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <returns>
        /// The GUID value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <returns>
        /// The string value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader"/> for the specified column ordinal.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Data.IDataReader"/> for the specified column ordinal.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <param name="i">The index of the field to find. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public int FieldCount { get; private set; }

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <returns>
        /// The column located at the specified index as an <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="i">The zero-based index of the column to get. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
        object IDataRecord.this[int i] => throw new NotImplementedException();

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <returns>
        /// The column with the specified name as an <see cref="T:System.Object"/>.
        /// </returns>
        /// <param name="name">The name of the column to find. </param><exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found. </exception><filterpriority>2</filterpriority>
        object IDataRecord.this[string name] => throw new NotImplementedException();

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader"/> Object.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable"/> that describes the column metadata of the <see cref="T:System.Data.IDataReader"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable"/> that describes the column metadata.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.IDataReader"/> is closed. </exception><filterpriority>2</filterpriority>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool NextResult()
        {
            return false;
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool Read()
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <returns>
        /// The level of nesting.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Depth { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <returns>
        /// true if the data reader is closed; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <returns>
        /// The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int RecordsAffected => -1;
    }


    public static class GetDataReaderMethods
    {


        public static IDataReader GetTable(this ClientDatabaseBase<HistorianKey, HistorianValue> database, DateTime start, DateTime stop, TableDefinition tableDefinition)
        {

            return null;
        }

        public static DataTable GetDataReaderTable(this ClientDatabaseBase<HistorianKey, HistorianValue> database, ulong start, ulong stop, IList<ISignalWithName> columns)
        {
            if (columns.Any((x) => !x.HistorianId.HasValue))
            {
                throw new Exception("All columns must be contained in the historian for this function to work.");
            }

            Dictionary<ulong, SignalDataBase> results = database.GetSignals(start, stop, columns);
            int[] columnPosition = new int[columns.Count];
            object[] rowValues = new object[columns.Count + 1];
            SignalDataBase[] signals = new SignalDataBase[columns.Count];

            DataTable table = new DataTable();
            table.Columns.Add("Time", typeof(DateTime));
            foreach (ISignalWithName signal in columns)
            {
                table.Columns.Add(signal.TagName, typeof(double));
            }

            for (int x = 0; x < columns.Count; x++)
            {
                signals[x] = results[columns[x].HistorianId.Value];
            }

            while (true)
            {
                ulong minDate = ulong.MaxValue;
                for (int x = 0; x < columns.Count; x++)
                {
                    SignalDataBase signal = signals[x];
                    if (signal.Count < columnPosition[x])
                    {
                        minDate = Math.Min(minDate, signals[x].GetDate(columnPosition[x]));
                    }
                }

                rowValues[0] = null;
                for (int x = 0; x < columns.Count; x++)
                {
                    SignalDataBase signal = signals[x];
                    if (signal.Count < columnPosition[x] && minDate == signals[x].GetDate(columnPosition[x]))
                    {
                        signals[x].GetData(columnPosition[x], out ulong date, out double value);
                        rowValues[x + 1] = value;
                        columnPosition[x]++;
                    }
                    else
                    {
                        rowValues[x + 1] = null;
                    }
                }

                if (minDate == ulong.MaxValue && rowValues.All((x) => x is null))
                    return table;
                rowValues[0] = minDate;

                table.Rows.Add(rowValues);
            }
        }
    }
}