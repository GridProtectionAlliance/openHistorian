//******************************************************************************************************
//  SortedTreeTypeBase`1.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/1/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace GSF.SortedTreeStore.Tree
{
    /// <summary>
    /// The interface that is required to use as a value in <see cref="SortedTree"/> 
    /// </summary>
    /// <typeparam name="T">A class that has a default constructor</typeparam>
    /// <remarks>
    /// It is highly recommended to override many of the base class methods as many of these methods are slow.
    /// 
    /// The following methods should be overriden if possible:
    /// Read(byte*)
    /// Write(byte*)
    /// IsLessThan(T)
    /// IsEqualTo(T)
    /// IsGreaterThan(T)
    /// IsLessThanOrEqualTo(T)
    /// IsBetween(T,T)
    /// 
    /// For better random I/O inserts, it is also a good idea to implement a custom
    /// <see cref="SortedTreeTypeMethods{T}"/> that overrides 
    /// the <see cref="SortedTreeTypeMethods{T}.BinarySearch"/> method.
    /// </remarks>
    public abstract class SortedTreeTypeBase<T>
        : SortedTreeTypeBase, IComparable<T>, IEquatable<T>, IComparer<T>
        where T : SortedTreeTypeBase<T>, new()
    {
        /// <summary>
        /// Copies the source to the destination
        /// </summary>
        /// <param name="destination"></param>
        public abstract void CopyTo(T destination);

        /// <summary>
        /// Compares the current instance to <see cref="other"/>.
        /// </summary>
        /// <param name="other">the key to compare to</param>
        /// <returns></returns>
        public abstract int CompareTo(T other);
        
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
        public virtual int Compare(T x, T y)
        {
            return x.CompareTo(y);
        }

        /// <summary>
        /// Creates a class that contains the necessary methods for the SortedTree.
        /// </summary>
        /// <returns></returns>
        public virtual SortedTreeTypeMethods<T> CreateValueMethods()
        {
            return new SortedTreeTypeMethods<T>();
        }

        /// <summary>
        /// Is the current instance equal to <see cref="right"/>
        /// </summary>
        /// <param name="right">the key to compare to</param>
        /// <returns></returns>
        public virtual bool IsEqualTo(T right)
        {
            return CompareTo(right) == 0;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual bool Equals(T other)
        {
            return IsEqualTo(other);
        }

        /// <summary>
        /// Gets if left != right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsNotEqualTo(T right)
        {
            return CompareTo(right) != 0;
        }

        /// <summary>
        /// Gets if lowerBounds &lt;= key &lt; upperBounds
        /// </summary>
        /// <param name="lowerBounds"></param>
        /// <param name="upperBounds"></param>
        /// <returns></returns>
        public virtual bool IsBetween(T lowerBounds, T upperBounds)
        {
            return lowerBounds.IsLessThanOrEqualTo((T)this) && IsLessThan(upperBounds);
        }

        /// <summary>
        /// Gets if left &lt;= right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsLessThanOrEqualTo(T right)
        {
            return CompareTo(right) <= 0;
        }

        /// <summary>
        /// Gets if left &lt; right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsLessThan(T right)
        {
            return CompareTo(right) < 0;
        }

        /// <summary>
        /// Gets if left &gt; right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsGreaterThan(T right)
        {
            return CompareTo(right) > 0;
        }

        /// <summary>
        /// Gets if left &gt;= right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsGreaterThanOrEqualTo(T right)
        {
            return CompareTo(right) >= 0;
        }
    }
}
