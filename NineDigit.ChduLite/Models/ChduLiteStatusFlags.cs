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
        PRINTER_INVALID = 0x0010
    }
}
