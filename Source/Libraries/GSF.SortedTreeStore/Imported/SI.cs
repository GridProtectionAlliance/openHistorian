//******************************************************************************************************
//  SI.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/25/2008 - J. Ritchie Carroll
//       Initial version of source generated.
//  09/11/2008 - J. Ritchie Carroll
//       Converted to C#.
//  08/10/2009 - Josh L. Patterson
//       Edited Comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

#region [ Contributor License Agreements ]

/**************************************************************************\
   Copyright © 2009 - J. Ritchie Carroll
   All rights reserved.
  
   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:
  
      * Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.
       
      * Redistributions in binary form must reproduce the above
        copyright notice, this list of conditions and the following
        disclaimer in the documentation and/or other materials provided
        with the distribution.
  
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER "AS IS" AND ANY
   EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
   IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
   PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
   OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  
\**************************************************************************/

#endregion

using System;
using System.Text;

namespace GSF.Units
{
    /// <summary>
    /// Defines constant factors for SI units of measure to handle metric conversions.
    /// </summary>
    // ReSharper disable RedundantNameQualifier
    public static class SI
    {
        // Unit factor SI names
        private static readonly string[] s_names = { "yocto", "zepto", "atto", "femto", "pico", "nano", "micro", "milli", "centi", "deci", "deca", "hecto", "kilo", "mega", "giga", "tera", "peta", "exa", "zetta", "yotta" };

        // Unit factor SI symbols
        private static readonly string[] s_symbols = { "y", "z", "a", "f", "p", "n", "µ", "m", "c", "d", "da", "h", "k", "M", "G", "T", "P", "E", "Z", "Y" };

        // Unit factor SI factors
        private static readonly double[] s_factors = { Yocto, Zepto, Atto, Femto, Pico, Nano, Micro, Milli, Centi, Deci, Deca, Hecto, Kilo, Mega, Giga, Tera, Peta, Exa, Zetta, Yotta };

        /// <summary>
        /// SI prefix Y, 10^24
        /// </summary>
        public const double Yotta = 1.0e+24D;

        /// <summary>
        /// SI prefix Z, 10^21
        /// </summary>
        public const double Zetta = 1.0e+21D;

        /// <summary>
        /// SI prefix E, 10^18
        /// </summary>
        public const double Exa = 1.0e+18D;

        /// <summary>
        /// SI prefix P, 10^15
        /// </summary>
        public const double Peta = 1.0e+15D;

        /// <summary>
        /// SI prefix T, 10^12
        /// </summary>
        public const double Tera = 1.0e+12D;

        /// <summary>
        /// SI prefix G, 10^9
        /// </summary>
        public const double Giga = 1.0e+9D;

        /// <summary>
        /// SI prefix M, 10^6
        /// </summary>
        public const double Mega = 1.0e+6D;

        /// <summary>
        /// SI prefix k, 10^3
        /// </summary>
        public const double Kilo = 1.0e+3D;

        /// <summary>
        /// SI prefix h, 10^2
        /// </summary>
        public const double Hecto = 1.0e+2D;

        /// <summary>
        /// SI prefix da, 10^1
        /// </summary>
        public const double Deca = 1.0e+1D;

        /// <summary>
        /// SI prefix d, 10^-1
        /// </summary>
        public const double Deci = 1.0e-1D;

        /// <summary>
        /// SI prefix c, 10^-2
        /// </summary>
        public const double Centi = 1.0e-2D;

        /// <summary>
        /// SI prefix m, 10^-3
        /// </summary>
        public const double Milli = 1.0e-3D;

        /// <summary>
        /// SI prefix µ, 10^-6
        /// </summary>
        public const double Micro = 1.0e-6D;

        /// <summary>
        /// SI prefix n, 10^-9
        /// </summary>
        public const double Nano = 1.0e-9D;

        /// <summary>
        /// SI prefix p, 10^-12
        /// </summary>
        public const double Pico = 1.0e-12D;

        /// <summary>
        /// SI prefix f, 10^-15
        /// </summary>
        public const double Femto = 1.0e-15D;

        /// <summary>
        /// SI prefix a, 10^-18
        /// </summary>
        public const double Atto = 1.0e-18D;

        /// <summary>
        /// SI prefix z, 10^-21
        /// </summary>
        public const double Zepto = 1.0e-21D;

        /// <summary>
        /// SI prefix y, 10^-24
        /// </summary>
        public const double Yocto = 1.0e-24D;

        /// <summary>
        /// Gets an array of all the defined unit factor SI names ordered from least (<see cref="Yocto"/>) to greatest (<see cref="Yotta"/>).
        /// </summary>
        public static string[] Names
        {
            get
            {
                return s_names;
            }
        }

        /// <summary>
        /// Gets an array of all the defined unit factor SI prefix symbols ordered from least (<see cref="Yocto"/>) to greatest (<see cref="Yotta"/>).
        /// </summary>
        public static string[] Symbols
        {
            get
            {
                return s_symbols;
            }
        }

        /// <summary>
        /// Gets an array of all the defined SI unit factors ordered from least (<see cref="Yocto"/>) to greatest (<see cref="Yotta"/>).
        /// </summary>
        public static double[] Factors
        {
            get
            {
                return s_factors;
            }
        }

