/*
 * NPlot - A charting library for .NET
 * 
 * PiAxis.cs
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

namespace NPlot
{

	/// <summary>
	/// Axis with labels in multiples of Pi. Maybe needs a better name.
	/// Lots of functionality still to be added - currently only puts labels
	/// at whole increments of pi, want arbitrary increments, automatically
	/// determined and dependance on physical length. 
	/// Volunteers? 
	/// </summary>
	public class PiAxis : Axis
	{

		/// <summary>
		/// Deep copy of PiAxis.
		/// </summary>
		/// <returns>A copy of the LinearAxis Class.</returns>
		public override object Clone()
		{
			PiAxis a = new PiAxis();
			// ensure that this isn't being called on a derived type. If it is, then oh no!
			if (this.GetType() != a.GetType())
			{
				throw new NPlotException( "Error. Clone method is not defined in derived type." );
			}
			DoClone( this, a );
			return a;
		}


		/// <summary>
		/// Helper method for Clone.
		/// </summary>
		/// <param name="a">The original object to clone.</param>
		/// <param name="b">The cloned object.</param>
		protected static void DoClone( PiAxis b, PiAxis a )
		{
			Axis.DoClone( b, a );
		}


		/// <summary>
		/// Initialise PiAxis to default state.
		/// </summary>
		private void Init()
		{
		}


		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="a">The Axis to clone.</param>
		/// <remarks>TODO: [review notes] I don't think this will work as desired.</remarks>
		public PiAxis( Axis a )
			: base( a )
		{
			Init();
		}


		/// <summary>
		/// Default constructor
		/// </summary>
		public PiAxis()
			: base()
		{
			Init();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="worldMin">Minimum world value</param>
		/// <param name="worldMax">Maximum world value</param>
		public PiAxis( double worldMin, double worldMax )
			: base( worldMin, worldMax )
		{
			Init();
		}


		/// <summary>
		/// Given Graphics surface, and physical extents of axis, draw ticks and
		/// associated labels.
		/// </summary>
		/// <param name="g">The GDI+ Graphics surface on which to draw.</param>
		/// <param name="physicalMin">The physical location of the world min point</param>
		/// <param name="physicalMax">The physical location of the world max point</param>
		/// <param name="boundingBox">out: smallest box that completely encompasses all of the ticks and tick labels.</param>
		/// <param name="labelOffset">out: a suitable offset from the axis to draw the axis label.</param>
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

			int start = (int)Math.Ceiling( this.WorldMin / Math.PI );
			int end = (int)Math.Floor( this.WorldMax / Math.PI );
			
			// sanity checking.
			if ( end - start < 0 || end - start > 30 )
			{
				return;
			}

			for (int i=start; i<=end; ++i)
			{
				string label = i.ToString() + "Pi";

				if (i == 0)
					label = "0";
				else if (i == 1)
					label = "Pi";

				this.DrawTick( g, i*Math.PI, this.LargeTickSize, 
					label,
					new Point(0,0), 
					physicalMin, physicalMax,
					out tLabelOffset, out tBoundingBox );

				Axis.UpdateOffsetAndBounds( 
					ref labelOffset, ref boundingBox, 
					tLabelOffset, tBoundingBox );
			}

		}


		/// <summary>
		/// Determines the positions, in world coordinates, of the large ticks. 
		/// 
		/// Label axes do not have small ticks.
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
	
			int start = (int)Math.Ceiling( this.WorldMin / Math.PI );
			int end = (int)Math.Floor( this.WorldMax / Math.PI );

			// sanity checking.
			if ( end - start < 0 || end - start > 30 )
			{
				return;
			}

			for (int i=start; i<end; ++i)
			{
				largeTickPositions.Add( i*Math.PI ); 
			}

		}


	}
}
