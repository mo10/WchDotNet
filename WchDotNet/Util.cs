using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WchDotNet
{
    public static class Util
    {
        public static byte OverflowingSum(this IEnumerable<byte> array)
        {
            byte result = 0x00;
            foreach (var item in array)
            {
                result = result.OverflowingAdd(item);
            }
            return result;
        }
        public static byte OverflowingAdd(this byte x, byte y)
        {
            return (byte)((x + y) & 0xff);
        }
        public static UInt16 OverflowingAdd(this UInt16 x, UInt16 y)
        {
            return (UInt16)((x + y) & 0xffff);
        }
        public static UInt32 OverflowingAdd(this UInt32 x, UInt32 y)
        {
            return (UInt32)((x + y) & 0xffffffff);
        }
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }
    }
}
