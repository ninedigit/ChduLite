using System;

namespace NineDigit.ChduLite
{
    [Flags]
    internal enum ChduLiteStatusFlags
    {
        /// <summary>
        /// Zariadenie naštartovalo v poriadku a je pripravené na použitie
        /// </summary>
        STORAGE_OK = 0x0001,
        /// <summary>
        /// Tlačiareň je pripravená
        /// </summary>
        PRINTER_READY = 0x0002,
        /// <summary>
        /// Zariadenie je uzamknuté, zápis nie je možný
        /// </summary>
        DEVICE_LOCK = 0x0004,
        /// <summary>
        /// Vyhradené pre interné použitie
        /// </summary>
        RESERVED = 0x0008,
        /// <summary>
        /// Konfigurácia portu RS232 je neplatná, výstup RS232 je neaktívny.
        /// </summary>
        PRINTER_INVALID = 0x0010,
        /// <summary>
        /// Dvierka na tlačiarni sú otvorené.
        /// </summary>
        PRINTER_COVER_OPEN = 0x0020,
        /// <summary>
        /// Na tlačiarni je stlačené tlačidlo "FEED".
        /// </summary>
        PRINTER_FEED_BTN = 0x0040,
        /// <summary>
        /// Chýbajúci papier v tlačiarni.
        /// </summary>
        PRINTER_PAPER_END = 0x0080,
        /// <summary>
        /// Tlačiareň hlási chybový stav.
        /// </summary>
        PRINTER_ERROR = 0x0100,
        /// <summary>
        /// Nízky stav papiera v tlačiarni 
        /// </summary>
        PRINTER_PAPER_LOW = 0x0200,
        /// <summary>
        /// Zariadenie sa nachádza v diagnostickom režime
        /// </summary>
        DIAGNOSTIC_MODE = 0x8000,
    }

    internal static class ChduLiteStatusFlagsExtensions
    {
        /// <summary>
        /// Gets whether given flag is supported in specified firmware version.
        /// </summary>
        /// <param name="self">Flag to evaluate</param>
        /// <param name="firmwareVersion">Firmware version</param>
        /// <returns>True, if flag is supported in given firmware version, false otherwise.</returns>
        public static bool IsSupportedIn(this ChduLiteStatusFlags self, Version firmwareVersion)
        {
            switch (self)
            {
                case ChduLiteStatusFlags.PRINTER_COVER_OPEN:
                case ChduLiteStatusFlags.PRINTER_FEED_BTN:
                case ChduLiteStatusFlags.PRINTER_PAPER_END:
                case ChduLiteStatusFlags.PRINTER_ERROR:
                case ChduLiteStatusFlags.PRINTER_PAPER_LOW:
                case ChduLiteStatusFlags.DIAGNOSTIC_MODE:
                    return firmwareVersion.Major >= 1 && firmwareVersion.Minor >= 4;
                default:
                    return true;
            }
        }
    }
}
