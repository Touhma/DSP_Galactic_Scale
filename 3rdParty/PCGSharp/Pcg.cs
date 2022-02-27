// MIT License
// 
// Copyright (c) 2016 Bismur Studios Ltd.
// Copyright (c) 2016 Ioannis Giagkiozis
//
// This file is part of PCGSharp.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all copies or substantial 
//  portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
//  LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
//  NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
//  SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// This file is based on PCG, the original has the following license: 
/*
 * PCG Random Number Generation for C.
 *
 * Copyright 2014 Melissa O'Neill <oneill@pcg-random.org>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * For additional information about the PCG random number generation scheme,
 * including its license and other licensing options, visit
 *
 *       http://www.pcg-random.org
 */

using System;

// Note, you will see that API is duplicated in PCG and PCGExtended, this is not by accident or 
// neglect. It would be "cleaner" to have a base class and just implement the core of the generator
// in derived classes, however, this has a performance cost which for random number generators, in my
// view, is too high. That said, there is still some room for performance optimisation.

namespace PCGSharp
{
    /// <summary>
    ///     PCG (Permuted Congruential Generator) is a C# port from C the base PCG generator
    ///     presented in "PCG: A Family of Simple Fast Space-Efficient Statistically Good
    ///     Algorithms for Random Number Generation" by Melissa E. O'Neill. The code follows closely the one
    ///     made available by O'Neill at her site: http://www.pcg-random.org/download.html
    ///     To understand how exactly this generator works read this:
    ///     http://www.pcg-random.org/pdf/toms-oneill-pcg-family-v1.02.pdf
    /// </summary>
    public class Pcg
    {
        // This shifted to the left and or'ed with 1ul results in the default increment.
        private const ulong ShiftedIncrement = 721347520444481703ul;
        private const ulong Multiplier = 6364136223846793005ul;
        private const double ToDouble01 = 1.0 / 4294967296.0;
        private ulong _increment = 1442695040888963407ul;

        private ulong _state;

        public Pcg(int seed) : this((ulong)seed)
        {
        }

        public Pcg(ulong seed, ulong sequence = ShiftedIncrement)
        {
            Initialize(seed, sequence);
        }

        public int Next()
        {
            var result = NextUInt();
            return (int)(result >> 1);
        }

        public int Next(int maxExclusive)
        {
            if (maxExclusive <= 0)
                throw new ArgumentException("Max Exclusive must be positive");
            var uMaxExclusive = (uint)maxExclusive;
            var threshold = (uint)-uMaxExclusive % uMaxExclusive;
            while (true)
            {
                var result = NextUInt();
                if (result >= threshold)
                    return (int)(result % uMaxExclusive);
            }
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
                throw new ArgumentException("MaxExclusive must be larger than MinInclusive");
            var uMaxExclusive = unchecked((uint)(maxExclusive - minInclusive));
            var threshold = (uint)-uMaxExclusive % uMaxExclusive;

            while (true)
            {
                var result = NextUInt();
                if (result >= threshold)
                    return (int)unchecked(result % uMaxExclusive + minInclusive);
            }
        }

        public virtual uint NextUInt()
        {
            var oldState = _state;
            _state = unchecked(oldState * Multiplier + _increment);
            var xorShifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
            var rot = (int)(oldState >> 59);
            var result = (xorShifted >> rot) | (xorShifted << (-rot & 31));
            return result;
        }

        public uint NextUInt(uint maxExclusive)
        {
            var threshold = (uint)-maxExclusive % maxExclusive;

            while (true)
            {
                var result = NextUInt();
                if (result >= threshold)
                    return result % maxExclusive;
            }
        }

        public uint NextUInt(uint minInclusive, uint maxExclusive)
        {
            if (maxExclusive <= minInclusive)
                throw new ArgumentException();

            var diff = maxExclusive - minInclusive;
            var threshold = (uint)-diff % diff;

            while (true)
            {
                var result = NextUInt();
                if (result >= threshold)
                    return result % diff + minInclusive;
            }
        }

        public float NextFloat()
        {
            return (float)(NextUInt() * ToDouble01);
        }

        public float NextFloat(float maxInclusive)
        {
            if (maxInclusive <= 0)
                throw new ArgumentException("MaxInclusive must be larger than 0");

            return (float)(NextUInt() * ToDouble01) * maxInclusive;
        }

        public float NextFloat(float minInclusive, float maxInclusive)
        {
            if (maxInclusive < minInclusive)
                throw new ArgumentException("Max must be larger than min");

            return (float)(NextUInt() * ToDouble01) * (maxInclusive - minInclusive) + minInclusive;
        }

        public double NextDouble()
        {
            return NextUInt() * ToDouble01;
        }

        public double NextDouble(double maxInclusive)
        {
            if (maxInclusive <= 0)
                throw new ArgumentException("Max must be larger than 0");

            return NextUInt() * ToDouble01 * maxInclusive;
        }

        public double NextDouble(double minInclusive, double maxInclusive)
        {
            if (maxInclusive < minInclusive)
                throw new ArgumentException("Max must be larger than min");

            return NextUInt() * ToDouble01 * (maxInclusive - minInclusive) + minInclusive;
        }

        public bool NextBool()
        {
            var result = NextUInt();
            return result % 2 == 1;
        }

        public void SetStream(ulong sequence)
        {
            _increment = (sequence << 1) | 1;
        }

        protected void reseed(int seed)
        {
            Initialize((ulong)seed, ShiftedIncrement);
        }

        protected void Initialize(ulong seed, ulong initseq)
        {
            _state = 0ul;
            SetStream(initseq);
            NextUInt();
            _state += seed;
            NextUInt();
        }
    }
}