/*
 * NPlot - A charting library for .NET
 * 
 * PageAlignedPhysicalAxis.cs
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

namespace NPlot
{

	/// <summary>
	/// The bare minimum needed to do world->physical and physical->world transforms for
	/// vertical axes. Also includes tick placements. Built for speed.
	/// </summary>
	/// <remarks>currently unused</remarks>
	public class PageAlignedPhysicalAxis
	{

		private int pMin_;
		private int pMax_;
		private int pLength_;      // cached.

		private double worldMin_;
		private double worldMax_;
		private double worldLength_; // cached.
		

		/// <summary>
		/// Construct from a fully-blown physical axis.
		/// </summary>
		/// <param name="physicalAxis">the physical axis to get initial values from.</param>
		public PageAlignedPhysicalAxis( PhysicalAxis physicalAxis )
		{
			worldMin_ = physicalAxis.Axis.WorldMin;
			worldMax_ = physicalAxis.Axis.WorldMax;
			worldLength_ = worldMax_ - worldMin_;

			if ( physicalAxis.PhysicalMin.X == physicalAxis.PhysicalMax.X )
			{
				pMin_ = physicalAxis.PhysicalMin.Y;
				pMax_ = physicalAxis.PhysicalMax.Y;
			}
			else if ( physicalAxis.PhysicalMin.Y == physicalAxis.PhysicalMax.Y )
			{
				pMin_ = physicalAxis.PhysicalMin.X;
				pMax_ = physicalAxis.PhysicalMax.X;
			}
			else
			{
				throw new NPlotException( "Physical axis is not page aligned" );
			}

			pLength_ = pMax_ - pMin_;

		}

		
		/// <summary>
		/// return the physical coordinate corresponding to the supplied world coordinate.
		/// </summary>
		/// <param name="world">world coordinate to determine physical coordinate for.</param>
		/// <returns>the physical coordinate corresoindng to the supplied world coordinate.</returns>
		public float WorldToPhysical( double world )
		{
			return (float)(((world-worldMin_) / worldLength_) * (float)pLength_ + (float)pMin_);
		}


		/// <summary>
		/// return the physical coordinate corresponding to the supplied world coordinate,
		/// clipped if it is outside the bounds of the axis
		/// </summary>
		/// <param name="world">world coordinate to determine physical coordinate for.</param>
		/// <returns>the physical coordinate corresoindng to the supplied world coordinate.</returns>
		public float WorldToPhysicalClipped( double world )
		{
			if (world > worldMax_)
			{
				return pMax_;
			}
			
			if (world < worldMin_)
			{
				return pMin_;
			}
			
			// is this quicker than returning WorldToPhysical?
			return (float)(((world-worldMin_) / worldLength_) * (float)pLength_ + (float)pMin_);
		}
	

		/// <summary>
		/// return the world coordinate corresponding to the supplied physical coordinate.
		/// </summary>
		/// <param name="physical">physical coordinate to determine world coordinate for.</param>
		/// <returns>the world coordinate corresponding to the supplied </returns>
		public double PhysicalToWorld( float physical )
		{
			return ((float)(physical-pMin_) / (float)pLength_) * worldLength_ + worldMin_;
		}


	}
}
