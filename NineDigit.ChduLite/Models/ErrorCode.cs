namespace NineDigit.ChduLite
{
    public enum ErrorCode
    {
        Unknown = 0,
        /// <summary>
        /// Operation timeout
        /// </summary>
        Timeout,
        /// <summary>
        /// Invalid framing
        /// </summary>
        InvalidFrame,
        /// <summary>
        /// Incalid command
        /// </summary>
        InvalidCommand,
        /// <summary>
        /// Invalid command parameter (e.g. out of range)
        /// </summary>
        InvalidParameter,
        /// <summary>
        /// Invalid checksum
        /// </summary>
        InvalidCrc,
        /// <summary>
        /// The error raised when reading is attempted past the end of a memory.
        /// </summary>
        InvalidReadAddress,
        /// <summary>
        /// System locked
        /// </summary>
        MemoryLocked,
        /// <summary>
        /// Hardware failure (SD card communication failed)
        /// </summary>
        HardwareFailure,
        /// <summary>
        /// Printer not ready
        /// </summary>
        PrinterNotReady,
        /// <summary>
        /// Device is busy (processing another request).
        /// </summary>
        Busy
    }
}