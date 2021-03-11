/*
 * NPlot - A charting library for .NET
 * 
 * Utils.cs
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
using System.Collections.Generic;
using System.Drawing;

namespace NPlot
{
    /// <summary>
    /// General purpose utility functions used internally.
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// Numbers less than this are considered insignificant. This number is
        /// bigger than double.Epsilon.
        /// </summary>
        public const double Epsilon = double.Epsilon * 1000.0;


        /// <summary>
        /// Returns true if the absolute difference between parameters is less than Epsilon
        /// </summary>
        /// <param name="a">first number to compare</param>
        /// <param name="b">second number to compare</param>
        /// <returns>true if equal, false otherwise</returns>
        public static bool DoubleEqual(double a, double b)
        {
            if (Math.Abs(a - b) < Epsilon)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Swaps the value of two doubles.
        /// </summary>
        /// <param name="a">first value to swap.</param>
        /// <param name="b">second value to swap.</param>
        public static void Swap(ref double a, ref double b)
        {
            double c = a;
            a = b;
            b = c;
        }


        /// <summary>
        /// Calculate the distance between two points, a and b.
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>Distance between points a and b</returns>
        public static float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }


        /// <summary>
        /// Calculate the distance between two points, a and b.
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>Distance between points a and b</returns>
        public static int Distance(Point a, Point b)
        {
            return (int)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        /// <summary>
        /// Returns the minimum and maximum values in an IList. The members of the list
        /// can be of different types - any type for which the function Utils.ConvertToDouble
        /// knows how to convert into a double.
        /// </summary>
        /// <param name="a">The IList to search.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>true if min max set, false otherwise (a is null or zero length).</returns>
        public static bool ArrayMinMax(IList<double> a, out double min, out double max)
        {
            if (a is null || a.Count == 0)
            {
                min = 0.0;
                max = 0.0;
                return false;
            }

            min = a[0];
            max = a[0];

            foreach (double o in a)
            {
                double e = o;

                if (min.Equals(double.NaN) && !e.Equals(double.NaN))
                {
                    // if min/max are double.NaN and the current value not, then
                    // set them to the current value.
                    min = e;
                    max = e;
                }
                if (!double.IsNaN(e))
                {
                    if (e < min)
                    {
                        min = e;
                    }
                    if (e > max)
                    {
                        max = e;
                    }
                }
            }

            if (min.Equals(double.NaN))
            {
                // if min == double.NaN, then max is also double.NaN
                min = 0.0;
                max = 0.0;
                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns unit vector along the line  a->b.
        /// </summary>
        /// <param name="a">line start point.</param>
        /// <param name="b">line end point.</param>
        /// <returns>The unit vector along the specified line.</returns>
        public static PointF UnitVector(PointF a, PointF b)
        {
            PointF dir = new PointF(b.X - a.X, b.Y - a.Y);
            double dirNorm = Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y);
            if (dirNorm > 0.0f)
            {
                dir = new PointF(
                    (float)(1.0f / dirNorm * dir.X),
                    (float)(1.0f / dirNorm * dir.Y)); // normalised axis direction vector
            }
            return dir;
        }

        /// <summary>
        /// Get a Font exactly the same as the passed in one, except for scale factor.
        /// </summary>
        /// <param name="initial">The font to scale.</param>
        /// <param name="scale">Scale by this factor.</param>
        /// <returns>The scaled font.</returns>
        public static Font ScaleFont(Font initial, double scale)
        {
            FontStyle fs = initial.Style;
            GraphicsUnit gu = initial.Unit;
            double sz = initial.Size;
            sz = sz * scale;
            string nm = initial.Name;
            return new Font(nm, (float)sz, fs, gu);
        }
    }
}