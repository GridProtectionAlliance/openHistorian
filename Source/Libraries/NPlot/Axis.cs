/*
 * NPlot - A charting library for .NET
 * 
 * Axis.cs
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

using System.Drawing.Drawing2D;
using System.Drawing;
using System;
using System.Collections;

namespace NPlot
{

	/// <summary>
	/// Encapsulates functionality common to all axis classes. All specific
	/// axis classes derive from Axis. Axis can be used as a concrete class
	/// itself - it is an Axis without any embilishments [tick marks or tick
	/// mark labels].<br></br><br></br>
	/// This class encapsulates no physical information about where the axes
	/// are drawn. 
	/// </summary>
	public class Axis : System.ICloneable
	{
	
		/// <summary>
		/// If true, tick marks will cross the axis, with their centre on the axis line.
		/// If false, tick marks will be drawn as a line with origin starting on the axis line.
		/// </summary>
		public bool TicksCrossAxis
		{
			get
			{
				return ticksCrossAxis_;
			}
			set
			{
				ticksCrossAxis_ = value;
			}
		}
		bool ticksCrossAxis_ = false;


		/// <summary>
		/// The maximum world extent of the axis. Note that it is sensical if 
		/// WorldMax is less than WorldMin - the axis would just be descending
		/// not ascending. Currently Axes won't display properly if you do 
		/// this - use the Axis.Reversed property instead to achieve the same
		/// result.
		/// 
        /// Setting this raises the WorldMinChanged event and the WorldExtentsChanged event.
        /// </summary>
		public virtual double WorldMax
		{
			get
			{
				return worldMax_;
			}
			set
			{
				this.worldMax_ = value;
                /*
                if (this.WorldExtentsChanged != null)
                    this.WorldExtentsChanged(this, new WorldValueChangedArgs(worldMax_, WorldValueChangedArgs.MinMaxType.Max));
                if (this.WorldMaxChanged != null)
                    this.WorldMaxChanged(this, new WorldValueChangedArgs(worldMax_, WorldValueChangedArgs.MinMaxType.Max));
                */
            }
		}
		private double worldMax_;
		

		/// <summary>
		/// The minumum world extent of the axis. Note that it is sensical if 
		/// WorldMax is less than WorldMin - the axis would just be descending
		/// not ascending. Currently Axes won't display properly if you do 
		/// this - use the Axis.Reversed property instead to achieve the same
		/// result.
		/// 
		/// Setting this raises the WorldMinChanged event and the WorldExtentsChanged event.
		/// </summary>
		public virtual double WorldMin
		{
			get
			{
				return this.worldMin_;
			}
			set
			{
				this.worldMin_ = value;
                /*
                if (this.WorldExtentsChanged != null)
                    this.WorldExtentsChanged( this, new WorldValueChangedArgs( worldMin_, WorldValueChangedArgs.MinMaxType.Min) );
                if (this.WorldMinChanged != null)
                    this.WorldMinChanged( this, new WorldValueChangedArgs(worldMin_, WorldValueChangedArgs.MinMaxType.Min) );
                */
            }
		}
		private double worldMin_;


		/// <summary>
		/// Length (in pixels) of a large tick. <b>Not</b> the distance 
		/// between large ticks. The length of the tick itself.
		/// </summary>
		public int LargeTickSize
		{
			get
			{
				return largeTickSize_;
			}
			set
			{
				largeTickSize_ = value;
			}
		}
		private int largeTickSize_;


		/// <summary>
		/// Length (in pixels) of the small ticks.
		/// </summary>
		public int SmallTickSize
		{
			get
			{
				return smallTickSize_;
			}
			set
			{
				smallTickSize_ = value;
			}
		}
		private int smallTickSize_;


		/// <summary>
		/// The Axis Label
		/// </summary>
		public string Label
		{
			get
			{
				return label_;
			}
			set
			{
				label_ = value;
			}
		}
		private string label_;


		/// <summary>
		/// If true, text associated with tick marks will be drawn on the other side of the
		/// axis line [next to the axis]. If false, tick mark text will be drawn at the end
		/// of the tick mark [on the same of the axis line as the tick].
		/// </summary>
		public bool TickTextNextToAxis
		{
			get
			{
				return tickTextNextToAxis_;
			}
			set
			{
				tickTextNextToAxis_ = value;
			}
		}
		bool tickTextNextToAxis_;


		/// <summary>
		/// If set to true, the axis is hidden. That is, the axis line, ticks, tick 
		/// labels and axis label will not be drawn. 
		/// </summary>
		public bool Hidden
		{
			get
			{
				return hidden_;
			}
			set
			{
				hidden_ = value;
			}
		}
		private bool hidden_;


		/// <summary>
		/// If set true, the axis will behave as though the WorldMin and WorldMax values
		/// have been swapped.
		/// </summary>
		public bool Reversed
		{
			get
			{
				return reversed_;
			}
			set
			{
				reversed_ = value;
			}
		}
		private bool reversed_;


		/// <summary>
		/// If true, no text will be drawn next to any axis tick marks.
		/// </summary>
		public bool HideTickText
		{
			get
			{
				return hideTickText_;
			}
			set
			{
				hideTickText_ = value;
			}
		}
		private bool hideTickText_;


		/// <summary>
		/// This font is used for the drawing of text next to the axis tick marks.
		/// </summary>
		public Font TickTextFont
		{
			get
			{
				return this.tickTextFont_;
			}
			set
			{
				this.tickTextFont_ = value;
				UpdateScale();
			}
		}
		private Font tickTextFont_;
		private Font tickTextFontScaled_;


		/// <summary>
		/// This font is used to draw the axis label.
		/// </summary>
		public Font LabelFont
		{
			get
			{
				return labelFont_;
			}
			set
			{
				labelFont_ = value;
				UpdateScale();
			}
		}
		private Font labelFont_;
		private Font labelFontScaled_;


		/// <summary>
		/// Specifies the format used for drawing tick labels. See 
		/// StringBuilder.AppendFormat for a description of this 
		/// string.
		/// </summary>
		public string NumberFormat
		{
			get
			{
				return numberFormat_;
			}
			set
			{
				numberFormat_ = value;
			}
		}
		private string numberFormat_;


		/// <summary>
		/// If LargeTickStep isn't specified, then this will be calculated 
		/// automatically. The calculated value will not be less than this
		/// amount.
		/// </summary>
		public int MinPhysicalLargeTickStep
		{
			get
			{
				return minPhysicalLargeTickStep_;
			}
			set
			{
				minPhysicalLargeTickStep_ = value;
			}
		}
		private int minPhysicalLargeTickStep_ = 30;


		/// <summary>
		/// The color of the pen used to draw the ticks and the axis line.
		/// </summary>
		public System.Drawing.Color AxisColor
		{
			get
			{
				return linePen_.Color;
			}
			set
			{
				linePen_ = new Pen( (Color)value );
			}
		}


		/// <summary>
		/// The pen used to draw the ticks and the axis line.
		/// </summary>
		public System.Drawing.Pen AxisPen
		{
			get
			{
				return linePen_;
			}
			set
			{
				linePen_ = value;
			}
		}
		private System.Drawing.Pen linePen_;


		/// <summary>
		/// If true, automated tick placement will be independent of the physical
		/// extent of the axis. Tick placement will look good for charts of typical
		/// size (say physical dimensions of 640x480). If you want to produce the
		/// same chart on two graphics surfaces of different sizes [eg Windows.Forms
		/// control and printer], then you will want to set this property to true.
		/// If false [default], the number of ticks and their placement will be 
		/// optimally calculated to look the best for the given axis extent. This 
		/// is very useful if you are creating a cart with particularly small or
		/// large physical dimensions.
		/// </summary>
		public bool TicksIndependentOfPhysicalExtent
		{
			get
			{
				return ticksIndependentOfPhysicalExtent_;
			}
			set
			{
				ticksIndependentOfPhysicalExtent_ = value;
			}
		}
		private bool ticksIndependentOfPhysicalExtent_ = false;


		/// <summary>
		/// If true label is flipped about the text center line parallel to the text.
		/// </summary>
		public bool FlipTicksLabel
		{
			get 
			{
				return flipTicksLabel_; 
			}
			set 
			{ 
				flipTicksLabel_ = value; 
			}
		}
 		private bool flipTicksLabel_ = false;


		/// <summary>
		/// Angle to draw ticks at (measured anti-clockwise from axis direction).
		/// </summary>
		public float TicksAngle
		{
			get
			{
				return ticksAngle_;
			}
			set
			{
				ticksAngle_ = value;
			}
		}
		private float ticksAngle_ = (float)Math.PI / 2.0f;


		/// <summary>
		/// Angle to draw large tick labels at (clockwise from horizontal). Note: 
		/// this is currently only implemented well for the lower x-axis. 
		/// </summary>
		public float TicksLabelAngle
		{
			get
			{
				return ticksLabelAngle_;
			}
			set
			{
				ticksLabelAngle_ = value;
			}
		}
		private float ticksLabelAngle_ = 0.0f;


		/// <summary>
		/// The color of the brush used to draw the axis label.
		/// </summary>
		public Color LabelColor
		{
			set
			{
				labelBrush_ = new SolidBrush( value );
			}
		}
		
		
		/// <summary>
		/// The brush used to draw the axis label.
		/// </summary>
		public Brush LabelBrush
		{
			get
			{
				return labelBrush_;
			}
			set
			{
				labelBrush_ = value;
			}
		}
		private Brush labelBrush_;

		
		/// <summary>
		/// The color of the brush used to draw the axis tick labels.
		/// </summary>
		public Color TickTextColor
		{
			set
			{
				tickTextBrush_ = new SolidBrush( value );
			}
		}


		/// <summary>
		/// The brush used to draw the tick text.
		/// </summary>
		public Brush TickTextBrush
		{
			get
			{
				return tickTextBrush_;
			}
			set
			{
				tickTextBrush_ = value;
			}
		}
		private Brush tickTextBrush_;


		/// <summary>
		/// If true, label and tick text will be scaled to match size
		/// of PlotSurface2D. If false, they won't be.
		/// </summary>
		/// <remarks>Could also be argued this belongs in PlotSurface2D</remarks>
		public bool AutoScaleText
		{
			get
			{
				return autoScaleText_;
			}
			set
			{
				autoScaleText_ = value;
			}
		}
		private bool autoScaleText_;


		/// <summary>
		/// If true, tick lengths will be scaled to match size
		/// of PlotSurface2D. If false, they won't be.
		/// </summary>
		/// <remarks>Could also be argued this belongs in PlotSurface2D</remarks>
		public bool AutoScaleTicks
		{
			get
			{
				return autoScaleTicks_;
			}
			set
			{
				autoScaleTicks_ = value;
			}
		}
		private bool autoScaleTicks_;


		/// <summary>
		/// Deep copy of Axis.
		/// </summary>
		/// <remarks>
		/// This method includes a check that guards against derived classes forgetting
		/// to implement their own Clone method. If Clone is called on a object derived
		/// from Axis, and the Clone method hasn't been overridden by that object, then
		/// the test this.GetType == typeof(Axis) will fail.
		/// </remarks>
		/// <returns>A copy of the Axis Class</returns>
		public virtual object Clone()
		{
			// ensure that this isn't being called on a derived type. If that is the case
			// then the derived type didn't override this method as it should have.
			if (this.GetType() != typeof(Axis))
			{
				throw new NPlotException( "Clone not defined in derived type." );
			}
			
			Axis a = new Axis();
			DoClone( this, a );
			return a;
		}


		/// <summary>
		/// Helper method for Clone. Does all the copying - can be called by derived
		/// types so they don't need to implement this part of the copying themselves.
		/// also useful in constructor of derived types that takes Axis class.
		/// </summary>
		protected static void DoClone( Axis b, Axis a )
		{

			// value items
			a.autoScaleText_ = b.autoScaleText_;
			a.autoScaleTicks_ = b.autoScaleTicks_;
			a.worldMax_ = b.worldMax_;
			a.worldMin_ = b.worldMin_;
			a.tickTextNextToAxis_ = b.tickTextNextToAxis_;
			a.hidden_ = b.hidden_;
			a.hideTickText_ = b.hideTickText_;
			a.reversed_ = b.reversed_;
			a.ticksAngle_ = b.ticksAngle_;
			a.ticksLabelAngle_ = b.ticksLabelAngle_;
			a.minPhysicalLargeTickStep_ = b.minPhysicalLargeTickStep_;
			a.ticksIndependentOfPhysicalExtent_ = b.ticksIndependentOfPhysicalExtent_;
			a.largeTickSize_ = b.largeTickSize_;
			a.smallTickSize_ = b.smallTickSize_;
			a.ticksCrossAxis_ = b.ticksCrossAxis_;
			a.labelOffset_ = b.labelOffset_;
            a.labelOffsetAbsolute_ = b.labelOffsetAbsolute_;
            a.labelOffsetScaled_ = b.labelOffsetScaled_;

			// reference items.
			a.tickTextFont_ = (Font)b.tickTextFont_.Clone();
			a.label_ = (string)b.label_.Clone();
			if (b.numberFormat_ != null) 
			{
				a.numberFormat_ = (string)b.numberFormat_.Clone();
			}
			else
			{
				a.numberFormat_ = null;
			}

			a.labelFont_ = (Font)b.labelFont_.Clone();
			a.linePen_ = (Pen)b.linePen_.Clone();
			a.tickTextBrush_ = (Brush)b.tickTextBrush_.Clone();
			a.labelBrush_ = (Brush)b.labelBrush_.Clone();

			a.FontScale = b.FontScale;
			a.TickScale = b.TickScale;

		}



		/// <summary>
		/// Helper function for constructors.
		/// Do initialization here so that Clear() method is handled properly
		/// </summary>
		private void Init()
		{
			this.worldMax_ = double.NaN;
			this.worldMin_ = double.NaN;
			this.Hidden = false;
			this.SmallTickSize = 2;
			this.LargeTickSize = 6;
			this.FontScale = 1.0f;
			this.TickScale = 1.0f;
			this.AutoScaleTicks = false;
			this.AutoScaleText = false;
			this.TickTextNextToAxis = true;
			this.HideTickText = false;
			this.TicksCrossAxis = false;
			this.LabelOffset = 0.0f;
            this.LabelOffsetAbsolute = false;
            this.LabelOffsetScaled = true;

			this.Label = "" ;
			this.NumberFormat = null;
			this.Reversed = false;

			FontFamily fontFamily = new FontFamily( "Arial" );
			this.TickTextFont = new Font( fontFamily, 10, FontStyle.Regular, GraphicsUnit.Pixel );
			this.LabelFont = new Font( fontFamily, 12, FontStyle.Regular, GraphicsUnit.Pixel );
			this.LabelColor = System.Drawing.Color.Black;
			this.TickTextColor = System.Drawing.Color.Black;
			this.linePen_ = new Pen( System.Drawing.Color.Black );
			this.linePen_.Width = 1.0f;
			this.FontScale = 1.0f;

			// saves constructing these in draw method.
			drawFormat_ = new StringFormat();
			drawFormat_.Alignment = StringAlignment.Center;
		}

		StringFormat drawFormat_;


		/// <summary>
		/// Default constructor
		/// </summary>
		public Axis( )
		{
			this.Init();
		}


		/// <summary>
		/// Constructor that takes only world min and max values.
		/// </summary>
		/// <param name="worldMin">The minimum world coordinate.</param>
		/// <param name="worldMax">The maximum world coordinate.</param>
		public Axis( double worldMin, double worldMax )
		{
			this.Init();
			this.WorldMin = worldMin;
			this.WorldMax = worldMax;
		}


		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="a">The Axis to clone.</param>
		public Axis( Axis a )
		{
			Axis.DoClone( a, this );
		}


		/// <summary>
		/// Determines whether a world value is outside range WorldMin -> WorldMax
		/// </summary>
		/// <param name="coord">the world value to test</param>
		/// <returns>true if outside limits, false otherwise</returns>
		public bool OutOfRange( double coord )
		{
			if (double.IsNaN(WorldMin) || double.IsNaN(WorldMax))
			{
				throw new NPlotException( "world min / max not set" );
			}

			if (coord > this.WorldMax || coord < this.WorldMin)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Sets the world extent of the current axis to be just large enough
		/// to encompas the current world extent of the axis, and the world
		/// extent of the passed in axis
		/// </summary>
		/// <param name="a">The other Axis instance.</param>
		public void LUB( Axis a )
		{
			if (a == null)
			{
				return;
			}

			// mins
			if (!double.IsNaN(a.worldMin_))
			{
				if (double.IsNaN(worldMin_))
				{
					WorldMin = a.WorldMin;
				}
				else
				{
					if (a.WorldMin < WorldMin)
					{
						WorldMin = a.WorldMin;
					}
				}
			}

			// maxs.
			if (!double.IsNaN(a.worldMax_))
			{
				if (double.IsNaN(worldMax_))
				{
					WorldMax = a.WorldMax;
				}
				else
				{
					if (a.WorldMax > WorldMax)
					{
						WorldMax = a.WorldMax;
					}
				}
			}
		}


		/// <summary>
		/// World to physical coordinate transform.
		/// </summary>
		/// <param name="coord">The coordinate value to transform.</param>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="clip">if false, then physical value may extend outside worldMin / worldMax. If true, the physical value returned will be clipped to physicalMin or physicalMax if it lies outside this range.</param>
		/// <returns>The transformed coordinates.</returns>
		/// <remarks>Not sure how much time is spent in this often called function. If it's lots, then
		/// worth optimizing (there is scope to do so).</remarks>
		public virtual PointF WorldToPhysical( 
			double coord, 
			PointF physicalMin, 
			PointF physicalMax, 
			bool clip )
		{

			// (1) account for reversed axis. Could be tricky and move
			// this out, but would be a little messy.

			PointF _physicalMin;
			PointF _physicalMax;

			if ( this.Reversed )
			{
				_physicalMin = physicalMax;
				_physicalMax = physicalMin;
			}
			else
			{
				_physicalMin = physicalMin;
				_physicalMax = physicalMax;
			}


			// (2) if want clipped value, return extrema if outside range.
			
			if ( clip )
			{
				if ( WorldMin < WorldMax )
				{
					if ( coord > WorldMax )
					{
						return _physicalMax;
					}
					if ( coord < WorldMin )
					{
						return _physicalMin;
					}
				}
				else
				{
					if ( coord < WorldMax )
					{
						return _physicalMax;
					}
					if ( coord > WorldMin )
					{
						return _physicalMin;
					}
				}
			}


			// (3) we are inside range or don't want to clip.

			double range = WorldMax - WorldMin;
			double prop = (double)((coord - WorldMin) / range);

			// Force clipping at bounding box largeClip times that of real bounding box 
			// anyway. This is effectively at infinity.
			const double largeClip = 100.0;
			if (prop > largeClip && clip)
				prop = largeClip;

			if (prop < -largeClip && clip)
				prop = -largeClip;

			if (range == 0)
			{
				if (coord >= WorldMin)
					prop = largeClip;

				if (coord < WorldMin)
					prop = -largeClip;
			}

			// calculate the physical coordinate.
			PointF offset = new PointF( 
				(float)(prop * (_physicalMax.X - _physicalMin.X)),
				(float)(prop * (_physicalMax.Y - _physicalMin.Y)) );

			return new PointF( (_physicalMin.X + offset.X), (_physicalMin.Y + offset.Y) );
		}


		/// <summary>
		/// Return the world coordinate of the projection of the point p onto
		/// the axis.
		/// </summary>
		/// <param name="p">The point to project onto the axis</param>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="clip">If true, the world value will be clipped to WorldMin or WorldMax as appropriate if it lies outside this range.</param>
		/// <returns>The world value corresponding to the projection of the point p onto the axis.</returns>
		public virtual double PhysicalToWorld( 
			PointF p, 
			PointF physicalMin, 
			PointF physicalMax,
			bool clip )
		{

			// (1) account for reversed axis. Could be tricky and move
			// this out, but would be a little messy.

			PointF _physicalMin;
			PointF _physicalMax;

			if ( this.Reversed )
			{
				_physicalMin = physicalMax;
				_physicalMax = physicalMin;
			}
			else
			{
				_physicalMin = physicalMin;
				_physicalMax = physicalMax;
			}

			// normalised axis dir vector
			float axis_X = _physicalMax.X - _physicalMin.X;
			float axis_Y = _physicalMax.Y - _physicalMin.Y;
			float len = (float)Math.Sqrt( axis_X * axis_X + axis_Y * axis_Y );
			axis_X /= len;
			axis_Y /= len;

			// point relative to axis physical minimum.
			PointF posRel = new PointF( p.X - _physicalMin.X, p.Y - _physicalMin.Y );

			// dist of point projection on axis, normalised.
			float prop = ( axis_X * posRel.X + axis_Y * posRel.Y ) / len;

			double world = prop * (this.WorldMax - this.WorldMin) + this.WorldMin;

			// if want clipped value, return extrema if outside range.
			if (clip)
			{
				world = Math.Max( world, this.WorldMin );
				world = Math.Min( world, this.WorldMax );
			}

			return world;
		}


		/// <summary>
		/// Draw the Axis Label
		/// </summary>
		/// <param name="g">The GDI+ drawing surface on which to draw.</param>
		/// <param name="offset">offset from axis. Should be calculated so as to make sure axis label misses tick labels.</param>
		/// <param name="axisPhysicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="axisPhysicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <returns>boxed Rectangle indicating bounding box of label. null if no label printed.</returns>
		public object DrawLabel( 
			Graphics g, 
			Point offset, 
			Point axisPhysicalMin, 
			Point axisPhysicalMax )
		{

			if ( Label != "" )
			{
	
				// first calculate any extra offset for axis label spacing.
				float extraOffsetAmount = this.LabelOffset;
				extraOffsetAmount += 2.0f; // empirically determed - text was too close to axis before this.
				
				if (this.AutoScaleText)
				{
					if (this.LabelOffsetScaled)
					{
						extraOffsetAmount *= this.FontScale;
					}
				}

				// now extend offset.
				float offsetLength = (float)Math.Sqrt( offset.X*offset.X + offset.Y*offset.Y );
				if (offsetLength > 0.01)
				{
					float x_component = offset.X / offsetLength;
					float y_component = offset.Y / offsetLength;

					x_component *= extraOffsetAmount;
					y_component *= extraOffsetAmount;

                    if (this.LabelOffsetAbsolute)
                    {
                        offset.X = (int)x_component;
                        offset.Y = (int)y_component;
                    }
                    else
                    {
                        offset.X += (int)x_component;
                        offset.Y += (int)y_component;
                    }
                }
				
				// determine angle of axis in degrees
				double theta = Math.Atan2(
					axisPhysicalMax.Y - axisPhysicalMin.Y,
					axisPhysicalMax.X - axisPhysicalMin.X );
				theta = theta * 180.0f / Math.PI;

				PointF average = new PointF(
					(axisPhysicalMax.X + axisPhysicalMin.X)/2.0f,
					(axisPhysicalMax.Y + axisPhysicalMin.Y)/2.0f );

				g.TranslateTransform( offset.X , offset.Y );	// this is done last.
				g.TranslateTransform( average.X, average.Y );
				g.RotateTransform( (float)theta );				// this is done first.

				SizeF labelSize = g.MeasureString( Label, labelFontScaled_);

				//bounding box for label centered around zero.
				RectangleF drawRect = new RectangleF( 
					-labelSize.Width/2.0f,
					-labelSize.Height/2.0f,
					labelSize.Width,
					labelSize.Height );
				
				g.DrawString( 
					Label, 
					labelFontScaled_,
					labelBrush_, 
					drawRect,
					drawFormat_ );

				// now work out physical bounds of label. 
				Matrix m = g.Transform;
				PointF[] recPoints = new PointF[2];
				recPoints[0] = new PointF( -labelSize.Width/2.0f, -labelSize.Height/2.0f );
				recPoints[1] = new PointF( labelSize.Width/2.0f, labelSize.Height/2.0f );
				m.TransformPoints( recPoints );

				int x1 = (int)Math.Min( recPoints[0].X, recPoints[1].X );
				int x2 = (int)Math.Max( recPoints[0].X, recPoints[1].X );
				int y1 = (int)Math.Min( recPoints[0].Y, recPoints[1].Y );
				int y2 = (int)Math.Max( recPoints[0].Y, recPoints[1].Y );

				g.ResetTransform();

				// and return label bounding box.
				return new Rectangle( x1, y1, (x2-x1), (y2-y1) );
			}

			return null;
		}


		/// <summary>
		/// Draw a tick on the axis.
		/// </summary>
		/// <param name="g">The graphics surface on which to draw.</param>
		/// <param name="w">The tick position in world coordinates.</param>
		/// <param name="size">The size of the tick (in pixels)</param>
		/// <param name="text">The text associated with the tick</param>
		/// <param name="textOffset">The Offset to draw from the auto calculated position</param>
		/// <param name="axisPhysMin">The minimum physical extent of the axis</param>
		/// <param name="axisPhysMax">The maximum physical extent of the axis</param>
		/// <param name="boundingBox">out: The bounding rectangle for the tick and tickLabel drawn</param>
		/// <param name="labelOffset">out: offset from the axies required for axis label</param>
		public virtual void DrawTick( 
			Graphics g, 
			double w,
			float size,
			string text,
			Point textOffset,
			Point axisPhysMin,
			Point axisPhysMax,
			out Point labelOffset,
			out Rectangle boundingBox )
		{

			// determine physical location where tick touches axis. 
			PointF tickStart = WorldToPhysical( w, axisPhysMin, axisPhysMax, true );

			// determine offset from start point.
			PointF axisDir = Utils.UnitVector( axisPhysMin, axisPhysMax );

			// rotate axis dir clockwise by angle radians to get tick direction.
			float x1 = (float)(Math.Cos( -this.TicksAngle ) * axisDir.X + Math.Sin( -this.TicksAngle ) * axisDir.Y);
			float y1 = (float)(-Math.Sin( -this.TicksAngle ) * axisDir.X + Math.Cos( -this.TicksAngle ) * axisDir.Y);

			// now get the scaled tick vector.
			PointF tickVector = new PointF( this.TickScale * size * x1, this.TickScale * size * y1 );

			if (this.TicksCrossAxis)
			{
				tickStart = new PointF(
					tickStart.X - tickVector.X / 2.0f,
					tickStart.Y - tickVector.Y / 2.0f );
			}

			// and the end point [point off axis] of tick mark.
			PointF tickEnd = new PointF( tickStart.X + tickVector.X, tickStart.Y + tickVector.Y );

			// and draw it!
			if (g != null)
				g.DrawLine( this.linePen_, (int)tickStart.X, (int)tickStart.Y, (int)tickEnd.X, (int)tickEnd.Y );
			// note: casting to int for tick positions was necessary to ensure ticks drawn where we wanted
			// them. Not sure of the reason.

			// calculate bounds of tick.
			int minX = (int)Math.Min( tickStart.X, tickEnd.X );
			int minY = (int)Math.Min( tickStart.Y, tickEnd.Y );
			int maxX = (int)Math.Max( tickStart.X, tickEnd.X );
			int maxY = (int)Math.Max( tickStart.Y, tickEnd.Y );
			boundingBox = new Rectangle( minX, minY, maxX-minX, maxY-minY );
			
			// by default, label offset from axis is 0. TODO: revise this.
			labelOffset = new Point( 
				-(int)tickVector.X, 
				-(int)tickVector.Y );

			// ------------------------

			// now draw associated text.

			// **** TODO ****
			// The following code needs revising. A few things are hard coded when
			// they should not be. Also, angled tick text currently just works for
			// the bottom x-axis. Also, it's a bit hacky.

			if (text != "" && !HideTickText && g != null )
			{
				SizeF textSize = g.MeasureString( text, tickTextFontScaled_ );

				// determine the center point of the tick text.
				float textCenterX;
				float textCenterY;

				// if text is at pointy end of tick.
				if (!this.TickTextNextToAxis)
				{
					// offset due to tick.
					textCenterX = tickStart.X + tickVector.X*1.2f;
					textCenterY = tickStart.Y + tickVector.Y*1.2f;

					// offset due to text box size.
					textCenterX += 0.5f * x1 * textSize.Width;
					textCenterY += 0.5f * y1 * textSize.Height;
				}
					// else it's next to the axis.
				else
				{
					// start location.
					textCenterX = tickStart.X;
					textCenterY = tickStart.Y;

					// offset due to text box size.
					textCenterX -= 0.5f * x1 * textSize.Width;
					textCenterY -= 0.5f * y1 * textSize.Height;

					// bring text away from the axis a little bit.
					textCenterX -= x1*(2.0f+FontScale);
					textCenterY -= y1*(2.0f+FontScale);
				}

				// If tick text is angled.. 
				if (this.TicksLabelAngle != 0.0f)
				{

					// determine the point we want to rotate text about.
					
					PointF textScaledTickVector = new PointF( this.TickScale * x1 * (textSize.Height/2.0f), this.TickScale * y1 * (textSize.Height/2.0f) );

 					PointF rotatePoint;
 					if (this.TickTextNextToAxis) 
					{
 						rotatePoint = new PointF( tickStart.X - textScaledTickVector.X, tickStart.Y - textScaledTickVector.Y );
 					}
 					else 
					{
 						rotatePoint = new PointF( tickEnd.X + textScaledTickVector.X, tickEnd.Y + textScaledTickVector.Y );
 					}
 
 					float actualAngle;
					if (flipTicksLabel_) 
					{
 						double radAngle = (Math.PI / 180) * this.TicksLabelAngle;
 						rotatePoint.X += textSize.Width * (float)Math.Cos(radAngle);
 						rotatePoint.Y += textSize.Width * (float)Math.Sin(radAngle);
 						actualAngle = this.TicksLabelAngle + 180;
 					}
 					else 
					{
 						actualAngle = this.TicksLabelAngle;
					}
					

					g.TranslateTransform( rotatePoint.X, rotatePoint.Y );

					g.RotateTransform( actualAngle );
					
					Matrix m = g.Transform;
					PointF[] recPoints = new PointF[2];
					recPoints[0] = new PointF( 0.0f, -(textSize.Height / 2) );
					recPoints[1] = new PointF( textSize.Width, textSize.Height );
					m.TransformPoints( recPoints );

					float t_x1 = Math.Min( recPoints[0].X, recPoints[1].X );
					float t_x2 = Math.Max( recPoints[0].X, recPoints[1].X );
					float t_y1 = Math.Min( recPoints[0].Y, recPoints[1].Y );
					float t_y2 = Math.Max( recPoints[0].Y, recPoints[1].Y );
					
					boundingBox = Rectangle.Union(boundingBox, new Rectangle( (int)t_x1, (int)t_y1, (int)(t_x2-t_x1), (int)(t_y2-t_y1) ) );
					RectangleF drawRect = new RectangleF( 0.0f, -(textSize.Height / 2), textSize.Width, textSize.Height );

					g.DrawString( 
						text,
						tickTextFontScaled_,
						tickTextBrush_, 
						drawRect,
						drawFormat_ );

					t_x2 -= tickStart.X;
					t_y2 -= tickStart.Y;
					t_x2 *= 1.25f;
					t_y2 *= 1.25f;

					labelOffset = new Point( (int)t_x2, (int)t_y2 );

					g.ResetTransform();

					//g.DrawRectangle( new Pen(Color.Purple), boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height );

				}
				else
				{

					float bx1 = (textCenterX - textSize.Width/2.0f);
					float by1 = (textCenterY - textSize.Height/2.0f);
					float bx2 = textSize.Width;
					float by2 = textSize.Height;

					RectangleF drawRect = new RectangleF( bx1, by1, bx2, by2 );
					Rectangle drawRect_int = new Rectangle( (int)bx1, (int)by1, (int)bx2, (int)by2 );
					// g.DrawRectangle( new Pen(Color.Green), bx1, by1, bx2, by2 );

					boundingBox = Rectangle.Union( boundingBox, drawRect_int );

					// g.DrawRectangle( new Pen(Color.Purple), boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height );

					g.DrawString( 
						text,
						tickTextFontScaled_,
						tickTextBrush_,
						drawRect,
						drawFormat_ );

					textCenterX -= tickStart.X;
					textCenterY -= tickStart.Y;
					textCenterX *= 2.3f;
					textCenterY *= 2.3f;

					labelOffset = new Point( (int)textCenterX, (int)textCenterY );
				}
			} 

		}


		/// <summary>
		/// Draw the axis. This involves three steps:
		///  (1) Draw the axis line.
		///  (2) Draw the tick marks.
		///  (3) Draw the label.
		/// </summary>
		/// <param name="g">The drawing surface on which to draw.</param>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="boundingBox">out The bounding rectangle of the axis including axis line, label, tick marks and tick mark labels</param>
		public virtual void Draw( 
			System.Drawing.Graphics g,
			Point physicalMin,
			Point physicalMax, 
			out Rectangle boundingBox )
		{
			// calculate the bounds of the axis line only.
			int x1 = Math.Min( physicalMin.X, physicalMax.X );
			int x2 = Math.Max( physicalMin.X, physicalMax.X );
			int y1 = Math.Min( physicalMin.Y, physicalMax.Y );
			int y2 = Math.Max( physicalMin.Y, physicalMax.Y );
			Rectangle bounds = new Rectangle( x1, y1, x2-x1, y2-y1 );

			if (!Hidden)
			{
				
				// (1) Draw the axis line.
				g.DrawLine( this.linePen_, physicalMin.X, physicalMin.Y, physicalMax.X, physicalMax.Y );

				// (2) draw tick marks (subclass responsibility). 

				object labelOffset;
				object tickBounds;
				this.DrawTicks( g, physicalMin, physicalMax, out labelOffset, out tickBounds );

				// (3) draw the axis label
				object labelBounds = null;
				if (!this.HideTickText)
				{
					labelBounds = this.DrawLabel( g, (Point)labelOffset, physicalMin, physicalMax );
				}

				// (4) merge bounds and return.
				if (labelBounds != null)
					bounds = Rectangle.Union( bounds, (Rectangle)labelBounds );

				if (tickBounds != null)
					bounds = Rectangle.Union( bounds, (Rectangle)tickBounds );

			}

			boundingBox = bounds;
		}


		/// <summary>
		/// Update the bounding box and label offset associated with an axis
		/// to encompass the additionally specified mergeBoundingBox and 
		/// mergeLabelOffset respectively.
		/// </summary>
		/// <param name="labelOffset">Current axis label offset.</param>
		/// <param name="boundingBox">Current axis bounding box.</param>
		/// <param name="mergeLabelOffset">the label offset to merge. The current label offset will be replaced by this if it's norm is larger.</param>
		/// <param name="mergeBoundingBox">the bounding box to merge. The current bounding box will be replaced by this if null, or by the least upper bound of bother bounding boxes otherwise.</param>
		protected static void UpdateOffsetAndBounds( 
			ref object labelOffset, ref object boundingBox, 
			Point mergeLabelOffset, Rectangle mergeBoundingBox )
		{
			// determining largest label offset and use it.
			Point lo = (Point)labelOffset;
			double norm1 = Math.Sqrt( lo.X*lo.X + lo.Y*lo.Y );
			double norm2 = Math.Sqrt( mergeLabelOffset.X*mergeLabelOffset.X + mergeLabelOffset.Y*mergeLabelOffset.Y );
			if (norm1 < norm2)
			{
				labelOffset = mergeLabelOffset;
			}

			// determining bounding box.
			Rectangle b = mergeBoundingBox;
			if (boundingBox == null)
			{
				boundingBox = b;
			}
			else
			{
				boundingBox = Rectangle.Union( (Rectangle)boundingBox, b );
			}
		}


		/// <summary>
		/// DrawTicks method. In base axis class this does nothing.
		/// </summary>
		/// <param name="g">The graphics surface on which to draw</param>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="labelOffset">is set to a suitable offset from the axis to draw the axis label. In this base method, set to null.</param>
		/// <param name="boundingBox">is set to the smallest box that bounds the ticks and the tick text. In this base method, set to null.</param>
		protected virtual void DrawTicks( 
			Graphics g, 
			Point physicalMin, 
			Point physicalMax, 
			out object labelOffset,
			out object boundingBox )
		{
			labelOffset = null;
			boundingBox = null;
			// do nothing. This class is not abstract because a subclass may
			// want to override the Axis.Draw method to one that doesn't 
			// require DrawTicks.
		}



		/// <summary>
		/// World extent of the axis.
		/// </summary>
		public double WorldLength
		{
			get
			{
				return Math.Abs( worldMax_ - worldMin_ );
			}
		}

		/// <summary>
		/// Determines the positions, in world coordinates, of the large ticks. 
		/// When the physical extent of the axis is small, some of the positions 
		/// that were generated in this pass may be converted to small tick 
		/// positions and returned as well.
		/// 
		/// This default implementation returns empty large ticks list and null
		/// small tick list.
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">ArrayList containing the positions of the large ticks.</param>
		/// <param name="smallTickPositions">ArrayList containing the positions of the small ticks if calculated, null otherwise.</param>
		internal virtual void WorldTickPositions_FirstPass(
			Point physicalMin, 
			Point physicalMax,
			out ArrayList largeTickPositions,
			out ArrayList smallTickPositions
			)
		{
			largeTickPositions = new ArrayList();
			smallTickPositions = null;
		}


		/// <summary>
		/// Determines the positions, in world coordinates, of the small ticks
		/// if they have not already been generated.
		/// 
		/// This default implementation creates an empty smallTickPositions list 
		/// if it doesn't already exist.
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">The positions of the large ticks.</param>
		/// <param name="smallTickPositions">If null, small tick positions are returned via this parameter. Otherwise this function does nothing.</param>
		internal virtual void WorldTickPositions_SecondPass( 
			Point physicalMin,
			Point physicalMax,
			ArrayList largeTickPositions, 
			ref ArrayList smallTickPositions )
		{
			if (smallTickPositions == null)
				smallTickPositions = new ArrayList();
		}


		/// <summary>
		/// Determines the positions of all Large and Small ticks.
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">ArrayList containing the positions of the large ticks.</param>
		/// <param name="smallTickPositions">ArrayList containing the positions of the small ticks.</param>
		public void WorldTickPositions(
			Point physicalMin,
			Point physicalMax,
			out ArrayList largeTickPositions,
			out ArrayList smallTickPositions
			)
		{
			WorldTickPositions_FirstPass( physicalMin, physicalMax, out largeTickPositions, out smallTickPositions );
			WorldTickPositions_SecondPass( physicalMin, physicalMax, largeTickPositions, ref smallTickPositions );
		}


		/// <summary>
		/// Moves the world min and max values so that the world axis
		/// length is [percent] bigger. If the current world
		/// max and min values are the same, they are moved appart 
		/// an arbitrary amount. This arbitrary amount is currently
		/// 0.01, and will probably be configurable in the future.
		/// </summary>
		/// <param name="percent">Percentage to increase world length by.</param>
		/// <remarks>Works for the case WorldMax is less than WorldMin.</remarks>
		public void IncreaseRange( double percent )
		{
			double range = WorldMax - WorldMin;
			
			if ( !Utils.DoubleEqual( range, 0.0 ) )
			{
				range *= percent;
			}
			else
			{
				// arbitrary number. 
				// TODO make this configurable.
				range = 0.01;
			}

			WorldMax += range;
			WorldMin -= range;
		}


		/// <summary>
		/// Scale label and tick fonts by this factor. Set by PlotSurface2D 
		/// Draw method.
		/// </summary>
		internal float FontScale 
		{
			get 
			{
				return fontScale_;
			}
 
			set 
			{
				fontScale_ = value;
				UpdateScale();
			}
		}
		private float fontScale_;


		/// <summary>
		/// Scale tick mark lengths by this factor. Set by PlotSurface2D
		/// Draw method.
		/// </summary>		
		internal float TickScale 
		{
			get 
			{
				return tickScale_;
			}
			set 
			{
				tickScale_ = value;
			}
		}
		private float tickScale_;


		private void UpdateScale()	
		{
			if (labelFont_ != null)
				this.labelFontScaled_ = Utils.ScaleFont( labelFont_, FontScale );
			
			if (tickTextFont_ != null)
				this.tickTextFontScaled_ = Utils.ScaleFont( tickTextFont_, FontScale );
		}


		/// <summary>
		/// Get whether or not this axis is linear.
		/// </summary>
		public virtual bool IsLinear
		{
			get
			{
				return true;
			}
		}

		private float labelOffset_ = 0;
		/// <summary>
		/// If LabelOffsetAbsolute is false (default) then this is the offset 
		/// added to default axis label position. If LabelOffsetAbsolute is 
		/// true, then this is the absolute offset of the label from the axis.
		/// 
		/// If positive, offset is further away from axis, if negative, towards
		/// the axis.
		/// </summary>
		public float LabelOffset
		{
			get
			{
				return labelOffset_;
			}
			set
			{
				labelOffset_ = value;
			}
		}

        private bool labelOffsetAbsolute_ = false;
        /// <summary>
        /// If true, the value specified by LabelOffset is the absolute distance
        /// away from the axis that the label is drawn. If false, the value 
        /// specified by LabelOffset is added to the pre-calculated value to 
        /// determine the axis label position.
        /// </summary>
        /// <value></value>
        public bool LabelOffsetAbsolute
        {
            get
            {
                return labelOffsetAbsolute_;
            }
            set
            {
                labelOffsetAbsolute_ = value;
            }
        }

        private bool labelOffsetScaled_ = true;
		/// <summary>
		/// Whether or not the supplied LabelOffset should be scaled by 
		/// a factor as specified by FontScale.
		/// </summary>
		public bool LabelOffsetScaled
		{
			get
			{
				return labelOffsetScaled_;
			}
			set
			{
				labelOffsetScaled_ = value;
			}
		}


		/// <summary>
		/// returns a suitable offset for the axis label in the case that there are no
		/// ticks or tick text in the way.
		/// </summary>
		/// <param name="physicalMin">physical point corresponding to the axis world maximum.</param>
		/// <param name="physicalMax">physical point corresponding to the axis world minimum.</param>
		/// <returns>axis label offset</returns>
		protected Point getDefaultLabelOffset( Point physicalMin, Point physicalMax )
		{
			System.Drawing.Rectangle tBoundingBox;
			System.Drawing.Point tLabelOffset;

			this.DrawTick( null, this.WorldMax, this.LargeTickSize, 
				"",
				new Point(0,0),
				physicalMin, physicalMax,
				out tLabelOffset, out tBoundingBox );

			return tLabelOffset;
		}


        /// <summary>
        /// Set the Axis color (sets all of axis line color, Tick text color, and label color).
        /// </summary>
        public Color Color
        {
            set
            {
                this.AxisColor = value;
                this.TickTextColor = value;
                this.LabelColor = value;
            }
        }


        /*

        // finish implementation of this at some point.

        public class WorldValueChangedArgs
        {
            public WorldValueChangedArgs( double value, MinMaxType minOrMax )
            {
                Value = value;
                MinOrMax = minOrMax;
            }

            public double Value;

            public enum MinMaxType
            {
                Min = 0,
                Max = 1
            }

            public MinMaxType MinOrMax;
        }


        public delegate void WorldValueChangedHandler( object sender, WorldValueChangedArgs e );

        public event WorldValueChangedHandler WorldMinChanged;
        public event WorldValueChangedHandler WorldMaxChanged;
        public event WorldValueChangedHandler WorldExtentsChanged;

        */

    }
}
