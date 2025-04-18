using Bardez.EmptyFileFinder.Configuration;
using Microsoft.Extensions.Options;

namespace Bardez.EmptyFileFinder.Business;

internal class Checker(EmptyReporter emptyFileReporter, IOptions<CheckerOptions> options)
{
    internal static async Task CheckForEmptyFiles(CancellationToken cancel)
    {
        using EmptyReporter emptyFileReporter = new EmptyReporter();
        await emptyFileReporter.CheckForEmptyFiles(cancel);
    }

    internal static async Task CheckForEmptyFiles(string path, CancellationToken cancel)
    {
        using EmptyReporter emptyFileReporter = new EmptyReporter();
        await emptyFileReporter.CheckForEmptyFiles(path, cancel);
    }
}
