namespace NineDigit.ChduLite
{
    internal enum ChduLiteCommandId
    {
        ReadStatus = 0x5A,
        ReadData = 0xB1,
        WriteData = 0xB2,
        WriteDataAndPrint = 0xB3,
        WriteDataAndPrintWithOffset = 0xB4,
        OpenDrawer = 0x12,
        OpenDrawer2 = 0x13,
        LockRequest = 0xC1,
        LockActivate = 0xC2,
        VolumeSerial = 0x5B,
        Version2 = 0x5C
    }
}
