using System;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class GetVolumeInfoCommand : ChduLiteCommand<ChduLiteVolumeInfo>
    {
        public GetVolumeInfoCommand()
            : base(ChduLiteCommandId.VolumeSerial)
        { }

        public override uint ResponseBlocksCount => 1;

        public override ChduLiteVolumeInfo ProcessResponse(ResponseMessage[] response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Length != this.ResponseBlocksCount)
                throw new InvalidOperationException("Unexpected response blocks count.");

            var firstBlock = response[0];

            return new ChduLiteVolumeInfo(firstBlock);
        }
    }
}
