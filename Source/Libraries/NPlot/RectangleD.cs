/*
 * NPlot - A charting library for .NET
 * 
 * RectangleD.cs
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
	/// Stores a set of four double numbers that represent the location and size of
	/// a rectangle. TODO: implement more functionality similar to Drawing.RectangleF.
	/// </summary>
	public struct RectangleD
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RectangleD( double x, double y, double width, double height )
		{
			x_ = x;
			y_ = y;
			width_ = width;
			height_ = height;
		}

		/// <summary>
		/// The rectangle height.
		/// </summary>
		public double Height
		{
			get
			{
				return height_;
			}
			set
			{
				height_ = value;
			}
		}

		/// <summary>
		/// The rectangle width.
		/// </summary>
		public double Width
		{
			get
			{
				return width_;
			}
			set
			{
				width_ = value;
			}
		}

		/// <summary>
		/// The minimum x coordinate of the rectangle.
		/// </summary>
		public double X
		{
			get
			{
				return x_;
			}
			set
			{
				x_ = value;
			}
		}


		/// <summary>
		/// The minimum y coordinate of the rectangle.
		/// </summary>
		public double Y
		{
			get
			{
				return y_;
			}
			set
			{
				y_ = value;
			}
		}


		private double x_;
		private double y_;
		private double width_;
		private double height_;

	}
}
