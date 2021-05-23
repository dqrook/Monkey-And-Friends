using System;
using System.Collections.Generic;
using System.Text;

namespace Ryzm.Blockchain
{
    public static class Base58
    {
        static string Base58characters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        static char[] _alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
        static int[] _indexes = new int[128];

        public static byte[] Decode(string source)
        {
            byte[] destination;
            int i = 0;
            while (i < source.Length)
            {
                if (source[i] == 0 || !Char.IsWhiteSpace(source[i]))
                {
                    break;
                }
                i++;
            }
            int zeros = 0;
            while (source[i] == '1')
            {
                zeros++;
                i++;
            }
            byte[] b256 = new byte[(source.Length - i) * 733 / 1000 + 1];
            while (i < source.Length && !Char.IsWhiteSpace(source[i]))
            {
                int ch = Base58characters.IndexOf(source[i]);
                if (ch == -1) //null
                {
                    return null;
                }
                int carry = Base58characters.IndexOf(source[i]);
                for (int k = b256.Length - 1; k >= 0; k--)
                {
                    carry += 58 * b256[k];
                    b256[k] = (byte)(carry % 256);
                    carry /= 256;
                }
                i++;
            }
            while (i < source.Length && Char.IsWhiteSpace(source[i]))
            {
                i++;
            }
            if (i != source.Length)
            {
                return null;
            }
            int j = 0;
            while (j < b256.Length && b256[j] == 0)
            {
                j++;
            }
            destination = new byte[zeros + (b256.Length - j)];
            for (int kk = 0; kk < destination.Length; kk++)
            {
                if (kk < zeros)
                {
                    destination[kk] = 0x00;
                }
                else
                {
                    destination[kk] = b256[j++];
                }
            }
            return destination;
        }

        public static string Encode(byte[] input)
        {
            for (int i = 0; i < _indexes.Length; i++)
            {
                _indexes[i] = -1;
            }
            for (int i = 0; i < _alphabet.Length; i++)
            {
                _indexes[_alphabet[i]] = i;
            }

            if (0 == input.Length)
            {
                return String.Empty;
            }
            input = CopyOfRange(input, 0, input.Length);
            // Count leading zeroes.
            int zeroCount = 0;
            while (zeroCount < input.Length && input[zeroCount] == 0)
            {
                zeroCount ++;
            }
            // The actual encoding.
            byte[] temp = new byte[input.Length * 2];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input.Length)
            {
                byte mod = DivMod58(input, startAt);
                if (input[startAt] == 0)
                {
                    startAt ++;
                }
                temp[--j] = (byte)_alphabet[mod];
            }

            // Strip extra '1' if there are some after decoding.
            while (j < temp.Length && temp[j] == _alphabet[0])
            {
                ++j;
            }
            // Add as many leading '1' as there were leading zeros.
            while (--zeroCount >= 0)
            {
                temp[--j] = (byte)_alphabet[0];
            }

            byte[] output = CopyOfRange(temp, j, temp.Length);
            try
            {
                return Encoding.ASCII.GetString(output);
            }
            catch (DecoderFallbackException e)
            {
                Console.WriteLine(e.ToString());
                return String.Empty;
            }
        }

        static byte DivMod58(byte[] number, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number.Length; i++)
            {
                int digit256 = (int)number[i] & 0xFF;
                int temp = remainder * 256 + digit256;

                number[i] = (byte)(temp / 58);

                remainder = temp % 58;
            }

            return (byte)remainder;
        }

        static byte DivMod256(byte[] number58, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number58.Length; i++)
            {
                int digit58 = (int)number58[i] & 0xFF;
                int temp = remainder * 58 + digit58;

                number58[i] = (byte)(temp / 256);

                remainder = temp % 256;
            }

            return (byte)remainder;
        }

        static byte[] CopyOfRange(byte[] source, int from, int to)
        {
            byte[] range = new byte[to - from];
            for (int i = 0; i < to - from; i++)
            {
                range[i] = source[from + i];
            }
            return range;
        }
    }
}
