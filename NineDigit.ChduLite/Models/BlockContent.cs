using System;
using System.IO;
using System.Linq;

namespace NineDigit.ChduLite
{
    public abstract class BlockContentBase
    {
        private readonly byte[] payload;

        internal BlockContentBase(byte[] payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            var payloadLength = payload.Length;
            this.payload = new byte[payloadLength];
            Array.Copy(payload, this.payload, payloadLength);
        }

        /// <summary>
        /// Payload length
        /// </summary>
        public int Length => this.payload.Length;

        public byte this[int index] => payload[index];

        public byte[] ToArray() => this.payload.ToArray();

        public void WriteTo(Stream stream, int offset, int count)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            stream.Write(this.payload, offset, count);
        }

        public void CopyTo(Array destinationArray, int destinationIndex, int length)
            => Array.Copy(this.payload, sourceIndex: 0, destinationArray, destinationIndex, length);

        /// <summary>
        /// Gets payload in form suitable for storing in memory.
        /// </summary>
        /// <returns></returns>
        internal abstract byte[] GetDataToStore();
    }

    public sealed class BlockContent : BlockContentBase
    {
        public const int MinLength = 1;
        public const int MaxLength = 505;

        public BlockContent(byte[] payload)
            : base(payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length < MinLength)
                throw new ArgumentException("Data must not be empty.", nameof(payload));

            if (payload.Length > MaxLength)
                throw new ArgumentException("Maximal data length exceeded.", nameof(payload));
        }

        /// <summary>
        /// Due to an issue in firmware version 1.4, the command payload length (including 4 leading bytes) must not be divisible by 64.
        /// See: https://github.com/ninedigit/ChduLiteIssues/issues/16
        /// </summary>
        /// <param name="payloadLength"></param>
        /// <returns></returns>
        private static bool IsTailingZeroByteRequired(int payloadLength)
            => (payloadLength + 4) % 64 == 0;

        internal override byte[] GetDataToStore()
        {
            var payloadLength = this.Length;
            var isTailingZeroByteRequired = IsTailingZeroByteRequired(payloadLength);
            var result = new byte[payloadLength + (isTailingZeroByteRequired ? 1 : 0)];
            
            this.CopyTo(result, 0, payloadLength);

            return result;
        }

        internal static BlockContent FromStoredData(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var dataLength = data.Length;

            if (IsTailingZeroByteRequired(dataLength - 1) && data[dataLength - 1] == default)
            {
                var payload = new byte[dataLength - 1];
                Array.Copy(data, payload, dataLength - 1);
                return new BlockContent(payload);
            }
            else
            {
                return new BlockContent(data);
            }
        }
    }

    public sealed class OffsettedBlockContent : BlockContentBase
    {
        public const int MinLength = 1;
        public const int MaxLength = 503;
        private const int OffsetLength = sizeof(ushort);

        /// <summary>
        /// </summary>
        /// <param name="offset">Number of bytes that sould not be printed.</param>
        /// <param name="payload">Data to store and print.</param>
        public OffsettedBlockContent(ushort offset, byte[] payload)
            : base(payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            var payloadLength = payload.Length;

            if (payloadLength < MinLength)
                throw new ArgumentException("Data must not be empty.", nameof(payload));

            if (payloadLength > MaxLength)
                throw new ArgumentException("Maximal data length exceeded.", nameof(payload));

            if (offset > payloadLength)
                throw new ArgumentException("Print offset must not be higher than payload length.", nameof(offset));

            Offset = offset;
        }

        public ushort Offset { get; }

        /// <summary>
        /// Due to an issue in firmware version 1.4, the command payload length (including 4 leading bytes) must not be divisible by 64.
        /// See: https://github.com/ninedigit/ChduLiteIssues/issues/16
        /// </summary>
        /// <param name="payloadLength"></param>
        /// <returns></returns>
        private static bool IsTailingZeroByteRequired(int payloadLength)
            => (OffsetLength + payloadLength + 4) % 64 == 0;

        internal override byte[] GetDataToStore()
        {
            var payloadLength = this.Length;
            var isTailingZeroByteRequired = IsTailingZeroByteRequired(payloadLength);
            var result = new byte[OffsetLength + payloadLength + (isTailingZeroByteRequired ? 1 : 0)];

            Array.Copy(BitConverter.GetBytes(Offset), 0, result, 0, OffsetLength);
            this.CopyTo(result, OffsetLength, payloadLength);

            return result;
        }

        internal static OffsettedBlockContent FromStoredData(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var dataLength = data.Length;

            if (dataLength < (OffsetLength + 1))
                throw new ArgumentException($"At least {OffsetLength + 1} bytes expected in data.", nameof(data));

            var offset = BitConverter.ToUInt16(data, 0);
            var payloadLength = dataLength - OffsetLength;

            if (IsTailingZeroByteRequired(payloadLength - 1) && data[dataLength - 1] == default)
                payloadLength -= 1; // Remove trailing zero byte if it was added.

            var payload = new byte[payloadLength];
            Array.Copy(data, OffsetLength, payload, 0, payloadLength);

            return new OffsettedBlockContent(offset, payload);
        }
    }
}
