using System;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class RequestLockCommand : ChduLiteCommand<uint>
    {
        readonly uint magicNumber;

        public RequestLockCommand(uint magicNumber)
            : base(ChduLiteCommandId.LockRequest)
        {
            this.magicNumber = magicNumber;
        }

        public override uint ResponseBlocksCount => 1;

        public override uint ProcessResponse(ResponseMessage[] response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Length != this.ResponseBlocksCount)
                throw new InvalidOperationException("Unexpected response blocks count.");

            var firstBlockPayload = response[0].GetPayloadContent();

            var authCode = BitConverter.ToUInt32(firstBlockPayload, 0);

            return authCode;
        }

        protected override byte[] GetArguments()
        {
            var buffer = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(magicNumber), 0, buffer, 0, 4);

            return buffer;
        }
    }
}
