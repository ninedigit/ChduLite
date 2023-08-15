using NineDigit.SerialTransport;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite
{
    internal static class ITransportExtensions
    {
        public static async Task ExecuteSoftResetAsync(this ITransport transport, CancellationToken cancellationToken)
        {
            // "soft-reset" je procedura ktora spociva v tom ze:
            // 1. posle sa do CHDU 525 bajtov EOT(0x04), co sposobi ze sa resetne parsovanie packetov
            // 2. vykonat DiscardBuffers, co dropne nezosynchronizovany garbage z bufferov

            var softResetData = Enumerable.Repeat((byte)0x04, 525).ToArray();
            await transport.WriteAsync(softResetData, cancellationToken).ConfigureAwait(false);
            transport.DiscardBuffers();
        }
    }
}
