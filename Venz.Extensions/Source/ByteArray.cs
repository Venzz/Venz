using System;

namespace Venz.Extensions
{
    public static class ByteArrayExtensions
    {
        public static Byte[] ToBigEndian(Int32 value)
        {
            var littleEndianValue = BitConverter.GetBytes(value);
            Array.Reverse(littleEndianValue);
            return littleEndianValue;
        }

        public static Byte[] ToBigEndian(Int16 value)
        {
            var littleEndianValue = BitConverter.GetBytes(value);
            Array.Reverse(littleEndianValue);
            return littleEndianValue;
        }

        public static Int32 FromBigEndian(Byte[] data, Int32 index, Byte length)
        {
            if (length > 4)
                length = 4;

            var multiplier = 1;
            var value = 0;
            for (var i = length - 1; i >= 0; i--)
            {
                value += data[index + i] * multiplier;
                multiplier *= 256;
            }

            return value;
        }

        public static String ToHexView(this Byte[] source, Boolean writeTotalSize = true)
        {
            return ToHexView(source, 0, source.Length, writeTotalSize);
        }

        public static String ToHexView(this Byte[] source, Int32 index, Boolean writeTotalSize = true)
        {
            return ToHexView(source, index, source.Length, writeTotalSize);
        }

        public static String ToHexView(this Byte[] source, Int32 index, Int32 length, Boolean writeTotalSize = true)
        {
            var representaion = "";
            var ascii = "";
            var i = 0;
            for (i = index; i < index + length; i++)
            {
                if ((i - index) % 16 == 0)
                    representaion += $"{(i - index):X4} | ";
                representaion += $"{source[i]:X2} ";
                ascii += ((source[i] > 125) || (source[i] < 32)) ? '.' : Convert.ToChar(source[i]);
                if ((i - index) % 16 == 15)
                {
                    representaion += "  " + ascii;
                    representaion += "\r\n";
                    ascii = "";
                }
            }
            if ((i - index) % 16 != 15)
            {
                while (i++ % 16 != 15)
                    representaion += "   ";
                representaion += "   ";
                representaion += "  " + ascii;
            }
            if (writeTotalSize)
                representaion += $"\r\n\r\nTotal length: {source.Length - index} bytes";
            return representaion;
        }

        public static String ToTestView(this Byte[] source)
        {
            var representaion = "";
            var i = 0;
            for (i = 0; i < source.Length; i++)
                representaion += $"{source[i]:X2} ";
            return representaion;
        }

        public static String ToByteView(this Byte[] source)
        {
            var representaion = "";
            var i = 0;
            representaion += "{ ";
            for (i = 0; i < source.Length; i++)
                representaion += $"0x{source[i]:X2}, ";
            representaion += "}";
            return representaion;
        }

        public static Byte[] Concat(this Byte[] source, Byte[] data)
        {
            if (data == null)
                return source;

            var newArray = new Byte[source.Length + data.Length];
            Array.Copy(source, 0, newArray, 0, source.Length);
            Array.Copy(data, 0, newArray, source.Length, data.Length);
            return newArray;
        }

        public static Byte[] Concat(this Byte[] source, Int32 zeroBytes)
        {
            if (zeroBytes < 1)
                return source;

            var newArray = new Byte[source.Length + zeroBytes];
            Array.Copy(source, 0, newArray, 0, source.Length);
            return newArray;
        }

        public static Byte[] Revert(this Byte[] source)
        {
            var array = new Byte[source.Length];
            Array.Copy(source, array, source.Length);
            Array.Reverse(array);
            return array;
        }

        public static Byte[] SubArray(this Byte[] source, Int32 offset, Int32 length)
        {
            var array = new Byte[length];
            Array.Copy(source, offset, array, 0, length);
            return array;
        }

        public static Byte[] SubArray(this Byte[] source, Int32 offset)
        {
            var array = new Byte[source.Length - offset];
            Array.Copy(source, offset, array, 0, source.Length - offset);
            return array;
        }

        public static Boolean ValuesEquals(this Byte[] source, Byte[] value)
        {
            if ((value == null) || (value.Length != source.Length))
                return false;
            for (var i = 0; i < source.Length; i++)
                if (source[i] != value[i])
                    return false;
            return true;
        }
    }
}
