/*
 * NPlot - A charting library for .NET
 * 
 * Transform2D.cs
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

namespace NPlot
{

	/// <summary>
	/// Encapsulates functionality for transforming world to physical coordinates optimally.
	/// </summary>
	/// <remarks>The existence of the whole ITransform2D thing might need revising. Not convinced it's the best way.</remarks>
	public class Transform2D
	{

		/// <summary>
		/// Constructs the optimal ITransform2D object for the supplied x and y axes.
		/// </summary>
		/// <param name="xAxis">The xAxis to use for the world to physical transform.</param>
		/// <param name="yAxis">The yAxis to use for the world to physical transform.</param>
		/// <returns>An ITransform2D derived object for converting from world to physical coordinates.</returns>
		public static ITransform2D GetTransformer( PhysicalAxis xAxis, PhysicalAxis yAxis )
		{
			ITransform2D ret = null;

//			if (xAxis.Axis.IsLinear && yAxis.Axis.IsLinear && !xAxis.Axis.Reversed && !yAxis.Axis.Reversed)
//				ret = new FastTransform2D( xAxis, yAxis );
//			else 
//				ret = new DefaultTransform2D( xAxis, yAxis );

			ret = new DefaultTransform2D( xAxis, yAxis );

			return ret;
		}


		/// <summary>
		/// This class does world -> physical transforms for the general case
		/// </summary>
		public class DefaultTransform2D : ITransform2D
		{
			private PhysicalAxis xAxis_;
			private PhysicalAxis yAxis_;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="xAxis">The x-axis to use for transforms</param>
			/// <param name="yAxis">The y-axis to use for transforms</param>
			public DefaultTransform2D( PhysicalAxis xAxis, PhysicalAxis yAxis )
			{
				xAxis_ = xAxis;
				yAxis_ = yAxis;
			}


			/// <summary>
			/// Transforms the given world point to physical coordinates
			/// </summary>
			/// <param name="x">x coordinate of world point to transform.</param>
			/// <param name="y">y coordinate of world point to transform.</param>
			/// <returns>the corresponding physical point.</returns>
			public PointF Transform( double x, double y )
			{
				return new PointF(
					xAxis_.WorldToPhysical( x, false ).X,
					yAxis_.WorldToPhysical( y, false ).Y );
			}


			/// <summary>
			/// Transforms the given world point to physical coordinates
			/// </summary>
			/// <param name="worldPoint">the world point to transform</param>
			/// <returns>the corresponding physical point</returns>
			public PointF Transform( PointD worldPoint )
			{
				return new PointF( 
					xAxis_.WorldToPhysical( worldPoint.X, false ).X,
					yAxis_.WorldToPhysical( worldPoint.Y, false ).Y );
			}

		}
	



		/// <summary>
		/// This class does highly efficient world->physical and physical->world transforms
		/// for linear axes. 
		/// </summary>
		public class FastTransform2D : ITransform2D
		{

			private PageAlignedPhysicalAxis xAxis_;
			private PageAlignedPhysicalAxis yAxis_;


			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="xAxis">The x-axis to use for transforms</param>
			/// <param name="yAxis">The y-axis to use for transforms</param>
			public FastTransform2D( PhysicalAxis xAxis, PhysicalAxis yAxis )
			{
				xAxis_ = new PageAlignedPhysicalAxis( xAxis );
				yAxis_ = new PageAlignedPhysicalAxis( yAxis );
			}


			/// <summary>
			/// Transforms the given world point to physical coordinates
			/// </summary>
			/// <param name="x">x coordinate of world point to transform.</param>
			/// <param name="y">y coordinate of world point to transform.</param>
			/// <returns>the corresponding physical point.</returns>
			public PointF Transform( double x, double y )
			{
				return new PointF(
					xAxis_.WorldToPhysicalClipped( x ),
					yAxis_.WorldToPhysicalClipped( y ) );
			}


			/// <summary>
			/// Transforms the given world point to physical coordinates
			/// </summary>
			/// <param name="worldPoint">the world point to transform</param>
			/// <returns>the corresponding physical point</returns>
			public PointF Transform( PointD worldPoint )
			{
				return new PointF( 
					xAxis_.WorldToPhysical( worldPoint.X ),
					yAxis_.WorldToPhysical( worldPoint.Y ) );
			}

		}


	}
}
