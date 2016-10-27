//******************************************************************************************************
//  RateLimiter.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Threading
{
    /// <summary>
    /// A rate limiting system based on tokes.
    /// </summary>
    public class RateLimiter
    {
        private double m_maxTokens;
        private double m_tokens;
        private double m_tokenPerSecond;
        private ShortTime m_updateTime;

        /// <summary>
        /// Creates a <see cref="RateLimiter"/>
        /// </summary>
        /// <param name="tokensPerSecond">The number of tokens per second that can be generated. Must be greater than zero and less than 1 billion</param>
        /// <param name="maxTokensQueue">The maximum number of tokens in the queue. Must be at least 1</param>
        public RateLimiter(double tokensPerSecond = 10, int maxTokensQueue = 10)
        {
            UpdateLimits(tokensPerSecond, maxTokensQueue);
        }

        /// <summary>
        /// Updates the limits associated with this rate limiter. Note, after this update, the tokens will be completely resupplied.
        /// </summary>
        /// <param name="tokensPerSecond">The number of tokens per second that can be generated. Must be greater than zero and less than 1 billion</param>
        /// <param name="maxTokensQueue">The maximum number of tokens in the queue. Must be at least 1</param>
        public void UpdateLimits(double tokensPerSecond, int maxTokensQueue)
        {
            if (double.IsNaN(tokensPerSecond))
                throw new ArgumentException("Cannot be NaN", "tokensPerSecond");
            tokensPerSecond = Math.Min(tokensPerSecond, 1000000000); //1 Billion tokens per second.
            tokensPerSecond = Math.Max(tokensPerSecond, 1.0 / 24.0 / 3600.0); //1 token per day.

            if (maxTokensQueue < 1)
                throw new ArgumentOutOfRangeException("maxTokensQueue");

            m_maxTokens = maxTokensQueue;
            m_tokens = maxTokensQueue;
            m_tokenPerSecond = tokensPerSecond;
            m_updateTime = ShortTime.Now;
        }

        /// <summary>
        /// Attempts to take a token from the rate limiter.
        /// </summary>
        /// <returns>true if token was successfully taken, False if all tokens were consumed.</returns>
        public bool TryTakeToken()
        {
            var currentTime = ShortTime.Now;
            var tokensGenerated = m_updateTime.ElapsedSeconds(currentTime) * m_tokenPerSecond;

            //To limit rounding errors at high call rates, 
            //do not add a token until it acumiliates to a relatively large value.
            if (tokensGenerated > 0.5)
            {
                m_tokens = Math.Min(m_tokens + tokensGenerated, m_maxTokens);
                m_updateTime = currentTime;
            }

            if (m_tokens < 0.99)
                return false;

            m_tokens--;
            return true;
        }
    }

}
