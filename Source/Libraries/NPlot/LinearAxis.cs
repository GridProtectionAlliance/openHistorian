/*
 * NPlot - A charting library for .NET
 * 
 * LinearAxis.cs
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

using System.Drawing;
using System.Collections;
using System;
using System.Text;
using System.Diagnostics;

namespace NPlot
{
	/// <summary>
	/// Provides functionality for drawing axes with a linear numeric scale.
	/// </summary>
	public class LinearAxis : Axis, System.ICloneable
	{

		/// <summary>
		/// Deep copy of LinearAxis.
		/// </summary>
		/// <returns>A copy of the LinearAxis Class</returns>
		public override object Clone()
		{
			LinearAxis a = new LinearAxis();
			// ensure that this isn't being called on a derived type. If it is, then oh no!
			if (this.GetType() != a.GetType())
			{
				throw new NPlotException( "Clone not defined in derived type. Help!" );
			}
			this.DoClone( this, a );
			return a;
		}


		/// <summary>
		/// Helper method for Clone.
		/// </summary>
		protected void DoClone( LinearAxis b, LinearAxis a )
		{
			Axis.DoClone( b, a );

			a.numberSmallTicks_ = b.numberSmallTicks_;
			a.largeTickValue_ = b.largeTickValue_;
			a.largeTickStep_ = b.largeTickStep_;

			a.offset_ = b.offset_;
			a.scale_ = b.scale_;
		}


		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="a">The Axis to clone</param>
		public LinearAxis( Axis a )
			: base( a )
		{
			Init();
		}


		/// <summary>
		/// Default constructor.
		/// </summary>
		public LinearAxis()
			: base()
		{
			Init();
		}


		/// <summary>
		/// Construct a linear axis with the provided world min and max values.
		/// </summary>
		/// <param name="worldMin">the world minimum value of the axis.</param>
		/// <param name="worldMax">the world maximum value of the axis.</param>
		public LinearAxis( double worldMin, double worldMax )
			: base( worldMin, worldMax )
		{
			Init();
		}


		private void Init() 
		{
			this.NumberFormat = "{0:g5}";
		}


		/// <summary>
		/// Draws the large and small ticks [and tick labels] for this axis.
		/// </summary>
		/// <param name="g">The graphics surface on which to draw.</param>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="boundingBox">out: smallest box that completely surrounds all ticks and associated labels for this axis.</param>
		/// <param name="labelOffset">out: offset from the axis to draw the axis label.</param>
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

			labelOffset = new Point( 0, 0 );
			boundingBox = null;

			if (largeTickPositions.Count > 0)
			{
				for (int i = 0; i < largeTickPositions.Count; ++i)
				{
					double labelNumber = (double)largeTickPositions[i];

					// TODO: Find out why zero is sometimes significantly not zero [seen as high as 10^-16].
					if (Math.Abs(labelNumber) < 0.000000000000001)
					{
						labelNumber = 0.0;
					}

					StringBuilder label = new StringBuilder();
					label.AppendFormat(this.NumberFormat, labelNumber);

					this.DrawTick( g, ((double)largeTickPositions[i]/this.scale_-this.offset_), 
						this.LargeTickSize, label.ToString(),
						new Point(0,0), physicalMin, physicalMax, 
						out tLabelOffset, out tBoundingBox );
					
					Axis.UpdateOffsetAndBounds( ref labelOffset, ref boundingBox, 
						tLabelOffset, tBoundingBox );

				}
			}

			for (int i = 0; i<smallTickPositions.Count; ++i)
			{
				this.DrawTick( g, ((double)smallTickPositions[i]/this.scale_-this.offset_), 
					this.SmallTickSize, "", 
					new Point(0, 0), physicalMin, physicalMax, 
					out tLabelOffset, out tBoundingBox );

				// assume bounding box and label offset unchanged by small tick bounds.
			}

		}


		/// <summary>
		/// Determines the positions, in world coordinates, of the small ticks
		/// if they have not already been generated.
		/// 
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">The positions of the large ticks.</param>
		/// <param name="smallTickPositions">If null, small tick positions are returned via this parameter. Otherwise this function does nothing.</param>
		internal override void WorldTickPositions_SecondPass( 
			Point physicalMin,
			Point physicalMax,
			ArrayList largeTickPositions, 
			ref ArrayList smallTickPositions )
		{

			// return if already generated.
			if (smallTickPositions != null)
				return;

			int physicalAxisLength = Utils.Distance( physicalMin, physicalMax );

			double adjustedMax = this.AdjustedWorldValue( WorldMax );
			double adjustedMin = this.AdjustedWorldValue( WorldMin );

			smallTickPositions = new ArrayList();

			// TODO: Can optimize this now.
			bool shouldCullMiddle;
			double bigTickSpacing = this.DetermineLargeTickStep( physicalAxisLength, out shouldCullMiddle );

			int nSmall = this.DetermineNumberSmallTicks( bigTickSpacing );
			double smallTickSpacing = bigTickSpacing / (double)nSmall;

			// if there is at least one big tick
			if (largeTickPositions.Count > 0)
			{
				double pos1 = (double)largeTickPositions[0] - smallTickSpacing;
				while (pos1 > adjustedMin)
				{
					smallTickPositions.Add( pos1 );
					pos1 -= smallTickSpacing;
				}
			}

			for (int i = 0; i < largeTickPositions.Count; ++i )
			{
				for (int j = 1; j < nSmall; ++j )
				{
					double pos = (double)largeTickPositions[i] + ((double)j) * smallTickSpacing;
					if (pos <= adjustedMax)
					{
						smallTickPositions.Add( pos );
					}
				}
			}

		}

		/// <summary>
		/// Adjusts a real world value to one that has been modified to
		/// reflect the Axis Scale and Offset properties.
		/// </summary>
		/// <param name="world">world value to adjust</param>
		/// <returns>adjusted world value</returns>
		public double AdjustedWorldValue( double world )
		{
			return world * this.scale_ + this.offset_;
		}


		/// <summary>
		/// Determines the positions, in world coordinates, of the large ticks. 
		/// When the physical extent of the axis is small, some of the positions 
		/// that were generated in this pass may be converted to small tick 
		/// positions and returned as well.
		///
		/// If the LargeTickStep isn't set then this is calculated automatically and
		/// depends on the physical extent of the axis. 
		/// </summary>
		/// <param name="physicalMin">The physical position corresponding to the world minimum of the axis.</param>
		/// <param name="physicalMax">The physical position corresponding to the world maximum of the axis.</param>
		/// <param name="largeTickPositions">ArrayList containing the positions of the large ticks.</param>
		/// <param name="smallTickPositions">ArrayList containing the positions of the small ticks if calculated, null otherwise.</param>
		internal override void WorldTickPositions_FirstPass(
			Point physicalMin, 
			Point physicalMax,
			out ArrayList largeTickPositions,
			out ArrayList smallTickPositions
			)
		{

			// (1) error check

			if ( double.IsNaN(WorldMin) || double.IsNaN(WorldMax) )
			{
				throw new NPlotException( "world extent of axis not set." );
			}
			
			double adjustedMax = this.AdjustedWorldValue( WorldMax );
			double adjustedMin = this.AdjustedWorldValue( WorldMin );

			// (2) determine distance between large ticks.
			bool shouldCullMiddle;
			double tickDist = this.DetermineLargeTickStep( 
				Utils.Distance(physicalMin, physicalMax),
				out shouldCullMiddle );

			// (3) determine starting position.
		
			double first = 0.0f;

			if (!double.IsNaN(largeTickValue_)) 
			{
				// this works for both case when largTickValue_ lt or gt adjustedMin.
				first = largeTickValue_ + (Math.Ceiling((adjustedMin-largeTickValue_)/tickDist))*tickDist;
			}

			else
			{
				if( adjustedMin > 0.0 )
				{
					double nToFirst = Math.Floor(adjustedMin / tickDist) + 1.0f;
					first = nToFirst * tickDist;
				}
				else
				{
					double nToFirst = Math.Floor(-adjustedMin/tickDist) - 1.0f;
					first = -nToFirst * tickDist;
				}

				// could miss one, if first is just below zero.
				if ((first - tickDist) >= adjustedMin)
				{
					first -= tickDist;
				}
			}


			// (4) now make list of large tick positions.
			
			largeTickPositions = new ArrayList();

			if (tickDist < 0.0) // some sanity checking. TODO: remove this.
				throw new NPlotException( "Tick dist is negative" );

			double position = first;
			int safetyCount = 0;
			while (
				(position <= adjustedMax) && 
				(++safetyCount < 5000) )
			{
				largeTickPositions.Add( position );
				position += tickDist;
			}

			// (5) if the physical extent is too small, and the middle 
			// ticks should be turned into small ticks, then do this now.
			smallTickPositions = null;
			if (shouldCullMiddle)
			{
				smallTickPositions = new ArrayList();

				if (largeTickPositions.Count > 2)
				{
					for (int i=1; i<largeTickPositions.Count-1; ++i)
					{
						smallTickPositions.Add( largeTickPositions[i] );
					}
				}

				ArrayList culledPositions = new ArrayList();
				culledPositions.Add( largeTickPositions[0] );
				culledPositions.Add( largeTickPositions[largeTickPositions.Count-1] );
				largeTickPositions = culledPositions;
			}

		}


		/// <summary>
		/// Calculates the world spacing between large ticks, based on the physical
		/// axis length (parameter), world axis length, Mantissa values and 
		/// MinPhysicalLargeTickStep. A value such that at least two 
		/// </summary>
		/// <param name="physicalLength">physical length of the axis</param>
		/// <param name="shouldCullMiddle">Returns true if we were forced to make spacing of 
		/// large ticks too small in order to ensure that there are at least two of 
		/// them. The draw ticks method should not draw more than two large ticks if this
		/// returns true.</param>
		/// <returns>Large tick spacing</returns>
		/// <remarks>TODO: This can be optimised a bit.</remarks>
		private double DetermineLargeTickStep( float physicalLength, out bool shouldCullMiddle )
		{
			shouldCullMiddle = false;

			if ( double.IsNaN(WorldMin) || double.IsNaN(WorldMax) )
			{
				throw new NPlotException( "world extent of axis not set." );
			}

			// if the large tick has been explicitly set, then return this.
			if ( !double.IsNaN(largeTickStep_) )
			{
				if ( largeTickStep_ <= 0.0f )
				{
					throw new NPlotException( 
						"can't have negative or zero tick step - reverse WorldMin WorldMax instead."
					);
				}
				return largeTickStep_;
			}

			// otherwise we need to calculate the large tick step ourselves.

			// adjust world max and min for offset and scale properties of axis.
			double adjustedMax = this.AdjustedWorldValue( WorldMax );
			double adjustedMin = this.AdjustedWorldValue( WorldMin );
			double range = adjustedMax - adjustedMin;

			// if axis has zero world length, then return arbitrary number.
			if ( Utils.DoubleEqual( adjustedMax, adjustedMin ) )
			{
				return 1.0f;
			}

			double approxTickStep;
			if (TicksIndependentOfPhysicalExtent)
			{
				approxTickStep = range / 6.0f;
			}
			else
			{
				approxTickStep = (MinPhysicalLargeTickStep / physicalLength) * range;
			}

			double exponent = Math.Floor( Math.Log10( approxTickStep ) );
			double mantissa = Math.Pow( 10.0, Math.Log10( approxTickStep ) - exponent );

			// determine next whole mantissa below the approx one.
			int mantissaIndex = Mantissas.Length-1;
			for (int i=1; i<Mantissas.Length; ++i)
			{
				if (mantissa < Mantissas[i])
				{
					mantissaIndex = i-1;
					break;
				}
			}
			
			// then choose next largest spacing. 
			mantissaIndex += 1;
			if (mantissaIndex == Mantissas.Length)
			{
				mantissaIndex = 0;
				exponent += 1.0;
			}

			if (!TicksIndependentOfPhysicalExtent)
			{
				// now make sure that the returned value is such that at least two 
				// large tick marks will be displayed.
				double tickStep = Math.Pow( 10.0, exponent ) * Mantissas[mantissaIndex];
				float physicalStep = (float)((tickStep / range) * physicalLength);

				while (physicalStep > physicalLength/2)
				{
					shouldCullMiddle = true;

					mantissaIndex -= 1;
					if (mantissaIndex == -1)
					{
						mantissaIndex = Mantissas.Length-1;
						exponent -= 1.0;
					}

					tickStep = Math.Pow( 10.0, exponent ) * Mantissas[mantissaIndex];
					physicalStep = (float)((tickStep / range) * physicalLength);
				}
			}

			// and we're done.
			return Math.Pow( 10.0, exponent ) * Mantissas[mantissaIndex];

		}


		/// <summary>
		/// Given the large tick step, determine the number of small ticks that should
		/// be placed in between.
		/// </summary>
		/// <param name="bigTickDist">the large tick step.</param>
		/// <returns>the number of small ticks to place between large ticks.</returns>
		private int DetermineNumberSmallTicks( double bigTickDist )
		{

			if (this.numberSmallTicks_ != null)
			{
				return (int)this.numberSmallTicks_+1;
			}

			if (this.SmallTickCounts.Length != this.Mantissas.Length)
			{
				throw new NPlotException( "Mantissa.Length != SmallTickCounts.Length" );
			}

			if (bigTickDist > 0.0f)
			{

				double exponent = Math.Floor( Math.Log10( bigTickDist ) );
				double mantissa = Math.Pow( 10.0, Math.Log10( bigTickDist ) - exponent );

				for (int i=0; i<Mantissas.Length; ++i)
				{
					if ( Math.Abs(mantissa-Mantissas[i]) < 0.001 )
					{
						return SmallTickCounts[i]+1;
					}
				}

			}
				
			return 0;

		}


		/// <summary>
		/// The distance between large ticks. If this is set to NaN [default],
		/// this distance will be calculated automatically.
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
		/// If set !NaN, gives the distance between large ticks.
		/// </summary>
		private double largeTickStep_ = double.NaN;


		/// <summary>
		/// If set, a large tick will be placed at this position, and other large ticks will 
		/// be placed relative to this position.
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
		private double largeTickValue_ = double.NaN;

		/// <summary>
		/// The number of small ticks between large ticks.
		/// </summary>
		public int NumberOfSmallTicks
		{
			set
			{
				numberSmallTicks_ = value;
			}
			get
			{
				// TODO: something better here.
				return (int)numberSmallTicks_;
			}
		}
		private object numberSmallTicks_ = null;

		/// <summary>
		/// Scale to apply to world values when labelling axis:
		/// (labelWorld = world * scale + offset). This does not
		/// affect the "real" world range of the axis. 
		/// </summary>
		public double Scale
		{
			get
			{
				return scale_;
			}
			set
			{
				scale_ = value;
			}
		}

		/// <summary>
		/// Offset to apply to world values when labelling the axis:
		/// (labelWorld = axisWorld * scale + offset). This does not
		/// affect the "real" world range of the axis.
		/// </summary>
		public double Offset
		{
			get
			{
				return offset_;
			}
			set
			{
				offset_ = value;
			}
		}

		/// <summary>
		/// If LargeTickStep isn't specified, then a suitable value is 
		/// calculated automatically. To determine the tick spacing, the
		/// world axis length is divided by ApproximateNumberLargeTicks
		/// and the next lowest distance m*10^e for some m in the Mantissas
		/// set and some integer e is used as the large tick spacing. 
		/// </summary>
		public float ApproxNumberLargeTicks = 3.0f;

		/// <summary>
		/// If LargeTickStep isn't specified, then a suitable value is
		/// calculated automatically. The value will be of the form
		/// m*10^e for some m in this set.
		/// </summary>
		public double[] Mantissas = {1.0, 2.0, 5.0};

		/// <summary>
		/// If NumberOfSmallTicks isn't specified then .... 
		/// If specified LargeTickStep manually, then no small ticks unless
		/// NumberOfSmallTicks specified.
		/// </summary>
		public int[] SmallTickCounts = {4, 1, 4};


		private double offset_ = 0.0;

		private double scale_ = 1.0;
	}
}
