using System;

namespace NineDigit.ChduLite.Commands
{
    internal abstract class ChduLiteCommand
    {
        internal ChduLiteCommand(ChduLiteCommandId id)
        {
            this.Id = id;
        }

        internal ChduLiteCommandId Id { get; }

        protected virtual byte[] GetArguments() => Array.Empty<byte>();

        public byte[] GetBytes()
        {
            var arguments = this.GetArguments();
            var argumentsLength = arguments.Length;
            var dataLength = argumentsLength + 1; // LEN zodpovedá dĺžke dátovej oblasti (id + args) v bajtoch

            // <STX> + lengthL + lengthH + id + args + <ETX>
            var buffer = new byte[dataLength + 4];

            buffer[0] = (byte)ControlChars.STX;
            buffer[1] = (byte)(dataLength % 256); // length L
            buffer[2] = (byte)(dataLength / 256); // length H
            buffer[3] = (byte)this.Id; // command id

            if (argumentsLength > 0)
                Buffer.BlockCopy(arguments, 0, buffer, 4, argumentsLength);

            buffer[buffer.Length - 1] = (byte)ControlChars.EOT;

            return buffer;
        }
    }

    internal interface IChduLiteResponseCommand
    {
        uint ResponseBlocksCount { get; }
        /// <summary>
        /// Gets minimal number of data bytes in command response (excluding STX, two bytes for length and EOT).
        /// </summary>
        uint MinResponseDataBytes { get; }
        /// <summary>
        /// Gets whether packet confirmation is required by device.
        /// </summary>
        bool IsMultiPacketTransferSupported { get; }
    }

    internal abstract class ChduLiteCommand<TResponse> : ChduLiteCommand, IChduLiteResponseCommand
    {
        internal ChduLiteCommand(ChduLiteCommandId id)
            : base(id)
        { }

        public abstract uint ResponseBlocksCount { get; }
        public abstract uint MinResponseDataBytes { get; }
        public virtual bool IsMultiPacketTransferSupported => false;

        public abstract TResponse ProcessResponse(ResponseMessage[] response);
    }
}
