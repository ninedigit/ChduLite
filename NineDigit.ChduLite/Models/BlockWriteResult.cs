using System;

namespace NineDigit.ChduLite
{
    /// <summary>
    /// Výsledok zapísania bloku
    /// </summary>
    public class BlockWriteResult
    {
        readonly ResponseMessage message;

        internal BlockWriteResult(ResponseMessage message)
        {
            this.message = message
                ?? throw new ArgumentNullException(nameof(message));

            if (message.PayloadLength != 4)
                throw new ArgumentException("Neočakávaná dĺžka odpovede o výsledku zápisu.", nameof(message));
        }

        /// <summary>
        /// Adresa bloku, na ktorom bol vykonaný zápis.
        /// </summary>
        public BlockAddress Address
        {
            get
            {
                var addressBytes = message.GetPayloadContent(0, 4);
                return BitConverter.ToUInt32(addressBytes, 0);
            }
        }
    }
}
