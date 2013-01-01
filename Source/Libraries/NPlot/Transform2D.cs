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
    /// This class does highly efficient world->physical and physical->world transforms
    /// for linear axes. 
    /// </summary>
    public class Transform2D
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xAxis">The x-axis to use for transforms</param>
        /// <param name="yAxis">The y-axis to use for transforms</param>
        public Transform2D(PhysicalAxis xAxis, PhysicalAxis yAxis)
        {
            InitializeX(xAxis);
            InitializeY(yAxis);
        }

        double m_pMinX;
        double m_worldMinX;
        double m_divideWorldLengthTimesPLengthX;

        double m_pMinY;
        double m_worldMinY;
        double m_divideWorldLengthTimesPLengthY;

        public void InitializeX(PhysicalAxis physicalAxis)
        {
            m_worldMinX = physicalAxis.Axis.WorldMin;
            double worldMax = physicalAxis.Axis.WorldMax;
            double worldLength = worldMax - m_worldMinX;
            m_pMinX = physicalAxis.PhysicalMin.X;
            double pMax = physicalAxis.PhysicalMax.X;
            double pLength = pMax - m_pMinX;
            m_divideWorldLengthTimesPLengthX = pLength / worldLength;
        }

        public void InitializeY(PhysicalAxis physicalAxis)
        {
            m_worldMinY = physicalAxis.Axis.WorldMin;
            double worldMax = physicalAxis.Axis.WorldMax;
            double worldLength = worldMax - m_worldMinY;
            m_pMinY = physicalAxis.PhysicalMin.Y;
            double pMax = physicalAxis.PhysicalMax.Y;
            double pLength = pMax - m_pMinY;
            m_divideWorldLengthTimesPLengthY = pLength / worldLength;
        }

        float WorldToPhysicalX(double world)
        {
            return (float)((world - m_worldMinX) * m_divideWorldLengthTimesPLengthX + m_pMinX);
        }

        public float WorldToPhysicalY(double world)
        {
            return (float)((world - m_worldMinY) * m_divideWorldLengthTimesPLengthY + m_pMinY);
        }

        /// <summary>
        /// Transforms the given world point to physical coordinates
        /// </summary>
        /// <param name="x">x coordinate of world point to transform.</param>
        /// <param name="y">y coordinate of world point to transform.</param>
        /// <returns>the corresponding physical point.</returns>
        public PointF Transform(double x, double y)
        {
            return new PointF(WorldToPhysicalX(x), WorldToPhysicalY(y));
        }

        /// <summary>
        /// Transforms the given world point to physical coordinates
        /// </summary>
        /// <param name="worldPoint">the world point to transform</param>
        /// <returns>the corresponding physical point</returns>
        public PointF Transform(PointD worldPoint)
        {
            return new PointF(WorldToPhysicalX(worldPoint.X), WorldToPhysicalY(worldPoint.Y));
        }



    }
}
