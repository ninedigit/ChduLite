namespace NineDigit.ChduLite
{
    /// <summary>
    /// Constants of used non-printable control characters.
    /// </summary>
    internal static class ControlChars
    {
        /// <summary>
        /// Null (0d / 0x00)
        /// </summary>
        public const char NUL = (char)0x00;

        /// <summary>
        /// Start of text (2d / 0x02)
        /// </summary>
        public const char STX = (char)0x02;

        /// <summary>
        /// End of transmission (4d / 0x04)
        /// </summary>
        public const char EOT = (char)0x04;

        /// <summary>
        /// Acknowledge (6d / 0x06)
        /// </summary>
        public const char ACK = (char)0x06;

        /// <summary>
        /// Bell (7d / 0x07)
        /// </summary>
        public const char BEL = (char)0x07;

        /// <summary>
        /// Negative acknowledge (21d / 0x15)
        /// </summary>
        public const char NAK = (char)0x015;
    }
}