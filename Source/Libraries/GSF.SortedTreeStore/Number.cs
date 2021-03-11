using System;

namespace GSF
{
    //Got code ideas from http://code.google.com/p/stringencoders/
    //specifically from: http://code.google.com/p/stringencoders/source/browse/trunk/src/modp_numtoa.c

    public static class Number
    {
        private static readonly double[] PowersOf10d = new double[] { 1f, 10f, 100f, 1000f, 10000f, 100000f, 1000000f, 10000000f, 100000000f, 1000000000f, 10000000000f, 100000000000f };

        public static int WriteToChars(this float value, char[] str, int position)
        {
            int pos = position;
            if (str.Length - position < 32)
                throw new Exception("Insufficient buffer space");

            if (Single.IsNaN(value))
            {
                str[0] = 'n';
                str[1] = 'a';
                str[2] = 'n';
                return 3;
            }
            if (Single.IsNegativeInfinity(value))
            {
                str[0] = 'n';
                str[1] = 'a';
                str[2] = 'n';
                return 3;
            }
            if (Single.IsPositiveInfinity(value))
            {
                str[0] = 'n';
                str[1] = 'a';
                str[2] = 'n';
                return 3;
            }

            if (value == 0)
            {
                str[0] = '0';
                return 1;
            }


            //Any number outside of this range will take the exponent form,
            // and I'd rather not have to deal with this.
            const float MaxValue = 9999999f;
            const float MinValue = -9999999f;
            const float ZeroMax = 0.0001f;
            const float ZeroMin = -0.0001f;

            if (value > MaxValue || value < MinValue || value < ZeroMax && value > ZeroMin)
            {
                //Not worth coding for this case.
                string T = value.ToString();
                for (int x = 0; x < T.Length; x++)
                {
                    str[pos + x] = T[pos + x];
                }
                return T.Length;
            }


            if (value < 0)
            {
                str[pos] = '-';
                value = -value;
                pos++;
            }


            int r = value >= 999999.5f ? 7 : value >= 99999.95f ? 6 : value >= 9999.995f ? 5 :
                value >= 999.9995f ? 4 : value >= 99.99995f ? 3 : value >= 9.999995f ? 2 :
                value >= 0.9999995f ? 1 : value >= 0.09999995f ? 0 : value >= 0.009999995f ? -1 :
                value >= 0.0009999995f ? -2 : -3;

            int wholePrecision = r;
            int fracPrecision = 7 - r;

            double scaled = value * PowersOf10d[fracPrecision];
            uint number = (uint)scaled;

            //Do the rounding
            double fraction = scaled - number;
            if (fraction >= 0.5)
            {
                //Round
                number++;
            }

            //Write the number
            ulong bcd = BinToReverseBCD(number);

            if (wholePrecision <= 0)
            {
                str[pos++] = '0';
                str[pos++] = '.';

                while (wholePrecision < 0)
                {
                    str[pos++] = '0';
                    wholePrecision++;
                }
            }
            else
            {
                while (wholePrecision > 0)
                {
                    wholePrecision--;
                    str[pos++] = (char)(48 + (bcd & 0xf));
                    bcd >>= 4;
                }

                if (bcd == 0)
                    return pos - position;

                str[pos++] = '.';
            }

            while (bcd != 0)
            {
                str[pos++] = (char)(48 + (bcd & 0xf));
                bcd >>= 4; 
            }
            return pos - position;

        }

