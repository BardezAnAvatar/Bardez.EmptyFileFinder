using Bardez.EmptyFileFinder.Configuration;
using Microsoft.Extensions.Options;

namespace Bardez.EmptyFileFinder.Business;

internal class Checker(EmptyReporter emptyFileReporter, IOptions<CheckerOptions> options)
{
    internal async Task CheckForEmptyFiles(bool useCurrentDir, CancellationToken cancel)
    {
        var path = useCurrentDir ? Directory.GetCurrentDirectory() : options.Value.Path;

        Console.WriteLine(DateTimeOffset.UtcNow);
        Console.WriteLine($"Evaluating `{path}` ...");
        Console.WriteLine("Checking for NUL-only files...");

        await emptyFileReporter.CheckForEmptyFiles(path, cancel);

        Console.WriteLine(string.Empty);
        Console.WriteLine(string.Empty);
        Console.WriteLine("Process complete!");
        Console.WriteLine(DateTimeOffset.UtcNow);
    }
}
