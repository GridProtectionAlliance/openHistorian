/*
 * NPlot - A charting library for .NET
 * 
 * PhysicalAxis.cs
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
using System.Drawing.Drawing2D;
using System.Collections;

namespace NPlot
{
	/// <summary>
	/// This class adds physical positioning information [PhysicalMin, PhysicalMax]
	/// and related functionality on top of a specific Axis class. 
	/// 
	/// It's an interesting
	/// question where to put this information. It belongs with every specific axis
	/// type, but on the other hand, users of the library as it is normally used 
	/// should not see it because
	/// positioning of axes is handled internally by PlotSurface2D. Therefore it doesn't make sense
	/// to put it in the Axis class unless it is internal. But if this were done it would restrict
	/// use of this information outside the library always, which is not what is wanted.
	/// The main disadvantage with the method chosen is that there is a lot of passing
	/// of the positional information between physical axis and the underlying logical
	/// axis type.
	/// 
	///	C# doesn't have templates. If it did, I might derive PhysicalAxis from the 
	/// templated Axis type (LinearAxis etc). Instead, have used a has-a relationship
	/// with an Axis superclass.
	/// </summary>
	public class PhysicalAxis
	{

		/// <summary>
		/// Prevent default construction.
		/// </summary>
		private PhysicalAxis()
		{
		}


		/// <summary>
		/// Construct
		/// </summary>
		/// <param name="a">The axis this is a physical representation of.</param>
		/// <param name="physicalMin">the physical position of the world minimum axis value.</param>
		/// <param name="physicalMax">the physical position of the world maximum axis value.</param>
		public PhysicalAxis( Axis a, Point physicalMin, Point physicalMax )
		{
			this.Axis = a;
			this.PhysicalMin = physicalMin;
			this.PhysicalMax = physicalMax;
		}


		/// <summary>
		/// Returns the smallest rectangle that completely contains all parts of the axis [including ticks and label].
		/// </summary>
		/// <returns>the smallest rectangle that completely contains all parts of the axis [including ticks and label].</returns>
		public virtual Rectangle GetBoundingBox()
		{
			System.Drawing.Bitmap scratchArea_ = new System.Drawing.Bitmap( 1, 1 );
			Graphics g = Graphics.FromImage( scratchArea_ );
			Rectangle bounds;
			this.Draw( g, out bounds );
			return bounds;
		}

	
		/// <summary>
		/// Draws the axis on the given graphics surface.
		/// </summary>
		/// <param name="g">The graphics surface on which to draw.</param>
		/// <param name="boundingBox">out: the axis bounding box - the smallest rectangle that
		/// completely contains all parts of the axis [including ticks and label].</param>
		public virtual void Draw( System.Drawing.Graphics g, out Rectangle boundingBox )
		{
			this.Axis.Draw( g, PhysicalMin, PhysicalMax, out boundingBox );
		}


		/// <summary>
		/// Given a world coordinate value, returns the physical position of the 
		/// coordinate along the axis.
		/// </summary>
		/// <param name="coord">the world coordinate</param>
		/// <param name="clip">if true, the physical position returned will be clipped to the physical max / min position as appropriate if the world value is outside the limits of the axis.</param>
		/// <returns>the physical position of the coordinate along the axis.</returns>
		public PointF WorldToPhysical( double coord, bool clip )
		{
			return Axis.WorldToPhysical( coord, PhysicalMin, PhysicalMax, clip );
		}


		/// <summary>
		/// Given a physical point on the graphics surface, returns the world
		/// value of it's projection onto the axis [i.e. closest point on the axis]. 
		/// The function is implemented for axes of arbitrary orientation.
		/// </summary>
		/// <param name="p">Physical point to find corresponding world value of.</param>
		/// <param name="clip">if true, returns a world position outside WorldMin / WorldMax
		/// range if this is closer to the axis line. If false, such values will
		/// be clipped to be either WorldMin or WorldMax as appropriate.</param>
		/// <returns>the world value of the point's projection onto the axis.</returns>
		public double PhysicalToWorld( Point p, bool clip )
		{
			return Axis.PhysicalToWorld( p, PhysicalMin, PhysicalMax, clip );
		}


		/// <summary>
		/// This sets new world limits for the axis from two physical points
		/// selected within the plot area.
		/// </summary>
		/// <param name="min">The upper left point of the selection.</param>
		/// <param name="max">The lower right point of the selection.</param>
		public void SetWorldLimitsFromPhysical( Point min, Point max )
		{
			double minc;
			double maxc;
			if (Axis != null)
			{
				minc = Axis.WorldMin;
				maxc = Axis.WorldMax;
				if ( !Axis.Reversed ) 
				{
					double tmp = this.PhysicalToWorld(min,true);
					Axis.WorldMax = this.PhysicalToWorld(max,true);
					Axis.WorldMin = tmp;
				}
				else
				{
					double tmp = this.PhysicalToWorld(min,true);
					Axis.WorldMin = this.PhysicalToWorld(max,true);
					Axis.WorldMax = tmp;
				}
				// need to trap somehow if the user selects an 
				// arbitrarily small range. Otherwise the GDI+ 
				// drawing routines lead to an overflow in painting 
				// the picture. This may be not the optimal solution,
				// but if the GDI+ draw leads to an overflow the
				// graphic surface becomes unusable anymore and I
				// had difficulty to trap the error.
				double half = (Axis.WorldMin + Axis.WorldMax)/2;
				double width = Axis.WorldMax - Axis.WorldMin;
				if (Math.Abs(half/width) > 1.0e12)
				{
					Axis.WorldMin = minc;
					Axis.WorldMax = maxc;
				}
			}
		}


		/// <summary>
		/// The physical position corresponding to WorldMin.
		/// </summary>
		public Point PhysicalMin
		{
			get
			{
				return physicalMin_;
			}
			set
			{
				physicalMin_ = value;
			}
		}
		private Point physicalMin_;


		/// <summary>
		/// The physical position corresponding to WorldMax.
		/// </summary>
		public Point PhysicalMax
		{
			get
			{
				return physicalMax_;
			}
			set
			{
				physicalMax_ = value;
			}
		}
		private Point physicalMax_;


		/// <summary>
		/// The axis this object adds physical extents to.
		/// </summary>
		public Axis Axis
		{
			get
			{
				return axis_;
			}
			set
			{
				axis_ = value;
			}
		}
		private Axis axis_;
	

		/// <summary>
		/// The length in pixels of the axis.
		/// </summary>
		public int PhysicalLength
		{
			get
			{
				return Utils.Distance( PhysicalMin, PhysicalMax );
			}
		}

		/// <summary>
		/// The length in world coordinates of one pixel. 
		/// </summary>
		public double PixelWorldLength
		{
			get
			{
				return this.Axis.WorldLength / this.PhysicalLength;
			}
		}

	}
}
