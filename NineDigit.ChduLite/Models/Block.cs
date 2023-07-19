using System;

namespace NineDigit.ChduLite
{
    /// <summary>
    /// Represents data between 1 and 505 bytes of an single block,
    /// returned from the device.
    /// </summary>
    public sealed class Block
    {
        readonly ResponseMessage responseMessage;

        /// <summary>
        /// Represents max raw length of block, as received from device when reading the block.
        /// Content length (2B) + Command ID (1B) + content (up to 505B)
        /// </summary>
        public const int MaxRawDataLength = ResponseMessage.HeaderLength + 1 + BlockContent.MaxLength;

        internal Block(ResponseMessage responseMessage)
        {
            this.responseMessage = responseMessage
                ?? throw new ArgumentNullException(nameof(responseMessage));

            if (responseMessage.PayloadLength < 2) // block type + at least one content byte
                throw new ArgumentException("Response message containing block content is expected to have at least two bytes.");

            IsCrcValid = responseMessage.IsCrcValid;

            var commandId = (ChduLiteCommandId)responseMessage.GetPayloadContentAt(0);
            WriteMode = commandId.ToBlockWriteMode();

            switch (commandId)
            {
                case ChduLiteCommandId.WriteData:
                case ChduLiteCommandId.WriteDataAndPrint:
                    Content = new BlockContent(responseMessage.GetPayloadContent(offset: 1));
                    break;
                case ChduLiteCommandId.WriteDataAndPrintWithOffset:
                    var offsetData = responseMessage.GetPayloadContent(offset: 1, length: 2);
                    var offset = BitConverter.ToUInt16(offsetData, 0);
                    Content = new OffsettedBlockContent(offset, responseMessage.GetPayloadContent(offset: 3));
                    break;
                default:
                    throw new InvalidOperationException("Unknown command id received when reading block.");
            }
        }

        /// <summary>
        /// Gets whether data checksum is valid and block content is not malformed.
        /// </summary>
        public bool? IsCrcValid { get; }

        public BlockWriteMode WriteMode { get; }

        public BlockContentBase Content { get; }

        /// <summary>
        /// Returns payload in format as stored in storage.
        /// </summary>
        /// <returns></returns>
        public byte[] GetRawData() => responseMessage.GetRawData();

        public static Block FromRawData(byte[] rawData)
        {
            var responseMessage = new ResponseMessage(rawData, isCrcValid: null);
            return new Block(responseMessage);
        }
    }
}
