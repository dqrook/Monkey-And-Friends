using System;

namespace Ryzm.Utils
{
    public static class Base58
    {
        private static string Base58characters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
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
    }
}
