//******************************************************************************************************
//  CharExtensions.cs - Gbtc
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
//  12/13/2008 - F. Russell Robertson
//       Generated original version of source code.
//  08/3/2009 - Josh L. Patterson
//       Updated comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  01/04/2010 - Andrew K. Hill
//       Modified the following methods per unit testing:
//       IsAny(char, IEnumerable<char>)
//  12/02/2010 - J. Ritchie Carroll
//       Modified IsWordTerminator to use standard Unicode character tests.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace GSF
{
    /// <summary>
    /// Defines extension functions related to character manipulation.
    /// </summary>
    public static class CharExtensions
    {
        // so that this only happens one time
        private static readonly char[] wordSeperators = { '\"', '/', '\\', '<', '>', '=', '{', '}', '(', ')', '[', ']', '@', '*' };
        private static readonly char[] numericValues = { '-', '+', ',', '.' };

        /// <summary>
        /// Encodes the specified Unicode character in proper Regular Expression format.
        /// </summary>
        /// <param name="item">Unicode character to encode in Regular Expression format.</param>
        /// <returns>Specified Unicode character in proper Regular Expression format.</returns>
        public static string RegexEncode(this char item)
        {
            return "\\u" + Convert.ToUInt16(item).ToString("x").PadLeft(4, '0');
        }

        /// <summary>
        /// Tests a character to determine if it marks the end of a typical English word.
        /// </summary>
        /// <param name="value">Input character to check.</param>
        /// <returns><c>true</c> if character is a work separator.</returns>
        /// <remarks>
        /// Performs no testing for ASCII codes &gt; 127.<br/>
        /// Does not separate words based on punctuation of ' %  - _  <br/>
        /// However does include the angle bracket symbols &lt; &gt; as separators<br/>
        /// <br/>
        /// For reference the standard char tests are:
        /// <ul>
        /// <li>"IsSperator (1) == simple space (32 or 160) only.</li>
        /// <li>IsPunctuation (23) == . , ! ? : ; " ' [ ] { } ( ) \ / @ % # * &amp; - _  (plus other char's &gt; 127)</li>
        /// <li>IsSymbol (8) == $ + &lt; &gt; = ^ ` ~</li>
        /// <li>IsWhiteSpace (6) == control char's 9 thru 13, plus 32 -- TAB, LF, VT, FF, CR, SP</li>
        /// </ul>
        /// </remarks>
        public static bool IsWordTerminator(this char value)
        {
            if (char.IsPunctuation(value) || char.IsWhiteSpace(value) || char.IsSymbol(value) || char.IsControl(value))
                return true;

            return value.IsAnyOf(wordSeperators);
        }

        /// <summary>
        /// Tests a character to determine if is a common part of a numeric string (digits or one of "+ - , .")
        /// </summary>
        /// <param name="value">The character to check.</param>
        /// <returns><c>true</c> if numeric character.</returns>
        public static bool IsNumeric(this char value)
        {
            if (char.IsDigit(value))
                return true;

            return value.IsAnyOf(numericValues);
        }

        /// <summary>
        /// Determines if a character matches any character in a sent array.
        /// </summary>
        /// <param name="value">The character to check.</param>
        /// <param name="testChars">The array of characters to test.</param>
        /// <returns>Boolean value indicating a that the character is in the array.</returns>
        public static bool IsAnyOf(this char value, IEnumerable<char> testChars)
        {
            if ((object)testChars == null)
                throw new ArgumentNullException("testChars");

            foreach (char c in testChars)
            {
                if (value == c)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Tests a character to determine if it is between a specified character range
        /// </summary>
        /// <param name="value">Input character to process.</param>
        /// <param name="startOfRange">Beginning of range character.</param>
        /// <param name="endOfRange">End of range character.</param>
        /// <returns><c>true</c> is the character is within the range.</returns>
        public static bool IsInRange(this char value, char startOfRange, char endOfRange)
        {
            if (value >= startOfRange && value <= endOfRange)
                return true;

            return false;
        }
    }
}