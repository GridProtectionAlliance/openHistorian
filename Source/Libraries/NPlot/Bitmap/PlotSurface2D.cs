/*
 * NPlot - A charting library for .NET
 * 
 * Bitmap.PlotSurface2D.cs
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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace NPlot
{
    namespace Bitmap
    {
        /// <summary>
        /// Wrapper around NPlot.PlotSurface2D that provides extra functionality
        /// specific to drawing to Bitmaps.
        /// </summary>
        public class PlotSurface2D
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="width">width of the bitmap.</param>
            /// <param name="height">height of the bitmap.</param>
            public PlotSurface2D(int width, int height)
            {
                b_ = new System.Drawing.Bitmap(width, height);
                ps_ = new NPlot.PlotSurface2D();
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="b">The Bitmap where the plot is to be rendered.</param>
            public PlotSurface2D(System.Drawing.Bitmap b)
            {
                b_ = b;
                ps_ = new NPlot.PlotSurface2D();
            }


            /// <summary>
            /// Renders the plot.
            /// </summary>
            /// <param name="g">The graphics surface.</param>
            /// <param name="bounds">The rectangle storing the bounds for rendering.</param>
            public void Draw(Graphics g, Rectangle bounds)
            {
                ps_.Draw(g, bounds);
            }


            /// <summary>
            /// Clears the plot.
            /// </summary>
            public void Clear()
            {
                ps_.Clear();
            }


            /// <summary>
            /// Adds a drawable object to the plot surface. If the object is an IPlot, 
            /// the PlotSurface2D axes will also be updated.
            /// </summary>
            /// <param name="p">The IDrawable object to add to the plot surface.</param>
            public void Add(IDrawable p)
            {
                ps_.Add(p);
            }

            /// <summary>
            /// Adds a drawable object to the plot surface. If the object is an IPlot, 
            /// the PlotSurface2D axes will also be updated.
            /// </summary>
            /// <param name="p">The IDrawable object to add to the plot surface.</param>
            /// <param name="zOrder">The z-ordering when drawing (objects with lower numbers are drawn first)</param>
            public void Add(IDrawable p, int zOrder)
            {
                ps_.Add(p, zOrder);
            }

            /// <summary>
            /// The plot surface title.
            /// </summary>
            public string Title
            {
                get => ps_.Title;
                set => ps_.Title = value;
            }


            /// <summary>
            /// The plot title font.
            /// </summary>
            public Font TitleFont
            {
                get => ps_.TitleFont;
                set => ps_.TitleFont = value;
            }


            /// <summary>
            /// The distance in pixels to leave between of the edge of the bounding rectangle
            /// supplied to the Draw method, and the markings that make up the plot.
            /// </summary>
            public int Padding
            {
                get => ps_.Padding;
                set => ps_.Padding = value;
            }


            /// <summary>
            /// The bottom abscissa axis.
            /// </summary>
            public DateTimeAxis XAxis1
            {
                get => ps_.XAxis1;
                set => ps_.XAxis1 = value;
            }


            /// <summary>
            /// The left ordinate axis.
            /// </summary>
            public LinearAxis YAxis1
            {
                get => ps_.YAxis1;
                set => ps_.YAxis1 = value;
            }


            /// <summary>
            /// The top abscissa axis.
            /// </summary>
            public DateTimeAxis XAxis2
            {
                get => ps_.XAxis2;
                set => ps_.XAxis2 = value;
            }


            /// <summary>
            /// The right ordinate axis.
            /// </summary>
            public LinearAxis YAxis2
            {
                get => ps_.YAxis2;
                set => ps_.YAxis2 = value;
            }

            /// <summary>
            /// A color used to paint the plot background. Mutually exclusive with PlotBackImage and PlotBackBrush
            /// </summary>
            public Color PlotBackColor
            {
                set => ps_.PlotBackColor = value;
            }


            /// <summary>
            /// Smoothing mode to use when drawing plots.
            /// </summary>
            public SmoothingMode SmoothingMode
            {
                get => ps_.SmoothingMode;
                set => ps_.SmoothingMode = value;
            }


            /// <summary>
            /// The bitmap width
            /// </summary>
            public int Width => b_.Width;


            /// <summary>
            /// The bitmap height
            /// </summary>
            public int Height => b_.Height;


            /// <summary>
            /// Renders the bitmap to a MemoryStream. Useful for returning the bitmap from
            /// an ASP.NET page.
            /// </summary>
            /// <returns>The MemoryStream object.</returns>
            public MemoryStream ToStream(ImageFormat imageFormat)
            {
                MemoryStream stream = new MemoryStream();
                ps_.Draw(Graphics.FromImage(this.Bitmap), new Rectangle(0, 0, b_.Width, b_.Height));
                this.Bitmap.Save(stream, imageFormat);
                return stream;
            }


            /// <summary>
            /// The bitmap to use as the drawing surface.
            /// </summary>
            public System.Drawing.Bitmap Bitmap
            {
                get => b_;
                set => b_ = value;
            }


            /// <summary>
            /// The bitmap background color outside the bounds of the plot surface.
            /// </summary>
            public Color BackColor
            {
                set => backColor_ = value;
            }

            private object backColor_;


            /// <summary>
            /// Refreshes (draws) the plot.
            /// </summary>
            public void Refresh()
            {
                if (this.backColor_ != null)
                {
                    Graphics g = Graphics.FromImage(b_);
                    g.FillRectangle(new Pen((Color)this.backColor_).Brush, 0, 0, b_.Width, b_.Height);
                }
                ps_.Draw(Graphics.FromImage(b_), new Rectangle(0, 0, b_.Width, b_.Height));
            }


            private readonly NPlot.PlotSurface2D ps_;
            private System.Drawing.Bitmap b_;


            /// <summary>
            /// Add an axis constraint to the plot surface. Axis constraints can
            /// specify relative world-pixel scalings, absolute axis positions etc.
            /// </summary>
            /// <param name="c">The axis constraint to add.</param>
            public void AddAxesConstraint(AxesConstraint c)
            {
                ps_.AddAxesConstraint(c);
            }


            /// <summary>
            /// Whether or not the title will be scaled according to size of the plot 
            /// surface.
            /// </summary>
            public bool AutoScaleTitle
            {
                get => ps_.AutoScaleTitle;
                set => ps_.AutoScaleTitle = value;
            }


            /// <summary>
            /// When plots are added to the plot surface, the axes they are attached to
            /// are immediately modified to reflect data of the plot. If 
            /// AutoScaleAutoGeneratedAxes is true when a plot is added, the axes will
            /// be turned in to auto scaling ones if they are not already [tick marks,
            /// tick text and label size scaled to size of plot surface]. If false,
            /// axes will not be autoscaling.
            /// </summary>
            public bool AutoScaleAutoGeneratedAxes
            {
                get => ps_.AutoScaleAutoGeneratedAxes;
                set => ps_.AutoScaleAutoGeneratedAxes = value;
            }


            /// <summary>
            /// Sets the title to be drawn using a solid brush of this color.
            /// </summary>
            public Color TitleColor
            {
                set => ps_.TitleColor = value;
            }


            /// <summary>
            /// The brush used for drawing the title.
            /// </summary>
            public Brush TitleBrush
            {
                get => ps_.TitleBrush;
                set => ps_.TitleBrush = value;
            }

            /// <summary>
            /// Remove a drawable object from the plot surface.
            /// </summary>
            /// <param name="p">the drawable to remove</param>
            /// <param name="updateAxes">whether or not to update the axes after removing the idrawable.</param>
            public void Remove(IDrawable p, bool updateAxes)
            {
                ps_.Remove(p, updateAxes);
            }


            /// <summary>
            /// Gets an array list containing all drawables currently added to the PlotSurface2D.
            /// </summary>
            public List<IDrawable> Drawables => ps_.Drawables;
        }
    }
}