using System;

namespace NineDigit.ChduLite
{
    /// <summary>
    /// Informácie o logickom zväzku dát.
    /// </summary>
    public sealed class ChduLiteVolumeInfo
    {
        readonly byte[] payload;

        internal ChduLiteVolumeInfo(ResponseMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            this.payload = message.GetPayloadContent();

            if (this.payload.Length < 4)
                throw new ArgumentException("Unexpected response message payload length. At least 4 bytes expected.", nameof(message));
        }

        /// <summary>
        /// Sériové číslo logického zväzku dát.
        /// <para>
        /// Pokiaľ nie je špecifikované inak, toto sériové číslo je pri výrobe naplnené časovou značkou (Unix epoch).
        /// </para>
        /// </summary>
        public uint VolumeSerialNumber =>  BitConverter.ToUInt32(payload, 0);

        /// <summary>
        /// Získa časovú značku zo sériového čísla logického zväzku dát, vyjadrenú v UTC.
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset GetManufacturingDateUtc()
            => DateTimeOffset.FromUnixTimeSeconds(this.VolumeSerialNumber);
    }
}