        /// <summary>
        /// Turns the given number of units into a textual representation with an appropriate unit scaling.
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "m/h" for meters per hour).</param>
        /// <param name="symbolNames">Optional SI factor symbol or name array to use during textual conversion, defaults to <see cref="Symbols"/>.</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI.Yocto"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI.Yotta"/>.</param>
        /// <remarks>
        /// The <paramref name="symbolNames"/> array needs one string entry for each defined SI item ordered from
        /// least (<see cref="Yocto"/>) to greatest (<see cref="Yotta"/>), see <see cref="Names"/> or <see cref="Symbols"/>
        /// arrays for examples.
        /// </remarks>
        /// <returns>A <see cref="string"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(double totalUnits, string unitName, string[] symbolNames = null, double minimumFactor = SI.Yocto, double maximumFactor = SI.Yotta)
        {
            return ToScaledString(totalUnits, 2, unitName, symbolNames, minimumFactor, maximumFactor);
        }

        /// <summary>
        /// Turns the given number of units into a textual representation with an appropriate unit scaling.
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="format">A numeric string format for scaled <paramref name="totalUnits"/>.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "m/h" for meters per hour).</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI.Yocto"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI.Yotta"/>.</param>
        /// <remarks>
        /// <see cref="Symbols"/> array is used for displaying SI symbol prefix for <paramref name="unitName"/>.
        /// </remarks>
        /// <returns>A <see cref="string"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(double totalUnits, string format, string unitName, double minimumFactor = SI.Yocto, double maximumFactor = SI.Yotta)
        {
            return ToScaledString(totalUnits, format, unitName, s_symbols, -1, minimumFactor, maximumFactor);
        }

        /// <summary>
        /// Turns the given number of units into a textual representation with an appropriate unit scaling.
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="decimalPlaces">Number of decimal places to display.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "m/h" for meters per hour).</param>
        /// <param name="symbolNames">Optional SI factor symbol or name array to use during textual conversion, defaults to <see cref="Symbols"/>.</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI.Yocto"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI.Yotta"/>.</param>
        /// <remarks>
        /// The <paramref name="symbolNames"/> array needs one string entry for each defined SI item ordered from
        /// least (<see cref="Yocto"/>) to greatest (<see cref="Yotta"/>), see <see cref="Names"/> or <see cref="Symbols"/>
        /// arrays for examples.
        /// </remarks>
        /// <returns>A <see cref="String"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="decimalPlaces"/> cannot be negative.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(double totalUnits, int decimalPlaces, string unitName, string[] symbolNames = null, double minimumFactor = SI.Yocto, double maximumFactor = SI.Yotta)
        {
            if (decimalPlaces < 0)
                throw new ArgumentOutOfRangeException("decimalPlaces", "decimalPlaces cannot be negative");

            return ToScaledString(totalUnits, "R", unitName, symbolNames ?? s_symbols, decimalPlaces, minimumFactor, maximumFactor);
        }

        /// <summary>
        /// Turns the given number of units into a textual representation with an appropriate unit scaling
        /// given string array of factor names or symbols.
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="format">A numeric string format for scaled <paramref name="totalUnits"/>.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "m/h" for meters per hour).</param>
        /// <param name="symbolNames">SI factor symbol or name array to use during textual conversion.</param>
        /// <param name="decimalPlaces">Optional number of decimal places to display.</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI.Yocto"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI.Yotta"/>.</param>
        /// <remarks>
        /// The <paramref name="symbolNames"/> array needs one string entry for each defined SI item ordered from
        /// least (<see cref="Yocto"/>) to greatest (<see cref="Yotta"/>), see <see cref="Names"/> or <see cref="Symbols"/>
        /// arrays for examples.
        /// </remarks>
        /// <returns>A <see cref="String"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(double totalUnits, string format, string unitName, string[] symbolNames, int decimalPlaces = -1, double minimumFactor = SI.Yocto, double maximumFactor = SI.Yotta)
        {
            StringBuilder image = new StringBuilder();

            double factor;
            int minimumIndex, maximumIndex;

            minimumIndex = GetFactorIndex(minimumFactor);

            if (minimumIndex < 0)
                throw new ArgumentOutOfRangeException("minimumFactor", "Unknown SI factor " + minimumFactor);

            maximumIndex = GetFactorIndex(maximumFactor);

            if (maximumIndex < 0)
                throw new ArgumentOutOfRangeException("maximumFactor", "Unknown SI factor " + maximumFactor);

            for (int i = maximumIndex; i >= minimumIndex; i--)
            {
                // See if total number of units ranges in the specified factor range
                factor = totalUnits / s_factors[i];

                if (factor >= 1.0D)
                {
                    if (decimalPlaces > -1)
                        factor = Math.Round(factor, decimalPlaces);

                    image.Append(factor.ToString(format));
                    image.Append(' ');
                    image.Append(symbolNames[i]);
                    image.Append(unitName);
                    break;
                }
            }

            if (image.Length == 0)
            {
                // Display total number of units
                if (decimalPlaces > -1)
                    totalUnits = Math.Round(totalUnits, decimalPlaces);

                image.Append(totalUnits.ToString(format));
                image.Append(' ');
                image.Append(unitName);
            }

            return image.ToString();
        }

        private static int GetFactorIndex(double factor)
        {
            for (int i = 0; i < s_factors.Length; i++)
            {
                if (s_factors[i] == factor)
                    return i;
            }

            return -1;
        }
    }
}