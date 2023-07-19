using System;

namespace NineDigit.ChduLite.Helpers
{
    internal class EndiannessConverter
    {
        /// <summary>
        /// Ensures that byte order of value is in big endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static short ToBigEndian(short value)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(value) : value;
        }

        /// <summary>
        /// Ensures that byte order of value is in little endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static short ToLittleEndian(short value)
        {
            return BitConverter.IsLittleEndian ? value : ReverseBytes(value);
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="bigEndianValue">Big endian value</param>
        /// <returns></returns>
        public static short FromBigEndian(short bigEndianValue)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(bigEndianValue) : bigEndianValue;
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="littleEndianValue">Little endian value</param>
        /// <returns></returns>
        public static short FromLittleEndian(short littleEndianValue)
        {
            return BitConverter.IsLittleEndian ? littleEndianValue : ReverseBytes(littleEndianValue);
        }

        /// <summary>
        /// Ensures that byte order of value is in big endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static ushort ToBigEndian(ushort value)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(value) : value;
        }

        /// <summary>
        /// Ensures that byte order of value is in little endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static ushort ToLittleEndian(ushort value)
        {
            return BitConverter.IsLittleEndian ? value : ReverseBytes(value);
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="bigEndianValue">Big endian value</param>
        /// <returns></returns>
        public static ushort FromBigEndian(ushort bigEndianValue)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(bigEndianValue) : bigEndianValue;
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="littleEndianValue">Little endian value</param>
        /// <returns></returns>
        public static ushort FromLittleEndian(ushort littleEndianValue)
        {
            return BitConverter.IsLittleEndian ? littleEndianValue : ReverseBytes(littleEndianValue);
        }

        /// <summary>
        /// Ensures that byte order of value is in big endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static int ToBigEndian(int value)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(value) : value;
        }

        /// <summary>
        /// Ensures that byte order of value is in little endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static int ToLittleEndian(int value)
        {
            return BitConverter.IsLittleEndian ? value : ReverseBytes(value);
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="bigEndianValue">Big endian value</param>
        /// <returns></returns>
        public static int FromBigEndian(int bigEndianValue)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(bigEndianValue) : bigEndianValue;
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="littleEndianValue">Little endian value</param>
        /// <returns></returns>
        public static int FromLittleEndian(int littleEndianValue)
        {
            return BitConverter.IsLittleEndian ? littleEndianValue : ReverseBytes(littleEndianValue);
        }

        /// <summary>
        /// Ensures that byte order of value is in big endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static uint ToBigEndian(uint value)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(value) : value;
        }

        /// <summary>
        /// Ensures that byte order of value is in little endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static uint ToLittleEndian(uint value)
        {
            return BitConverter.IsLittleEndian ? value : ReverseBytes(value);
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="bigEndianValue">Big endian value</param>
        /// <returns></returns>
        public static uint FromBigEndian(uint bigEndianValue)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(bigEndianValue) : bigEndianValue;
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="littleEndianValue">Little endian value</param>
        /// <returns></returns>
        public static uint FromLittleEndian(uint littleEndianValue)
        {
            return BitConverter.IsLittleEndian ? littleEndianValue : ReverseBytes(littleEndianValue);
        }

        /// <summary>
        /// Ensures that byte order of value is in big endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static long ToBigEndian(long value)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(value) : value;
        }

        /// <summary>
        /// Ensures that byte order of value is in little endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static long ToLittleEndian(long value)
        {
            return BitConverter.IsLittleEndian ? value : ReverseBytes(value);
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="bigEndianValue">Big endian value</param>
        /// <returns></returns>
        public static long FromBigEndian(long bigEndianValue)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(bigEndianValue) : bigEndianValue;
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="littleEndianValue">Little endian value</param>
        /// <returns></returns>
        public static long FromLittleEndian(long littleEndianValue)
        {
            return BitConverter.IsLittleEndian ? littleEndianValue : ReverseBytes(littleEndianValue);
        }

        /// <summary>
        /// Ensures that byte order of value is in big endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static ulong ToBigEndian(ulong value)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(value) : value;
        }

        /// <summary>
        /// Ensures that byte order of value is in little endian.
        /// </summary>
        /// <param name="value">Value with endiannes based on this computers architecture.</param>
        /// <returns></returns>
        public static ulong ToLittleEndian(ulong value)
        {
            return BitConverter.IsLittleEndian ? value : ReverseBytes(value);
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="bigEndianValue">Big endian value</param>
        /// <returns></returns>
        public static ulong FromBigEndian(ulong bigEndianValue)
        {
            return BitConverter.IsLittleEndian ? ReverseBytes(bigEndianValue) : bigEndianValue;
        }

        /// <summary>
        /// Ensures that value endiannes matches with this computer architecture.
        /// </summary>
        /// <param name="littleEndianValue">Little endian value</param>
        /// <returns></returns>
        public static ulong FromLittleEndian(ulong littleEndianValue)
        {
            return BitConverter.IsLittleEndian ? littleEndianValue : ReverseBytes(littleEndianValue);
        }

        private static short ReverseBytes(short value)
        {
            var temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToInt16(temp, 0);
        }

        private static ushort ReverseBytes(ushort value)
        {
            var temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToUInt16(temp, 0);
        }

        private static int ReverseBytes(int value)
        {
            var temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }

        private static uint ReverseBytes(uint value)
        {
            var temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp, 0);
        }

        private static long ReverseBytes(long value)
        {
            var temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToInt64(temp, 0);
        }

        private static ulong ReverseBytes(ulong value)
        {
            var temp = BitConverter.GetBytes(value);
            Array.Reverse(temp);
            return BitConverter.ToUInt64(temp, 0);
        }
    }
}
