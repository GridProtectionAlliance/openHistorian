/*
 * NPlot - A charting library for .NET
 * 
 * Windows.PlotSurface2d.cs
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
using System.Drawing;
using System.Windows.Forms;

namespace NPlot.Windows
{
    public partial class PlotSurface2D
    {
        /// <summary>
        /// Encapsulates a number of separate "Interactions". An interaction is basically 
        /// a set of handlers for mouse and keyboard events that work together in a 
        /// specific way. 
        /// </summary>
        public abstract class Interactions
        {
            /// <summary>
            /// Base class for an interaction. All methods are virtual. Not abstract as not all interactions
            /// need to use all methods. Default functionality for each method is to do nothing. 
            /// </summary>
            public class Interaction
            {
                /// <summary>
                /// Handler for this interaction if a mouse down event is received.
                /// </summary>
                /// <param name="e">event args</param>
                /// <param name="ctr">reference to the control</param>
                /// <returns>true if plot surface needs refreshing.</returns>
                public virtual bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    return false;
                }

                /// <summary>
                /// Handler for this interaction if a mouse up event is received.
                /// </summary>
                /// <param name="e">event args</param>
                /// <param name="ctr">reference to the control</param>
                /// <returns>true if plot surface needs refreshing.</returns>
                public virtual bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    return false;
                }

                /// <summary>
                /// Handler for this interaction if a mouse move event is received.
                /// </summary>
                /// <param name="e">event args</param>
                /// <param name="ctr">reference to the control</param>
                /// <param name="lastKeyEventArgs"></param>
                /// <returns>true if plot surface needs refreshing.</returns>
                public virtual bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    return false;
                }

                /// <summary>
                /// Handler for this interaction if a mouse move event is received.
                /// </summary>
                /// <param name="e">event args</param>
                /// <param name="ctr">reference to the control</param>
                /// <returns>true if plot surface needs refreshing.</returns>
                public virtual bool DoMouseWheel(MouseEventArgs e, Control ctr)
                {
                    return false;
                }

                /// <summary>
                /// Handler for this interaction if a mouse Leave event is received.
                /// </summary>
                /// <param name="e">event args</param>
                /// <param name="ctr">reference to the control</param>
                /// <returns>true if the plot surface needs refreshing.</returns>
                public virtual bool DoMouseLeave(EventArgs e, Control ctr)
                {
                    return false;
                }

                /// <summary>
                /// Handler for this interaction if a paint event is received.
                /// </summary>
                /// <param name="pe">paint event args</param>
                /// <param name="width"></param>
                /// <param name="height"></param>
                public virtual void DoPaint(PaintEventArgs pe, int width, int height)
                {
                }
            }

            #region RubberBandSelection

            /// <summary>
            /// 
            /// </summary>
            public class RubberBandSelection : Interaction
            {
                private bool selectionInitiated_;

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    // keep track of the start point and flag that select initiated.
                    selectionInitiated_ = true;
                    startPoint_.X = e.X;
                    startPoint_.Y = e.Y;

                    // invalidate the end point
                    endPoint_ = unset_;

                    return false;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    if (e.Button == MouseButtons.Left && selectionInitiated_)
                    {
                        // we are here
                        Point here = new Point(e.X, e.Y);

                        // delete the previous box
                        if (endPoint_ != unset_)
                        {
                            this.DrawRubberBand(startPoint_, endPoint_, ctr);
                        }
                        endPoint_ = here;

                        // and redraw the last one
                        this.DrawRubberBand(startPoint_, endPoint_, ctr);
                    }

                    return false;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    // handle left button (selecting region).
                    if (e.Button == MouseButtons.Left && selectionInitiated_)
                    {
                        endPoint_.X = e.X;
                        endPoint_.Y = e.Y;

                        // flag stopped selecting.
                        selectionInitiated_ = false;

                        if (endPoint_ != unset_)
                        {
                            this.DrawRubberBand(startPoint_, endPoint_, ctr);
                        }

                        Point minPoint = new Point(0, 0);
                        minPoint.X = Math.Min(startPoint_.X, endPoint_.X);
                        minPoint.Y = Math.Min(startPoint_.Y, endPoint_.Y);

                        Point maxPoint = new Point(0, 0);
                        maxPoint.X = Math.Max(startPoint_.X, endPoint_.X);
                        maxPoint.Y = Math.Max(startPoint_.Y, endPoint_.Y);

                        Rectangle r = ps.PlotAreaBoundingBoxCache;
                        if (minPoint != maxPoint && (r.Contains(minPoint) || r.Contains(maxPoint)))
                        {
                            ((PlotSurface2D)ctr).CacheAxes();

                            ((PlotSurface2D)ctr).PhysicalXAxis1Cache.SetWorldLimitsFromPhysical(minPoint, maxPoint);
                            ((PlotSurface2D)ctr).PhysicalXAxis2Cache.SetWorldLimitsFromPhysical(minPoint, maxPoint);
                            ((PlotSurface2D)ctr).PhysicalYAxis1Cache.SetWorldLimitsFromPhysical(maxPoint, minPoint);
                            ((PlotSurface2D)ctr).PhysicalYAxis2Cache.SetWorldLimitsFromPhysical(maxPoint, minPoint);

                            // reset the start/end points
                            startPoint_ = unset_;
                            endPoint_ = unset_;

                            ((PlotSurface2D)ctr).InteractionOccured(this);

                            return true;
                        }
                    }

                    return false;
                }

                /// <summary>
                /// Draws a rectangle representing selection area. 
                /// </summary>
                /// <param name="start">a corner of the rectangle.</param>
                /// <param name="end">a corner of the rectangle diagonally opposite the first.</param>
                /// <param name="ctr">The control to draw to - this may not be us, if we have
                /// been contained by a PlotSurface.</param>
                private void DrawRubberBand(Point start, Point end, Control ctr)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    Rectangle rect = new Rectangle();

                    // the clipping rectangle in screen coordinates
                    Rectangle clip = ctr.RectangleToScreen(
                        new Rectangle(
                            ps.PlotAreaBoundingBoxCache.X,
                            ps.PlotAreaBoundingBoxCache.Y,
                            ps.PlotAreaBoundingBoxCache.Width,
                            ps.PlotAreaBoundingBoxCache.Height));

                    // convert to screen coords
                    start = ctr.PointToScreen(start);
                    end = ctr.PointToScreen(end);

                    // now, "normalize" the rectangle
                    if (start.X < end.X)
                    {
                        rect.X = start.X;
                        rect.Width = end.X - start.X;
                    }
                    else
                    {
                        rect.X = end.X;
                        rect.Width = start.X - end.X;
                    }
                    if (start.Y < end.Y)
                    {
                        rect.Y = start.Y;
                        rect.Height = end.Y - start.Y;
                    }
                    else
                    {
                        rect.Y = end.Y;
                        rect.Height = start.Y - end.Y;
                    }
                    rect = Rectangle.Intersect(rect, clip);

                    ControlPaint.DrawReversibleFrame(
                        new Rectangle(rect.X, rect.Y, rect.Width, rect.Height),
                        Color.White, FrameStyle.Dashed);
                }

                private Point startPoint_ = new Point(-1, -1);
                private Point endPoint_ = new Point(-1, -1);
                // this is the condition for an unset point
                private readonly Point unset_ = new Point(-1, -1);
            }

            #endregion

            #region HorizontalGuideline

            /// <summary>
            /// Horizontal line interaction
            /// </summary>
            public class HorizontalGuideline : Interaction
            {
                private int barPos_;
                private readonly Color color_;

                /// <summary>
                /// Constructor
                /// </summary>
                public HorizontalGuideline()
                {
                    color_ = Color.Black;
                }

                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="lineColor"></param>
                public HorizontalGuideline(Color lineColor)
                {
                    color_ = lineColor;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="pe"></param>
                /// <param name="width"></param>
                /// <param name="height"></param>
                public override void DoPaint(PaintEventArgs pe, int width, int height)
                {
                    barPos_ = -1;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    // if mouse isn't in plot region, then don't draw horizontal line
                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom - 1)
                    {
                        if (ps.PhysicalXAxis1Cache != null)
                        {
                            // the clipping rectangle in screen coordinates
                            Rectangle clip = ctr.RectangleToScreen(
                                new Rectangle(
                                    ps.PlotAreaBoundingBoxCache.X,
                                    ps.PlotAreaBoundingBoxCache.Y,
                                    ps.PlotAreaBoundingBoxCache.Width,
                                    ps.PlotAreaBoundingBoxCache.Height));

                            Point p = ctr.PointToScreen(new Point(e.X, e.Y));

                            if (barPos_ != -1)
                            {
                                ControlPaint.DrawReversibleLine(
                                    new Point(clip.Left, barPos_),
                                    new Point(clip.Right, barPos_), color_);
                            }

                            if (p.Y < clip.Bottom && p.Y > clip.Top)
                            {
                                ControlPaint.DrawReversibleLine(
                                    new Point(clip.Left, p.Y),
                                    new Point(clip.Right, p.Y), color_);

                                barPos_ = p.Y;
                            }
                            else
                            {
                                barPos_ = -1;
                            }
                        }
                    }
                    else
                    {
                        if (barPos_ != -1)
                        {
                            Rectangle clip = ctr.RectangleToScreen(
                                new Rectangle(
                                    ps.PlotAreaBoundingBoxCache.X,
                                    ps.PlotAreaBoundingBoxCache.Y,
                                    ps.PlotAreaBoundingBoxCache.Width,
                                    ps.PlotAreaBoundingBoxCache.Height));

                            ControlPaint.DrawReversibleLine(
                                new Point(clip.Left, barPos_),
                                new Point(clip.Right, barPos_), color_);
                            barPos_ = -1;
                        }
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <returns></returns>
                public override bool DoMouseLeave(EventArgs e, Control ctr)
                {
                    if (barPos_ != -1)
                    {
                        NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                        Rectangle clip = ctr.RectangleToScreen(
                            new Rectangle(
                                ps.PlotAreaBoundingBoxCache.X,
                                ps.PlotAreaBoundingBoxCache.Y,
                                ps.PlotAreaBoundingBoxCache.Width,
                                ps.PlotAreaBoundingBoxCache.Height));

                        ControlPaint.DrawReversibleLine(
                            new Point(clip.Left, barPos_),
                            new Point(clip.Right, barPos_), color_);

                        barPos_ = -1;
                    }
                    return false;
                }
            }

            #endregion

            #region VerticalGuideline

            /// <summary>
            /// 
            /// </summary>
            public class VerticalGuideline : Interaction
            {
                private int barPos_;
                private readonly Color color_;

                /// <summary>
                /// 
                /// </summary>
                public VerticalGuideline()
                {
                    color_ = Color.Black;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="lineColor"></param>
                public VerticalGuideline(Color lineColor)
                {
                    color_ = lineColor;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="pe"></param>
                /// <param name="width"></param>
                /// <param name="height"></param>
                public override void DoPaint(PaintEventArgs pe, int width, int height)
                {
                    barPos_ = -1;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    // if mouse isn't in plot region, then don't draw horizontal line
                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right - 1 &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    {
                        if (ps.PhysicalXAxis1Cache != null)
                        {
                            // the clipping rectangle in screen coordinates
                            Rectangle clip = ctr.RectangleToScreen(
                                new Rectangle(
                                    ps.PlotAreaBoundingBoxCache.X,
                                    ps.PlotAreaBoundingBoxCache.Y,
                                    ps.PlotAreaBoundingBoxCache.Width,
                                    ps.PlotAreaBoundingBoxCache.Height));

                            Point p = ctr.PointToScreen(new Point(e.X, e.Y));

                            if (barPos_ != -1)
                            {
                                ControlPaint.DrawReversibleLine(
                                    new Point(barPos_, clip.Top),
                                    new Point(barPos_, clip.Bottom), color_);
                            }

                            if (p.X < clip.Right && p.X > clip.Left)
                            {
                                ControlPaint.DrawReversibleLine(
                                    new Point(p.X, clip.Top),
                                    new Point(p.X, clip.Bottom), color_);
                                barPos_ = p.X;
                            }
                            else
                            {
                                barPos_ = -1;
                            }
                        }
                    }
                    else
                    {
                        if (barPos_ != -1)
                        {
                            Rectangle clip = ctr.RectangleToScreen(
                                new Rectangle(
                                    ps.PlotAreaBoundingBoxCache.X,
                                    ps.PlotAreaBoundingBoxCache.Y,
                                    ps.PlotAreaBoundingBoxCache.Width,
                                    ps.PlotAreaBoundingBoxCache.Height)
                                );

                            ControlPaint.DrawReversibleLine(
                                new Point(barPos_, clip.Top),
                                new Point(barPos_, clip.Bottom), color_);

                            barPos_ = -1;
                        }
                    }

                    return false;
                }

                /// <summary>
                /// Handler for mouse leave event
                /// </summary>
                /// <param name="e">event args</param>
                /// <param name="ctr"></param>
                /// <returns></returns>
                public override bool DoMouseLeave(EventArgs e, Control ctr)
                {
                    if (barPos_ != -1)
                    {
                        NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                        Rectangle clip = ctr.RectangleToScreen(
                            new Rectangle(
                                ps.PlotAreaBoundingBoxCache.X,
                                ps.PlotAreaBoundingBoxCache.Y,
                                ps.PlotAreaBoundingBoxCache.Width,
                                ps.PlotAreaBoundingBoxCache.Height));

                        ControlPaint.DrawReversibleLine(
                            new Point(barPos_, clip.Top),
                            new Point(barPos_, clip.Bottom), color_);
                        barPos_ = -1;
                    }
                    return false;
                }
            }

            #endregion

            #region HorizontalDrag

            /// <summary>
            /// 
            /// </summary>
            public class HorizontalDrag : Interaction
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    {
                        dragInitiated_ = true;

                        lastPoint_.X = e.X;
                        lastPoint_.Y = e.Y;
                    }

                    return false;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    if (e.Button == MouseButtons.Left && dragInitiated_)
                    {
                        int diffX = e.X - lastPoint_.X;

                        ((PlotSurface2D)ctr).CacheAxes();

                        // original code was using PixelWorldLength of the physical axis
                        // but it was not working for non-linear axes - the code below works
                        // in all cases
                        if (ps.XAxis1 != null)
                        {
                            Axis axis = ps.XAxis1;
                            PointF pMin = ps.PhysicalXAxis1Cache.PhysicalMin;
                            PointF pMax = ps.PhysicalXAxis1Cache.PhysicalMax;


                            PointF physicalWorldMin = pMin;
                            PointF physicalWorldMax = pMax;
                            physicalWorldMin.X -= diffX;
                            physicalWorldMax.X -= diffX;
                            double newWorldMin = axis.PhysicalToWorld(physicalWorldMin, pMin, pMax, false);
                            double newWorldMax = axis.PhysicalToWorld(physicalWorldMax, pMin, pMax, false);
                            axis.WorldMin = newWorldMin;
                            axis.WorldMax = newWorldMax;
                        }
                        if (ps.XAxis2 != null)
                        {
                            Axis axis = ps.XAxis2;
                            PointF pMin = ps.PhysicalXAxis2Cache.PhysicalMin;
                            PointF pMax = ps.PhysicalXAxis2Cache.PhysicalMax;

                            PointF physicalWorldMin = pMin;
                            PointF physicalWorldMax = pMax;
                            physicalWorldMin.X -= diffX;
                            physicalWorldMax.X -= diffX;
                            double newWorldMin = axis.PhysicalToWorld(physicalWorldMin, pMin, pMax, false);
                            double newWorldMax = axis.PhysicalToWorld(physicalWorldMax, pMin, pMax, false);
                            axis.WorldMin = newWorldMin;
                            axis.WorldMax = newWorldMax;
                        }

                        lastPoint_ = new Point(e.X, e.Y);

                        ((PlotSurface2D)ctr).InteractionOccured(this);

                        return true;
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    if (e.Button == MouseButtons.Left && dragInitiated_)
                    {
                        lastPoint_ = unset_;
                        dragInitiated_ = false;
                    }
                    return false;
                }

                private bool dragInitiated_;
                private Point lastPoint_ = new Point(-1, -1);
                // this is the condition for an unset point
                private readonly Point unset_ = new Point(-1, -1);
            }

            #endregion

            #region VerticalDrag

            /// <summary>
            /// 
            /// </summary>
            public class VerticalDrag : Interaction
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    {
                        dragInitiated_ = true;

                        lastPoint_.X = e.X;
                        lastPoint_.Y = e.Y;
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    if (e.Button == MouseButtons.Left && dragInitiated_)
                    {
                        int diffY = e.Y - lastPoint_.Y;

                        ((PlotSurface2D)ctr).CacheAxes();

                        if (ps.YAxis1 != null)
                        {
                            Axis axis = ps.YAxis1;
                            PointF pMin = ps.PhysicalYAxis1Cache.PhysicalMin;
                            PointF pMax = ps.PhysicalYAxis1Cache.PhysicalMax;

                            PointF physicalWorldMin = pMin;
                            PointF physicalWorldMax = pMax;
                            physicalWorldMin.Y -= diffY;
                            physicalWorldMax.Y -= diffY;
                            double newWorldMin = axis.PhysicalToWorld(physicalWorldMin, pMin, pMax, false);
                            double newWorldMax = axis.PhysicalToWorld(physicalWorldMax, pMin, pMax, false);
                            axis.WorldMin = newWorldMin;
                            axis.WorldMax = newWorldMax;
                        }
                        if (ps.YAxis2 != null)
                        {
                            Axis axis = ps.YAxis2;
                            PointF pMin = ps.PhysicalYAxis2Cache.PhysicalMin;
                            PointF pMax = ps.PhysicalYAxis2Cache.PhysicalMax;

                            PointF physicalWorldMin = pMin;
                            PointF physicalWorldMax = pMax;
                            physicalWorldMin.Y -= diffY;
                            physicalWorldMax.Y -= diffY;
                            double newWorldMin = axis.PhysicalToWorld(physicalWorldMin, pMin, pMax, false);
                            double newWorldMax = axis.PhysicalToWorld(physicalWorldMax, pMin, pMax, false);
                            axis.WorldMin = newWorldMin;
                            axis.WorldMax = newWorldMax;
                        }

                        lastPoint_ = new Point(e.X, e.Y);

                        ((PlotSurface2D)ctr).InteractionOccured(this);

                        return true;
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    if (e.Button == MouseButtons.Left && dragInitiated_)
                    {
                        lastPoint_ = unset_;
                        dragInitiated_ = false;
                    }

                    return false;
                }

                private bool dragInitiated_;
                private Point lastPoint_ = new Point(-1, -1);
                // this is the condition for an unset point
                private readonly Point unset_ = new Point(-1, -1);
            }

            #endregion

            #region HorizontalRangeSelection

            /// <summary>
            /// This plot intraction allows the user to select horizontal regions.
            /// </summary>
            public class HorizontalRangeSelection : Interaction
            {
                private bool selectionInitiated_;
                private Point startPoint_ = new Point(-1, -1);
                private Point endPoint_ = new Point(-1, -1);
                private Point previousPoint_ = new Point(-1, -1);
                private readonly Point unset_ = new Point(-1, -1);
                private int minimumPixelDistanceForSelect_ = 5;
                private double smallestAllowedRange_ = double.Epsilon * 100.0;


                /// <summary>
                /// Default constructor
                /// </summary>
                public HorizontalRangeSelection()
                {
                }


                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="smallestAllowedRange">the smallest distance between the selected xmin and xmax for the selection to be performed.</param>
                public HorizontalRangeSelection(double smallestAllowedRange)
                {
                    this.smallestAllowedRange_ = smallestAllowedRange;
                }


                /// <summary>
                /// The minimum width of the selected region (in pixels) for the interaction to zoom.
                /// </summary>
                public int MinimumPixelDistanceForSelect
                {
                    get => minimumPixelDistanceForSelect_;
                    set => minimumPixelDistanceForSelect_ = value;
                }


                /// <summary>
                /// The smallest range (distance between world min and world max) selectable.
                /// If a smaller region is selected, the selection will do nothing.
                /// </summary>
                public double SmallestAllowedRange
                {
                    get => smallestAllowedRange_;
                    set => smallestAllowedRange_ = value;
                }


                /// <summary>
                /// Handler for mouse down event for this interaction
                /// </summary>
                /// <param name="e">the mouse event args</param>
                /// <param name="ctr">the plot surface this event applies to</param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    {
                        // keep track of the start point and flag that select initiated.
                        selectionInitiated_ = true;
                        startPoint_.X = e.X;
                        startPoint_.Y = e.Y;

                        previousPoint_.X = e.X;
                        previousPoint_.Y = e.Y;

                        // invalidate the end point
                        endPoint_ = unset_;

                        return false;
                    }

                    selectionInitiated_ = false;
                    endPoint_ = unset_;
                    startPoint_ = unset_;
                    return false;
                }


                /// <summary>
                /// Handler for mouse move event for this interaction
                /// </summary>
                /// <param name="e">the mouse event args</param>
                /// <param name="ctr">the plot surface this event applies to</param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    // if dragging on axis to zoom.
                    if (e.Button == MouseButtons.Left && selectionInitiated_)
                    {
                        Point endPoint_ = previousPoint_;
                        if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                            e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                        {
                            endPoint_ = new Point(e.X, e.Y);
                            this.DrawHorizontalSelection(previousPoint_, endPoint_, ctr);
                            previousPoint_ = endPoint_;
                        }
                        else
                        {
                            endPoint_ = new Point(e.X, e.Y);
                            if (e.X < ps.PlotAreaBoundingBoxCache.Left) endPoint_.X = ps.PlotAreaBoundingBoxCache.Left + 1;
                            if (e.X > ps.PlotAreaBoundingBoxCache.Right) endPoint_.X = ps.PlotAreaBoundingBoxCache.Right - 1;
                            this.DrawHorizontalSelection(previousPoint_, endPoint_, ctr);
                            previousPoint_ = endPoint_;
                        }
                    }

                    return false;
                }


                /// <summary>
                /// Handler for mouse up event for this interaction
                /// </summary>
                /// <param name="e">the mouse event args</param>
                /// <param name="ctr">the plot surface this event applies to</param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    if (e.Button == MouseButtons.Left && selectionInitiated_)
                    {
                        endPoint_.X = e.X;
                        endPoint_.Y = e.Y;
                        if (e.X < ps.PlotAreaBoundingBoxCache.Left) endPoint_.X = ps.PlotAreaBoundingBoxCache.Left + 1;
                        if (e.X > ps.PlotAreaBoundingBoxCache.Right) endPoint_.X = ps.PlotAreaBoundingBoxCache.Right - 1;

                        // flag stopped selecting.
                        selectionInitiated_ = false;

                        if (endPoint_ != unset_)
                        {
                            this.DrawHorizontalSelection(startPoint_, endPoint_, ctr);
                        }

                        // ignore very small selections
                        if (Math.Abs(endPoint_.X - startPoint_.X) < minimumPixelDistanceForSelect_)
                        {
                            return false;
                        }

                        ((PlotSurface2D)ctr).CacheAxes();

                        // determine the new x axis 1 world limits (and check to see if they are far enough appart).
                        double xAxis1Min = double.NaN;
                        double xAxis1Max = double.NaN;
                        if (ps.XAxis1 != null)
                        {
                            int x1 = Math.Min(endPoint_.X, startPoint_.X);
                            int x2 = Math.Max(endPoint_.X, startPoint_.X);
                            int y = ps.PhysicalXAxis1Cache.PhysicalMax.Y;

                            xAxis1Min = ps.PhysicalXAxis1Cache.PhysicalToWorld(new Point(x1, y), true);
                            xAxis1Max = ps.PhysicalXAxis1Cache.PhysicalToWorld(new Point(x2, y), true);
                            if (xAxis1Max - xAxis1Min < this.smallestAllowedRange_)
                            {
                                return false;
                            }
                        }

                        // determine the new x axis 2 world limits (and check to see if they are far enough appart).
                        double xAxis2Min = double.NaN;
                        double xAxis2Max = double.NaN;
                        if (ps.XAxis2 != null)
                        {
                            int x1 = Math.Min(endPoint_.X, startPoint_.X);
                            int x2 = Math.Max(endPoint_.X, startPoint_.X);
                            int y = ps.PhysicalXAxis2Cache.PhysicalMax.Y;

                            xAxis2Min = ps.PhysicalXAxis2Cache.PhysicalToWorld(new Point(x1, y), true);
                            xAxis2Max = ps.PhysicalXAxis2Cache.PhysicalToWorld(new Point(x2, y), true);
                            if (xAxis2Max - xAxis2Min < smallestAllowedRange_)
                            {
                                return false;
                            }
                        }

                        // now actually update the world limits.

                        if (ps.XAxis1 != null)
                        {
                            ps.XAxis1.WorldMax = xAxis1Max;
                            ps.XAxis1.WorldMin = xAxis1Min;
                        }

                        if (ps.XAxis2 != null)
                        {
                            ps.XAxis2.WorldMax = xAxis2Max;
                            ps.XAxis2.WorldMin = xAxis2Min;
                        }

                        ((PlotSurface2D)ctr).InteractionOccured(this);

                        return true;
                    }

                    return false;
                }


                private void DrawHorizontalSelection(Point start, Point end, Control ctr)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    // the clipping rectangle in screen coordinates
                    Rectangle clip = ctr.RectangleToScreen(
                        new Rectangle(
                            ps.PlotAreaBoundingBoxCache.X,
                            ps.PlotAreaBoundingBoxCache.Y,
                            ps.PlotAreaBoundingBoxCache.Width,
                            ps.PlotAreaBoundingBoxCache.Height));

                    start = ctr.PointToScreen(start);
                    end = ctr.PointToScreen(end);

                    ControlPaint.FillReversibleRectangle(
                        new Rectangle(Math.Min(start.X, end.X), clip.Y, Math.Abs(end.X - start.X), clip.Height),
                        Color.White);
                }
            }

            #endregion

            #region AxisDrag

            /// <summary>
            /// 
            /// </summary>
            public class AxisDrag : Interaction
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="enableDragWithCtr"></param>
                public AxisDrag(bool enableDragWithCtr)
                {
                    enableDragWithCtr_ = enableDragWithCtr;
                }

                private readonly bool enableDragWithCtr_;

                private Axis axis_;
                private bool doing_;
                private Point lastPoint_;
                private PhysicalAxis physicalAxis_;
                private Point startPoint_;


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    // if the mouse is inside the plot area [the tick marks are here and part of the 
                    // axis], then don't invoke drag. 
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;
                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    {
                        return false;
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        // see if hit with axis.
                        ArrayList objects = ps.HitTest(new Point(e.X, e.Y));

                        foreach (object o in objects)
                        {
                            if (o is Axis)
                            {
                                doing_ = true;
                                axis_ = (Axis)o;
                                _ = new PhysicalAxis[] { ps.PhysicalXAxis1Cache, ps.PhysicalXAxis2Cache, ps.PhysicalYAxis1Cache, ps.PhysicalYAxis2Cache };

                                if (ps.PhysicalXAxis1Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalXAxis1Cache;
                                else if (ps.PhysicalXAxis2Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalXAxis2Cache;
                                else if (ps.PhysicalYAxis1Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalYAxis1Cache;
                                else if (ps.PhysicalYAxis2Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalYAxis2Cache;

                                lastPoint_ = startPoint_ = new Point(e.X, e.Y);

                                return false;
                            }
                        }
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    // if dragging on axis to zoom.
                    if (e.Button == MouseButtons.Left && doing_ && physicalAxis_ != null)
                    {
                        if (enableDragWithCtr_ && lastKeyEventArgs != null && lastKeyEventArgs.Control)
                        {
                        }
                        else
                        {
                            float dist =
                                e.X - lastPoint_.X + -e.Y + lastPoint_.Y;

                            lastPoint_ = new Point(e.X, e.Y);

                            if (dist > sensitivity_ / 3.0f)
                            {
                                dist = sensitivity_ / 3.0f;
                            }

                            PointF pMin = physicalAxis_.PhysicalMin;
                            PointF pMax = physicalAxis_.PhysicalMax;
                            double physicalWorldLength = Math.Sqrt((pMax.X - pMin.X) * (pMax.X - pMin.X) + (pMax.Y - pMin.Y) * (pMax.Y - pMin.Y));

                            float prop = (float)(physicalWorldLength * dist / sensitivity_);
                            prop *= 2;

                            ((PlotSurface2D)ctr).CacheAxes();

                            float relativePosX = (startPoint_.X - pMin.X) / (pMax.X - pMin.X);
                            float relativePosY = (startPoint_.Y - pMin.Y) / (pMax.Y - pMin.Y);

                            if (float.IsInfinity(relativePosX) || float.IsNaN(relativePosX)) relativePosX = 0.0f;
                            if (float.IsInfinity(relativePosY) || float.IsNaN(relativePosY)) relativePosY = 0.0f;

                            PointF physicalWorldMin = pMin;
                            PointF physicalWorldMax = pMax;

                            physicalWorldMin.X += relativePosX * prop;
                            physicalWorldMax.X -= (1 - relativePosX) * prop;
                            physicalWorldMin.Y -= relativePosY * prop;
                            physicalWorldMax.Y += (1 - relativePosY) * prop;

                            double newWorldMin = axis_.PhysicalToWorld(physicalWorldMin, pMin, pMax, false);
                            double newWorldMax = axis_.PhysicalToWorld(physicalWorldMax, pMin, pMax, false);
                            axis_.WorldMin = newWorldMin;
                            axis_.WorldMax = newWorldMax;

                            ((PlotSurface2D)ctr).InteractionOccured(this);

                            return true;
                        }
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    if (doing_)
                    {
                        doing_ = false;
                        axis_ = null;
                        physicalAxis_ = null;
                        lastPoint_ = new Point();
                    }

                    return false;
                }

                private float sensitivity_ = 200.0f;

                /// <summary>
                /// 
                /// </summary>
                /// <value></value>
                public float Sensitivity
                {
                    get => sensitivity_;
                    set => sensitivity_ = value;
                }
            }

            public class AxisDragX : Interaction
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="enableDragWithCtr"></param>
                public AxisDragX(bool enableDragWithCtr)
                {
                    enableDragWithCtr_ = enableDragWithCtr;
                    ZoomLocation = Location.MouseLocation;
                }

                public enum Location
                {
                    MouseLocation,
                    FrontOfGraph
                }

                public Location ZoomLocation
                {
                    get;
                    set;
                }

                private readonly bool enableDragWithCtr_;

                private Axis axis_;
                private bool doing_;
                private Point lastPoint_;
                private PhysicalAxis physicalAxis_;
                private Point startPoint_;


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    // if the mouse is inside the plot area [the tick marks are here and part of the 
                    // axis], then don't invoke drag. 
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;
                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    {
                        return false;
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        // see if hit with axis.
                        ArrayList objects = ps.HitTest(new Point(e.X, e.Y));

                        foreach (object o in objects)
                        {
                            if (o is Axis)
                            {
                                _ = new PhysicalAxis[] { ps.PhysicalXAxis1Cache, ps.PhysicalXAxis2Cache };

                                if (ps.PhysicalXAxis1Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalXAxis1Cache;
                                else if (ps.PhysicalXAxis2Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalXAxis2Cache;

                                doing_ = true;
                                axis_ = (Axis)o;

                                lastPoint_ = startPoint_ = new Point(e.X, e.Y);

                                return false;
                            }
                        }
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    // if dragging on axis to zoom.
                    if (e.Button == MouseButtons.Left && doing_ && physicalAxis_ != null)
                    {
                        NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                        // see if hit with axis.
                        ArrayList objects = ps.HitTest(new Point(e.X, e.Y));

                        foreach (object o in objects)
                        {
                            if (o is Axis)
                            {
                                PhysicalAxis[] physicalAxisList;
                                _ = new PhysicalAxis[] { ps.PhysicalXAxis1Cache, ps.PhysicalXAxis2Cache };

                                if (ps.PhysicalXAxis1Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalXAxis1Cache;
                                else if (ps.PhysicalXAxis2Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalXAxis2Cache;

                                axis_ = (Axis)o;
                            }
                        }

                        if (enableDragWithCtr_ && lastKeyEventArgs != null && lastKeyEventArgs.Control)
                        {
                        }
                        else
                        {
                            float dist =
                                e.X - lastPoint_.X;

                            lastPoint_ = new Point(e.X, e.Y);

                            if (dist > sensitivity_ / 3.0f)
                            {
                                dist = sensitivity_ / 3.0f;
                            }

                            PointF pMin = physicalAxis_.PhysicalMin;
                            PointF pMax = physicalAxis_.PhysicalMax;
                            double physicalWorldLength = pMax.X - pMin.X;

                            float prop = (float)(physicalWorldLength * dist / sensitivity_);
                            prop *= 2;

                            ((PlotSurface2D)ctr).CacheAxes();

                            float relativePosX = (startPoint_.X - pMin.X) / (pMax.X - pMin.X);

                            if (float.IsInfinity(relativePosX) || float.IsNaN(relativePosX)) relativePosX = 0.0f;

                            PointF physicalWorldMin = pMin;
                            PointF physicalWorldMax = pMax;

                            if (ZoomLocation == Location.MouseLocation)
                            {
                                physicalWorldMin.X += relativePosX * prop;
                                physicalWorldMax.X -= (1 - relativePosX) * prop;
                            }
                            else
                            {
                                physicalWorldMin.X += prop;
                            }

                            double newWorldMin = axis_.PhysicalToWorld(physicalWorldMin, pMin, pMax, false);
                            double newWorldMax = axis_.PhysicalToWorld(physicalWorldMax, pMin, pMax, false);
                            axis_.WorldMin = newWorldMin;
                            axis_.WorldMax = newWorldMax;

                            ((PlotSurface2D)ctr).InteractionOccured(this);

                            return true;
                        }
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    if (doing_)
                    {
                        doing_ = false;
                        axis_ = null;
                        physicalAxis_ = null;
                        lastPoint_ = new Point();
                    }

                    return false;
                }

                private float sensitivity_ = 200.0f;

                /// <summary>
                /// 
                /// </summary>
                /// <value></value>
                public float Sensitivity
                {
                    get => sensitivity_;
                    set => sensitivity_ = value;
                }
            }

            public class AxisDragY : Interaction
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="enableDragWithCtr"></param>
                public AxisDragY(bool enableDragWithCtr)
                {
                    enableDragWithCtr_ = enableDragWithCtr;
                }

                private readonly bool enableDragWithCtr_;

                private Axis axis_;
                private bool doing_;
                private Point lastPoint_;
                private PhysicalAxis physicalAxis_;
                private Point startPoint_;

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    // if the mouse is inside the plot area [the tick marks are here and part of the 
                    // axis], then don't invoke drag. 
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;
                    if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                        e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    {
                        return false;
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        // see if hit with axis.
                        ArrayList objects = ps.HitTest(new Point(e.X, e.Y));

                        foreach (object o in objects)
                        {
                            if (o is Axis)
                            {
                                doing_ = true;
                                axis_ = (Axis)o;
                                _ = new PhysicalAxis[] { ps.PhysicalYAxis1Cache, ps.PhysicalYAxis2Cache };

                                if (ps.PhysicalYAxis1Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalYAxis1Cache;
                                else if (ps.PhysicalYAxis2Cache.Axis == axis_)
                                    physicalAxis_ = ps.PhysicalYAxis2Cache;

                                lastPoint_ = startPoint_ = new Point(e.X, e.Y);

                                return false;
                            }
                        }
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                /// <param name="lastKeyEventArgs"></param>
                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    NPlot.PlotSurface2D ps = ((PlotSurface2D)ctr).Inner;

                    // if dragging on axis to zoom.
                    if (e.Button == MouseButtons.Left && doing_ && physicalAxis_ != null)
                    {
                        if (enableDragWithCtr_ && lastKeyEventArgs != null && lastKeyEventArgs.Control)
                        {
                        }
                        else
                        {
                            float dist = -e.Y + lastPoint_.Y;

                            lastPoint_ = new Point(e.X, e.Y);

                            if (dist > sensitivity_ / 3.0f)
                            {
                                dist = sensitivity_ / 3.0f;
                            }

                            PointF pMin = physicalAxis_.PhysicalMin;
                            PointF pMax = physicalAxis_.PhysicalMax;
                            double physicalWorldLength = pMax.Y - pMin.Y;

                            float prop = (float)(physicalWorldLength * dist / sensitivity_);
                            prop *= 2;

                            ((PlotSurface2D)ctr).CacheAxes();

                            float relativePosY = (startPoint_.Y - pMin.Y) / (pMax.Y - pMin.Y);

                            if (float.IsInfinity(relativePosY) || float.IsNaN(relativePosY)) relativePosY = 0.0f;

                            PointF physicalWorldMin = pMin;
                            PointF physicalWorldMax = pMax;

                            physicalWorldMin.Y -= relativePosY * prop;
                            physicalWorldMax.Y += (1 - relativePosY) * prop;

                            double newWorldMin = axis_.PhysicalToWorld(physicalWorldMin, pMin, pMax, false);
                            double newWorldMax = axis_.PhysicalToWorld(physicalWorldMax, pMin, pMax, false);
                            axis_.WorldMin = newWorldMin;
                            axis_.WorldMax = newWorldMax;

                            ((PlotSurface2D)ctr).InteractionOccured(this);

                            return true;
                        }
                    }

                    return false;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    if (doing_)
                    {
                        doing_ = false;
                        axis_ = null;
                        physicalAxis_ = null;
                        lastPoint_ = new Point();
                    }

                    return false;
                }

                private float sensitivity_ = 200.0f;

                /// <summary>
                /// 
                /// </summary>
                /// <value></value>
                public float Sensitivity
                {
                    get => sensitivity_;
                    set => sensitivity_ = value;
                }
            }

            #endregion

            #region MouseWheelZoom

            /// <summary>
            /// 
            /// </summary>
            public class MouseWheelZoom : Interaction
            {
                private Point point_ = new Point(-1, -1);
                //private bool mouseDown_ = false;

                public MouseWheelZoom()
                {
                    ZoomLocation = Location.MouseLocation;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseUp(MouseEventArgs e, Control ctr)
                {
                    //mouseDown_ = false;
                    return false;
                }

                public override bool DoMouseMove(MouseEventArgs e, Control ctr, KeyEventArgs lastKeyEventArgs)
                {
                    if (!ctr.Focused && ReferenceEquals(GetParentForm(ctr), Form.ActiveForm))
                    {
                        ctr.Focus();
                    }
                    return false;
                }

                private Form GetParentForm(Control ctr)
                {
                    Form parentForm = ctr as Form;
                    if (parentForm != null)
                        return parentForm;

                    Control parent = ctr.Parent;
                    if (parent != null)
                        return GetParentForm(parent);
                    return null;
                }

                public enum Location
                {
                    MouseLocation,
                    FrontOfGraph
                }

                public Location ZoomLocation
                {
                    get;
                    set;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseDown(MouseEventArgs e, Control ctr)
                {
                    //NPlot.PlotSurface2D ps = ((Windows.PlotSurface2D)ctr).Inner;

                    //if (e.X > ps.PlotAreaBoundingBoxCache.Left && e.X < ps.PlotAreaBoundingBoxCache.Right &&
                    //    e.Y > ps.PlotAreaBoundingBoxCache.Top && e.Y < ps.PlotAreaBoundingBoxCache.Bottom)
                    //{
                    //    point_.X = e.X;
                    //    point_.Y = e.Y;
                    //    mouseDown_ = true;
                    //}
                    return false;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="ctr"></param>
                public override bool DoMouseWheel(MouseEventArgs e, Control ctr)
                {
                    try
                    {
                        const double zoom = 0.8;
                        PlotSurface2D plot = ctr as PlotSurface2D;
                        if (plot is null)
                            return false;

                        Point point = ctr.PointToScreen(e.Location);
                        point = plot.PointToClient(point);

                        if ((ModifierKeys & Keys.Control) != 0)
                        {
                            double y = plot.Inner.PhysicalYAxis1Cache.PhysicalToWorld(point, false);
                            double sizeYAxis = plot.Inner.YAxis1.WorldLength;
                            double yPercent = (y - plot.Inner.YAxis1.WorldMin) / sizeYAxis;
                            if (e.Delta > 0)
                            {
                                sizeYAxis *= zoom;
                            }
                            else if (e.Delta < 0)
                            {
                                sizeYAxis /= zoom;
                            }
                            plot.Inner.YAxis1.WorldMin = y - sizeYAxis * yPercent;
                            plot.Inner.YAxis1.WorldMax = plot.Inner.YAxis1.WorldMin + sizeYAxis;
                        }
                        else
                        {
                            double x = plot.Inner.PhysicalXAxis1Cache.PhysicalToWorld(point, false);
                            double sizeXAxis = plot.Inner.XAxis1.WorldLength;
                            double xPercent = (x - plot.Inner.XAxis1.WorldMin) / sizeXAxis;
                            if (e.Delta > 0)
                            {
                                sizeXAxis *= zoom;
                            }
                            else if (e.Delta < 0)
                            {
                                sizeXAxis /= zoom;
                            }

                            if (ZoomLocation == Location.MouseLocation)
                            {
                                plot.Inner.XAxis1.WorldMin = x - sizeXAxis * xPercent;
                                plot.Inner.XAxis1.WorldMax = plot.Inner.XAxis1.WorldMin + sizeXAxis;
                            }
                            else
                            {
                                plot.Inner.XAxis1.WorldMin = plot.Inner.XAxis1.WorldMax - sizeXAxis;
                                //plot.Inner.XAxis1.WorldMax = plot.Inner.XAxis1.WorldMin + sizeXAxis;
                            }

                            plot.InteractionOccured(this);
                        }
                    }
                    catch
                    {
                    }
                    return true;
                }


                /// <summary>
                /// Number of screen pixels equivalent to one wheel step.
                /// </summary>
                public float Sensitivity
                {
                    get => sensitivity_;
                    set => sensitivity_ = value;
                }

                private float sensitivity_ = 60.0f;
            }

            #endregion
        }
    }
}