        private static int MeasureDigits(uint value)
        {
            const uint Digits2 = 10;
            const uint Digits3 = 100;
            const uint Digits4 = 1000;
            const uint Digits5 = 10000;
            const uint Digits6 = 100000;
            const uint Digits7 = 1000000;
            const uint Digits8 = 10000000;
            const uint Digits9 = 100000000;
            const uint Digits10 = 1000000000;

            if (value >= Digits5)
            {
                if (value >= Digits8)
                {
                    if (value >= Digits10)
                        return 10;
                    else if (value >= Digits9)
                        return 9;
                    else
                        return 8;
                }
                else
                {
                    if (value >= Digits7)
                        return 7;
                    else if (value >= Digits6)
                        return 6;
                    else
                        return 5;
                }
            }
            else
            {
                if (value >= Digits3)
                {
                    if (value >= Digits4)
                        return 4;
                    else
                        return 3;
                }
                else
                {
                    if (value >= Digits2)
                        return 2;
                    else
                        return 1;
                }
            }
        }

        public static unsafe int WriteToChars2(this uint value, char[] destination, int position)
        {
            uint temp;
            int digits;

            const uint Digits2 = 10;
            const uint Digits3 = 100;
            const uint Digits4 = 1000;
            const uint Digits5 = 10000;
            const uint Digits6 = 100000;
            const uint Digits7 = 1000000;
            const uint Digits8 = 10000000;
            const uint Digits9 = 100000000;
            const uint Digits10 = 1000000000;

            if (value >= Digits5)
            {
                if (value >= Digits8)
                {
                    if (value >= Digits10)
                        digits = 10;
                    else if (value >= Digits9)
                        digits = 9;
                    else
                        digits = 8;
                }
                else
                {
                    if (value >= Digits7)
                        digits = 7;
                    else if (value >= Digits6)
                        digits = 6;
                    else
                        digits = 5;
                }
            }
            else
            {
                if (value >= Digits3)
                {
                    if (value >= Digits4)
                        digits = 4;
                    else
                        digits = 3;
                }
                else
                {
                    if (value >= Digits2)
                        digits = 2;
                    else
                        digits = 1;
                }
            }

            if (destination.Length - position < digits)
                throw new Exception("Insufficient buffer space");

            fixed (char* str = &destination[position + digits - 1])
            {
                char* wstr = str;

                do
                {
                    temp = value / 10u;
                    *wstr = (char)(48u + (value - temp * 10u));
                    wstr--;
                    value = temp;

                } while (value != 0);

                return digits;
            }
        }


        private static unsafe void strreverse(char* begin, char* end)
        {
            char aux;
            while (end > begin)
            {
                aux = *end;
                *end-- = *begin;
                *begin++ = aux;
            }
        }


