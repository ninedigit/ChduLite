using NineDigit.ChduLite.Exceptions;
using System;
using System.Globalization;

namespace NineDigit.ChduLite
{
    /// <summary>
    /// Status
    /// </summary>
    public class ChduLiteStatus
    {
        readonly ChduLiteStatusFlags flags;
        readonly byte[] payload;

        internal ChduLiteStatus(ResponseMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            this.payload = message.GetPayloadContent();

            if (this.payload.Length != 24)
                throw new ArgumentException("Unexpected response message payload length", nameof(message));

            this.flags = (ChduLiteStatusFlags)BitConverter.ToUInt16(payload, 2);
            this.Version = new(payload[1], payload[0]);
        }

        /// <summary>
        /// Verzia firmvéru
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets version prefixed with chatracter 'v' and two numeric segments.
        /// </summary>
        public string VersionString => "v" + this.Version.ToString(2);

        /// <summary>
        /// Zariadenie naštartovalo v poriadku a je pripravené na použitie.
        /// </summary>
        public bool IsStorageReady => this.flags.HasFlag(ChduLiteStatusFlags.STORAGE_OK);

        /// <summary>
        /// Zariadenie je uzamknuté, zápis nie je možný.
        /// </summary>
        public bool IsDeviceLocked => this.flags.HasFlag(ChduLiteStatusFlags.DEVICE_LOCK);

        /// <summary>
        /// Zariadenie je v stave, ktorý nepodporuje zápis.
        /// Úložisko je uzamknuté (<see cref="IsDeviceLocked"/>), alebo dátové médium nie je pripravené na zápis (<see cref="IsStorageReady"/> je <c>false</c>)".
        /// </summary>
        public bool IsReadOnly => !IsStorageReady || IsDeviceLocked;

        /// <summary>
        /// Tlačiareň je pripravená.
        /// </summary>
        public bool IsPrinterReady => this.flags.HasFlag(ChduLiteStatusFlags.PRINTER_READY);

        /// <summary>
        /// Konfigurácia portu RS232 je neplatná, výstup RS232 je neaktívny.
        /// </summary>
        public bool? IsPrinterInitializationInvalid => this.flags.HasFlag(ChduLiteStatusFlags.PRINTER_INVALID);

        /// <summary>
        /// Inidkuje, či dvierka na tlačiarni sú otvorené.
        /// <para>
        /// Vráti hodnotu <c>null</c> v prípade, ak táto informácia nie je podporovaná vo verzii firmvéru použitom na zariadení.
        /// </para>
        /// </summary>
        public bool? IsPrinterPaperCoverOpen => this.GetSupportedStatusFlagOrNull(ChduLiteStatusFlags.PRINTER_COVER_OPEN);

        /// <summary>
        /// Na tlačiarni je stlačené tlačidlo "FEED".
        /// <para>
        /// Vráti hodnotu <c>null</c> v prípade, ak táto informácia nie je podporovaná vo verzii firmvéru použitom na zariadení.
        /// </para>
        /// </summary>
        public bool? IsPrinterFeedButtonPressed => this.GetSupportedStatusFlagOrNull(ChduLiteStatusFlags.PRINTER_FEED_BTN);

        /// <summary>
        /// Nízky stav papiera v tlačiarni 
        /// <para>
        /// Vráti hodnotu <c>null</c> v prípade, ak táto informácia nie je podporovaná vo verzii firmvéru použitom na zariadení.
        /// </para>
        /// </summary>
        public bool? IsPrinterPaperLow => this.GetSupportedStatusFlagOrNull(ChduLiteStatusFlags.PRINTER_PAPER_LOW);

        /// <summary>
        /// Chýbajúci papier v tlačiarni.
        /// <para>
        /// Vráti hodnotu <c>null</c> v prípade, ak táto informácia nie je podporovaná vo verzii firmvéru použitom na zariadení.
        /// </para>
        /// </summary>
        public bool? IsPrinterPaperEmpty => this.GetSupportedStatusFlagOrNull(ChduLiteStatusFlags.PRINTER_PAPER_END);

