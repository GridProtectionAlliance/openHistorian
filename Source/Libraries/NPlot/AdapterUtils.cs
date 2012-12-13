/*
 * NPlot - A charting library for .NET
 * 
 * AdapterUtils.cs
 * Copyright (C) 2003-2006 Matt Howlett and others.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 * 3. Neither the name of NPlot nor the names of its contributors may
 *    be used to endorse or promote products derived from this software without
 *    specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Data;

namespace NPlot
{

    /// <summary>
    /// Encapsulates functionality relating to exposing data in various
    /// different data structures in a consistent way.
    /// </summary>
    /// <remarks>It would be more efficient to have iterator style access
    /// to the data, rather than index based, and Count.</remarks>
	public class AdapterUtils
	{

		#region AxisSuggesters

		/// <summary>
		/// Interface for classes that can suggest an axis for data they contain.
		/// </summary>
		public interface IAxisSuggester
		{
			/// <summary>
			/// Calculates a suggested axis for the data contained by the implementing class.
			/// </summary>
			/// <returns>the suggested axis</returns>
			Axis Get();
		}


		/// <summary>
		/// Implements functionality for suggesting an axis suitable for charting 
		/// data in multiple columns of a DataRowCollection.
		/// </summary>
		/// <remarks>This is currently not used.</remarks>
		public class AxisSuggester_MultiColumns : IAxisSuggester
		{			

			DataRowCollection rows_;
			string abscissaName_;


			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="rows">The DataRowCollection containing the data.</param>
			/// <param name="abscissaName">the column with this name is not considered</param>
			public AxisSuggester_MultiColumns(DataRowCollection rows, string abscissaName)
			{
				rows_ = rows;
				abscissaName_ = abscissaName;
			}


			/// <summary>
			/// Calculates a suggested axis for the DataRowCollection data.
			/// </summary>
			/// <returns>the suggested axis</returns>
			public Axis Get()
			{
				double t_min = double.MaxValue;
				double t_max = double.MinValue;

				System.Collections.IEnumerator en = rows_[0].Table.Columns.GetEnumerator();
				
				while (en.MoveNext())
				{
					string colName = ((DataColumn)en.Current).Caption;
					
					if (colName == abscissaName_)
					{
						continue;
					}

					double min;
					double max;
					if (Utils.RowArrayMinMax(rows_, out min, out max, colName))
					{
						if (min < t_min)
						{
							t_min = min;
						}
						if (max > t_max)
						{
							t_max = max;
						}
					}
				}

				return new LinearAxis(t_min, t_max);
			}
		
		}

	
        /// <summary>
        /// This class gets an axis suitable for plotting the data contained in an IList.
        /// </summary>
        public class AxisSuggester_IList : IAxisSuggester
        {
            private IList data_;

            /// <summary>
            /// Constructor. 
            /// </summary>
            /// <param name="data">the data we want to find a suitable axis for.</param>
            public AxisSuggester_IList(IList data)
            {
                data_ = data;
            }

			/// <summary>
			/// Calculates a suggested axis for the IList data.
			/// </summary>
			/// <returns>the suggested axis</returns>
            public Axis Get()
            {
                double min;
                double max;

                if (Utils.ArrayMinMax(data_, out min, out max))
                {
                    if (data_[0] is DateTime)
                    {
                        return new DateTimeAxis(min, max);
                    }

                    else
                    {
                        return new LinearAxis(min, max);
                    }

                    // perhaps return LogAxis here if range large enough 
                    // + other constraints?
                }

                return new LinearAxis(0.0, 1.0);
            }
        }


        /// <summary>
        /// This class is responsible for supplying a default axis via the IAxisSuggester interface.
        /// </summary>
        public class AxisSuggester_Null : IAxisSuggester
        {
			/// <summary>
			/// Returns a default axis.
			/// </summary>
			/// <returns>the suggested axis</returns>
            public Axis Get()
            {
                return new LinearAxis(0.0, 1.0);
            }
        }

        /// <summary>
        /// Provides default axis if only data corresponding to orthogonal axis is provided.
        /// </summary>
        public class AxisSuggester_Auto : IAxisSuggester
        {
            
			IList ordinateData_;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="ordinateData">Data corresponding to orthogonal axis.</param>
            public AxisSuggester_Auto(IList ordinateData)
            {
                ordinateData_ = ordinateData;
            }

			/// <summary>
			/// Calculates a suggested axis given the data specified in the constructor.
			/// </summary>
			/// <returns>the suggested axis</returns>
            public Axis Get()
            {
                if (ordinateData_!=null && ordinateData_.Count>0)
                {
                    return new LinearAxis(0, ordinateData_.Count - 1);

                }

                else
                {
                    return new LinearAxis(0.0, 1.0);
                }
            }
        }


        /// <summary>
        /// Provides default axis if only data corresponding to orthogonal axis is provided.
        /// </summary>
        public class AxisSuggester_RowAuto : IAxisSuggester
        {
            DataRowCollection ordinateData_;

            /// <summary>
            /// Construbtor
            /// </summary>
            /// <param name="ordinateData">Data corresponding to orthogonal axis.</param>
            public AxisSuggester_RowAuto(DataRowCollection ordinateData)
            {
                ordinateData_ = ordinateData;
            }

			/// <summary>
			/// Calculates a suggested axis given the data specified in the constructor.
			/// </summary>
			/// <returns>the suggested axis</returns>
            public Axis Get()
            {
                if (ordinateData_!=null && ordinateData_.Count>0)
                {
                	return new LinearAxis(0, ordinateData_.Count - 1);
                }

                else
                {
                	return new LinearAxis(0.0, 1.0);
                }
            }
        }

        /// <summary>
        /// Provides axis for data in a given column of a DataRowCollection.
        /// </summary>
        public class AxisSuggester_Rows : IAxisSuggester
        {
            DataRowCollection rows_;
            string columnName_;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="rows">DataRowCollection containing the data to suggest axis for.</param>
            /// <param name="columnName">the column to get data.</param>
            public AxisSuggester_Rows(DataRowCollection rows, string columnName)
            {
                rows_ = rows;
                columnName_ = columnName;
            }

			/// <summary>
			/// Calculates a suggested axis given the data specified in the constructor.
			/// </summary>
			/// <returns>the suggested axis</returns>
            public Axis Get()
            {
                double min;
                double max;

                if (Utils.RowArrayMinMax(rows_, out min, out max, columnName_))
                {
                    if ((rows_[0])[columnName_] is DateTime)
                    {
                        return new DateTimeAxis(min, max);
                    }

                    else
                    {
                        return new LinearAxis(min, max);
                    }
                }

                return new LinearAxis(0.0, 1.0);
            }
        }


        /// <summary>
        /// Provides axis suggestion for data in a particular column of a DataView.
        /// </summary>
        public class AxisSuggester_DataView : IAxisSuggester
        {
            DataView data_;
            string columnName_;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">DataView that contains data to suggest axis for</param>
            /// <param name="columnName">the column of interest in the DataView</param>
            public AxisSuggester_DataView(DataView data, string columnName)
            {
                data_ = data;
                columnName_ = columnName;
            }

			/// <summary>
			/// Calculates a suggested axis given the data specified in the constructor.
			/// </summary>
			/// <returns>the suggested axis</returns>
            public Axis Get()
            {
                double min;
                double max;

                if (Utils.DataViewArrayMinMax(data_, out min, out max, columnName_))
                {
                    if ((data_[0])[columnName_] is DateTime)
                    {
                        return new DateTimeAxis(min, max);
                    }

                    else
                    {
                        return new LinearAxis(min, max);
                    }
                }

                return new LinearAxis(0.0, 1.0);
            }
        }

        #endregion
        #region Counters

        /// <summary>
        /// Interface that enables a dataholding class to report how many data items it holds.
        /// </summary>
        public interface ICounter
        {
            /// <summary>
            /// Number of data items in container.
            /// </summary>
            /// <value>Number of data items in container.</value>
            int Count { get; }
        }

        /// <summary>
        /// Class that provides the number of items in an IList via the ICounter interface.
        /// </summary>
        public class Counter_IList : ICounter
        {
            private IList data_;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">the IList data to provide count of</param>
            public Counter_IList(IList data)
            {
                data_ = data;
            }

			/// <summary>
			/// Number of data items in container.
			/// </summary>
			/// <value>Number of data items in container.</value>
            public int Count
            {
                get
                {
                    return data_.Count;
                }
            }
        }

		/// <summary>
		/// Class that returns 0 via the ICounter interface.
		/// </summary>
        public class Counter_Null : ICounter
        {
			/// <summary>
			/// Number of data items in container.
			/// </summary>
			/// <value>Number of data items in container.</value>
            public int Count
            {
                get
                {
                    return 0;
                }
            }
        }

		/// <summary>
		/// Class that provides the number of items in a DataRowCollection via the ICounter interface.
		/// </summary>
        public class Counter_Rows : ICounter
        {
            DataRowCollection rows_;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="rows">the DataRowCollection data to provide count of number of rows of.</param>
            public Counter_Rows(DataRowCollection rows)
            {
                rows_ = rows;
            }

			/// <summary>
			/// Number of data items in container.
			/// </summary>
			/// <value>Number of data items in container.</value>
            public int Count
            {
                get
                {
                    return rows_.Count;
                }
            }
        }

		/// <summary>
		/// Class that provides the number of items in a DataView via the ICounter interface.
		/// </summary>
        public class Counter_DataView : ICounter
        {
            DataView dataView_;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="dataView">the DataBiew data to provide count of number of rows of.</param>
            public Counter_DataView(DataView dataView)
            {
                dataView_ = dataView;
            }

			/// <summary>
			/// Number of data items in container.
			/// </summary>
			/// <value>Number of data items in container.</value>
            public int Count
            {
                get
                {
                    return dataView_.Count;
                }
            }

        }

        #endregion
        #region DataGetters

        /// <summary>
        /// Interface for data holding classes that allows users to get the ith value.
        /// </summary>
		/// <remarks>
		/// TODO: should change this to GetNext() and Reset() for more generality.
		/// </remarks>
		public interface IDataGetter
        {
            /// <summary>
            /// Gets the ith data value.
            /// </summary>
            /// <param name="i">sequence number of data to get.</param>
            /// <returns>ith data value.</returns>
            double Get(int i);
        }

        /// <summary>
        /// Provides data in an IList via the IDataGetter interface.
        /// </summary>
        public class DataGetter_IList : IDataGetter
        {
            private IList data_;
            
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">IList that contains the data</param>
            public DataGetter_IList(IList data)
            {
                data_ = data;
            }

			/// <summary>
			/// Gets the ith data value.
			/// </summary>
			/// <param name="i">sequence number of data to get.</param>
			/// <returns>ith data value.</returns>
            public double Get(int i)
            {
                return Utils.ToDouble(data_[i]);
            }
        }


        /// <summary>
        /// Provides data in an array of doubles via the IDataGetter interface.
        /// </summary>
        /// <remarks>
        /// A speed-up version of DataDetter_IList; no boxing/unboxing overhead.
        /// </remarks>
		public class DataGetter_DoublesArray : IDataGetter
		{
			private double[] data_;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="data">array of doubles that contains the data</param>
			public DataGetter_DoublesArray(double[] data)          
			{
				data_ = data;
			}
		
			/// <summary>
			/// Gets the ith data value.
			/// </summary>
			/// <param name="i">sequence number of data to get.</param>
			/// <returns>ith data value.</returns>
			public double Get(int i)          
			{
				return data_[i];
			}
		}


		/// <summary>
		/// Provides no data.
		/// </summary>
        public class DataGetter_Null : IDataGetter
        {
			/// <summary>
			/// Gets the ith data value.
			/// </summary>
			/// <param name="i">sequence number of data to get.</param>
			/// <returns>ith data value.</returns>
            public double Get(int i)
            {
                throw new NPlotException( "No Data!" );
            }
        }

		/// <summary>
		/// Provides the natural numbers (and 0) via the IDataGetter interface.
		/// </summary>
        public class DataGetter_Count : IDataGetter
        {
			/// <summary>
			/// Gets the ith data value.
			/// </summary>
			/// <param name="i">sequence number of data to get.</param>
			/// <returns>ith data value.</returns>
            public double Get(int i)
            {
                return (double)i;
            }
        }


		/// <summary>
		/// Provides data in a DataRowCollection via the IDataGetter interface.
		/// </summary>
        public class DataGetter_Rows : IDataGetter
        {
            private DataRowCollection rows_;
            private string columnName_;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="rows">DataRowCollection to get data from</param>
            /// <param name="columnName">Get data in this column</param>
            public DataGetter_Rows(DataRowCollection rows, string columnName)
            {
                rows_ = rows;
                columnName_ = columnName;
            }

			/// <summary>
			/// Gets the ith data value.
			/// </summary>
			/// <param name="i">sequence number of data to get.</param>
			/// <returns>ith data value.</returns>
            public double Get(int i)
            {
                return Utils.ToDouble((rows_[i])[columnName_]);
            }
        }

		/// <summary>
		/// Provides data in a DataView via the IDataGetter interface.
		/// </summary>
        public class DataGetter_DataView : IDataGetter
        {
            private DataView data_;
            private string columnName_;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">DataView to get data from.</param>
            /// <param name="columnName">Get data in this column</param>
            public DataGetter_DataView(DataView data, string columnName)
            {
                data_ = data;
                columnName_ = columnName;
            }

			/// <summary>
			/// Gets the ith data value.
			/// </summary>
			/// <param name="i">sequence number of data to get.</param>
			/// <returns>ith data value.</returns>
            public double Get(int i)
            {
                return Utils.ToDouble((data_[i])[columnName_]);
            }

        }

		
		/// <summary>
		/// Gets data 
		/// </summary>
		/// <remarks>Note: Does not implement IDataGetter... Currently this class is not used.</remarks>
		public class DataGetter_MultiRows 
		{
			
			DataRowCollection rows_;
			string abscissaName_;
			int abscissaColumnNumber_;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="rows">DataRowCollection to get data from.</param>
			/// <param name="omitThisColumn">don't get data from this column</param>
			public DataGetter_MultiRows(DataRowCollection rows, string omitThisColumn )
			{
				rows_ = rows;
				abscissaName_ = omitThisColumn;

				abscissaColumnNumber_ = rows_[0].Table.Columns.IndexOf( omitThisColumn );
				if (abscissaColumnNumber_ < 0)
					throw new NPlotException( "invalid column name" );
			}

			/// <summary>
			/// Number of data points
			/// </summary>
			public int Count
			{
				get
				{
					return rows_[0].Table.Columns.Count-1;
				}
			}

			/// <summary>
			/// Gets data at a given index, in the given series (column number).
			/// </summary>
			/// <param name="index">index in the series to get data for</param>
			/// <param name="seriesIndex">series number (column number) to get data for.</param>
			/// <returns>the required data point.</returns>
			public double PointAt( int index, int seriesIndex )
			{
				if (seriesIndex < abscissaColumnNumber_)
					return Utils.ToDouble( rows_[index][seriesIndex] );
				else
					return Utils.ToDouble( rows_[index][seriesIndex+1] );
			}

		}


        #endregion

    }
}
