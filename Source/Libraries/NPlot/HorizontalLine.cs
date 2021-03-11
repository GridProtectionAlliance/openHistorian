/*
 * NPlot - A charting library for .NET
 * 
 * HorizontalLine.cs
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
using System.Drawing;

namespace NPlot
{
    /// <summary>
    /// Encapsulates functionality for drawing a horizontal line on a plot surface.
    /// </summary>
    public class HorizontalLine : IPlot
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="yValue">ordinate (Y) value of line.</param>
        public HorizontalLine(double yValue)
        {
            this.value_ = yValue;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="yValue">ordinate (Y) value of line.</param>
        /// <param name="color">draw the line using this color.</param>
        public HorizontalLine(double yValue, Color color)
        {
            this.value_ = yValue;
            this.pen_ = new Pen(color);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="yValue">ordinate (Y) value of line.</param>
        /// <param name="pen">Pen to use to draw the line.</param>
        public HorizontalLine(double yValue, Pen pen)
        {
            this.value_ = yValue;
            this.pen_ = pen;
        }

        /// <summary>
        /// Returns null indicating that x extremities of the line are variable.
        /// </summary>
        /// <returns>null</returns>
        public DateTimeAxis SuggestXAxis()
        {
            return null;
        }


        /// <summary>
        /// Returns a y-axis that is suitable for drawing this plot.
        /// </summary>
        /// <returns>A suitable y-axis.</returns>
        public LinearAxis SuggestYAxis()
        {
            return new LinearAxis(this.value_, this.value_);
        }

        /// <summary>
        /// Draws the horizontal line plot on a GDI+ surface against the provided x and y axes.
        /// </summary>
        /// <param name="g">The GDI+ surface on which to draw.</param>
        /// <param name="xAxis">The X-Axis to draw against.</param>
        /// <param name="yAxis">The Y-Axis to draw against.</param>
        public void Draw(Graphics g, PhysicalAxis xAxis, PhysicalAxis yAxis)
        {
            int xMin = xAxis.PhysicalMin.X;
            int xMax = xAxis.PhysicalMax.X;

            xMin += pixelIndent_;
            xMax -= pixelIndent_;

            float length = Math.Abs(xMax - xMin);
            float lengthDiff = length - length * scale_;
            float indentAmount = lengthDiff / 2;

            xMin += (int)indentAmount;
            xMax -= (int)indentAmount;

            int yPos = (int)yAxis.WorldToPhysical(value_, false).Y;

            g.DrawLine(pen_, new Point(xMin, yPos), new Point(xMax, yPos));

            // todo:  clip and proper logic for flipped axis min max.
        }

        private double value_;

        /// <summary>
        /// ordinate (Y) value to draw horizontal line at.
        /// </summary>
        public double YValue
        {
            get => value_;
            set => value_ = value;
        }

        private Pen pen_ = new Pen(Color.Black);

        /// <summary>
        /// Pen to use to draw the horizontal line.
        /// </summary>
        public Pen Pen
        {
            get => pen_;
            set => pen_ = value;
        }


        /// <summary>
        /// Each end of the line is indented by this many pixels. 
        /// </summary>
        public int PixelIndent
        {
            get => pixelIndent_;
            set => pixelIndent_ = value;
        }

        private int pixelIndent_;


        /// <summary>
        /// The line length is multiplied by this amount. Default
        /// corresponds to a value of 1.0.
        /// </summary>
        public float LengthScale
        {
            get => scale_;
            set => scale_ = value;
        }

        private float scale_ = 1.0f;
    }
}