using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite
{
    internal static class SerialPortExtensions
    {
        public static async Task<byte> ReadByteAsync(this Stream self, CancellationToken cancellationToken)
        {
            if (self is null)
                throw new ArgumentNullException(nameof(self));

            var tmp = new byte[1];

            await self.ReadAsync(tmp, 0, 1, cancellationToken).ConfigureAwait(false);

            return tmp[0];
        }
    }
}
