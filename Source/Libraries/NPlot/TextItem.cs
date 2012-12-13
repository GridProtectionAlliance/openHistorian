/*
 * NPlot - A charting library for .NET
 * 
 * TextItem.cs
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
	/// This class implements drawing text against two physical axes.
	/// </summary>
	public class TextItem : IDrawable
	{
		private void Init()
		{
			FontFamily fontFamily = new FontFamily("Arial");
			font_ = new Font(fontFamily, 10, FontStyle.Regular, GraphicsUnit.Pixel);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="position">The position the text starts.</param>
		/// <param name="text">The text.</param>
		public TextItem( PointD position, string text )
		{
			start_ = position;
			text_ = text;
			Init();
		}


		/// <summary>
		/// Text associated.
		/// </summary>
		public string Text 
		{
			get
			{
				return text_;
			}
			set
			{
				text_ = value;
			}
		}
		private string text_ = "";


		/// <summary>
		/// The starting point for the text.
		/// </summary>
		public PointD Start
		{
			get
			{
				return start_;
			}
			set
			{
				start_ = value;
			}
		}
		private PointD start_;


		/// <summary>
		/// Draws the text on a plot surface.
		/// </summary>
		/// <param name="g">graphics surface on which to draw</param>
		/// <param name="xAxis">The X-Axis to draw against.</param>
		/// <param name="yAxis">The Y-Axis to draw against.</param>
		public void Draw( System.Drawing.Graphics g, PhysicalAxis xAxis, PhysicalAxis yAxis )
		{
			Point startPoint = new Point( 
				(int)xAxis.WorldToPhysical( start_.X, true ).X,
				(int)yAxis.WorldToPhysical( start_.Y, true ).Y );

			g.DrawString(text_, font_, textBrush_,(int)startPoint.X,(int)startPoint.Y);
		}


		/// <summary>
		/// The brush used to draw the text.
		/// </summary>
		public Brush TextBrush
		{
			get
			{
				return textBrush_;
			}
			set
			{
				textBrush_ = value;
			}
		}

	
		/// <summary>
		/// Set the text to be drawn with a solid brush of this color.
		/// </summary>
		public Color TextColor
		{
			set
			{
				textBrush_ = new SolidBrush( value );
			}
		}
	
		/// <summary>
		/// The font used to draw the text associated with the arrow.
		/// </summary>
		public Font TextFont
		{
			get
			{
				return this.font_;
			}
			set
			{
				this.font_ = value;
			}
		}

		private Brush textBrush_ = new SolidBrush( Color.Black );
		private Pen pen_ = new Pen( Color.Black );
		private Font font_;
	}
}
