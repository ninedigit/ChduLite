// let's use more stricter 10 ms (instead of config default value 1200 ms).

using System.Diagnostics;
using NineDigit.ChduLite;

var timeout = TimeSpan.FromMilliseconds(10);

using var chdu = new Chdu("COM4", 38400, timeout); // /dev/ttyS0

var cancellationToken = CancellationToken.None;
var status = await chdu.GetStatusAsync(cancellationToken).ConfigureAwait(true);
var firmwareVersionDescription = await chdu.GetFirmwareVersionDescriptionAsync(cancellationToken);
var count = status.GetUsedBlocksCount();

var volumeInfo = await chdu.GetVolumeInfoAsync(cancellationToken).ConfigureAwait(true);
var manufacturingDate = volumeInfo.GetManufacturingDateUtc();
Console.WriteLine($"Manufacturing date: {manufacturingDate}");

//try
//{
//    var x = await chdu.ReadBlocksAsync(count - 20, 10, cancellationToken);
//}
//catch (Exception ex)
//{
//}

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