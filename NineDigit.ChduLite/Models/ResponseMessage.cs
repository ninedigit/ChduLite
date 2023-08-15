using System;
using System.Linq;

namespace NineDigit.ChduLite
{
    internal sealed class ResponseMessage
    {
        readonly byte[] buffer;

        internal const int HeaderLength = 2;

        public ResponseMessage(byte[] buffer, bool? isCrcValid)
        {
            this.buffer = buffer
                ?? throw new ArgumentNullException(nameof(buffer));

            IsCrcValid = isCrcValid;

            // raw message contains two leading bytes that indicates payload length.
            PayloadLength = ParsePayloadLength(buffer);

            // double checking lengths
            var expectedTotalBufferLength = PayloadLength + HeaderLength;
            if (expectedTotalBufferLength > buffer.Length)
                throw new ArgumentException("Indicated payload length must not be longer than actual response message.", nameof(buffer));
        }

        public int PayloadLength { get; }

        public bool? IsCrcValid { get; }

        public byte[] GetPayloadContent(int offset = 0, int? length = null)
        {
            var resultLength = Math.Max(length ?? PayloadLength - offset, 0);
            var result = new byte[resultLength];
            Array.Copy(buffer, HeaderLength + offset, result, 0, resultLength);
            return result;
        }

        public byte GetPayloadContentAt(int index)
            => buffer[HeaderLength + index];

        public byte[] GetRawData() => buffer.ToArray();

        public static int ParsePayloadLength(byte[] data)
        {
            if (!TryParsePayloadLength(data, out int result))
                throw new InvalidOperationException("Payload length cannot be parsed.");

            return result;
        }

        public static bool TryParsePayloadLength(byte[] data, out int payloadLength)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length < 2)
            {
                payloadLength = 0;
                return false;
            }

            payloadLength = data[0] + data[1] * 256;

            return true;
        }
    }
}
