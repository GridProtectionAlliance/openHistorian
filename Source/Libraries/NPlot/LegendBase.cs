/*
 * NPlot - A charting library for .NET
 * 
 * LegendBase.cs
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
using System.Drawing;

namespace NPlot
{

	/// <summary>
	/// Provides functionality for drawing legends.
	/// </summary>
	/// <remarks>
	/// The class is quite closely tied to PlotSurface2D. 
	/// </remarks>
	public class LegendBase
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		public LegendBase()
		{
			this.Font = new Font( new FontFamily("Arial"), 10, FontStyle.Regular, GraphicsUnit.Pixel );
			this.BackgroundColor = Color.White;
			this.BorderColor = Color.Black;
			this.TextColor = Color.Black;
			this.borderStyle_ = BorderType.Shadow;
			this.autoScaleText_ = false;
		}


		/// <summary>
		/// Get the bounding box of the rectangle.
		/// </summary>
		/// <param name="position">the position of the top left of the legend.</param>
		/// <param name="plots">Array of plot objects to appear in the legend.</param>
		/// <param name="scale">if the legend is set to scale, the amount to scale by.</param>>
		/// <returns></returns>
		/// <remarks>do implementation that doesn't call draw. Change xPos, yPos to PointF</remarks>
		public Rectangle GetBoundingBox( Point position, ArrayList plots, float scale )
		{
			System.Drawing.Bitmap b = new System.Drawing.Bitmap(1,1);
			Graphics g = Graphics.FromImage(b);
			return this.Draw( g, position, plots, scale );
		}


		/// <summary>
		/// Draw The legend
		/// </summary>
		/// <param name="g">The graphics surface on which to draw</param>
		/// <param name="position">The position of the top left of the axis.</param>
		/// <param name="plots">Array of plot objects to appear in the legend.</param>
		/// <param name="scale">if the legend is set to scale, the amount to scale by.</param>
		/// <returns>bounding box</returns>
		public Rectangle Draw( Graphics g, Point position, ArrayList plots, float scale )
		{

			// first of all determine the Font to use in the legend.
			Font textFont;
			if (this.AutoScaleText)
			{
				textFont = Utils.ScaleFont( this.font_, scale );
			}
			else
			{
				textFont = this.font_;
			}

			// determine max width and max height of label strings and
			// count the labels. 
			int labelCount = 0;
			int maxHt = 0;
			int maxWd = 0;
			int unnamedCount = 0;
			for (int i=0; i<plots.Count; ++i)
			{
				if (!(plots[i] is IPlot))
				{
					continue;
				}

				IPlot p = (IPlot)plots[i];

				if (!p.ShowInLegend)
				{
					continue;
				}
				string label = p.Label;
				if (label == "")
				{
					unnamedCount += 1;
					label = "Series " + unnamedCount.ToString();
				}
				SizeF labelSize = g.MeasureString( label, textFont );
				if ( labelSize.Height > maxHt )
				{
					maxHt = (int)labelSize.Height;
				}
				if ( labelSize.Width > maxWd )
				{
					maxWd = (int)labelSize.Width;
				}

				++labelCount;
			}

			bool extendingHorizontally = numberItemsHorizontally_ == -1;
			bool extendingVertically = numberItemsVertically_ == -1;


			// determine width in legend items count units.
			int widthInItemCount = 0;
			if (extendingVertically)
			{
				if (labelCount >= numberItemsHorizontally_)
				{
					widthInItemCount = numberItemsHorizontally_;
				}
				else
				{
					widthInItemCount = labelCount;
				}
			}
			else if (extendingHorizontally)
			{
				widthInItemCount = labelCount / numberItemsVertically_;
				if (labelCount % numberItemsVertically_ != 0)
					widthInItemCount += 1;
			}
			else
			{
				throw new NPlotException( "logic error in legend base" );
			}


			// determine height of legend in items count units.
			int heightInItemCount = 0;
			if (extendingHorizontally)
			{
				if (labelCount >= numberItemsVertically_)
				{
					heightInItemCount = numberItemsVertically_;
				}
				else
				{
					heightInItemCount = labelCount;
				}
			}
			else // extendingVertically
			{
				heightInItemCount = labelCount / numberItemsHorizontally_;
				if (labelCount % numberItemsHorizontally_ != 0)
					heightInItemCount += 1;
			}

			int lineLength = 20;
			int hSpacing = (int)(5.0f * scale);
			int vSpacing = (int)(3.0f * scale);
			int boxWidth = (int) ((float)widthInItemCount * (lineLength + maxWd + hSpacing * 2.0f ) + hSpacing); 
			int boxHeight = (int)((float)heightInItemCount * (maxHt + vSpacing) + vSpacing);

			int totalWidth = boxWidth;
			int totalHeight = boxHeight;

			// draw box around the legend.

			if ( this.BorderStyle == BorderType.Line )
			{
				g.FillRectangle( new SolidBrush( this.bgColor_ ), position.X, position.Y, boxWidth, boxHeight );
				g.DrawRectangle( new Pen( this.borderColor_ ), position.X, position.Y, boxWidth, boxHeight );
			}
			else if ( this.BorderStyle == BorderType.Shadow )
			{
				int offset = (int)(4.0f * scale);
				g.FillRectangle( new SolidBrush( Color.FromArgb(128, Color.Gray) ), position.X+offset, position.Y+offset, boxWidth, boxHeight );
				g.FillRectangle( new SolidBrush( this.bgColor_ ), position.X, position.Y, boxWidth, boxHeight );
				g.DrawRectangle( new Pen( this.borderColor_ ), position.X, position.Y, boxWidth, boxHeight );

				totalWidth += offset;
				totalHeight += offset;
			}

			/*
			   else if ( this.BorderStyle == BorderType.Curved )
			   {
				   // TODO. make this nice.
			   }
			*/

			else
			{
				// do nothing.
			}

			// now draw entries in box..
			labelCount = 0;
			unnamedCount = 0;

			int plotCount = -1;
			for (int i=0; i<plots.Count; ++i)
			{
				if (!(plots[i] is IPlot))
				{
					continue;
				}

				IPlot p = (IPlot)plots[i];

				if (!p.ShowInLegend)
				{
					continue;
				}

				plotCount += 1;

				int xpos, ypos;
				if (extendingVertically)
				{
					xpos = plotCount % numberItemsHorizontally_;
					ypos = plotCount / numberItemsHorizontally_;
				}
				else
				{
					xpos = plotCount / numberItemsVertically_;
					ypos = plotCount % numberItemsVertically_;
				}

				int lineXPos = (int)(position.X + hSpacing + xpos * (lineLength + maxWd + hSpacing * 2.0f));
				int lineYPos = (int)(position.Y + vSpacing + ypos * (vSpacing + maxHt));
				p.DrawInLegend( g, new Rectangle( lineXPos, lineYPos, lineLength, maxHt ) );
				
				int textXPos = lineXPos + hSpacing + lineLength;
				int textYPos = lineYPos;
				string label = p.Label;
				if (label == "")
				{
					unnamedCount += 1;
					label = "Series " + unnamedCount.ToString();
				}

				g.DrawString( label, textFont,
					new SolidBrush( this.textColor_ ), textXPos, textYPos );

				++labelCount;
			}
			return new Rectangle( position.X, position.Y, totalWidth, totalHeight );

		}


		/// <summary>
		/// The font used to draw text in the legend.
		/// </summary>
		public Font Font
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
		private Font font_;


		/// <summary>
		/// The color used to draw text in the legend.
		/// </summary>
		public Color TextColor
		{
			get
			{
				return this.textColor_;
			}
			set
			{
				this.textColor_ = value;
			}
		}
		Color textColor_;


		/// <summary>
		/// The background color of the legend.
		/// </summary>
		public Color BackgroundColor
		{
			get
			{
				return bgColor_;
			}
			set
			{
				bgColor_ = value;
			}
		}
		Color bgColor_;


		/// <summary>
		/// The color of the legend border.
		/// </summary>
		public Color BorderColor
		{
			get
			{
				return borderColor_;
			}
			set
			{
				borderColor_ = value;
			}
		}
		Color borderColor_;


		/// <summary>
		/// The types of legend borders (enum).
		/// </summary>
		public enum BorderType
		{
			/// <summary>
			/// No border.
			/// </summary>
			None = 0,
			/// <summary>
			/// Line border.
			/// </summary>
			Line = 1,
			/// <summary>
			/// Shaded border.
			/// </summary>
			Shadow = 2
			//Curved = 3
		}


		/// <summary>
		/// The border style to use for the legend.
		/// </summary>
		public Legend.BorderType BorderStyle
		{
			get
			{
				return borderStyle_;
			}
			set
			{
				borderStyle_ = value;
			}
		}
		private NPlot.Legend.BorderType borderStyle_;


		/// <summary>
		/// Whether or not to auto scale text in the legend according the physical
		/// dimensions of the plot surface.
		/// </summary>
		public bool AutoScaleText
		{
			get
			{
				return autoScaleText_;
			}
			set
			{
				autoScaleText_ = value;
			}
		}
		bool autoScaleText_;


		/// <summary>
		/// Setting this does two things. First of all, it sets the maximum number of 
		/// items in the legend vertically. Second of all, it makes the legend grow
		/// horizontally (as it must given this constraint).
		/// </summary>
		public int NumberItemsVertically
		{
			set
			{
				this.numberItemsVertically_ = value;
				this.numberItemsHorizontally_ = -1;
			}
		}
		int numberItemsVertically_ = -1;


		/// <summary>
		/// Setting this does two things. First of all, it sets the maximum number of 
		/// items in the legend horizontally. Second of all, it makes the legend grow
		/// vertically (as it must given this constraint).
		/// </summary>
		public int NumberItemsHorizontally
		{
			set
			{
				this.numberItemsHorizontally_ = value;
				this.numberItemsVertically_ = -1;
			}
		}
		int numberItemsHorizontally_ = 1;

	}

}
