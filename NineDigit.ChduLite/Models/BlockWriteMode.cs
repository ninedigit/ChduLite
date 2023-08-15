using System;

namespace NineDigit.ChduLite
{
    /// <summary>
    /// Mód zápisu bloku
    /// </summary>
    public enum BlockWriteMode
    {
        /// <summary>
        /// Blok zapísaný v pamäťovom priestore úložiska.
        /// </summary>
        Save = 0xB2,
        /// <summary>
        /// Blok zapísaný v pamäťovom priestore úložiska a zároveň vytlačený na tlačiareň
        /// </summary>
        SaveAndPrint = 0xB3,
    }

    internal static class BlockWriteModeExtensions
    {
        public static BlockWriteMode ToBlockWriteMode(this ChduLiteCommandId self)
            => self switch
            {
                ChduLiteCommandId.WriteData => BlockWriteMode.Save,
                ChduLiteCommandId.WriteDataAndPrint or ChduLiteCommandId.WriteDataAndPrintWithOffset => BlockWriteMode.SaveAndPrint,
                _ => throw new InvalidOperationException($"Command ID {self} cannot be converted to block write mode."),
            };
    }
}