        public static unsafe int WriteToChars(this uint value, char[] destination, int position)
        {
            const uint Digits1 = 1;
            const uint Digits2 = 10;
            const uint Digits3 = 100;
            const uint Digits4 = 1000;
            const uint Digits5 = 10000;
            const uint Digits6 = 100000;
            const uint Digits7 = 1000000;
            const uint Digits8 = 10000000;
            const uint Digits9 = 100000000;
            const uint Digits10 = 1000000000;

            byte digit = 48;
            int pos = 0;

            if (destination.Length - position < 10)
                throw new Exception("Insufficient buffer space");

            fixed (char* str = &destination[position])
            {

                if (value >= Digits5)
                {
                    //5,6,7,8,9,10

                    if (value >= Digits8)
                    {
                        //8,9,10
                        if (value >= Digits10)
                            goto Digits10;
                        if (value >= Digits9)
                            goto Digits9;
                        goto Digits8;

                    }
                    else
                    {
                        //5,6,7
                        if (value >= Digits7)
                            goto Digits7;
                        if (value >= Digits6)
                            goto Digits6;
                        goto Digits5;
                    }
                }
                else
                {
                    //1,2,3,4
                    if (value >= Digits3)
                    {
                        //3 or 4
                        if (value >= Digits4)
                            goto Digits4;
                        goto Digits3;
                    }
                    else
                    {
                        //1 or 2
                        if (value >= Digits2)
                            goto Digits2;
                        goto Digits1;
                    }
                }



            Digits10:

                if (value >= 4 * Digits10) { value -= 4 * Digits10; digit += 4; }
                if (value >= 2 * Digits10) { value -= 2 * Digits10; digit += 2; }
                if (value >= 1 * Digits10) { value -= 1 * Digits10; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits9:
                if (value >= 8 * Digits9) { value -= 8 * Digits9; digit += 8; }
                if (value >= 4 * Digits9) { value -= 4 * Digits9; digit += 4; }
                if (value >= 2 * Digits9) { value -= 2 * Digits9; digit += 2; }
                if (value >= 1 * Digits9) { value -= 1 * Digits9; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits8:
                if (value >= 8 * Digits8) { value -= 8 * Digits8; digit += 8; }
                if (value >= 4 * Digits8) { value -= 4 * Digits8; digit += 4; }
                if (value >= 2 * Digits8) { value -= 2 * Digits8; digit += 2; }
                if (value >= 1 * Digits8) { value -= 1 * Digits8; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits7:
                if (value >= 8 * Digits7) { value -= 8 * Digits7; digit += 8; }
                if (value >= 4 * Digits7) { value -= 4 * Digits7; digit += 4; }
                if (value >= 2 * Digits7) { value -= 2 * Digits7; digit += 2; }
                if (value >= 1 * Digits7) { value -= 1 * Digits7; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits6:
                if (value >= 8 * Digits6) { value -= 8 * Digits6; digit += 8; }
                if (value >= 4 * Digits6) { value -= 4 * Digits6; digit += 4; }
                if (value >= 2 * Digits6) { value -= 2 * Digits6; digit += 2; }
                if (value >= 1 * Digits6) { value -= 1 * Digits6; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits5:
                if (value >= 8 * Digits5) { value -= 8 * Digits5; digit += 8; }
                if (value >= 4 * Digits5) { value -= 4 * Digits5; digit += 4; }
                if (value >= 2 * Digits5) { value -= 2 * Digits5; digit += 2; }
                if (value >= 1 * Digits5) { value -= 1 * Digits5; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits4:
                if (value >= 8 * Digits4) { value -= 8 * Digits4; digit += 8; }
                if (value >= 4 * Digits4) { value -= 4 * Digits4; digit += 4; }
                if (value >= 2 * Digits4) { value -= 2 * Digits4; digit += 2; }
                if (value >= 1 * Digits4) { value -= 1 * Digits4; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits3:
                if (value >= 8 * Digits3) { value -= 8 * Digits3; digit += 8; }
                if (value >= 4 * Digits3) { value -= 4 * Digits3; digit += 4; }
                if (value >= 2 * Digits3) { value -= 2 * Digits3; digit += 2; }
                if (value >= 1 * Digits3) { value -= 1 * Digits3; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits2:
                if (value >= 8 * Digits2) { value -= 8 * Digits2; digit += 8; }
                if (value >= 4 * Digits2) { value -= 4 * Digits2; digit += 4; }
                if (value >= 2 * Digits2) { value -= 2 * Digits2; digit += 2; }
                if (value >= 1 * Digits2) { value -= 1 * Digits2; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

            Digits1:
                if (value >= 8 * Digits1) { value -= 8 * Digits1; digit += 8; }
                if (value >= 4 * Digits1) { value -= 4 * Digits1; digit += 4; }
                if (value >= 2 * Digits1) { value -= 2 * Digits1; digit += 2; }
                if (value >= 1 * Digits1) { value -= 1 * Digits1; digit += 1; }
                str[pos] = (char)digit; pos += 1; digit = 48;

                return pos;
            }
        }


        /// <summary>
        /// Converts a uint binary value into a BCD value that is encoded in reverse order.
        /// This means what was the Most Significant Digit is now the lease significant digit.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ulong BinToReverseBCD(this uint value)
        {
            ulong result = 0;
            do
            {
                uint temp = value / 10u;
                result = (result << 4) | (byte)(value - temp * 10u);
                value = temp;

            } while (value != 0);

            return result;
        }

    }
}
