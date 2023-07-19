using System;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class ActivateLockCommand : ChduLiteCommand
    {
        readonly uint magicNumber;
        readonly uint authCode;

        public ActivateLockCommand(uint magicNumber, uint authCode)
            : base(ChduLiteCommandId.LockActivate)
        {
            this.magicNumber = magicNumber;
            this.authCode = authCode;
        }

        protected override byte[] GetArguments()
        {
            var buffer = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(magicNumber ^ authCode), 0, buffer, 1, 4);

            return buffer;
        }
    }
}
