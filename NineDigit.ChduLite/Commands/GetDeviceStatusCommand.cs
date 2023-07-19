using System;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class GetDeviceStatusCommand : ChduLiteCommand<ChduLiteStatus>
    {
        public GetDeviceStatusCommand()
            : base(ChduLiteCommandId.ReadStatus)
        { }

        public override uint ResponseBlocksCount => 1;

        public override ChduLiteStatus ProcessResponse(ResponseMessage[] response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Length != this.ResponseBlocksCount)
                throw new InvalidOperationException("Unexpected response blocks count.");

            var firstBlock = response[0];

            return new ChduLiteStatus(firstBlock);
        }
    }
}
