using System;
using System.Text;

namespace NineDigit.ChduLite.Commands
{
    /// <summary>
    /// Reads full firmware version description.
    /// </summary>
    internal sealed class GetFirmwareVersionDescriptionCommand : ChduLiteCommand<string>
    {
        public GetFirmwareVersionDescriptionCommand()
            : base(ChduLiteCommandId.Version2)
        { }

        public override uint ResponseBlocksCount => 1;

        public override string ProcessResponse(ResponseMessage[] response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Length != this.ResponseBlocksCount)
                throw new InvalidOperationException("Unexpected response blocks count.");

            var firstBlock = response[0];

            // skip tailing 0x00 byte.
            var data = firstBlock.GetPayloadContent(0, length: firstBlock.PayloadLength - 1);
            var result = Encoding.ASCII.GetString(data)?.Trim();

            return result;
        }
    }
}
