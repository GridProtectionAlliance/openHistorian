//******************************************************************************************************
//  SI2.cs - Gbtc
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
    /// Defines constant factors based on 1024 for related binary SI units of measure used in computational measurements.
    /// </summary>
    /// <remarks>
    /// See <a href="http://physics.nist.gov/cuu/Units/binary.html">NIST Reference</a> for information on IEC standard names.
    /// </remarks>
    // ReSharper disable RedundantNameQualifier
    public static class SI2
    {
        // Common unit factor SI names
        private static readonly string[] s_names = { "kilo", "mega", "giga", "tera", "peta", "exa" };

        // Common unit factor SI symbols
        private static readonly string[] s_symbols = { "K", "M", "G", "T", "P", "E" };

        // IEC unit factor SI names
        private static readonly string[] s_iecNames = { "kibi", "mebi", "gibi", "tebi", "pebi", "exbi" };

        // IEC unit factor SI symbols
        private static readonly string[] s_iecSymbols = { "Ki", "Mi", "Gi", "Ti", "Pi", "Ei" };

        // Unit factor SI factors
        private static readonly long[] s_factors = { Kilo, Mega, Giga, Tera, Peta, Exa };

        /// <summary>
        /// 1 exa, binary (E) = 1,152,921,504,606,846,976
        /// </summary>
        /// <remarks>
        /// This is the common name.
        /// </remarks>
        public const long Exa = 1024L * Peta;

        /// <summary>
        /// 1 exbi (Ei) = 1,152,921,504,606,846,976
        /// </summary>
        /// <remarks>
        /// This is the IEC standard name.
        /// </remarks>
        public const long Exbi = Exa;

        /// <summary>
        /// 1 peta, binary (P) = 1,125,899,906,842,624
        /// </summary>
        /// <remarks>
        /// This is the common name.
        /// </remarks>
        public const long Peta = 1024L * Tera;

        /// <summary>
        /// 1 pebi (Pi) = 1,125,899,906,842,624
        /// </summary>
        /// <remarks>
        /// This is the IEC standard name.
        /// </remarks>
        public const long Pebi = Peta;

        /// <summary>
        /// 1 tera, binary (T) = 1,099,511,627,776
        /// </summary>
        /// <remarks>
        /// This is the common name.
        /// </remarks>
        public const long Tera = 1024L * Giga;

        /// <summary>
        /// 1 tebi (Ti) = 1,099,511,627,776
        /// </summary>
        /// <remarks>
        /// This is the IEC standard name.
        /// </remarks>
        public const long Tebi = Tera;

        /// <summary>
        /// 1 giga, binary (G) = 1,073,741,824
        /// </summary>
        /// <remarks>
        /// This is the common name.
        /// </remarks>
        public const long Giga = 1024L * Mega;

        /// <summary>
        /// 1 gibi (Gi) = 1,073,741,824
        /// </summary>
        /// <remarks>
        /// This is the IEC standard name.
        /// </remarks>
        public const long Gibi = Giga;

        /// <summary>
        /// 1 mega, binary (M) = 1,048,576
        /// </summary>
        /// <remarks>
        /// This is the common name.
        /// </remarks>
        public const long Mega = 1024L * Kilo;

        /// <summary>
        /// 1 mebi (Mi) = 1,048,576
        /// </summary>
        /// <remarks>
        /// This is the IEC standard name.
        /// </remarks>
        public const long Mebi = Mega;

        /// <summary>
        /// 1 kilo, binary (K) = 1,024
        /// </summary>
        /// <remarks>
        /// This is the common name.
        /// </remarks>
        public const long Kilo = 1024L;

        /// <summary>
        /// 1 kibi (Ki) = 1,024
        /// </summary>
        /// <remarks>
        /// This is the IEC standard name.
        /// </remarks>
        public const long Kibi = Kilo;

        /// <summary>
        /// Gets an array of all the defined common binary unit factor SI names ordered from least (<see cref="Kilo"/>) to greatest (<see cref="Exa"/>).
        /// </summary>
        public static string[] Names
        {
            get
            {
                return s_names;
            }
        }

        /// <summary>
        /// Gets an array of all the defined common binary unit factor SI prefix symbols ordered from least (<see cref="Kilo"/>) to greatest (<see cref="Exa"/>).
        /// </summary>
        public static string[] Symbols
        {
            get
            {
                return s_symbols;
            }
        }

        /// <summary>
        /// Gets an array of all the defined IEC binary unit factor SI names ordered from least (<see cref="Kibi"/>) to greatest (<see cref="Exbi"/>).
        /// </summary>
        public static string[] IECNames
        {
            get
            {
                return s_iecNames;
            }
        }

        /// <summary>
        /// Gets an array of all the defined IEC binary unit factor SI prefix symbols ordered from least (<see cref="Kibi"/>) to greatest (<see cref="Exbi"/>).
        /// </summary>
        public static string[] IECSymbols
        {
            get
            {
                return s_iecSymbols;
            }
        }

        /// <summary>
        /// Gets an array of all the defined binary SI unit factors ordered from least (<see cref="Kilo"/>) to greatest (<see cref="Exa"/>).
        /// </summary>
        public static long[] Factors
        {
            get
            {
                return s_factors;
            }
        }

        /// <summary>
        /// Turns the given number of units (e.g., bytes) into a textual representation with an appropriate unit scaling
        /// and common named representation (e.g., KB, MB, GB, TB, etc.).
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "B" for bytes).</param>
        /// <param name="symbolNames">Optional SI factor symbol or name array to use during textual conversion, defaults to <see cref="Symbols"/>.</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI2.Kilo"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI2.Exa"/>.</param>
        /// <remarks>
        /// The <paramref name="symbolNames"/> array needs one string entry for each defined SI item ordered from
        /// least (<see cref="Kilo"/>) to greatest (<see cref="Exa"/>), see <see cref="Names"/> or <see cref="Symbols"/>
        /// arrays for examples.
        /// </remarks>
        /// <returns>A <see cref="string"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(long totalUnits, string unitName, string[] symbolNames = null, long minimumFactor = SI2.Kilo, long maximumFactor = SI2.Exa)
        {
            return ToScaledString(totalUnits, 2, unitName, symbolNames, minimumFactor, maximumFactor);
        }

        /// <summary>
        /// Turns the given number of units (e.g., bytes) into a textual representation with an appropriate unit scaling
        /// and common named representation (e.g., KB, MB, GB, TB, etc.).
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="format">A numeric string format for scaled <paramref name="totalUnits"/>.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "B" for bytes).</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI2.Kilo"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI2.Exa"/>.</param>
        /// <remarks>
        /// <see cref="Symbols"/> array is used for displaying SI symbol prefix for <paramref name="unitName"/>.
        /// </remarks>
        /// <returns>A <see cref="string"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(long totalUnits, string format, string unitName, long minimumFactor = SI2.Kilo, long maximumFactor = SI2.Exa)
        {
            return ToScaledString(totalUnits, format, unitName, s_symbols, -1, minimumFactor, maximumFactor);
        }

        /// <summary>
        /// Turns the given number of units (e.g., bytes) into a textual representation with an appropriate unit scaling
        /// and common named representation (e.g., KB, MB, GB, TB, etc.).
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="decimalPlaces">Number of decimal places to display.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "B" for bytes).</param>
        /// <param name="symbolNames">Optional SI factor symbol or name array to use during textual conversion, defaults to <see cref="Symbols"/>.</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI2.Kilo"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI2.Exa"/>.</param>
        /// <remarks>
        /// The <paramref name="symbolNames"/> array needs one string entry for each defined SI item ordered from
        /// least (<see cref="Kilo"/>) to greatest (<see cref="Exa"/>), see <see cref="Names"/> or <see cref="Symbols"/>
        /// arrays for examples.
        /// </remarks>
        /// <returns>A <see cref="String"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="decimalPlaces"/> cannot be negative.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(long totalUnits, int decimalPlaces, string unitName, string[] symbolNames = null, long minimumFactor = SI2.Kilo, long maximumFactor = SI2.Exa)
        {
            if (decimalPlaces < 0)
                throw new ArgumentOutOfRangeException("decimalPlaces", "decimalPlaces cannot be negative");

            return ToScaledString(totalUnits, "R", unitName, symbolNames ?? s_symbols, decimalPlaces, minimumFactor, maximumFactor);
        }

        /// <summary>
        /// Turns the given number of units (e.g., bytes) into a textual representation with an appropriate unit scaling
        /// given string array of factor names or symbols.
        /// </summary>
        /// <param name="totalUnits">Total units to represent textually.</param>
        /// <param name="format">A numeric string format for scaled <paramref name="totalUnits"/>.</param>
        /// <param name="unitName">Name of unit display (e.g., you could use "B" for bytes).</param>
        /// <param name="symbolNames">SI factor symbol or name array to use during textual conversion.</param>
        /// <param name="decimalPlaces">Optional number of decimal places to display.</param>
        /// <param name="minimumFactor">Optional minimum SI factor. Defaults to <see cref="SI2.Kilo"/>.</param>
        /// <param name="maximumFactor">Optional maximum SI factor. Defaults to <see cref="SI2.Exa"/>.</param>
        /// <remarks>
        /// The <paramref name="symbolNames"/> array needs one string entry for each defined SI item ordered from
        /// least (<see cref="Kilo"/>) to greatest (<see cref="Exa"/>), see <see cref="Names"/> or <see cref="Symbols"/>
        /// arrays for examples.
        /// </remarks>
        /// <returns>A <see cref="String"/> representation of the number of units.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimumFactor"/> or <paramref name="maximumFactor"/> is not defined in <see cref="Factors"/> array.</exception>
        public static string ToScaledString(long totalUnits, string format, string unitName, string[] symbolNames, int decimalPlaces = -1, long minimumFactor = SI2.Kilo, long maximumFactor = SI2.Exa)
        {
            StringBuilder image = new StringBuilder();

            double factor;
            int minimumIndex, maximumIndex;

            minimumIndex = GetFactorIndex(minimumFactor);

            if (minimumIndex < 0)
                throw new ArgumentOutOfRangeException("minimumFactor", "Unknown SI2 factor " + minimumFactor);

            maximumIndex = GetFactorIndex(maximumFactor);

            if (maximumIndex < 0)
                throw new ArgumentOutOfRangeException("maximumFactor", "Unknown SI2 factor " + maximumFactor);

            for (int i = maximumIndex; i >= minimumIndex; i--)
            {
                // See if total number of units ranges in the specified factor range
                factor = totalUnits / (double)s_factors[i];

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
                image.Append(totalUnits);
                image.Append(' ');
                image.Append(unitName);
            }

            return image.ToString();
        }

        private static int GetFactorIndex(long factor)
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
