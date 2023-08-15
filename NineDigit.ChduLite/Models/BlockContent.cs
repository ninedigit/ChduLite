using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NineDigit.ChduLite
{
    public abstract class BlockContentBase
    {
        private readonly IReadOnlyList<byte> payload;

        internal BlockContentBase(byte[] payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            this.payload = payload.ToList().AsReadOnly();
        }

        protected byte[] GetPayload()
            => payload.ToArray();

        /// <summary>
        /// Payload length
        /// </summary>
        public int Length => payload.Count;

        public byte this[int index] => payload[index];

        public byte[] ToArray() => payload.ToArray();

        public void WriteTo(Stream stream, int offset, int count)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var payload = GetPayload();
            stream.Write(payload, offset, count);
        }

        internal abstract byte[] GetRawData();
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

        internal override byte[] GetRawData()
            => GetPayload();
    }

    public sealed class OffsettedBlockContent : BlockContentBase
    {
        public const int MinLength = 1;
        public const int MaxLength = 503;

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

        internal override byte[] GetRawData()
        {
            var payload = GetPayload();
            var payloadLength = payload.Length;

            var buffer = new byte[payloadLength + 2];
            Array.Copy(BitConverter.GetBytes(Offset), 0, buffer, 0, 2);
            Array.Copy(payload, 0, buffer, 2, payloadLength);

            return buffer;
        }
    }
}
