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
using System.Drawing;

namespace NPlot
{

    /// <summary>
    /// Encapsulates functionality for plotting data as a line chart.
    /// </summary>
    public class LinePlot : BasePlot, IPlot
    {
        
        LineData m_lineData;
        
        /// <summary>
        /// Gets or sets the data, or column name for the ordinate [y] axis.
        /// </summary>
        IList<double> YData { get; set; }

        /// <summary>
        /// Gets or sets the data, or column name for the abscissa [x] axis.
        /// </summary>
        IList<double> XData { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="yData">the ordinate data to associate with this plot.</param>
        /// <param name="xData">the abscissa data to associate with this plot.</param>
        public LinePlot(IList<double> yData, IList<double> xData)
        {
            this.YData = yData;
            this.XData = xData;
            m_lineData = new LineData(xData, yData);
        }

        /// <summary>
        /// Writes text data of the plot object to the supplied string builder. It is 
        /// possible to specify that only data in the specified range be written.
        /// </summary>
        /// <param name="sb">the StringBuilder object to write to.</param>
        /// <param name="region">a region used if onlyInRegion is true.</param>
        /// <param name="onlyInRegion">If true, only data enclosed in the provided region will be written.</param>
        public void WriteData(System.Text.StringBuilder sb, RectangleD region, bool onlyInRegion)
        {
            sb.Append("Label: ");
            sb.Append(this.Label);
            sb.Append("\r\n");
            m_lineData.WriteData(sb, region, onlyInRegion);
        }


        /// <summary>
        /// Draws the line plot on a GDI+ surface against the provided x and y axes.
        /// </summary>
        /// <param name="g">The GDI+ surface on which to draw.</param>
        /// <param name="xAxis">The X-Axis to draw against.</param>
        /// <param name="yAxis">The Y-Axis to draw against.</param>
        public void DrawLine(Graphics g, PhysicalAxis xAxis, PhysicalAxis yAxis)
        {
            ITransform2D t = Transform2D.GetTransformer(xAxis, yAxis);

            int numberPoints = m_lineData.Count;

            if (m_lineData.Count == 0)
            {
                return;
            }

            // clipping is now handled assigning a clip region in the
            // graphic object before this call
            if (numberPoints == 1)
            {
                PointF physical = t.Transform(m_lineData.Get(0));

                g.DrawLine(Pen, physical.X - 0.5f, physical.Y, physical.X + 0.5f, physical.Y);
            }
            else
            {

                List<PointF> points = new List<PointF>();
                PointF lastPoint = new PointF(Single.NaN, Single.NaN);
                for (int i = 0; i < numberPoints; ++i)
                {
                    // check to see if any values null. If so, then continue.
                    PointD pt = m_lineData.Get(i);
                    if (Double.IsNaN(pt.X) || Double.IsNaN(pt.Y))
                    {
                        continue;
                    }

                    PointF p1 = t.Transform(pt);

                    if (p1.Equals(lastPoint))
                        continue;

                    points.Add(p1);

                }
                System.Drawing.Drawing2D.GraphicsPath graphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                graphicsPath.AddLines(points.ToArray());
                g.DrawPath(Pen, graphicsPath);

                //// prepare for clipping
                //double leftCutoff = xAxis.PhysicalToWorld(xAxis.PhysicalMin, false);
                //double rightCutoff = xAxis.PhysicalToWorld(xAxis.PhysicalMax, false);
                //if (leftCutoff > rightCutoff)
                //{
                //    Utils.Swap(ref leftCutoff, ref rightCutoff);
                //}

                //for (int i = 1; i < numberPoints; ++i)
                //{
                //    // check to see if any values null. If so, then continue.
                //    double dx1 = data[i - 1].X;
                //    double dx2 = data[i].X;
                //    double dy1 = data[i - 1].Y;
                //    double dy2 = data[i].Y;
                //    if (Double.IsNaN(dx1) || Double.IsNaN(dy1) ||
                //        Double.IsNaN(dx2) || Double.IsNaN(dy2))
                //    {
                //        continue;
                //    }

                //    // do horizontal clipping here, to speed up
                //    if ((dx1 < leftCutoff || rightCutoff < dx1) &&
                //        (dx2 < leftCutoff || rightCutoff < dx2))
                //    {
                //        continue;
                //    }

                //    // else draw line.
                //    PointF p1 = t.Transform(data[i - 1]);
                //    PointF p2 = t.Transform(data[i]);

                //    // when very far zoomed in, points can fall ontop of each other,
                //    // and g.DrawLine throws an overflow exception
                //    if (p1.Equals(p2))
                //        continue;


                //    g.DrawLine(Pen, p1.X, p1.Y, p2.X, p2.Y);
                //}
            }

        }


        /// <summary>
        /// Draws the line plot on a GDI+ surface against the provided x and y axes.
        /// </summary>
        /// <param name="g">The GDI+ surface on which to draw.</param>
        /// <param name="xAxis">The X-Axis to draw against.</param>
        /// <param name="yAxis">The Y-Axis to draw against.</param>
        public void Draw(Graphics g, PhysicalAxis xAxis, PhysicalAxis yAxis)
        {
            this.DrawLine(g, xAxis, yAxis);
        }


        /// <summary>
        /// Returns an x-axis that is suitable for drawing this plot.
        /// </summary>
        /// <returns>A suitable x-axis.</returns>
        public Axis SuggestXAxis()
        {
            return m_lineData.GetX();
        }


        /// <summary>
        /// Returns a y-axis that is suitable for drawing this plot.
        /// </summary>
        /// <returns>A suitable y-axis.</returns>
        public Axis SuggestYAxis()
        {
            return m_lineData.GetY();
        }

        /// <summary>
        /// Draws a representation of this plot in the legend.
        /// </summary>
        /// <param name="g">The graphics surface on which to draw.</param>
        /// <param name="startEnd">A rectangle specifying the bounds of the area in the legend set aside for drawing.</param>
        public virtual void DrawInLegend(Graphics g, Rectangle startEnd)
        {
            g.DrawLine(pen_, startEnd.Left, (startEnd.Top + startEnd.Bottom) / 2,
                startEnd.Right, (startEnd.Top + startEnd.Bottom) / 2);
        }


        /// <summary>
        /// The pen used to draw the plot
        /// </summary>
        public Pen Pen
        {
            get
            {
                return pen_;
            }
            set
            {
                pen_ = value;
            }
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
            get
            {
                return pen_.Color;
            }
        }


    }
}
