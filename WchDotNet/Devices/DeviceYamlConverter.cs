using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace WchDotNet.Devices
{
    public class DeviceYamlConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(IEnumerable<byte>)
                || type == typeof(UInt32);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (type == typeof(IEnumerable<byte>))
            {
                List<byte> bytes = new List<byte>();
                IEnumerable<byte> result = bytes;

                parser.Consume<SequenceStart>();
                while (parser.Current.GetType() == typeof(Scalar))
                {
                    var value = parser.Consume<Scalar>().Value;
                    if (value == "ALL")
                    {
                        result = Enumerable.Range(0, 0xff).Select(i => (byte)i);
                        break;
                    }
                    bytes.Add(Convert.ToByte(value, 16));
                }
                parser.Consume<SequenceEnd>();

                return result;
            }
            else if (type == typeof(UInt32))
            {
                var value = parser.Consume<Scalar>().Value;
                return DeserializeIntegerHelper(TypeCode.UInt32, value);
            }
            return null;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }

        public static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo
        {
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = "_",
            CurrencyGroupSizes = new[] { 3 },
            CurrencySymbol = string.Empty,
            CurrencyDecimalDigits = 99,
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = "_",
            NumberGroupSizes = new[] { 3 },
            NumberDecimalDigits = 99,
            NaNSymbol = ".nan",
            PositiveInfinitySymbol = ".inf",
            NegativeInfinitySymbol = "-.inf"
        };

        private static readonly IReadOnlyDictionary<char, (int, int)> BinSuffixes =
        new Dictionary<char, (int, int)>
        {
                    { 'K', (2, 10) },
                    { 'M', (2, 20) },
                    { 'G', (2, 30) },
        };

        public static object DeserializeIntegerHelper(TypeCode typeCode, string value)
        {
            var numberBuilder = new StringBuilder();
            var currentIndex = 0;
            var isNegative = false;
            int numberBase;
            ulong result = 0;
            var isQuantity = false;
            int quantityBase = 0;
            int quantityExponent = 0;

            if (BinSuffixes.TryGetValue(value.Last(), out var val))
            {
                (quantityBase, quantityExponent) = val;
                isQuantity = true;
                value = value.Remove(value.Length - 1);
            }

            if (value[0] == '-')
            {
                currentIndex++;
                isNegative = true;
            }

            else if (value[0] == '+')
            {
                currentIndex++;
            }

            if (value[currentIndex] == '0')
            {
                // Could be binary, octal, hex, decimal (0)

                // If there are no characters remaining, it's a decimal zero
                if (currentIndex == value.Length - 1)
                {
                    numberBase = 10;
                    result = 0;
                }

                else
                {
                    // Check the next character
                    currentIndex++;

                    if (value[currentIndex] == 'b')
                    {
                        // Binary
                        numberBase = 2;

                        currentIndex++;
                    }

                    else if (value[currentIndex] == 'x')
                    {
                        // Hex
                        numberBase = 16;

                        currentIndex++;
                    }

                    else
                    {
                        // Octal
                        numberBase = 8;
                    }
                }

                // Copy remaining digits to the number buffer (skip underscores)
                while (currentIndex < value.Length)
                {
                    if (value[currentIndex] != '_')
                    {
                        numberBuilder.Append(value[currentIndex]);
                    }
                    currentIndex++;
                }

                // Parse the magnitude of the number
                switch (numberBase)
                {
                    case 2:
                    case 8:
                        // TODO: how to incorporate the numberFormat?
                        result = Convert.ToUInt64(numberBuilder.ToString(), numberBase);
                        break;

                    case 16:
                        result = ulong.Parse(numberBuilder.ToString(), NumberStyles.HexNumber, NumberFormat);
                        break;

                    case 10:
                        // Result is already zero
                        break;
                }
            }

            else
            {
                // Could be decimal or base 60
                var chunks = value.Substring(currentIndex).Split(':');
                result = 0;

                for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
                {
                    result *= 60;

                    // TODO: verify that chunks after the first are non-negative and less than 60
                    result += ulong.Parse(chunks[chunkIndex].Replace("_", ""));
                }
            }
            if (isQuantity)
            {
                result *= (ulong)Math.Pow(quantityBase, quantityExponent);
            }
            if (isNegative)
            {
                long toCast;

                // we do this because abs(long.minvalue) is 1 more than long.maxvalue.
                if (result == 9223372036854775808) // abs(long.minvalue) => ulong
                {
                    toCast = long.MinValue;
                }
                else
                {
                    // this will throw if it's too big.
                    toCast = checked(-(long)result);
                }

                return CastInteger(toCast, typeCode);
            }
            else
            {
                return CastInteger(result, typeCode);
            }
        }
        private static object CastInteger(ulong number, TypeCode typeCode)
        {
            checked
            {
                return typeCode switch
                {
                    TypeCode.Byte => (byte)number,
                    TypeCode.Int16 => (short)number,
                    TypeCode.Int32 => (int)number,
                    TypeCode.Int64 => (long)number,
                    TypeCode.SByte => (sbyte)number,
                    TypeCode.UInt16 => (ushort)number,
                    TypeCode.UInt32 => (uint)number,
                    TypeCode.UInt64 => number,
                    _ => number,
                };
            }
        }
        private static object CastInteger(long number, TypeCode typeCode)
        {
            checked
            {
                return typeCode switch
                {
                    TypeCode.Byte => (byte)number,
                    TypeCode.Int16 => (short)number,
                    TypeCode.Int32 => (int)number,
                    TypeCode.Int64 => number,
                    TypeCode.SByte => (sbyte)number,
                    TypeCode.UInt16 => (ushort)number,
                    TypeCode.UInt32 => (uint)number,
                    TypeCode.UInt64 => (ulong)number,
                    _ => number,
                };
            }
        }
    }
}
