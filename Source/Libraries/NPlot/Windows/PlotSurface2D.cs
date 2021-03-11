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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace NPlot.Windows
{
    /// <summary>
    /// A Windows.Forms PlotSurface2D control.
    /// </summary>
    /// <remarks>
    /// Unfortunately it's not possible to derive from both Control and NPlot.PlotSurface2D.
    /// </remarks>
    [ToolboxBitmap(typeof(PlotSurface2D), "PlotSurface2D.ico")]
    public partial class PlotSurface2D : Control
    {
        private ToolTip coordinates_;

        private ArrayList selectedObjects_;
        private readonly NPlot.PlotSurface2D ps_;

        private DateTimeAxis xAxis1ZoomCache_;
        private LinearAxis yAxis1ZoomCache_;
        private DateTimeAxis xAxis2ZoomCache_;
        private LinearAxis yAxis2ZoomCache_;

        /// <summary>
        /// Flag to display a coordinates in a tooltip.
        /// </summary>
        [
            Category("PlotSurface2D"),
            Description("Whether or not to show coordinates in a tool tip when the mouse hovers above the plot area."),
            Browsable(true),
            Bindable(true)
        ]
        public bool ShowCoordinates
        {
            get => this.coordinates_.Active;
            set => this.coordinates_.Active = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PlotSurface2D()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // double buffer, and update when resize.
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.ResizeRedraw = true;

            ps_ = new NPlot.PlotSurface2D();

            this.InteractionOccured += new InteractionHandler(OnInteractionOccured);
            this.PreRefresh += new PreRefreshHandler(OnPreRefresh);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        /// <remarks>Modified! :-)</remarks>
        private void InitializeComponent()
        {
            this.components = new Container();
            this.coordinates_ = new ToolTip(this.components);
            // 
            // PlotSurface2D
            // 
            this.BackColor = SystemColors.ControlLightLight;
            this.Size = new Size(328, 272);
        }


        private KeyEventArgs lastKeyEventArgs_;

        /// <summary>
        /// the key down callback
        /// </summary>
        /// <param name="e">information pertaining to the event</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            lastKeyEventArgs_ = e;
        }

        /// <summary>
        /// The key up callback.
        /// </summary>
        /// <param name="e">information pertaining to the event</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            lastKeyEventArgs_ = e;
        }

        /// <summary>
        /// the paint event callback.
        /// </summary>
        /// <param name="pe">the PaintEventArgs</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            DoPaint(pe, this.Width, this.Height);
            base.OnPaint(pe);
        }


        /// <summary>
        /// All functionality of the OnPaint method is provided by this function.
        /// This allows use of the all encompasing PlotSurface.
        /// </summary>
        /// <param name="pe">the PaintEventArgs from paint event.</param>
        /// <param name="width">width of the control</param>
        /// <param name="height">height of the control</param>
        public void DoPaint(PaintEventArgs pe, int width, int height)
        {
            this.PreRefresh(this);

            foreach (Interactions.Interaction i in interactions_)
            {
                i.DoPaint(pe, width, height);
            }

            /*
            // make sure don't redraw after a refresh.
            this.horizontalBarPos_ = -1;
            this.verticalBarPos_ = -1;
            */

            Graphics g = pe.Graphics;

            Rectangle border = new Rectangle(0, 0, width, height);

            if (g is null)
            {
                throw new NPlotException("null graphics context!");
            }

            if (ps_ is null)
            {
                throw new NPlotException("null NPlot.PlotSurface2D");
            }

            if (border == Rectangle.Empty)
            {
                throw new NPlotException("null border context");
            }

            this.Draw(g, border);
        }

        private System.Drawing.Bitmap m_asyncImage;
        private bool m_drawAsyncImage;

        public override void Refresh()
        {
            if (!InvokeRequired)
            {
                base.Refresh();
            }
            else
            {
                m_asyncImage = new System.Drawing.Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(m_asyncImage);
                g.Clear(Color.White);
                ps_.Draw(g, new Rectangle(0, 0, m_asyncImage.Width - 1, m_asyncImage.Height - 1));
                m_drawAsyncImage = true;
                BeginInvoke(new Action(base.Refresh));
            }
        }

        /// <summary>
        /// Draws the plot surface on the supplied graphics surface [not the control surface].
        /// </summary>
        /// <param name="g">The graphics surface on which to draw</param>
        /// <param name="bounds">A bounding box on this surface that denotes the area on the
        /// surface to confine drawing to.</param>
        public void Draw(Graphics g, Rectangle bounds)
        {
            // If we are not in design mode then draw as normal.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                this.drawDesignMode(g, bounds);
            }
            if (m_drawAsyncImage)
            {
                g.DrawImageUnscaled(m_asyncImage, 0, 0);
                m_drawAsyncImage = false;
            }
            else
            {
                ps_.Draw(g, bounds);
            }
        }


        /// <summary>
        /// Draw a lightweight representation of us for design mode.
        /// </summary>
        private void drawDesignMode(Graphics g, Rectangle bounds)
        {
            g.DrawRectangle(new Pen(Color.Black), bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 4);
            g.DrawString("PlotSurface2D: " + this.Title, this.TitleFont, this.TitleBrush, bounds.X + bounds.Width / 2.0f, bounds.Y + bounds.Height / 2.0f);
        }


        /// <summary>
        /// Clears the plot and resets to default values.
        /// </summary>
        public void Clear()
        {
            xAxis1ZoomCache_ = null;
            yAxis1ZoomCache_ = null;
            xAxis2ZoomCache_ = null;
            yAxis2ZoomCache_ = null;
            ps_.Clear();
            interactions_.Clear();
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
        /// Whether or not the title will be scaled according to size of the plot 
        /// surface.
        /// </summary>
        [
            Browsable(true),
            Bindable(true),
            Description("Whether or not the title will be scaled according to size of the plot surface."),
            Category("PlotSurface2D")
        ]
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
        [
            Browsable(true),
            Bindable(true),
            Description("When plots are added to the plot surface, the axes they are attached to are immediately modified " +
                        "to reflect data of the plot. If AutoScaleAutoGeneratedAxes is true when a plot is added, the axes will be " +
                        "turned in to auto scaling ones if they are not already [tick marks, tick text and label size scaled to size " +
                        "of plot surface]. If false, axes will not be autoscaling."),
            Category("PlotSurface2D")
        ]
        public bool AutoScaleAutoGeneratedAxes
        {
            get => ps_.AutoScaleAutoGeneratedAxes;
            set => ps_.AutoScaleAutoGeneratedAxes = value;
        }


        /// <summary>
        /// The plot surface title.
        /// </summary>
        [
            Category("PlotSurface2D"),
            Description("The plot surface title"),
            Browsable(true),
            Bindable(true)
        ]
        public string Title
        {
            get => ps_.Title;
            set => ps_.Title = value;
            //helpful in design view. But crap in applications!
            //this.Refresh();
        }


        /// <summary>
        /// The font used to draw the title.
        /// </summary>
        [
            Category("PlotSurface2D"),
            Description("The font used to draw the title."),
            Browsable(true),
            Bindable(false)
        ]
        public Font TitleFont
        {
            get => ps_.TitleFont;
            set => ps_.TitleFont = value;
        }


        /// <summary>
        /// Padding of this width will be left between what is drawn and the control border.
        /// </summary>
        [
            Category("PlotSurface2D"),
            Description("Padding of this width will be left between what is drawn and the control border."),
            Browsable(true),
            Bindable(true)
        ]
        public int Padding
        {
            get => ps_.Padding;
            set => ps_.Padding = value;
        }


        /// <summary>
        /// The first abscissa axis.
        /// </summary>
        /// 
        [
            Browsable(false)
        ]
        public DateTimeAxis XAxis1
        {
            get => ps_.XAxis1;
            set => ps_.XAxis1 = value;
        }


        /// <summary>
        /// The first ordinate axis.
        /// </summary>
        [
            Browsable(false)
        ]
        public LinearAxis YAxis1
        {
            get => ps_.YAxis1;
            set => ps_.YAxis1 = value;
        }


        /// <summary>
        /// The second abscissa axis.
        /// </summary>
        [
            Browsable(false)
        ]
        public DateTimeAxis XAxis2
        {
            get => ps_.XAxis2;
            set => ps_.XAxis2 = value;
        }


        /// <summary>
        /// The second ordinate axis.
        /// </summary>
        [
            Browsable(false)
        ]
        public LinearAxis YAxis2
        {
            get => ps_.YAxis2;
            set => ps_.YAxis2 = value;
        }


        /// <summary>
        /// The physical XAxis1 that was last drawn.
        /// </summary>
        [
            Browsable(false)
        ]
        public PhysicalAxis PhysicalXAxis1Cache => ps_.PhysicalXAxis1Cache;


        /// <summary>
        /// The physical YAxis1 that was last drawn.
        /// </summary>
        [
            Browsable(false)
        ]
        public PhysicalAxis PhysicalYAxis1Cache => ps_.PhysicalYAxis1Cache;


        /// <summary>
        /// The physical XAxis2 that was last drawn.
        /// </summary>
        [
            Browsable(false)
        ]
        public PhysicalAxis PhysicalXAxis2Cache => ps_.PhysicalXAxis2Cache;


        /// <summary>
        /// The physical YAxis2 that was last drawn.
        /// </summary>
        [
            Browsable(false)
        ]
        public PhysicalAxis PhysicalYAxis2Cache => ps_.PhysicalYAxis2Cache;


        /// <summary>
        /// A color used to paint the plot background. Mutually exclusive with PlotBackImage and PlotBackBrush
        /// </summary>
        /// <remarks>not browsable or bindable because only set method.</remarks>
        [
            Category("PlotSurface2D"),
            Description("Set the plot background color."),
            Browsable(true),
            Bindable(false)
        ]
        public Color PlotBackColor
        {
            set => ps_.PlotBackColor = value;
        }

        /// <summary>
        /// Sets the title to be drawn using a solid brush of this color.
        /// </summary>
        /// <remarks>not browsable or bindable because only set method.</remarks>
        [
            Browsable(false),
            Bindable(false)
        ]
        public Color TitleColor
        {
            set => ps_.TitleColor = value;
        }


        /// <summary>
        /// The brush used for drawing the title.
        /// </summary>
        [
            Browsable(true),
            Bindable(true),
            Description("The brush used for drawing the title."),
            Category("PlotSurface2D")
        ]
        public Brush TitleBrush
        {
            get => ps_.TitleBrush;
            set => ps_.TitleBrush = value;
        }


        /// <summary>
        /// Set smoothing mode for drawing plot objects.
        /// </summary>
        [
            Category("PlotSurface2D"),
            Description("Set smoothing mode for drawing plot objects."),
            Browsable(true),
            Bindable(true)
        ]
        public SmoothingMode SmoothingMode
        {
            get => ps_.SmoothingMode;
            set => ps_.SmoothingMode = value;
        }


        /// <summary>
        /// Mouse down event handler.
        /// </summary>
        /// <param name="e">the event args.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            DoMouseDown(e);
            base.OnMouseDown(e);
        }


        /// <summary>
        /// All functionality of the OnMouseDown function is contained here.
        /// This allows use of the all encompasing PlotSurface.
        /// </summary>
        /// <param name="e">The mouse event args from the window we are drawing to.</param>
        public void DoMouseDown(MouseEventArgs e)
        {
            bool dirty = false;
            foreach (Interactions.Interaction i in interactions_)
            {
                i.DoMouseDown(e, this);
                dirty |= i.DoMouseDown(e, this);
            }
            if (dirty)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Mouse Wheel event handler.
        /// </summary>
        /// <param name="e">the event args</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            DoMouseWheel(e);
            base.OnMouseWheel(e);
        }


        /// <summary>
        /// All functionality of the OnMouseWheel function is containd here.
        /// This allows use of the all encompasing PlotSurface.
        /// </summary>
        /// <param name="e">the event args.</param>
        public void DoMouseWheel(MouseEventArgs e)
        {
            bool dirty = false;
            foreach (Interactions.Interaction i in interactions_)
            {
                i.DoMouseWheel(e, this);
                dirty |= i.DoMouseWheel(e, this);
            }
            if (dirty)
            {
                Refresh();
            }
        }


        /// <summary>
        /// All functionality of the OnMouseMove function is contained here.
        /// This allows use of the all encompasing PlotSurface.
        /// </summary>
        /// <param name="e">The mouse event args from the window we are drawing to.</param>
        /// <param name="ctr">The control that the mouse event happened in.</param>
        public void DoMouseMove(MouseEventArgs e, Control ctr)
        {
            bool dirty = false;
            foreach (Interactions.Interaction i in interactions_)
            {
                i.DoMouseMove(e, ctr, lastKeyEventArgs_);
                dirty |= i.DoMouseMove(e, ctr, lastKeyEventArgs_);
            }
            if (dirty)
            {
                Refresh();
            }

            // Update coordinates if necessary. 

            if (coordinates_.Active)
            {
                // we are here
                Point here = new Point(e.X, e.Y);
                if (ps_.PlotAreaBoundingBoxCache.Contains(here))
                {
                    coordinates_.ShowAlways = true;

                    // according to Måns Erlandson, this can sometimes be the case.
                    if (this.PhysicalXAxis1Cache is null)
                        return;
                    if (this.PhysicalYAxis1Cache is null)
                        return;

                    double x = this.PhysicalXAxis1Cache.PhysicalToWorld(here, true);
                    double y = this.PhysicalYAxis1Cache.PhysicalToWorld(here, true);
                    string s = "";
                    if (!DateTimeToolTip)
                    {
                        s = "(" + x.ToString("g4") + "," + y.ToString("g4") + ")";
                    }
                    else
                    {
                        DateTime dateTime = new DateTime((long)x);
                        s = dateTime.ToShortDateString() + " " + dateTime.ToLongTimeString() + Environment.NewLine + y.ToString("f4");
                    }
                    //Bug fix. Windows 7 will do an infinate loop if this is set.
                    if (coordinates_.GetToolTip(this) != s)
                        coordinates_.SetToolTip(this, s);
                }
                else
                {
                    coordinates_.ShowAlways = false;
                }
            }
        }


        /// <summary>
        /// MouseMove event handler.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            DoMouseMove(e, this);
            base.OnMouseMove(e);
        }


        /// <summary>
        /// MouseLeave event handler. It has to invalidate the control to get rid of
        /// any remnant of vertical and horizontal guides.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            DoMouseLeave(e, this);
            base.OnMouseLeave(e);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="ctr"></param>
        public void DoMouseLeave(EventArgs e, Control ctr)
        {
            bool dirty = false;
            foreach (Interactions.Interaction i in interactions_)
            {
                dirty = i.DoMouseLeave(e, this) || dirty;
            }
            if (dirty)
                Refresh();
        }


        /// <summary>
        /// When true, tool tip will display x value as a DateTime. Quick hack - this will probably be 
        /// changed at some point.
        /// </summary>
        [
            Bindable(true),
            Browsable(true),
            Category("PlotSurface2D"),
            Description("When true, tool tip will display x value as a DateTime. Quick hack - this will probably be changed at some point.")
        ]
        public bool DateTimeToolTip
        {
            get => dateTimeToolTip_;
            set => dateTimeToolTip_ = value;
        }

        private bool dateTimeToolTip_;


        /// <summary>
        /// All functionality of the OnMouseUp function is contained here.
        /// This allows use of the all encompasing PlotSurface.
        /// </summary>
        /// <param name="e">The mouse event args from the window we are drawing to.</param>
        /// <param name="ctr">The control that the mouse event happened in.</param>
        public void DoMouseUp(MouseEventArgs e, Control ctr)
        {
            bool dirty = false;

            foreach (Interactions.Interaction i in interactions_)
            {
                dirty |= i.DoMouseUp(e, ctr);
            }
            if (dirty)
            {
                Refresh();
            }
        }


        /// <summary>
        /// mouse up event handler.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            DoMouseUp(e, this);
            base.OnMouseUp(e);
        }


        /// <summary>
        /// sets axes to be those saved in the cache.
        /// </summary>
        public void OriginalDimensions()
        {
            if (xAxis1ZoomCache_ != null)
            {
                this.XAxis1 = xAxis1ZoomCache_;
                this.XAxis2 = xAxis2ZoomCache_;
                this.YAxis1 = yAxis1ZoomCache_;
                this.YAxis2 = yAxis2ZoomCache_;

                xAxis1ZoomCache_ = null;
                xAxis2ZoomCache_ = null;
                yAxis1ZoomCache_ = null;
                yAxis2ZoomCache_ = null;
            }
            this.Refresh();
        }

        private void DrawHorizontalSelection(Point start, Point end, UserControl ctr)
        {
            // the clipping rectangle in screen coordinates
            Rectangle clip = ctr.RectangleToScreen(
                new Rectangle(
                    ps_.PlotAreaBoundingBoxCache.X,
                    ps_.PlotAreaBoundingBoxCache.Y,
                    ps_.PlotAreaBoundingBoxCache.Width,
                    ps_.PlotAreaBoundingBoxCache.Height));

            start = ctr.PointToScreen(start);
            end = ctr.PointToScreen(end);

            ControlPaint.FillReversibleRectangle(
                new Rectangle(Math.Min(start.X, end.X), clip.Y, Math.Abs(end.X - start.X), clip.Height),
                Color.White);
        }


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
        /// Print the chart as currently shown by the control
        /// </summary>
        /// <param name="preview">If true, show print preview window.</param>
        public void Print(bool preview)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(NPlot_PrintPage);
            printDocument.DefaultPageSettings.Landscape = true;

            DialogResult result;
            if (!preview)
            {
                PrintDialog dlg = new PrintDialog();
                dlg.Document = printDocument;
                result = dlg.ShowDialog();
            }
            else
            {
                PrintPreviewDialog dlg = new PrintPreviewDialog();
                dlg.Document = printDocument;
                result = dlg.ShowDialog();
            }
            if (result == DialogResult.OK)
            {
                try
                {
                    printDocument.Print();
                }
                catch
                {
                    Console.WriteLine("caught\n");
                }
            }
        }


        private void NPlot_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Rectangle r = ev.MarginBounds;
            this.Draw(ev.Graphics, r);
            ev.HasMorePages = false;
        }

        /// <summary>
        /// Coppies the chart currently shown in the control to the clipboard as an image.
        /// </summary>
        public void CopyToClipboard()
        {
            System.Drawing.Bitmap b = new System.Drawing.Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            this.Draw(g, new Rectangle(0, 0, b.Width - 1, b.Height - 1));
            Clipboard.SetDataObject(b, true);
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
        [
            Browsable(false),
            Bindable(false)
        ]
        public List<IDrawable> Drawables => ps_.Drawables;

        /// <summary>
        /// Allows access to the PlotSurface2D.
        /// </summary>
        [
            Browsable(false),
            Bindable(false)
        ]
        public NPlot.PlotSurface2D Inner => ps_;


        /// <summary>
        /// Remembers the current axes - useful in interactions.
        /// </summary>
        public void CacheAxes()
        {
            if (xAxis1ZoomCache_ is null && xAxis2ZoomCache_ is null &&
                yAxis1ZoomCache_ is null && yAxis2ZoomCache_ is null)
            {
                if (this.XAxis1 != null)
                {
                    xAxis1ZoomCache_ = (DateTimeAxis)this.XAxis1.Clone();
                }
                if (this.XAxis2 != null)
                {
                    xAxis2ZoomCache_ = (DateTimeAxis)this.XAxis2.Clone();
                }
                if (this.YAxis1 != null)
                {
                    yAxis1ZoomCache_ = (LinearAxis)this.YAxis1.Clone();
                }
                if (this.YAxis2 != null)
                {
                    yAxis2ZoomCache_ = (LinearAxis)this.YAxis2.Clone();
                }
            }
        }

        private readonly List<Interactions.Interaction> interactions_ = new List<Interactions.Interaction>();


        /// <summary>
        /// Adds and interaction to the plotsurface that adds functionality that responds 
        /// to a set of mouse / keyboard events. 
        /// </summary>
        /// <param name="i">the interaction to add.</param>
        public void AddInteraction(Interactions.Interaction i)
        {
            interactions_.Add(i);
        }


        /// <summary>
        /// Remove a previously added interaction
        /// </summary>
        /// <param name="i">interaction to remove</param>
        public void RemoveInteraction(Interactions.Interaction i)
        {
            interactions_.Remove(i);
        }


        /// <summary>
        /// This is the signature of the function used for InteractionOccurred events.
        /// 
        /// TODO: expand this to include information about the event. 
        /// </summary>
        /// <param name="sender"></param>
        public delegate void InteractionHandler(object sender);


        /// <summary>
        /// Event is fired when an interaction happens with the plot that causes it to be modified.
        /// </summary>
        public event InteractionHandler InteractionOccured;

        /// <summary>
        /// Default function called when plotsurface modifying interaction occured. 
        /// 
        /// Override this, or add method to InteractionOccured event.
        /// </summary>
        /// <param name="sender"></param>
        protected void OnInteractionOccured(object sender)
        {
            // do nothing.
        }

        /// <summary>
        /// This is the signature of the function used for PreRefresh events.
        /// </summary>
        /// <param name="sender"></param>
        public delegate void PreRefreshHandler(object sender);


        /// <summary>
        /// Event fired when we are about to paint.
        /// </summary>
        public event PreRefreshHandler PreRefresh;


        /// <summary>
        /// Default function called just before a refresh happens.
        /// </summary>
        /// <param name="sender"></param>
        protected void OnPreRefresh(object sender)
        {
            // do nothing.
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        private IContainer components;
    }
}