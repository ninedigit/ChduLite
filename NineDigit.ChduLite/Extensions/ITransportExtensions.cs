using NineDigit.SerialTransport;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite
{
    internal static class ITransportExtensions
    {
        const int SoftResetBufferSize = 525;
        const int SoftResetValue = 0x04;

        public static async Task ExecuteSoftResetAsync(this ITransport transport, CancellationToken cancellationToken)
        {
            // "soft-reset" je procedura ktora spociva v tom ze:
            // 1. posle sa do CHDU 525 bajtov EOT(0x04), co sposobi ze sa resetne parsovanie packetov
            // 2. vykonat DiscardBuffers, co dropne nezosynchronizovany garbage z bufferov

            var softResetData = new byte[SoftResetBufferSize];
            for (int i = 0; i < SoftResetBufferSize; i++)
                softResetData[i] = SoftResetValue;
            
            await transport.WriteAsync(softResetData, cancellationToken).ConfigureAwait(false);
            transport.DiscardBuffers();
        }
    }
}
