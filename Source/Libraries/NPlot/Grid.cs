/*
 * NPlot - A charting library for .NET
 * 
 * Grid.cs
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
	/// Encapsulates a Grid IDrawable object. Instances of this  to a PlotSurface2D 
	/// instance to produce a grid.
	/// </summary>
	public class Grid : IDrawable
	{

		/// <summary>
		/// 
		/// </summary>
		public enum GridType
		{
			/// <summary>
			/// No grid.
			/// </summary>
			None = 0,
			/// <summary>
			/// Coarse grid. Lines at large tick positions only.
			/// </summary>
			Coarse = 1,
			/// <summary>
			/// Fine grid. Lines at both large and small tick positions.
			/// </summary>
			Fine = 2
		}


		/// <summary>
		/// Default constructor
		/// </summary>
		public Grid()
		{
			minorGridPen_ = new Pen( Color.LightGray );
			float[] pattern = {1.0f, 2.0f};
			minorGridPen_.DashPattern = pattern;
			
			majorGridPen_ = new Pen( Color.LightGray );

			horizontalGridType_ = GridType.Coarse;
			
			verticalGridType_ = GridType.Coarse;
		}


		/// <summary>
		/// Specifies the horizontal grid type (none, coarse or fine).
		/// </summary>
		public GridType HorizontalGridType
		{
			get
			{
				return horizontalGridType_;
			}
			set
			{
				horizontalGridType_ = value;
			}
		}
		GridType horizontalGridType_;


		/// <summary>
		/// Specifies the vertical grid type (none, coarse, or fine).
		/// </summary>
		public GridType VerticalGridType
		{
			get
			{
				return verticalGridType_;
			}
			set
			{
				verticalGridType_ = value;
			}
		}
		GridType verticalGridType_;


		/// <summary>
		/// The pen used to draw major (coarse) grid lines.
		/// </summary>
		public System.Drawing.Pen MajorGridPen
		{
			get
			{
				return majorGridPen_;
			}
			set
			{
				majorGridPen_ = value;
			}
		}
		private Pen majorGridPen_;


		/// <summary>
		/// The pen used to draw minor (fine) grid lines.
		/// </summary>
		public System.Drawing.Pen MinorGridPen
		{
			get
			{
				return minorGridPen_;
			}
			set
			{
				minorGridPen_ = value;
			}
		}
		private Pen minorGridPen_;


		/// <summary>
		/// Does all the work in drawing grid lines.
		/// </summary>
		/// <param name="g">The graphics surface on which to render.</param>
		/// <param name="axis">TODO</param>
		/// <param name="orthogonalAxis">TODO</param>
		/// <param name="a">the list of world values to draw grid lines at.</param>
		/// <param name="horizontal">true if want horizontal lines, false otherwise.</param>
		/// <param name="p">the pen to use to draw the grid lines.</param>
		private void DrawGridLines( 
			Graphics g, PhysicalAxis axis, PhysicalAxis orthogonalAxis,
			System.Collections.ArrayList a, bool horizontal, Pen p )
		{
			for (int i=0; i<a.Count; ++i)
			{
				PointF p1 = axis.WorldToPhysical((double)a[i], true);
				PointF p2 = p1;
				PointF p3 = orthogonalAxis.PhysicalMax;
				PointF p4 = orthogonalAxis.PhysicalMin;
				if (horizontal)
				{
					p1.Y = p4.Y;
					p2.Y = p3.Y;
				}
				else
				{
					p1.X = p4.X;
					p2.X = p3.X;
				}
				// note: casting all drawing was necessary for sane display. why?
				g.DrawLine( p, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y );
			}
		}

		/// <summary>
		/// Draws the grid
		/// </summary>
		/// <param name="g">The graphics surface on which to draw</param>
		/// <param name="xAxis">The physical x axis to draw horizontal lines parallel to.</param>
		/// <param name="yAxis">The physical y axis to draw vertical lines parallel to.</param>
		public void Draw( Graphics g, PhysicalAxis xAxis, PhysicalAxis yAxis )
		{

			ArrayList xLargePositions = null;
			ArrayList yLargePositions = null;
			ArrayList xSmallPositions = null;
			ArrayList ySmallPositions = null;

			if (this.horizontalGridType_ != GridType.None)
			{
				xAxis.Axis.WorldTickPositions_FirstPass( xAxis.PhysicalMin, xAxis.PhysicalMax, out xLargePositions, out xSmallPositions );
				DrawGridLines( g, xAxis, yAxis, xLargePositions, true, this.MajorGridPen );	
			}

			if (this.verticalGridType_ != GridType.None)
			{
				yAxis.Axis.WorldTickPositions_FirstPass( yAxis.PhysicalMin, yAxis.PhysicalMax, out yLargePositions, out ySmallPositions );
				DrawGridLines( g, yAxis, xAxis, yLargePositions, false, this.MajorGridPen );
			}


			if (this.horizontalGridType_ == GridType.Fine)
			{
				xAxis.Axis.WorldTickPositions_SecondPass( xAxis.PhysicalMin, xAxis.PhysicalMax, xLargePositions, ref xSmallPositions );
				DrawGridLines( g, xAxis, yAxis, xSmallPositions, true, this.MinorGridPen );
			}

			if (this.verticalGridType_ == GridType.Fine)
			{
				yAxis.Axis.WorldTickPositions_SecondPass( yAxis.PhysicalMin, yAxis.PhysicalMax, yLargePositions, ref ySmallPositions );
				DrawGridLines( g, yAxis, xAxis, ySmallPositions, false, this.MinorGridPen );
			}

		}

	}
}
