/*
 * NPlot - A charting library for .NET
 * 
 * LinePlot.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NPlot
{
    /// <summary>
    /// Encapsulates functionality for plotting data as a line chart.
    /// </summary>
    public class LinePlot : IPlot
    {
        private readonly LineData m_lineData;

        /// <summary>
        /// Gets or sets the data, or column name for the ordinate [y] axis.
        /// </summary>
        private IList<double> YData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data, or column name for the abscissa [x] axis.
        /// </summary>
        private IList<double> XData
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="yData">the ordinate data to associate with this plot.</param>
        /// <param name="xData">the abscissa data to associate with this plot.</param>
        public LinePlot(IList<double> yData, IList<double> xData)
        {
            YData = yData;
            XData = xData;
            m_lineData = new LineData(xData, yData);
        }

        /// <summary>
        /// Draws the line plot on a GDI+ surface against the provided x and y axes.
        /// </summary>
        /// <param name="g">The GDI+ surface on which to draw.</param>
        /// <param name="xAxis">The X-Axis to draw against.</param>
        /// <param name="yAxis">The Y-Axis to draw against.</param>
        public void Draw(Graphics g, PhysicalAxis xAxis, PhysicalAxis yAxis)
        {
            try
            {



                double xVal, yVal;

                int pointCount = m_lineData.Count;
                if (pointCount == 0)
                    return;

                Stopwatch sw = StepTimer.Start("Transform");

                Transform2D t = new Transform2D(xAxis, yAxis);

                // clipping is now handled assigning a clip region in the
                // graphic object before this call
                if (pointCount == 1)
                {
                    m_lineData.Get(0, out xVal, out yVal);
                    PointF physical = t.Transform(xVal, yVal);
                    g.DrawLine(Pen, physical.X - 0.5f, physical.Y, physical.X + 0.5f, physical.Y);
                }
                else
                {
                    int index = 0;
                    PointF[] points = new PointF[pointCount];
                    PointF lastPoint = new PointF(Single.NaN, Single.NaN);
                    for (int i = 0; i < pointCount; ++i)
                    {
                        // check to see if any values null. If so, then continue.
                        m_lineData.Get(i, out xVal, out yVal);

                        if (!Double.IsNaN(xVal + yVal)) //Adding a NaN with anything yeilds NaN
                        {
                            const float GDIMax = 1000000000f;
                            const float GDIMin = -1000000000f;
                            PointF p1 = t.Transform(xVal, yVal);
                            if (p1.X > GDIMax)
                                p1.X = GDIMax;
                            if (p1.X < GDIMin)
                                p1.X = GDIMin;

                            if (p1.Y > GDIMax)
                                p1.Y = GDIMax;
                            if (p1.Y < GDIMin)
                                p1.Y = GDIMin;

                            if (!float.IsNaN(p1.X) && !float.IsNaN(p1.Y))
                            {
                                if (p1 != lastPoint)
                                {
                                    lastPoint = p1;
                                    points[index] = p1;
                                    index++;
                                }
                            }
                        }
                    }
                    //System.Drawing.Drawing2D.GraphicsPath graphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                    //graphicsPath.AddLines(points.ToArray());
                    //g.DrawPath(Pen, graphicsPath);
                    //g.CompositingQuality = CompositingQuality.HighQuality;
                    //g.SmoothingMode = SmoothingMode.HighQuality;
                    StepTimer.Stop(sw);

                    sw = StepTimer.Start("GDI+");
                    if (index == 0)
                        return;
                    else if (index == 1)
                    {
                        PointF physical = points[0];
                        g.DrawLine(Pen, physical.X - 0.5f, physical.Y, physical.X + 0.5f, physical.Y);
                        return;
                    }
                    if (index == points.Length)
                    {
                        g.DrawLines(Pen, points);
                    }
                    else
                    {
                        PointF[] newArray = new PointF[index];
                        Array.Copy(points, 0, newArray, 0, index);
                        g.DrawLines(Pen, newArray);
                    }
                    StepTimer.Stop(sw);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Returns an x-axis that is suitable for drawing this plot.
        /// </summary>
        /// <returns>A suitable x-axis.</returns>
        public DateTimeAxis SuggestXAxis()
        {
            return m_lineData.GetX();
        }


        /// <summary>
        /// Returns a y-axis that is suitable for drawing this plot.
        /// </summary>
        /// <returns>A suitable y-axis.</returns>
        public LinearAxis SuggestYAxis()
        {
            LinearAxis a = m_lineData.GetY();
            a.IncreaseRange(0.08);
            return a;
        }

        /// <summary>
        /// The pen used to draw the plot
        /// </summary>
        public Pen Pen
        {
            get => pen_;
            set => pen_ = value;
        }

        private Pen pen_ = new Pen(Color.Black);


        /// <summary>
        /// The color of the pen used to draw lines in this plot.
        /// </summary>
        public Color Color
        {
            set
            {
                if (pen_ != null)
                {
                    pen_.Color = value;
                }
                else
                {
                    pen_ = new Pen(value);
                }
            }
            get => pen_.Color;
        }
    }
}