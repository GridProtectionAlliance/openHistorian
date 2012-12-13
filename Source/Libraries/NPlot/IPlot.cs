/*
 * NPlot - A charting library for .NET
 * 
 * IPlot.cs
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

namespace NPlot
{

	/// <summary>
	/// Defines the interface for objects that (a) can draw a representation of 
	/// themselves in the legend and (b) can recommend a good axis to draw themselves
	/// against.
	/// </summary>
	public interface IPlot : IDrawable
	{	
	
		/// <summary>
		/// Method used to draw a representation of the plot in a legend.
		/// </summary>
		void DrawInLegend( Graphics g, Rectangle startEnd );

		
		/// <summary>
		/// The label associated with the plot [used in legend]
		/// </summary>
		string Label { get; set; }

		
		/// <summary>
		/// Whether or not to include an entry for this plot in the legend if it exists.
		/// </summary>
		bool ShowInLegend { get; set; }


		/// <summary>
		/// The method used to set the default abscissa axis.
		/// </summary>
		Axis SuggestXAxis();

		
		/// <summary>
		/// The method used to set the default ordinate axis.
		/// </summary>
		Axis SuggestYAxis();


		/// <summary>
		/// Write data associated with the plot as text.
		/// </summary>
		/// <param name="sb">the string builder to write to.</param>
		/// <param name="region">Only write out data in this region if onlyInRegion is true.</param>
		/// <param name="onlyInRegion">If true, only data in region is written, else all data is written.</param>
		void WriteData( System.Text.StringBuilder sb, RectangleD region, bool onlyInRegion );

	}
}
