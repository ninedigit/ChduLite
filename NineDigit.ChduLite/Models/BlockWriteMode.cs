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
        {
            switch (self)
            {
                case ChduLiteCommandId.WriteData:
                    return BlockWriteMode.Save;
                case ChduLiteCommandId.WriteDataAndPrint:
                case ChduLiteCommandId.WriteDataAndPrintWithOffset:
                    return BlockWriteMode.SaveAndPrint;
                default:
                    throw new InvalidOperationException($"Command ID {self} cannot be converted to block write mode.");
            }
        }
    }
}
