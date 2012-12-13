/*
 * NPlot - A charting library for .NET
 * 
 * ISurface.cs
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
using System.Windows.Forms;

namespace NPlot
{

	/// <summary>
	/// All PlotSurface's implement this interface.
	/// </summary>
	/// <remarks>Some of the parameter lists will change to be made more uniform.</remarks>
	public interface ISurface
	{

		/// <summary>
		/// Provides functionality for drawing the control.
		/// </summary>
		/// <param name="pe">paint event args</param>
		/// <param name="width">width of the control.</param>
		/// <param name="height">height of the control.</param>
		void DoPaint( PaintEventArgs pe, int width, int height );
		
	
		/// <summary>
		/// Provides functionality for handling mouse up events.
		/// </summary>
		/// <param name="e">mouse event args</param>
		/// <param name="ctr">the control</param>
		void DoMouseUp( MouseEventArgs e, System.Windows.Forms.Control ctr );
		
		
		/// <summary>
		/// Provides functionality for handling mouse move events.
		/// </summary>
		/// <param name="e">mouse event args</param>
		/// <param name="ctr">the control</param>
		void DoMouseMove( MouseEventArgs e, System.Windows.Forms.Control ctr );
		
		
		/// <summary>
		/// Provides functionality for handling mouse down events.
		/// </summary>
		/// <param name="e">mouse event args</param>
		void DoMouseDown( MouseEventArgs e );
	
	}

}
