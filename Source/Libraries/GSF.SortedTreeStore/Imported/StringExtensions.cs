//******************************************************************************************************
//  StringExtensions.cs - Gbtc
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
//  02/23/2003 - J. Ritchie Carroll
//       Generated original version of source code.
//  01/24/2006 - J. Ritchie Carroll
//       Migrated 2.0 version of source code from 1.1 source (GSF.Shared.String).
//  06/01/2006 - J. Ritchie Carroll
//       Added ParseBoolean function to parse strings representing boolean's that may be numeric.
//  07/07/2006 - J. Ritchie Carroll
//       Added GetStringSegments function to break a string up into smaller chunks for parsing.
//       and/or displaying.
//  08/02/2007 - J. Ritchie Carroll
//       Added a CenterText method for centering strings in console applications or fixed width fonts.
//  08/03/2007 - Pinal C. Patel
//       Modified the CenterText method to handle multiple lines.
//  08/21/2007 - Darrell Zuercher
//       Edited code comments.
//  09/25/2007 - J. Ritchie Carroll
//       Added TitleCase function to format a string with the first letter of each word capitalized.
//  04/16/2008 - Pinal C. Patel
//       Made the keys of the string dictionary returned by ParseKeyValuePairs function case-insensitive.
//       Added JoinKeyValuePairs overloads that does the exact opposite of ParseKeyValuePairs.
//  09/19/2008 - J. Ritchie Carroll
//       Converted to C# extensions.
//  12/13/2008 - F. Russell Roberson
//       Generalized ParseBoolean to include "Y", and "T".
//       Added IndexOfRepeatedChar - Returns the index of the first character that is repeated.
//       Added Reverse - Reverses the order of characters in a string.
//       Added EnsureEnd - Ensures that a string ends with a specified char or string.
//       Added EnsureStart - Ensures that a string begins with a specified char or string.
//       Added IsNumeric - Test to see if a string only includes characters that can be interpreted as a number.
//       Added TrimWithEllipsisMiddle - Adds an ellipsis in the middle of a string as it is reduced to a specified length.
//       Added TrimWithEllipsisEnd - Trims a string to not exceed a fixed length and adds a ellipsis to string end.
//  02/10/2009 - J. Ritchie Carroll
//       Added ConvertToType overloaded extensions.
//  02/17/2009 - Josh L. Patterson
//       Edited Code Comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  01/11/2010 - Galen K. Riley
//      Issue fixes for unit tests:
//      ConvertToType - Fix to throw ArgumentNullException instead of NullReferenceException for null value
//      ConvertToType - Handling failed conversions better. Calling ConvertToType<int>("\0") returns properly
//      JoinKeyValuePairs - Fix to throw ArgumentNullException instead of NullReferenceException
//      ReplaceCharacters - Fix to throw ArgumentNullException instead of NullReferenceException for null replacementCharacter
//      RemoveCharacters - Fix to throw ArgumentNullException instead of NullReferenceException for null characterTestFunction
//      ReplaceCrLfs - Fix to throw ArgumentNullException instead of NullReferenceException for null value
//      RegexDecode - Fix to throw ArgumentNullException instead of NullReferenceException for null value
//  12/03/2010 - J. Ritchie Carroll
//      Modified ParseKeyValuePairs such that it could handle nested pairs to any needed depth.
//  12/05/2010 - Pinal C. Patel
//       Added an overload for ConvertToType() that takes CultureInfo as a parameter.
//  12/07/2010 - Pinal C. Patel
//       Updated ConvertToType() to return the type default if passed in string is null or empty.
//  01/04/2011 - J. Ritchie Carroll
//       Modified ConvertToType culture default to InvariantCulture for English style parsing defaults.
//  01/14/2011 - J. Ritchie Carroll
//       Modified JoinKeyValuePairs to delineate values that contain nested key/value pair expressions
//       such that the generated expression can correctly parsed.
//  03/23/2011 - J. Ritchie Carroll
//       Modified ParseKeyValuePairs to optionally ignore duplicate keys (default behavior now).
//       Removed overloads for ParseKeyValuePairs and JoinKeyValuePairs using optional parameters.
//  08/02/2011 - Pinal C. Patel
//       Added RemoveInvalidFileNameCharacters() and ReplaceInvalidFileNameCharacters() methods.
//  10/17/2012 - F Russell Robertson
//       Added QuoteWrap() method.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GSF
{
    /// <summary>Defines extension functions related to string manipulation.</summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Parses a string intended to represent a boolean value.
        /// </summary>
        /// <param name="value">String representing a boolean value.</param>
        /// <returns>Parsed boolean value.</returns>
        /// <remarks>
        /// This function, unlike Boolean.Parse, correctly parses a boolean value, even if the string value
        /// specified is a number (e.g., 0 or -1). Boolean.Parse expects a string to be represented as
        /// "True" or "False" (i.e., Boolean.TrueString or Boolean.FalseString respectively).
        /// </remarks>
        public static bool ParseBoolean(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            if (value.Length > 0)
            {
                // Try to parse string a number
                int iresult;

                if (int.TryParse(value, out iresult))
                    return (iresult != 0);

                // Try to parse string as a boolean
                bool bresult;

                if (bool.TryParse(value, out bresult))
                    return bresult;

                char test = value.ToUpper()[0];

                return (test == 'T' || test == 'Y');
            }

            return false;
        }

        /// <summary>
        /// Converts this string into the specified type.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to convert string to.</typeparam>
        /// <param name="value">Source string to convert to type.</param>
        /// <returns><see cref="string"/> converted to specified <see cref="Type"/>; default value of specified type T if conversion fails.</returns>
        /// <remarks>
        /// This function makes use of a <see cref="TypeConverter"/> to convert this <see cref="string"/> to the specified type T,
        /// the best way to make sure <paramref name="value"/> can be converted back to its original type is to use the same
        /// <see cref="TypeConverter"/> to convert the original object to a <see cref="string"/>; see the
        /// <see cref="Common.TypeConvertToString(object)"/> method for an easy way to do this.
        /// </remarks>
        public static T ConvertToType<T>(this string value)
        {
            return ConvertToType<T>(value, null);
        }

        /// <summary>
        /// Converts this string into the specified type.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to convert string to.</typeparam>
        /// <param name="value">Source string to convert to type.</param>
        /// <param name="type"><see cref="Type"/> to convert string to.</param>
        /// <returns><see cref="string"/> converted to specified <see cref="Type"/>; default value of specified type T if conversion fails.</returns>
        /// <remarks>
        /// This function makes use of a <see cref="TypeConverter"/> to convert this <see cref="string"/> to the specified <paramref name="type"/>,
        /// the best way to make sure <paramref name="value"/> can be converted back to its original type is to use the same
        /// <see cref="TypeConverter"/> to convert the original object to a <see cref="string"/>; see the
        /// <see cref="Common.TypeConvertToString(object)"/> method for an easy way to do this.
        /// </remarks>
        public static T ConvertToType<T>(this string value, Type type)
        {
            return ConvertToType<T>(value, type, null);
        }

        /// <summary>
        /// Converts this string into the specified type.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to convert string to.</typeparam>
        /// <param name="value">Source string to convert to type.</param>
        /// <param name="type"><see cref="Type"/> to convert string to.</param>
        /// <param name="culture"><see cref="CultureInfo"/> to use for the conversion.</param>
        /// <returns><see cref="string"/> converted to specified <see cref="Type"/>; default value of specified type T if conversion fails.</returns>
        /// <remarks>
        /// This function makes use of a <see cref="TypeConverter"/> to convert this <see cref="string"/> to the specified <paramref name="type"/>,
        /// the best way to make sure <paramref name="value"/> can be converted back to its original type is to use the same
        /// <see cref="TypeConverter"/> to convert the original object to a <see cref="string"/>; see the
        /// <see cref="Common.TypeConvertToString(object)"/> method for an easy way to do this.
        /// </remarks>
        public static T ConvertToType<T>(this string value, Type type, CultureInfo culture)
        {
            // Don't proceed further if string is empty.
            if (string.IsNullOrEmpty(value))
                return default(T);

            // Initialize return type if not specified.
            if ((object)type == null)
                type = typeof(T);

            // Initialize culture info if not specified.
            if ((object)culture == null)
                culture = CultureInfo.InvariantCulture;

            try
            {
                // Handle booleans as a special case to allow numeric entries as well as true/false
                if (type == typeof(bool))
                    return (T)(object)value.ParseBoolean();

                // Handle objects that have type converters (e.g., Enum, Color, Point, etc.)
                TypeConverter converter = TypeDescriptor.GetConverter(type);

                // ReSharper disable once AssignNullToNotNullAttribute
                return (T)converter.ConvertFromString(null, culture, value);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Converts string into a stream using the specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="value">Input to string to convert to a string.</param>
        /// <param name="encoding">String encoding to use; defaults to <see cref="Encoding.UTF8"/>.</param>
        /// <returns>String <paramref name="value"/> encoded onto a stream.</returns>
        public static Stream ToStream(this string value, Encoding encoding = null)
        {
            MemoryStream stream = new MemoryStream();

            using (StreamWriter writer = new StreamWriter(stream, encoding ?? Encoding.UTF8, 4096, true))
            {
                writer.Write(value);
                writer.Flush();
            }

            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Asynchronously converts string into a stream using the specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="value">Input to string to convert to a string.</param>
        /// <param name="encoding">String encoding to use; defaults to <see cref="Encoding.UTF8"/>.</param>
        /// <returns>String <paramref name="value"/> encoded onto a stream.</returns>
        public static async Task<Stream> ToStreamAsync(this string value, Encoding encoding = null)
        {
            MemoryStream stream = new MemoryStream();

            using (StreamWriter writer = new StreamWriter(stream, encoding ?? Encoding.UTF8, 4096, true))
            {
                await writer.WriteAsync(value);
                await writer.FlushAsync();
            }

            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Turns source string into an array of string segments - each with a set maximum width - for parsing or displaying.
        /// </summary>
        /// <param name="value">Input string to break up into segments.</param>
        /// <param name="segmentSize">Maximum size of returned segment.</param>
        /// <returns>Array of string segments as parsed from source string.</returns>
        /// <remarks>Returns a single element array with an empty string if source string is null or empty.</remarks>
        public static string[] GetSegments(this string value, int segmentSize)
        {
            if (segmentSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(segmentSize), "segmentSize must be greater than zero.");

            if (string.IsNullOrEmpty(value))
                return new[] { "" };

            int totalSegments = (int)Math.Ceiling(value.Length / (double)segmentSize);
            string[] segments = new string[totalSegments];

            for (int x = 0; x < segments.Length; x++)
            {
                if (x * segmentSize + segmentSize >= value.Length)
                    segments[x] = value.Substring(x * segmentSize);
                else
                    segments[x] = value.Substring(x * segmentSize, segmentSize);
            }

            return segments;
        }

        /// <summary>
        /// Combines a dictionary of key-value pairs in to a string.
        /// </summary>
        /// <param name="pairs">Dictionary of key-value pairs.</param>
        /// <param name="parameterDelimiter">Character that delimits one key-value pair from another (e.g. ';').</param>
        /// <param name="keyValueDelimiter">Character that delimits a key from its value (e.g. '=').</param>
        /// <param name="startValueDelimiter">Optional character that marks the start of a value such that value could contain other
        /// <paramref name="parameterDelimiter"/> or <paramref name="keyValueDelimiter"/> characters (e.g., "{").</param>
        /// <param name="endValueDelimiter">Optional character that marks the end of a value such that value could contain other
        /// <paramref name="parameterDelimiter"/> or <paramref name="keyValueDelimiter"/> characters (e.g., "}").</param>
        /// <returns>A string of key-value pairs.</returns>
        /// <remarks>
        /// Values will be escaped within <paramref name="startValueDelimiter"/> and <paramref name="endValueDelimiter"/>
        /// to contain nested key/value pair expressions like the following: <c>normalKVP=-1; nestedKVP={p1=true; p2=0.001}</c>,
        /// when either the <paramref name="parameterDelimiter"/> or <paramref name="keyValueDelimiter"/> are detected in the
        /// value of the key/value pair.
        /// </remarks>
        public static string JoinKeyValuePairs(this IDictionary<string, string> pairs, char parameterDelimiter = ';', char keyValueDelimiter = '=', char startValueDelimiter = '{', char endValueDelimiter = '}')
        {
            if ((object)pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            char[] delimiters = { parameterDelimiter, keyValueDelimiter };
            List<string> values = new List<string>();
            string value;

            foreach (string key in pairs.Keys)
            {
                value = pairs[key];

                if (value.IndexOfAny(delimiters) >= 0)
                    value = startValueDelimiter + value + endValueDelimiter;

                values.Add($"{key}{keyValueDelimiter}{value}");
            }

            return string.Join(parameterDelimiter + " ", values);
        }

        /// <summary>
        /// Parses key/value pair expressions from a string. Parameter pairs are delimited by <paramref name="keyValueDelimiter"/>
        /// and multiple pairs separated by <paramref name="parameterDelimiter"/>. Supports encapsulated nested expressions.
        /// </summary>
        /// <param name="value">String containing key/value pair expressions to parse.</param>
        /// <param name="parameterDelimiter">Character that delimits one key/value pair from another.</param>
        /// <param name="keyValueDelimiter">Character that delimits key from value.</param>
        /// <param name="startValueDelimiter">Optional character that marks the start of a value such that value could contain other
        /// <paramref name="parameterDelimiter"/> or <paramref name="keyValueDelimiter"/> characters.</param>
        /// <param name="endValueDelimiter">Optional character that marks the end of a value such that value could contain other
        /// <paramref name="parameterDelimiter"/> or <paramref name="keyValueDelimiter"/> characters.</param>
        /// <param name="ignoreDuplicateKeys">Flag determines whether duplicates are ignored. If flag is set to <c>false</c> an
        /// <see cref="ArgumentException"/> will be thrown when all key parameters are not unique.</param>
        /// <returns>Dictionary of key/value pairs.</returns>
        /// <remarks>
        /// <para>
        /// Parses a string containing key/value pair expressions (e.g., "localPort=5001; transportProtocol=UDP; interface=0.0.0.0").
        /// This method treats all "keys" as case-insensitive. Nesting of key/value pair expressions is allowed by encapsulating the
        /// value using the <paramref name="startValueDelimiter"/> and <paramref name="endValueDelimiter"/> values (e.g., 
        /// "dataChannel={Port=-1;Clients=localhost:8800}; commandChannel={Port=8900}; dataFormat=FloatingPoint;"). There must be one
        /// <paramref name="endValueDelimiter"/> for each encountered <paramref name="startValueDelimiter"/> in the value or a
        /// <see cref="FormatException"/> will be thrown. Multiple levels of nesting is supported. If the <paramref name="ignoreDuplicateKeys"/>
        /// flag is set to <c>false</c> an <see cref="ArgumentException"/> will be thrown when all key parameters are not unique. Note
        /// that keys within nested expressions are considered separate key/value pair strings and are not considered when checking
        /// for duplicate keys.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="ArgumentException">All delimiters must be unique -or- all keys must be unique when
        /// <paramref name="ignoreDuplicateKeys"/> is set to <c>false</c>.</exception>
        /// <exception cref="FormatException">Total nested key/value pair expressions are mismatched -or- encountered
        /// <paramref name="endValueDelimiter"/> before <paramref name="startValueDelimiter"/>.</exception>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static Dictionary<string, string> ParseKeyValuePairs(this string value, char parameterDelimiter = ';', char keyValueDelimiter = '=', char startValueDelimiter = '{', char endValueDelimiter = '}', bool ignoreDuplicateKeys = true)
        {
            if (value == (string)null)
                throw new ArgumentNullException(nameof(value));

            if (parameterDelimiter == keyValueDelimiter ||
                parameterDelimiter == startValueDelimiter ||
                parameterDelimiter == endValueDelimiter ||
                keyValueDelimiter == startValueDelimiter ||
                keyValueDelimiter == endValueDelimiter ||
                startValueDelimiter == endValueDelimiter)
                throw new ArgumentException("All delimiters must be unique");

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            StringBuilder escapedValue = new StringBuilder();
            string escapedParameterDelimiter = parameterDelimiter.RegexEncode();
            string escapedKeyValueDelimiter = keyValueDelimiter.RegexEncode();
            string escapedStartValueDelimiter = startValueDelimiter.RegexEncode();
            string escapedEndValueDelimiter = endValueDelimiter.RegexEncode();
            string backslashDelimiter = '\\'.RegexEncode();
            string[] elements;
            string key, unescapedValue;
            bool valueEscaped = false;
            int delimiterDepth = 0;
            char character;

            // Escape any parameter or key/value delimiters within tagged value sequences
            //      For example, the following string:
            //          "normalKVP=-1; nestedKVP={p1=true; p2=false}")
            //      would be encoded as:
            //          "normalKVP=-1; nestedKVP=p1\\u003dtrue\\u003b p2\\u003dfalse")
            for (int x = 0; x < value.Length; x++)
            {
                character = value[x];

                if (character == startValueDelimiter)
                {
                    if (!valueEscaped)
                    {
                        valueEscaped = true;
                        continue;   // Don't add tag start delimiter to final value
                    }

                    // Handle nested delimiters
                    delimiterDepth++;
                }

                if (character == endValueDelimiter)
                {
                    if (valueEscaped)
                    {
                        if (delimiterDepth > 0)
                        {
                            // Handle nested delimiters
                            delimiterDepth--;
                        }
                        else
                        {
                            valueEscaped = false;
                            continue;   // Don't add tag stop delimiter to final value
                        }
                    }
                    else
                    {
                        throw new FormatException($"Failed to parse key/value pairs: invalid delimiter mismatch. Encountered end value delimiter \'{endValueDelimiter}\' before start value delimiter \'{startValueDelimiter}\'.");
                    }
                }

                if (valueEscaped)
                {
                    // Escape any delimiter characters inside nested key/value pair
                    if (character == parameterDelimiter)
                        escapedValue.Append(escapedParameterDelimiter);
                    else if (character == keyValueDelimiter)
                        escapedValue.Append(escapedKeyValueDelimiter);
                    else if (character == startValueDelimiter)
                        escapedValue.Append(escapedStartValueDelimiter);
                    else if (character == endValueDelimiter)
                        escapedValue.Append(escapedEndValueDelimiter);
                    else if (character == '\\')
                        escapedValue.Append(backslashDelimiter);
                    else
                        escapedValue.Append(character);
                }
                else
                {
                    if (character == '\\')
                        escapedValue.Append(backslashDelimiter);
                    else
                        escapedValue.Append(character);
                }
            }

            if (delimiterDepth != 0 || valueEscaped)
            {
                // If value is still escaped, tagged expression was not terminated
                if (valueEscaped)
                    delimiterDepth = 1;

                throw new FormatException($"Failed to parse key/value pairs: invalid delimiter mismatch. Encountered more {(delimiterDepth > 0 ? "start value delimiters \'" + startValueDelimiter + "\'" : "end value delimiters \'" + endValueDelimiter + "\'")} than {(delimiterDepth < 0 ? "start value delimiters \'" + startValueDelimiter + "\'" : "end value delimiters \'" + endValueDelimiter + "\'")}.");
            }

            // Parse key/value pairs from escaped value
            foreach (string parameter in escapedValue.ToString().Split(parameterDelimiter))
            {
                // Parse out parameter's key/value elements
                elements = parameter.Split(keyValueDelimiter);

                if (elements.Length == 2)
                {
                    // Get key expression
                    key = elements[0].Trim();

                    // Get unescaped value expression
                    unescapedValue = elements[1].Trim().
                        Replace(escapedParameterDelimiter, parameterDelimiter.ToString()).
                        Replace(escapedKeyValueDelimiter, keyValueDelimiter.ToString()).
                        Replace(escapedStartValueDelimiter, startValueDelimiter.ToString()).
                        Replace(escapedEndValueDelimiter, endValueDelimiter.ToString()).
                        Replace(backslashDelimiter, "\\");

                    // Add key/value pair to dictionary
                    if (ignoreDuplicateKeys)
                    {
                        // Add or replace key elements with unescaped value
                        keyValuePairs[key] = unescapedValue;
                    }
                    else
                    {
                        // Add key elements with unescaped value throwing an exception for encountered duplicate keys
                        if (keyValuePairs.ContainsKey(key))
                            throw new ArgumentException($"Failed to parse key/value pairs: duplicate key encountered. Key \"{key}\" is not unique within the string: \"{value}\"");

                        keyValuePairs.Add(key, unescapedValue);
                    }
                }
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Ensures parameter is not an empty or null string. Returns a single space if test value is empty.
        /// </summary>
        /// <param name="testValue">Value to test for null or empty.</param>
        /// <returns>A non-empty string.</returns>
        public static string NotEmpty(this string testValue)
        {
            return testValue.NotEmpty(" ");
        }

        /// <summary>
        /// Ensures parameter is not an empty or null string.
        /// </summary>
        /// <param name="testValue">Value to test for null or empty.</param>
        /// <param name="nonEmptyReturnValue">Value to return if <paramref name="testValue">testValue</paramref> is null or empty.</param>
        /// <returns>A non-empty string.</returns>
        public static string NotEmpty(this string testValue, string nonEmptyReturnValue)
        {
            if (string.IsNullOrEmpty(nonEmptyReturnValue))
                throw new ArgumentException("nonEmptyReturnValue cannot be null or empty");

            if (string.IsNullOrEmpty(testValue))
                return nonEmptyReturnValue;

            return testValue;
        }

        /// <summary>
        /// Replaces all characters passing delegate test with specified replacement character.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="replacementCharacter">Character used to replace characters passing delegate test.</param>
        /// <param name="characterTestFunction">Delegate used to determine whether or not character should be replaced.</param>
        /// <returns>Returns <paramref name="value" /> with all characters passing delegate test replaced.</returns>
        /// <remarks>Allows you to specify a replacement character (e.g., you may want to use a non-breaking space: Convert.ToChar(160)).</remarks>
        public static string ReplaceCharacters(this string value, char replacementCharacter, Func<char, bool> characterTestFunction)
        {
            // <pex>
            if (characterTestFunction == (Func<char, bool>)null)
                throw new ArgumentNullException(nameof(characterTestFunction));
            // </pex>

            if (string.IsNullOrEmpty(value))
                return "";

            StringBuilder result = new StringBuilder();
            char character;

            for (int x = 0; x < value.Length; x++)
            {
                character = value[x];

                if (characterTestFunction(character))
                    result.Append(replacementCharacter);
                else
                    result.Append(character);
            }

            return result.ToString();
        }

        /// <summary>
        /// Removes all characters passing delegate test from a string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="characterTestFunction">Delegate used to determine whether or not character should be removed.</param>
        /// <returns>Returns <paramref name="value" /> with all characters passing delegate test removed.</returns>
        public static string RemoveCharacters(this string value, Func<char, bool> characterTestFunction)
        {
            // <pex>
            if (characterTestFunction == (Func<char, bool>)null)
                throw new ArgumentNullException(nameof(characterTestFunction));
            // </pex>

            if (string.IsNullOrEmpty(value))
                return "";

            StringBuilder result = new StringBuilder();
            char character;

            for (int x = 0; x < value.Length; x++)
            {
                character = value[x];

                if (!characterTestFunction(character))
                    result.Append(character);
            }

            return result.ToString();
        }

        /// <summary>
        /// Removes all white space (as defined by IsWhiteSpace) from a string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>Returns <paramref name="value" /> with all white space removed.</returns>
        public static string RemoveWhiteSpace(this string value)
        {
            return value.RemoveCharacters(char.IsWhiteSpace);
        }

        /// <summary>
        /// Replaces all white space characters (as defined by IsWhiteSpace) with specified replacement character.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="replacementCharacter">Character used to "replace" white space characters.</param>
        /// <returns>Returns <paramref name="value" /> with all white space characters replaced.</returns>
        /// <remarks>Allows you to specify a replacement character (e.g., you may want to use a non-breaking space: Convert.ToChar(160)).</remarks>
        public static string ReplaceWhiteSpace(this string value, char replacementCharacter)
        {
            return value.ReplaceCharacters(replacementCharacter, char.IsWhiteSpace);
        }

        /// <summary>
        /// Removes all control characters from a string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>Returns <paramref name="value" /> with all control characters removed.</returns>
        public static string RemoveControlCharacters(this string value)
        {
            return value.RemoveCharacters(char.IsControl);
        }

        /// <summary>
        /// Replaces all control characters in a string with a single space.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>Returns <paramref name="value" /> with all control characters replaced as a single space.</returns>
        public static string ReplaceControlCharacters(this string value)
        {
            return value.ReplaceControlCharacters(' ');
        }

        /// <summary>
        /// Replaces all control characters in a string with specified replacement character.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="replacementCharacter">Character used to "replace" control characters.</param>
        /// <returns>Returns <paramref name="value" /> with all control characters replaced.</returns>
        /// <remarks>Allows you to specify a replacement character (e.g., you may want to use a non-breaking space: Convert.ToChar(160)).</remarks>
        public static string ReplaceControlCharacters(this string value, char replacementCharacter)
        {
            return value.ReplaceCharacters(replacementCharacter, char.IsControl);
        }

        /// <summary>
        /// Removes all carriage returns and line feeds from a string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>Returns <paramref name="value" /> with all CR and LF characters removed.</returns>
        public static string RemoveCrLfs(this string value)
        {
            return value.RemoveCharacters(c => c == '\r' || c == '\n');
        }

        /// <summary>
        /// Replaces all carriage return and line feed characters (as well as CR/LF sequences) in a string with specified replacement character.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="replacementCharacter">Character used to "replace" CR and LF characters.</param>
        /// <returns>Returns <paramref name="value" /> with all CR and LF characters replaced.</returns>
        /// <remarks>Allows you to specify a replacement character (e.g., you may want to use a non-breaking space: Convert.ToChar(160)).</remarks>
        public static string ReplaceCrLfs(this string value, char replacementCharacter)
        {
            // <pex>
            if (value == (string)null)
                throw new ArgumentNullException(nameof(value));
            // </pex>

            return value.Replace(Environment.NewLine, replacementCharacter.ToString()).ReplaceCharacters(replacementCharacter, c => c == '\r' || c == '\n');
        }

        /// <summary>
        /// Removes duplicate character strings (adjoining replication) in a string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="duplicatedValue">String whose duplicates are to be removed.</param>
        /// <returns>Returns <paramref name="value" /> with all duplicated <paramref name="duplicatedValue" /> removed.</returns>
        public static string RemoveDuplicates(this string value, string duplicatedValue)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (string.IsNullOrEmpty(duplicatedValue))
                return value;

            string duplicate = duplicatedValue + duplicatedValue;

            while (value.IndexOf(duplicate, StringComparison.Ordinal) > -1)
            {
                value = value.Replace(duplicate, duplicatedValue);
            }

            return value;
        }

        /// <summary>
        /// Removes the terminator ('\0') from a null terminated string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>Returns <paramref name="value" /> with all characters to the left of the terminator.</returns>
        public static string RemoveNull(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            int nullPos = value.IndexOf('\0');

            if (nullPos > -1)
                return value.Substring(0, nullPos);

            return value;
        }

        /// <summary>
        /// Replaces all repeating white space with a single space.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>Returns <paramref name="value" /> with all duplicate white space removed.</returns>
        public static string RemoveDuplicateWhiteSpace(this string value)
        {
            return value.RemoveDuplicateWhiteSpace(' ');
        }

        /// <summary>
        /// Replaces all repeating white space with specified spacing character.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="spacingCharacter">Character value to use to insert as single white space value.</param>
        /// <returns>Returns <paramref name="value" /> with all duplicate white space removed.</returns>
        /// <remarks>This function allows you to specify spacing character (e.g., you may want to use a non-breaking space: <c>Convert.ToChar(160)</c>).</remarks>
        public static string RemoveDuplicateWhiteSpace(this string value, char spacingCharacter)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            StringBuilder result = new StringBuilder();
            bool lastCharWasSpace = false;
            char character;

            for (int x = 0; x < value.Length; x++)
            {
                character = value[x];

                if (char.IsWhiteSpace(character))
                {
                    lastCharWasSpace = true;
                }
                else
                {
                    if (lastCharWasSpace)
                        result.Append(spacingCharacter);

                    result.Append(character);
                    lastCharWasSpace = false;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Removes all invalid file name characters (\ / : * ? " &lt; &gt; |) from a string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>Returns <paramref name="value" /> with all invalid file name characters removed.</returns>
        public static string RemoveInvalidFileNameCharacters(this string value)
        {
            return value.RemoveCharacters(c => Array.IndexOf(Path.GetInvalidFileNameChars(), c) >= 0);
        }

        /// <summary>
        /// Replaces all invalid file name characters (\ / : * ? " &lt; &gt; |) in a string with the specified <paramref name="replacementCharacter"/>.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="replacementCharacter">Character used to replace the invalid characters.</param>
        /// <returns>>Returns <paramref name="value" /> with all invalid file name characters replaced.</returns>
        public static string ReplaceInvalidFileNameCharacters(this string value, char replacementCharacter)
        {
            return value.ReplaceCharacters(replacementCharacter, c => Array.IndexOf(Path.GetInvalidFileNameChars(), c) >= 0);
        }

        /// <summary>
        /// Wraps <paramref name="value"/> in the <paramref name="quoteChar"/>.
        /// </summary>
        /// <param name="value">Input string to process</param>
        /// <param name="quoteChar">The char to wrap <paramref name="value"/></param>
        /// <returns><paramref name="value"/> wrapped in <paramref name="quoteChar"/></returns>
        public static string QuoteWrap(this string value, char quoteChar = '\"')
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (quoteChar == 0)
                return value;

            if (value[0] != quoteChar)
                value = string.Concat(quoteChar, value);

            if (value[value.Length - 1] != quoteChar)
                value = string.Concat(value, quoteChar);

            if (!value.StartsWith(quoteChar.ToString(), StringComparison.Ordinal))
                value = string.Concat(quoteChar, value);

            return value;
        }

        /// <summary>
        /// Counts the total number of the occurrences of a character in the given string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="characterToCount">Character to be counted.</param>
        /// <returns>Total number of the occurrences of <paramref name="characterToCount" /> in the given string.</returns>
        public static int CharCount(this string value, char characterToCount)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            int total = 0;

            for (int x = 0; x < value.Length; x++)
            {
                if (value[x] == characterToCount)
                    total++;
            }

            return total;
        }

        /// <summary>
        /// Tests to see if a string is contains only digits based on Char.IsDigit function.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>True, if all string's characters are digits; otherwise, false.</returns>
        /// <seealso cref="char.IsDigit(char)"/>
        public static bool IsAllDigits(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            if (value.Length == 0)
                return false;

            for (int x = 0; x < value.Length; x++)
            {
                if (!char.IsDigit(value[x]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests to see if a string contains only numbers based on Char.IsNumber function.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>True, if all string's characters are numbers; otherwise, false.</returns>
        /// <seealso cref="char.IsNumber(char)"/>
        public static bool IsAllNumbers(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            if (value.Length == 0)
                return false;

            for (int x = 0; x < value.Length; x++)
            {
                if (!char.IsNumber(value[x]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests to see if a string's letters are all upper case.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>True, if all string's letter characters are upper case; otherwise, false.</returns>
        public static bool IsAllUpper(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            if (value.Length == 0)
                return false;

            for (int x = 0; x < value.Length; x++)
            {
                if (char.IsLetter(value[x]) && !char.IsUpper(value[x]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests to see if a string's letters are all lower case.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>True, if all string's letter characters are lower case; otherwise, false.</returns>
        public static bool IsAllLower(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            if (value.Length == 0)
                return false;

            for (int x = 0; x < value.Length; x++)
            {
                if (char.IsLetter(value[x]) && !char.IsLower(value[x]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests to see if a string contains only letters.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>True, if all string's characters are letters; otherwise, false.</returns>
        /// <remarks>Any non-letter character (e.g., punctuation marks) causes this function to return false (See overload to ignore punctuation
        /// marks.).</remarks>
        public static bool IsAllLetters(this string value)
        {
            return value.IsAllLetters(false);
        }

        /// <summary>
        /// Tests to see if a string contains only letters.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="ignorePunctuation">Set to True to ignore punctuation.</param>
        /// <returns>True, if all string's characters are letters; otherwise, false.</returns>
        public static bool IsAllLetters(this string value, bool ignorePunctuation)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            if (value.Length == 0)
                return false;

            for (int x = 0; x < value.Length; x++)
            {
                if (ignorePunctuation)
                {
                    if (!(char.IsLetter(value[x]) || char.IsPunctuation(value[x])))
                        return false;
                }
                else
                {
                    if (!char.IsLetter(value[x]))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tests to see if a string contains only letters or digits.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>True, if all string's characters are either letters or digits; otherwise, false.</returns>
        /// <remarks>Any non-letter, non-digit character (e.g., punctuation marks) causes this function to return false (See overload to ignore
        /// punctuation marks.).</remarks>
        public static bool IsAllLettersOrDigits(this string value)
        {
            return value.IsAllLettersOrDigits(false);
        }

        /// <summary>
        /// Tests to see if a string contains only letters or digits.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="ignorePunctuation">Set to True to ignore punctuation.</param>
        /// <returns>True, if all string's characters are letters or digits; otherwise, false.</returns>
        public static bool IsAllLettersOrDigits(this string value, bool ignorePunctuation)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            if (value.Length == 0)
                return false;

            for (int x = 0; x < value.Length; x++)
            {
                if (ignorePunctuation)
                {
                    if (!(char.IsLetterOrDigit(value[x]) || char.IsPunctuation(value[x])))
                        return false;
                }
                else
                {
                    if (!char.IsLetterOrDigit(value[x]))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Test to see if the provided string is null or contains only whitespace characters.
        /// </summary>
        /// <param name="value">the value to test.</param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if ((value == null) || (value.Length == 0))
                return true;
            foreach (char c in value)
                if (!Char.IsWhiteSpace(c))
                    return false;
            return true;
        }

        /// <summary>
        /// Decodes the specified Regular Expression character back into a standard Unicode character.
        /// </summary>
        /// <param name="value">Regular Expression character to decode back into a Unicode character.</param>
        /// <returns>Standard Unicode character representation of specified Regular Expression character.</returns>
        public static char RegexDecode(this string value)
        {
            // <pex>
            if (value == (string)null)
                throw new ArgumentNullException(nameof(value));
            // </pex>

            return Convert.ToChar(Convert.ToUInt16(value.Replace("\\u", "0x"), 16));
        }

        /// <summary>
        /// Encodes a string into a base-64 string.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <remarks>
        /// <para>Performs a base-64 style of string encoding useful for data obfuscation or safe XML data string transmission.</para>
        /// <para>Note: This function encodes a "String". Use the Convert.ToBase64String function to encode a binary data buffer.</para>
        /// </remarks>
        /// <returns>A <see cref="string"></see> value representing the encoded input of <paramref name="value"/>.</returns>
        public static string Base64Encode(this string value)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(value));
        }

        /// <summary>
        /// Decodes a given base-64 encoded string encoded with <see cref="Base64Encode" />.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <remarks>Note: This function decodes value back into a "String". Use the Convert.FromBase64String function to decode a base-64 encoded
        /// string back into a binary data buffer.</remarks>
        /// <returns>A <see cref="string"></see> value representing the decoded input of <paramref name="value"/>.</returns>
        public static string Base64Decode(this string value)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(value));
        }

        /// <summary>
        /// Converts the given string into a <see cref="SecureString"/>.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <returns>The given string as a <see cref="SecureString"/>.</returns>
        public static SecureString ToSecureString(this string value)
        {
            SecureString secureString;

            if ((object)value == null)
                return null;

            unsafe
            {
                fixed (char* chars = value)
                {
                    secureString = new SecureString(chars, value.Length);
                    secureString.MakeReadOnly();
                    return secureString;
                }
            }
        }

        /// <summary>
        /// Converts the given <see cref="SecureString"/> into a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The <see cref="SecureString"/> to be converted.</param>
        /// <returns>The given <see cref="SecureString"/> as a <see cref="string"/>.</returns>
        /// <remarks>
        /// This method is UNSAFE, as it stores your secure string data in clear text in memory.
        /// Since strings are immutable, that memory cannot be cleaned up until all references to
        /// the string are removed and the garbage collector deallocates it. Only use this method
        /// to interface with APIs that do not support the use of <see cref="SecureString"/> for
        /// sensitive text data.
        /// </remarks>
        public static string ToUnsecureString(this SecureString value)
        {
            IntPtr intPtr;

            if ((object)value == null)
                return null;

            intPtr = IntPtr.Zero;

            try
            {
                intPtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(intPtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
            }
        }

        /// <summary>
        /// Converts the provided string into title case (upper case first letter of each word).
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="culture">The <see cref="CultureInfo" /> that corresponds to the language rules applied for title casing of words; defaults to <see cref="CultureInfo.CurrentCulture"/>.</param>
        /// <remarks>
        /// Note: This function performs "ToLower" in input string then applies <see cref="TextInfo.ToTitleCase"/> for specified <paramref name="culture"/>.
        /// This way, even strings formatted in all-caps will still be properly formatted.
        /// </remarks>
        /// <returns>A <see cref="string"/> that has the first letter of each word capitalized.</returns>
        public static string ToTitleCase(this string value, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if ((object)culture == null)
                culture = CultureInfo.CurrentCulture;

            return culture.TextInfo.ToTitleCase(value.ToLower());
        }

        /// <summary>
        /// Converts first letter of string to upper-case.
        /// </summary>
        /// <param name="value">String to convert to pascal case.</param>
        /// <returns><paramref name="value"/> with first letter as upper-case.</returns>
        /// <remarks>
        /// This function will automatically trim <paramref name="value"/>.
        /// </remarks>
        public static string ToPascalCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            value = value.Trim();

            return char.ToUpperInvariant(value[0]) + (value.Length > 1 ? value.Substring(1) : "");
        }

        /// <summary>
        /// Converts first letter of string to lower-case.
        /// </summary>
        /// <param name="value">String to convert to camel case.</param>
        /// <returns><paramref name="value"/> with first letter as lower-case.</returns>
        /// <remarks>
        /// This function will automatically trim <paramref name="value"/>.
        /// </remarks>
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            value = value.Trim();

            return char.ToLowerInvariant(value[0]) + (value.Length > 1 ? value.Substring(1) : "");
        }

        /// <summary>
        /// Truncates the provided string from the left if it is longer that specified length.
        /// </summary>
        /// <param name="value">A <see cref="string"/> value that is to be truncated.</param>
        /// <param name="maxLength">The maximum number of characters that <paramref name="value"/> can be.</param>
        /// <returns>A <see cref="string"/> that is the truncated version of the <paramref name="value"/> string.</returns>
        public static string TruncateLeft(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Length > maxLength)
                return value.Substring(value.Length - maxLength);

            return value;
        }

        /// <summary>
        /// Truncates the provided string from the right if it is longer that specified length.
        /// </summary>
        /// <param name="value">A <see cref="string"/> value that is to be truncated.</param>
        /// <param name="maxLength">The maximum number of characters that <paramref name="value"/> can be.</param>
        /// <returns>A <see cref="string"/> that is the truncated version of the <paramref name="value"/> string.</returns>
        public static string TruncateRight(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Length > maxLength)
                return value.Substring(0, maxLength);

            return value;
        }

        /// <summary>
        /// Centers text within the specified maximum length, biased to the left.
        /// Text will be padded to the left and right with spaces.
        /// If value is greater than specified maximum length, value returned will be truncated from the right.
        /// </summary>
        /// <remarks>
        /// Handles multiple lines of text separated by Environment.NewLine.
        /// </remarks>
        /// <param name="value">A <see cref="string"/> to be centered.</param>
        /// <param name="maxLength">An <see cref="Int32"/> that is the maximum length of padding.</param>
        /// <returns>The centered string value.</returns>
        public static string CenterText(this string value, int maxLength)
        {
            return value.CenterText(maxLength, ' ');
        }

        /// <summary>
        /// Centers text within the specified maximum length, biased to the left.
        /// Text will be padded to the left and right with specified padding character.
        /// If value is greater than specified maximum length, value returned will be truncated from the right.
        /// </summary>
        /// <remarks>
        /// Handles multiple lines of text separated by <c>Environment.NewLine</c>.
        /// </remarks>
        /// <param name="value">A <see cref="string"/> to be centered.</param>
        /// <param name="maxLength">An <see cref="Int32"/> that is the maximum length of padding.</param>
        /// <param name="paddingCharacter">The <see cref="char"/> value to pad with.</param>
        /// <returns>The centered string value.</returns>
        public static string CenterText(this string value, int maxLength, char paddingCharacter)
        {
            if ((object)value == null)
                value = "";

            // If the text to be centered contains multiple lines, centers all the lines individually.
            StringBuilder result = new StringBuilder();
            string[] lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string line;
            int lastLineIndex = lines.Length - 1; //(lines.Length != 0 && lines[lines.Length - 1].Trim() == string.Empty ? lines.Length - 2 : lines.Length - 1);

            for (int i = 0; i <= lastLineIndex; i++)
            {
                // Gets current line.
                line = lines[i];

                // Skips the last empty line as a result of split if original text had multiple lines.
                if (i == lastLineIndex && line.Trim().Length == 0)
                    continue;

                if (line.Length >= maxLength)
                {
                    // Truncates excess characters on the right.
                    result.Append(line.Substring(0, maxLength));
                }
                else
                {
                    int remainingSpace = maxLength - line.Length;
                    int leftSpaces;
                    int rightSpaces;

                    // Splits remaining space between the left and the right.
                    leftSpaces = remainingSpace / 2;
                    rightSpaces = leftSpaces;

                    // Adds any remaining odd space to the right (bias text to the left).
                    if (remainingSpace % 2 > 0)
                        rightSpaces++;

                    result.Append(new string(paddingCharacter, leftSpaces));
                    result.Append(line);
                    result.Append(new string(paddingCharacter, rightSpaces));
                }

                // Creates a new line only if the original text contains multiple lines.
                if (i < lastLineIndex)
                    result.AppendLine();
            }

            return result.ToString();
        }
        /// <summary>
        /// Performs a case insensitive string replacement.
        /// </summary>
        /// <param name="value">The string to examine.</param>
        /// <param name="fromText">The value to replace.</param>
        /// <param name="toText">The new value to be inserted</param>
        /// <returns>A string with replacements.</returns>
        public static string ReplaceCaseInsensitive(this string value, string fromText, string toText)
        {
            return (new Regex(Regex.Escape(fromText), RegexOptions.IgnoreCase | RegexOptions.Multiline)).Replace(value, toText);
        }

        /// <summary>
        /// Ensures a string starts with a specific character.
        /// </summary>
        /// <param name="value">Input string to process.</param>
        /// <param name="startChar">The character desired at string start.</param>
        /// <returns>The sent string with character at the start.</returns>
        public static string EnsureStart(this string value, char startChar)
        {
            return EnsureStart(value, startChar, false);
        }

        /// <summary>
        /// Ensures a string starts with a specific character.
        /// </summary>
        /// <param name="value">Input string to process.</param>
        /// <param name="startChar">The character desired at string start.</param>
        /// <param name="removeRepeatingChar">Set to <c>true</c> to ensure one and only one instance of <paramref name="startChar"/>.</param>
        /// <returns>The sent string with character at the start.</returns>
        public static string EnsureStart(this string value, char startChar, bool removeRepeatingChar)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (startChar == 0)
                return value;

            if (value[0] == startChar)
            {
                if (removeRepeatingChar)
                    return value.Substring(LastIndexOfRepeatedChar(value, startChar, 0));

                return value;
            }

            return string.Concat(startChar, value);
        }

        /// <summary>
        /// Ensures a string starts with a specific string.
        /// </summary>
        /// <param name="value">Input string to process.</param>
        /// <param name="startString">The string desired at string start.</param>
        /// <returns>The sent string with string at the start.</returns>
        public static string EnsureStart(this string value, string startString)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (string.IsNullOrEmpty(startString))
                return value;

            if (value.IndexOf(startString, StringComparison.Ordinal) == 0)
                return value;

            return string.Concat(startString, value);
        }

        /// <summary>
        /// Ensures a string ends with a specific character.
        /// </summary>
        /// <param name="value">Input string to process.</param>
        /// <param name="endChar">The character desired at string's end.</param>
        /// <returns>The sent string with character at the end.</returns>
        public static string EnsureEnd(this string value, char endChar)
        {
            return EnsureEnd(value, endChar, false);
        }

        /// <summary>
        /// Ensures a string ends with a specific character.
        /// </summary>
        /// <param name="value">Input string to process.</param>
        /// <param name="endChar">The character desired at string's end.</param>
        /// <param name="removeRepeatingChar">Set to <c>true</c> to ensure one and only one instance of <paramref name="endChar"/>.</param>
        /// <returns>The sent string with character at the end.</returns>
        public static string EnsureEnd(this string value, char endChar, bool removeRepeatingChar)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (endChar == 0)
                return value;

            if (value[value.Length - 1] == endChar)
            {
                if (removeRepeatingChar)
                {
                    int i = LastIndexOfRepeatedChar(value.Reverse(), endChar, 0);
                    return value.Substring(0, value.Length - i);
                }

                return value;
            }

            return string.Concat(value, endChar);
        }

        /// <summary>
        /// Ensures a string ends with a specific string.
        /// </summary>
        /// <param name="value">Input string to process.</param>
        /// <param name="endString">The string desired at string's end.</param>
        /// <returns>The sent string with string at the end.</returns>
        public static string EnsureEnd(this string value, string endString)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (string.IsNullOrEmpty(endString))
                return value;

            if (value.EndsWith(endString, StringComparison.Ordinal))
                return value;

            return string.Concat(value, endString);
        }

        /// <summary>
        /// Reverses the order of the characters in a string.
        /// </summary>
        /// <param name="value">Input string to process.</param>
        /// <returns>The reversed string.</returns>
        public static string Reverse(this string value)
        {
            // Experimented with several approaches.  This is the fastest.
            // Replaced original code that yielded 1.5% performance increase.
            // This code is faster than Array.Reverse.

            if (string.IsNullOrEmpty(value))
                return "";

            char[] arrChar = value.ToCharArray();
            char temp;
            int arrLength = arrChar.Length;
            int j;

            // Works for odd and even length strings since middle char is not swapped for an odd length string
            for (int i = 0; i < arrLength / 2; i++)
            {
                j = arrLength - i - 1;
                temp = arrChar[i];
                arrChar[i] = arrChar[j];
                arrChar[j] = temp;
            }

            return new string(arrChar);
        }

        /// <summary>
        /// Searches a string for a repeated instance of the specified <paramref name="characterToFind"/> from specified <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <param name="characterToFind">The character of interest.</param>
        /// <param name="startIndex">The index from which to begin the search.</param>
        /// <returns>The index of the first instance of the character that is repeated or (-1) if no repeated chars found.</returns>
        public static int IndexOfRepeatedChar(this string value, char characterToFind, int startIndex)
        {
            if (string.IsNullOrEmpty(value))
                return -1;

            if (startIndex < 0)
                return -1;

            if (characterToFind == 0)
                return -1;

            char c = (char)0;

            for (int i = startIndex; i < value.Length; i++)
            {
                if (value[i] == characterToFind)
                {
                    if (value[i] != c)
                        c = value[i];
                    else
                    {
                        //at least one repeating character
                        return i - 1;
                    }
                }
                else
                {
                    c = (char)0;
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches a string for a repeated instance of the specified <paramref name="characterToFind"/>.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <param name="characterToFind">The character of interest.</param>
        /// <returns>The index of the first instance of the character that is repeated or (-1) if no repeated chars found.</returns>
        public static int IndexOfRepeatedChar(this string value, char characterToFind)
        {
            return IndexOfRepeatedChar(value, characterToFind, 0);
        }

        /// <summary>
        /// Searches a string for an instance of a repeated character.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <returns>The index of the first instance of any character that is repeated or (-1) if no repeated chars found.</returns>
        public static int IndexOfRepeatedChar(this string value)
        {
            return IndexOfRepeatedChar(value, 0);
        }

        /// <summary>
        /// Searches a string for an instance of a repeated character from specified <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <param name="startIndex">The index from which to begin the search.</param>
        /// <returns>The index of the first instance of any character that is repeated or (-1) if no repeated chars found.</returns>
        public static int IndexOfRepeatedChar(this string value, int startIndex)
        {
            if (string.IsNullOrEmpty(value))
                return -1;

            if (startIndex < 0)
                return -1;

            char c = (char)0;

            for (int i = startIndex; i < value.Length; i++)
            {
                if (value[i] != c)
                    c = value[i];
                else
                {
                    //at least one repeating character
                    return i - 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the last repeated index of the first group of repeated characters that begin with the <paramref name="characterToFind"/>.
        /// </summary>
        /// <param name="value">String to process.</param>
        /// <param name="characterToFind">The character of interest.</param>
        /// <param name="startIndex">The index from which to begin the search.</param>
        /// <returns>The index of the last instance of the character that is repeated or (-1) if no repeated chars found.</returns>
        private static int LastIndexOfRepeatedChar(string value, char characterToFind, int startIndex)
        {
            if (startIndex > value.Length - 1)
                return -1;

            int i = value.IndexOf(characterToFind, startIndex);

            if (i == -1)
                return -1;

            for (int j = i + 1; j < value.Length; j++)
            {
                if (value[j] != characterToFind)
                    return j - 1;
            }

            return value.Length - 1;
        }

        /// <summary>
        /// Places an ellipsis in the middle of a string as it is trimmed to length specified.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <param name="length">The maximum returned string length; minimum value is 5.</param>
        /// <returns>
        /// A trimmed string of the specified <paramref name="length"/> or empty string if <paramref name="value"/> is null or empty.
        /// </returns>
        /// <remarks>
        /// Returned string is not padded to fill field length if <paramref name="value"/> is shorter than length.
        /// </remarks>
        public static string TrimWithEllipsisMiddle(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (length < 5)
                length = 5;

            value = value.Trim();

            if (value.Length <= length)
                return value;

            int s1_Len = (int)(length / 2) - 1;

            return string.Concat(value.Substring(0, s1_Len), "...", value.Substring(value.Length - s1_Len + 1 - length % 2));
        }

        /// <summary>
        /// Places an ellipsis at the end of a string as it is trimmed to length specified.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <param name="length">The maximum returned string length; minimum value is 5.</param>
        /// <returns>
        /// A trimmed string of the specified <paramref name="length"/> or empty string if <paramref name="value"/> is null or empty.
        /// </returns>
        /// <remarks>
        /// Returned string is not padded to fill field length if <paramref name="value"/> is shorter than length.
        /// </remarks>
        public static string TrimWithEllipsisEnd(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (length < 5)
                length = 5;

            value = value.Trim();

            if (value.Length <= length)
                return value;

            return string.Concat(value.Substring(0, length - 3), "...");
        }

        /// <summary>
        /// Escapes string using URL encoding.
        /// </summary>
        /// <param name="value">The string to escape.</param>
        /// <returns>URL encoded string.</returns>
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string UriEncode(this string value)
        {
            return Uri.EscapeDataString(value);
        }
    }
}