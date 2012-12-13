/*
 * NPlot - A charting library for .NET
 * 
 * PointD.cs
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

namespace NPlot
{
	/// <summary>
	/// Represtents a point in two-dimensional space. Used for representation
	/// of points world coordinates.
	/// </summary>
	public struct PointD
	{
		/// <summary>
		/// X-Coordinate of the point.
		/// </summary>
		public double X;

		/// <summary>
		/// Y-Coordinate of the point.
		/// </summary>
		public double Y;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="x">X-Coordinate of the point.</param>
		/// <param name="y">Y-Coordinate of the point.</param>
		public PointD( double x, double y )
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// returns a string representation of the point.
		/// </summary>
		/// <returns>string representation of the point.</returns>
		public override string ToString()
		{
			return X.ToString() + "\t" + Y.ToString();
		}

	}
}
