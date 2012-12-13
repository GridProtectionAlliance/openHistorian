/*
 * NPlot - A charting library for .NET
 * 
 * SequenceAdapter.cs
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
    /// This class is responsible for interpreting the various ways you can 
    /// specify data to plot objects using the DataSource, DataMember, ordinateData
    /// and AbscissaData properties. It is a bridge that provides access to this
    /// data via a single interface.
    /// </summary>
    public class SequenceAdapter
    {
        private AdapterUtils.IAxisSuggester XAxisSuggester_;
        private AdapterUtils.IAxisSuggester YAxisSuggester_;
        private AdapterUtils.ICounter counter_;
        private AdapterUtils.IDataGetter xDataGetter_;
        private AdapterUtils.IDataGetter yDataGetter_;

        /// <summary>
        /// Constructor. The data source specifiers must be specified here.
        /// </summary>
        /// <param name="yValue">The source containing a list of values to plot on the ordinate axis, or a the name of the column to use for this data.</param>
        /// <param name="xValue">The source containing a list of values to plot on the abscissa axis, or a the name of the column to use for this data.</param>
        public SequenceAdapter(object yValue, object xValue)
        {
            if (yValue is IList)
            {
                this.YAxisSuggester_ = new AdapterUtils.AxisSuggester_IList((IList)yValue);
                if (yValue is Double[])
                    this.yDataGetter_ = new AdapterUtils.DataGetter_DoublesArray((Double[])yValue);
                else
                    this.yDataGetter_ = new AdapterUtils.DataGetter_IList((IList)yValue);

                this.counter_ = new AdapterUtils.Counter_IList((IList)yValue);

                if (xValue is IList)
                {
                    this.XAxisSuggester_ = new AdapterUtils.AxisSuggester_IList((IList)xValue);
                    if (xValue is Double[])
                        this.xDataGetter_ = new AdapterUtils.DataGetter_DoublesArray((Double[])xValue);
                    else
                        this.xDataGetter_ = new AdapterUtils.DataGetter_IList((IList)xValue);

                    return;
                }
                else if (xValue == null)
                {
                    this.XAxisSuggester_ = new AdapterUtils.AxisSuggester_Auto((IList)yValue);
                    this.xDataGetter_ = new AdapterUtils.DataGetter_Count();
                    return;
                }
            }

        }


        /// <summary>
        /// Returns the number of points.
        /// </summary>
        public int Count
        {
            get
            {
                return counter_.Count;
            }
        }


        /// <summary>
        /// Returns the ith point.
        /// </summary>
        public PointD this[int i]
        {
            get
            {
                return new PointD(this.xDataGetter_.Get(i), this.yDataGetter_.Get(i));
            }
        }


        /// <summary>
        /// Returns an x-axis that is suitable for drawing the data.
        /// </summary>
        /// <returns>A suitable x-axis.</returns>
        public Axis SuggestXAxis()
        {
            Axis a = this.XAxisSuggester_.Get();

            // The world length should never be returned as 0
            // This would result in an axis with a span of 0 units
            // which can not be properly displayed.
            if (a.WorldLength == 0.0)
            {
                // TODO make 0.08 a parameter.
                a.IncreaseRange(0.08);
            }
            return a;
        }


        /// <summary>
        /// Returns a y-axis that is suitable for drawing the data.
        /// </summary>
        /// <returns>A suitable y-axis.</returns>
        public Axis SuggestYAxis()
        {
            Axis a = this.YAxisSuggester_.Get();
            // TODO make 0.08 a parameter.
            a.IncreaseRange(0.08);
            return a;
        }


        /// <summary>
        /// Writes data out as text. 
        /// </summary>
        /// <param name="sb">StringBuilder to write to.</param>
        /// <param name="region">Only write out data in this region if onlyInRegion is true.</param>
        /// <param name="onlyInRegion">If true, only data in region is written, else all data is written.</param>
        public void WriteData(System.Text.StringBuilder sb, RectangleD region, bool onlyInRegion)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                if (!(onlyInRegion &&
                       (this[i].X >= region.X && this[i].X <= region.X + region.Width) &&
                       (this[i].Y >= region.Y && this[i].Y <= region.Y + region.Height)))
                    continue;

                sb.Append(this[i].ToString());
                sb.Append("\r\n");
            }
        }


    }
}
