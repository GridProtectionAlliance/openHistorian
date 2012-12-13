/*
 * NPlot - A charting library for .NET
 * 
 * LogAxis.cs
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
using System.Collections;
using System.Text;

namespace NPlot
{
	/// <summary>
	/// The class implementing logarithmic axes.
	/// </summary>
	public class LogAxis : Axis
	{

		/// <summary>
		/// Deep Copy of the LogAxis.
		/// </summary>
		/// <returns>A Copy of the LogAxis Class.</returns>
		public override object Clone()
		{
			LogAxis a = new LogAxis();
			if (this.GetType() != a.GetType())
			{
				throw new NPlotException("Clone not defined in derived type. Help!");
			}
			this.DoClone( this, a );
			return a;
		}


		/// <summary>
		/// Helper method for Clone (actual implementation)
		/// </summary>
		/// <param name="a">The original object to clone.</param>
		/// <param name="b">The cloned object.</param>
		protected void DoClone(LogAxis b, LogAxis a)
		{
			Axis.DoClone(b,a);
			// add specific elemtents of the class for the deep copy of the object
			a.numberSmallTicks_ = b.numberSmallTicks_;
			a.largeTickValue_ = b.largeTickValue_;
			a.largeTickStep_ = b.largeTickStep_;
		}


		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogAxis()
			: base()
		{
			Init();
		}


		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="a">The Axis to clone.</param>
		public LogAxis(Axis a)
			: base(a)
		{
			Init();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="worldMin">Minimum World value for the axis.</param>
		/// <param name="worldMax">Maximum World value for the axis.</param>
		public LogAxis(double worldMin, double worldMax)
			: base( worldMin, worldMax )
		{
			Init();
		}


		private void Init() 
		{
			this.NumberFormat = "{0:g5}";
		}


		/// <summary>
		/// Draw the ticks.
		/// </summary>
		/// <param name="g">The drawing surface on which to draw.</param>
		/// <param name="physicalMin">The minimum physical extent of the axis.</param>
		/// <param name="physicalMax">The maximum physical extent of the axis.</param>
		/// <param name="boundingBox">out: smallest box that completely encompasses all of the ticks and tick labels.</param>
		/// <param name="labelOffset">out: a suitable offset from the axis to draw the axis label.</param>
		/// <returns> An ArrayList containing the offset from the axis required for an axis label
		/// to miss this tick, followed by a bounding rectangle for the tick and tickLabel drawn.</returns>
		protected override void DrawTicks(
			Graphics g, 
			Point physicalMin, 
			Point physicalMax, 
			out object labelOffset,
			out object boundingBox )
		{

			Point tLabelOffset;
			Rectangle tBoundingBox;

			labelOffset = this.getDefaultLabelOffset( physicalMin, physicalMax );
			boundingBox = null;

			ArrayList largeTickPositions;
			ArrayList smallTickPositions;
			this.WorldTickPositions( physicalMin, physicalMax, out largeTickPositions, out smallTickPositions );

			Point offset = new Point( 0, 0 );
			object bb = null;
			// Missed this protection
			if (largeTickPositions.Count > 0)
			{
				for (int i=0; i<largeTickPositions.Count; ++i)
				{
					StringBuilder label = new StringBuilder();
					// do google search for "format specifier writeline" for help on this.
					label.AppendFormat(this.NumberFormat, (double)largeTickPositions[i]);
					this.DrawTick( g, (double)largeTickPositions[i], this.LargeTickSize, label.ToString(),
						new Point(0,0), physicalMin, physicalMax, out tLabelOffset, out tBoundingBox );

					Axis.UpdateOffsetAndBounds( ref labelOffset, ref boundingBox, tLabelOffset, tBoundingBox );
				}
			}
			else
			{
				// just get the axis bounding box)
				PointF dir = Utils.UnitVector(physicalMin,physicalMax);
				Rectangle rr = new Rectangle( physicalMin.X,
					(int)((physicalMax.X-physicalMin.X)*dir.X),
					physicalMin.Y,
					(int)((physicalMax.Y-physicalMin.Y)*dir.Y) );
				bb = rr;
			}
			

			// missed protection for zero ticks
			if (smallTickPositions.Count > 0)
			{
				for (int i=0; i<smallTickPositions.Count; ++i)
				{
					this.DrawTick( g, (double)smallTickPositions[i], this.SmallTickSize,
						"", new Point(0,0), physicalMin, physicalMax, out tLabelOffset, out tBoundingBox );
					// ignore r for now - assume bb unchanged by small tick bounds.
				}
			}

		}


		/// <summary>
		/// Determines the positions, in world coordinates, of the small ticks
		/// if they have not already been generated.
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">The positions of the large ticks, unchanged</param>
		/// <param name="smallTickPositions">If null, small tick positions are returned via this parameter. Otherwise this function does nothing.</param>
		internal override void WorldTickPositions_SecondPass( 
			Point physicalMin,
			Point physicalMax,
			ArrayList largeTickPositions, 
			ref ArrayList smallTickPositions )
		{

			if (smallTickPositions != null)
			{
				throw new NPlotException( "not expecting smallTickPositions to be set already." );
			}

			smallTickPositions = new ArrayList();

			// retrieve the spacing of the big ticks. Remember this is decades!
			double bigTickSpacing = this.DetermineTickSpacing();
			int nSmall = this.DetermineNumberSmallTicks( bigTickSpacing );

			// now we have to set the ticks
			// let us start with the easy case where the major tick distance
			// is larger than a decade
			if ( bigTickSpacing > 1.0f )
			{
				if (largeTickPositions.Count > 0)
				{
					// deal with the smallticks preceding the
					// first big tick
					double pos1 = (double)largeTickPositions[0];
					while (pos1 > this.WorldMin)
					{
						pos1 = pos1 / 10.0f;
						smallTickPositions.Add( pos1 );
					}
					// now go on for all other Major ticks
					for (int i=0; i<largeTickPositions.Count; ++i )
					{
						double pos = (double)largeTickPositions[i];
						for (int j=1; j<=nSmall; ++j )
						{
							pos=pos*10.0F;
							// check to see if we are still in the range
							if (pos < WorldMax)
							{
								smallTickPositions.Add( pos );
							}
						}
					}
				}
			}
			else
			{
				// guess what...
				double [] m = { 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f };
				// Then we deal with the other ticks
				if (largeTickPositions.Count > 0)
				{
					// first deal with the smallticks preceding the first big tick
					// positioning before the first tick
					double pos1=(double)largeTickPositions[0]/10.0f;
					for (int i=0; i<m.Length; i++)
					{
						double pos=pos1*m[i];
						if (pos>this.WorldMin)
						{
							smallTickPositions.Add(pos);
						}
					}
					// now go on for all other Major ticks
					for (int i=0; i<largeTickPositions.Count; ++i )
					{
						pos1=(double)largeTickPositions[i];
						for (int j=0; j<m.Length; ++j )
						{
							double pos=pos1*m[j];
							// check to see if we are still in the range
							if (pos < WorldMax)
							{
								smallTickPositions.Add( pos );
							}
						}
					}
				}
				else
				{
					// probably a minor tick would anyway fall in the range
					// find the decade preceding the minimum
					double dec=Math.Floor(Math.Log10(WorldMin));
					double pos1=Math.Pow(10.0,dec);
					for (int i=0; i<m.Length; i++)
					{
						double pos=pos1*m[i];
						if (pos>this.WorldMin && pos< this.WorldMax )
						{
							smallTickPositions.Add(pos);
						}
					}
				}
			}

		}

		private static double m_d5Log = -Math.Log10(0.5);   // .30103
		private static double m_d5RegionPos = Math.Abs(m_d5Log + ((1 - m_d5Log) / 2)); //	   ' .6505
		private static double m_d5RegionNeg = Math.Abs(m_d5Log / 2); //	   '.1505

		private void CalcGrids( double dLenAxis, int nNumDivisions, ref double dDivisionInterval)
		{
			double dMyInterval  = dLenAxis / nNumDivisions;
			double dPower = Math.Log10(dMyInterval);
			dDivisionInterval = 10 ^ (int)dPower;
			double dFixPower = dPower - (int)dPower;
			double d5Region = Math.Abs(dPower - dFixPower);
			double dMyMult;
			if (dPower < 0)
			{
				d5Region = -(dPower - dFixPower);
				dMyMult = 0.5;
			}
			else
			{
				d5Region = 1 - (dPower - dFixPower);
				dMyMult = 5;
			}
			if ((d5Region >= m_d5RegionNeg) && (d5Region <= m_d5RegionPos))
			{
				dDivisionInterval = dDivisionInterval * dMyMult;
			}
		}

		/// <summary>
		/// Determines the positions, in world coordinates, of the log spaced large ticks. 
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">ArrayList containing the positions of the large ticks.</param>
		/// <param name="smallTickPositions">null</param>
		internal override void WorldTickPositions_FirstPass(
			Point physicalMin, 
			Point physicalMax,
			out ArrayList largeTickPositions,
			out ArrayList smallTickPositions
			)
		{

			smallTickPositions = null;
			largeTickPositions = new ArrayList();

			if ( double.IsNaN(WorldMin) || double.IsNaN(WorldMax) )
			{
				throw new NPlotException( "world extent of axis not set." );
			}

			double roundTickDist = this.DetermineTickSpacing( );

			// now determine first tick position.
			double first = 0.0f;

			// if the user hasn't specified a large tick position.
			if (double.IsNaN(largeTickValue_))
			{
				if( WorldMin > 0.0 )
				{

					double nToFirst = Math.Floor(Math.Log10(WorldMin) / roundTickDist)+1.0f;
					first = nToFirst * roundTickDist;
				}

				// could miss one, if first is just below zero.
				if (first-roundTickDist >= Math.Log10(WorldMin))
				{
					first -= roundTickDist;
				}
			}
			
			// the user has specified one place they would like a large tick placed.
			else
			{
				first = Math.Log10( this.LargeTickValue );

				// TODO: check here not too much different.
				// could result in long loop.
				while (first < Math.Log10(WorldMin))
				{
					first += roundTickDist;
				}

				while (first > Math.Log10(WorldMin)+roundTickDist)
				{
					first -= roundTickDist;
				}
			}

			double mark = first;
			while (mark <= Math.Log10(WorldMax))
			{
				// up to here only logs are dealt with, but I want to return
				// a real value in the arraylist
				double val;
				val = Math.Pow( 10.0, mark );
				largeTickPositions.Add( val );
				mark += roundTickDist;
			}

		}


		/// <summary>
		/// Determines the tick spacing.
		/// </summary>
		/// <returns>The tick spacing (in decades!)</returns>
		private double DetermineTickSpacing( )
		{
			if ( double.IsNaN(WorldMin) || double.IsNaN(WorldMax) )
			{
				throw new NPlotException( "world extent of axis is not set." );
			}

			// if largeTickStep has been set, it is used
			if ( !double.IsNaN( this.largeTickStep_) )
			{
				if ( this.largeTickStep_ <= 0.0f )
				{
					throw new NPlotException( "can't have negative tick step - reverse WorldMin WorldMax instead." );
				}

				return this.largeTickStep_;
			}

			double MagRange = (double)(Math.Floor(Math.Log10(WorldMax)) - Math.Floor(Math.Log10(WorldMin))+1.0);

			if ( MagRange > 0.0 )
			{
				// for now, a simple logic
				// start with a major tick every order of magnitude, and
				// increment if in order not to have more than 10 ticks in
				// the plot.
				double roundTickDist=1.0F;
				int nticks=(int)(MagRange/roundTickDist);
				while (nticks > 10)
				{
					roundTickDist++;
					nticks=(int)(MagRange/roundTickDist);
				}
				return roundTickDist;
			}
			else
			{
				return 0.0f;
			}
		}


		/// <summary>
		/// Determines the number of small ticks between two large ticks.
		/// </summary>
		/// <param name="bigTickDist">The distance between two large ticks.</param>
		/// <returns>The number of small ticks.</returns>
		private int DetermineNumberSmallTicks( double bigTickDist )
		{
			// if the big ticks is more than one decade, the
			// small ticks are every decade, I don't let the user set it.
			if (this.numberSmallTicks_ != null && bigTickDist == 1.0f)
			{
				return (int)this.numberSmallTicks_+1;
			}

			// if we are plotting every decade, we have to
			// put the log ticks. As a start, I put every
			// small tick (.2,.3,.4,.5,.6,.7,.8,.9)
			if (bigTickDist == 1.0f)
			{
				return 8;
			}
				// easy, put a tick every missed decade
			else if (bigTickDist > 1.0f)
			{
				return (int)bigTickDist - 1;
			}
			else
			{
				throw new NPlotException("Wrong Major tick distance setting");
			}
		}


		/// <summary>
		/// The step between large ticks, expressed in decades for the Log scale.
		/// </summary>
		public double LargeTickStep
		{
			set
			{
				largeTickStep_ = value;
			}
			get
			{
				return largeTickStep_;
			}
		}


		/// <summary>
		/// Position of one of the large ticks [other positions will be calculated relative to this one].
		/// </summary>
		public double LargeTickValue
		{
			set
			{
				largeTickValue_ = value;
			}
			get
			{
				return largeTickValue_;
			}
		}


		/// <summary>
		/// The number of small ticks between large ticks.
		/// </summary>
		public int NumberSmallTicks
		{
			set
			{
				numberSmallTicks_ = value;
			}
		}


		// Private members
		private object numberSmallTicks_;
		private double largeTickValue_ = double.NaN;
		private double largeTickStep_ = double.NaN;

		/// <summary>
		/// World to physical coordinate transform.
		/// </summary>
		/// <param name="coord">The coordinate value to transform.</param>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="clip">if false, then physical value may extend outside worldMin / worldMax. If true, the physical value returned will be clipped to physicalMin or physicalMax if it lies outside this range.</param>
		/// <returns>The transformed coordinates.</returns>
		/// <remarks>TODO: make Reversed property work for this.</remarks>
		public override PointF WorldToPhysical( 
			double coord,
			PointF physicalMin, 
			PointF physicalMax,
			bool clip )
		{
			// if want clipped value, return extrema if outside range.
			if (clip)
			{
				if (coord > WorldMax)
				{
					return physicalMax;
				}
				if (coord < WorldMin)
				{
					return physicalMin;
				}
			}

			if (coord < 0.0f)
			{
				throw new NPlotException( "Cannot have negative values for data using Log Axis" );
			}

			// inside range or don't want to clip.
			double lrange = (double)(Math.Log10(WorldMax) - Math.Log10(WorldMin));
			double prop = (double)((Math.Log10(coord) - Math.Log10(WorldMin)) / lrange);
			PointF offset = new PointF( (float)(prop * (physicalMax.X - physicalMin.X)),
				(float)(prop * (physicalMax.Y - physicalMin.Y)) );

			return new PointF( physicalMin.X + offset.X, physicalMin.Y + offset.Y );
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
		public override double PhysicalToWorld( PointF p, PointF physicalMin, PointF physicalMax, bool clip )
		{
			// use the base method to do the projection on the axis.
			double t = base.PhysicalToWorld( p, physicalMin, physicalMax, clip );

			// now reconstruct phys dist prop along this assuming linear scale as base method did.
			double v = (t - this.WorldMin) / (this.WorldMax - this.WorldMin);

			double ret = WorldMin*Math.Pow( WorldMax / WorldMin, v );

			// if want clipped value, return extrema if outside range.
			if (clip)
			{
				ret = Math.Max( ret, WorldMin );
				ret = Math.Min( ret, WorldMax );
			}

			return ret;

		}


		/// <summary>
		/// The minimum world extent of the axis. Must be greater than zero.
		/// </summary>
		public override double WorldMin
		{
			get
			{
				return (double)base.WorldMin;
			}
			set
			{
				if (value > 0.0f)
				{
					base.WorldMin = value;
				}
				else
				{
					throw new NPlotException("Cannot have negative values in Log Axis");
				}
			}
		}


		/// <summary>
		/// The maximum world extent of the axis. Must be greater than zero.
		/// </summary>
		public override double WorldMax
		{
			get
			{
				return (double)base.WorldMax;
			}
			set
			{
				if (value > 0.0F)
				{
					base.WorldMax = value;
				}
				else
				{
					throw new NPlotException("Cannot have negative values in Log Axis");
				}
			}
		}

		/// <summary>
		/// Get whether or not this axis is linear. It is not.
		/// </summary>
		public override bool IsLinear
		{
			get
			{
				return false;
			}
		}

	}
}