        /// <summary>
        /// Tlačiareň hlási chybový stav.
        /// <para>
        /// Vráti hodnotu <c>null</c> v prípade, ak táto informácia nie je podporovaná vo verzii firmvéru použitom na zariadení.
        /// </para>
        /// </summary>
        public bool? IsPrinterInErrorState => this.GetSupportedStatusFlagOrNull(ChduLiteStatusFlags.PRINTER_ERROR);

        private bool? GetSupportedStatusFlagOrNull(ChduLiteStatusFlags flag)
            => flag.IsSupportedIn(this.Version) ? this.flags.HasFlag(flag) : null;

        /// <summary>
        /// Unikátne sériové číslo zariadenia.
        /// </summary>
        public string SerialNumber => BitConverter.ToUInt32(payload, 4).ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Celková kapacita zariadenia, uvedená v blokoch.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ChduLiteStorageNotReadyException">Ak <see cref="IsStorageReady"/> je <c>false</c></exception>
        public uint GetTotalBlocksCount()
        {
            ThrowIfStorageNotReady();
            return BitConverter.ToUInt32(payload, 8);
        }

        /// <summary>
        /// Celková kapacita zariadenia, uvedená v bajtoch.
        /// </summary>
        /// <exception cref="ChduLiteStorageNotReadyException">Ak <see cref="IsStorageReady"/> je <c>false</c></exception>
        public ulong GetTotalSizeInBytes() => (ulong)this.GetTotalBlocksCount() * BlockContent.MaxLength;

        /// <summary>
        /// Zostávajúca voľná kapacita zariadenia, uvedená v blokoch.
        /// Nadobúda predvolenú hodnotu (0), ak <see cref="IsStorageReady"/> je <c>false</c>.
        /// </summary>
        public uint FreeBlocksCount => BitConverter.ToUInt32(payload, 12);

        /// <summary>
        /// Zostávajúca voľná kapacita zariadenia, uvedená v bajtoch.
        /// </summary>
        /// <exception cref="ChduLiteStorageNotReadyException">Ak <see cref="IsStorageReady"/> je <c>false</c></exception>
        public ulong GetFreeSpaceInBytes() => (ulong)this.GetTotalBlocksCount() * BlockContent.MaxLength;

        /// <summary>
        /// Použitá kapacita zariadenia, uvedená v blokoch.
        /// </summary>
        /// <exception cref="ChduLiteStorageNotReadyException">Ak <see cref="IsStorageReady"/> je <c>false</c></exception>
        public uint GetUsedBlocksCount() => this.GetTotalBlocksCount() - this.FreeBlocksCount;

        /// <summary>
        /// Použitá kapacita zariadenia, uvedená v bajtoch.
        /// Nadobúda predvolenú hodnotu (0), ak <see cref="IsStorageReady"/> je <c>false</c>.
        /// </summary>
        public ulong GetUsedSpaceInBytes() => (ulong)this.GetUsedBlocksCount() * BlockContent.MaxLength;

        /// <summary>
        /// Veľkosť jedného bloku v byte-och (maximálna dátová kapacita jedného bloku).
        /// </summary>
        [Obsolete("This value (508) is larger than actual usable part of block (505)")]
        public uint GetBlockDataSizeInBytes()
        {
            ThrowIfStorageNotReady();
            return BitConverter.ToUInt32(payload, 16);
        }

        /// <summary>
        /// Nastavená rýchlosť pre tlačiareň
        /// </summary>
        public uint PrinterBaudRate => BitConverter.ToUInt32(payload, 20);

        private void ThrowIfStorageNotReady()
        {
            if (!this.IsStorageReady)
                throw new ChduLiteStorageNotReadyException();
        }
    }
}
