using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NineDigit.ChduLite;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
var logger = loggerFactory.CreateLogger<Program>();

var portName = configuration["portName"]!; // e.g. 'COM3' or '/dev/ttyS0'
var printerBaudRate = Convert.ToInt32(configuration["printerBaudRate"]!);
var timeoutMs = Convert.ToInt32(configuration["timeoutMs"]!);
var timeout = TimeSpan.FromMilliseconds(timeoutMs);
var cancellationToken = CancellationToken.None;

using var chdu = new Chdu(portName, printerBaudRate, timeout); 

while (true)
{
    logger.LogInformation(@"Select an option:
---------------
[1] Get status and device info
[2] Write data

[0] Exit
");

    var actionStr = Console.ReadLine();
    if (!int.TryParse(actionStr, out int action))
        continue;

    if (action == 0)
        break;

    try
    {
        switch (action)
        {
            case 0:
                break;
            case 1:
                var status = await chdu.GetStatusAsync(cancellationToken).ConfigureAwait(true);
                Console.WriteLine("Status flags: ");
                Console.WriteLine(" - " + nameof(status.Flags) + ": " + status.Flags);
                Console.WriteLine(" - " + nameof(status.IsStorageReady) + ": " + status.IsStorageReady);
                Console.WriteLine(" - " + nameof(status.IsDeviceLocked) + ": " + status.IsDeviceLocked);
                Console.WriteLine(" - " + nameof(status.IsPrinterReady) + ": " + status.IsPrinterReady);
                Console.WriteLine(" - " + nameof(status.IsPrinterInitializationInvalid) + ": " + status.IsPrinterInitializationInvalid);
                Console.WriteLine(" - " + nameof(status.IsPrinterPaperCoverOpen) + ": " + status.IsPrinterPaperCoverOpen);
                Console.WriteLine(" - " + nameof(status.IsPrinterFeedButtonPressed) + ": " + status.IsPrinterFeedButtonPressed);
                Console.WriteLine(" - " + nameof(status.IsPrinterPaperLow) + ": " + status.IsPrinterPaperLow);
                Console.WriteLine(" - " + nameof(status.IsPrinterPaperEmpty) + ": " + status.IsPrinterPaperEmpty);
                Console.WriteLine(" - " + nameof(status.IsPrinterInErrorState) + ": " + status.IsPrinterInErrorState);
                Console.WriteLine("Firmware: ");
                Console.WriteLine(" - " + nameof(status.Version) + ": " + status.Version);
                Console.WriteLine(" - " + nameof(status.VersionString) + ": " + status.VersionString);
                var firmwareVersionDescription = await chdu.GetFirmwareVersionDescriptionAsync(cancellationToken);
                Console.WriteLine($" - Firmware description: {firmwareVersionDescription}");
                Console.WriteLine("Volume info: ");
                var volumeInfo = await chdu.GetVolumeInfoAsync(cancellationToken).ConfigureAwait(true);
                var manufacturingDate = volumeInfo.GetManufacturingDateUtc();
                Console.WriteLine($" - Manufacturing date: {manufacturingDate}");
                var usedBlocksCount = status.GetUsedBlocksCount();
                Console.WriteLine($" - USed blocks count: {usedBlocksCount}");
                break;
            case 2:
                var dataToWriteHexString = "12-00-13-00-6F-38-58-72-2B-4E-73-51-7A-79-34-32-77-31-52-75-45-5A-6B-4C-45-2F-67-48-6B-59-78-44-63-76-6B-57-42-57-48-45-46-4C-6A-31-45-59-72-38-35-4F-31-51-49-61-34-4F-2F-38-78-56-71-34-45-38-65-4B-46-63-68-70-2F-7A-6C-62-6D-79-67-44-4F-65-6B-4C-31-32-33-65-54-70-66-46-72-4B-38-2F-68-39-6D-63-6A-63-32-64-48-34-74-6E-43-6D-69-4C-76-4E-41-4E-64-42-61-36-66-46-47-73-79-70-54-6A-4C-33-6C-52-4F-67-52-77-54-55-41-38-77-33-6B-4A-74-4E-67-79-6B-7A-67-77-73-70-30-68-79-30-64-6F-6A-55-4E-68-4F-75-67-6C-59-70-68-59-71-55-69-4B-73-67-49-68-44-36-7A-73-55-6B-62-65-77-77-79-6F-79-59-67-54-52-6C-46-47-48-4D-6F-37-70-53-61-53-65-31-77-72-42-75-42-63-58-47-77-4C-53-37-49-43-72-4E-6F-37-50-4E-32-32-47-69-5A-47-62-2B-6C-57-2F-77-37-74-65-6C-63-77-43-5A-61-67-30-55-64-5A-4F-58-64-78-4B-44-38-53-46-41-38-55-71-59-43-51-56-4B-67-3D-3D-3C-2F-50-4B-50-3E-3C-4F-4B-50-20-64-69-67-65-73-74-3D-5C-22-53-48-41-31-5C-22-20-65-6E-63-6F-64-69-6E-67-3D-5C-22-62-61-73-65-31-36-5C-22-3E-33-31-31-32-62-39-34-65-2D-39-37-66-65-36-38-64-30-2D-34-65-37-32-63-38-31-36-2D-64-38-37-38-37-30-66-35-2D-31-38-30-31-63-30-32-33-3C-2F-4F-4B-50-3E-3C-2F-56-61-6C-69-64-61-74-69-6F-6E-43-6F-64-65-3E-3C-2F-52-65-67-69-73-74-65-72-52-65-63-65-69-70-74-52-65-71-75-65-73-74-3E-3C-2F-73-6F-61-70-3A-42-6F-64-79-3E-3C-2F-73-6F-61-70-3A-45-6E-76-65-6C-6F-70-65-3E-22-7D";
                var dataToWrite = dataToWriteHexString.Split('-').Select(hex => Convert.ToByte(hex, 16)).ToArray();
                logger.LogInformation("The following data is written to device: {dataToWriteHexString} (decimal: {data}", dataToWriteHexString, dataToWrite);
                var block = new BlockContent(dataToWrite);
                var writeResult = await chdu.WriteBlockAsync(block, cancellationToken);
                break;
            default:
                logger.LogWarning("Unknown option selected.");
                continue;
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Exception occured during action execution.");
    }
}


/*
var batchBlocks = await chdu.ReadBlocksAsync(500, 1000, cancellationToken).ConfigureAwait(true);

var sw = new Stopwatch();

// sequential test
var rounds = new long[count];
if (count > 0)
{
    for (uint i = 0; i < count; i++)
    {
        sw.Restart();
        await chdu.ReadBlockAsync(i, cancellationToken).ConfigureAwait(true);
        sw.Stop();
        rounds[i] = sw.ElapsedMilliseconds;
    }

    Console.WriteLine(sw.ElapsedMilliseconds);
}

var sequentialReadTotalMs = rounds.Sum();
var sequentialReadAverageMs = rounds.Average();

Console.WriteLine($"Sequential block(s) read: " + count);
Console.WriteLine($"Sequential read total ms: " + sequentialReadTotalMs);
Console.WriteLine($"Sequential read avg ms: " + sequentialReadAverageMs);
// ~5.1ms/block

// batch test
var batchRepeat = 10;
var batchRounds = new long[batchRepeat];
var batchBlocksToRead = 127u;
for (int i = 0; i < batchRepeat; i++)
{
    sw.Restart();
    var blocks = await chdu.ReadBlocksAsync(0, batchBlocksToRead, cancellationToken).ConfigureAwait(true);
    batchRounds[i] = sw.ElapsedMilliseconds;
    sw.Stop();
}

Console.WriteLine($"Batch blocks read: " + batchBlocksToRead);
Console.WriteLine($"Batch read total ms: " + batchRounds.Sum());
Console.WriteLine($"Batch read min ms: " + batchRounds.Min());
Console.WriteLine($"Batch read max ms: " + batchRounds.Max());
Console.WriteLine($"Batch read avg ms: " + batchRounds.Average());
// ~3ms/block
// max blocks = 127 --> 381ms max timeout. 1200 is default timeout from config.
*/