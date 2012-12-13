/*
 * NPlot - A charting library for .NET
 * 
 * BaseSequencePlot.cs
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

namespace NPlot
{

	/// <summary>
	/// Adds additional basic functionality to BasePlot that is common to all
	/// plots that implement the ISequencePlot interface.
	/// </summary>
	/// <remarks>If C# had multiple inheritance, the heirachy would be different. The way it is isn't very nice.</remarks>
	public class BaseSequencePlot : BasePlot, ISequencePlot
	{

		/// <summary>
		/// Gets or sets the data, or column name for the ordinate [y] axis.
		/// </summary>
		public object YData
		{
			get
			{
				return this.m_yData;
			}
			set
			{
				this.m_yData = value;
			}
		}
		private object m_yData = null;


		/// <summary>
		/// Gets or sets the data, or column name for the abscissa [x] axis.
		/// </summary>
		public object XData
		{
			get
			{
				return this.m_xData;
			}
			set
			{
				this.m_xData = value;
			}
		}
		private object m_xData = null;


		/// <summary>
		/// Writes text data of the plot object to the supplied string builder. It is 
		/// possible to specify that only data in the specified range be written.
		/// </summary>
		/// <param name="sb">the StringBuilder object to write to.</param>
		/// <param name="region">a region used if onlyInRegion is true.</param>
		/// <param name="onlyInRegion">If true, only data enclosed in the provided region will be written.</param>
		public void WriteData( System.Text.StringBuilder sb, RectangleD region, bool onlyInRegion )
		{
			SequenceAdapter data_ = new SequenceAdapter(this.YData, this.XData );

			sb.Append( "Label: " );
			sb.Append( this.Label );
			sb.Append( "\r\n" );
			data_.WriteData( sb, region, onlyInRegion );
		}

	}
}
