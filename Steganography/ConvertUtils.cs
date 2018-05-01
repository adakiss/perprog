using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    static class ConvertUtils
    {
        public static int GetIntFromBitArray(BitArray bitArray)
        {
            if (bitArray.Length > 32)
            {
                throw new ArgumentException("Argument length shall be at most 32 bits.");
            }
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static byte GetByteFromBitArray(BitArray bitArray)
        {
            if (bitArray.Length > 8)
            {
                throw new ArgumentException("Argument length shall be at most 8 bits.");
            }
            byte[] array = new byte[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }
    }
}
