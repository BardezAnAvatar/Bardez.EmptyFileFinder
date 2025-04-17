using System.Threading.Tasks;

namespace Bardez.EmptyFileFinder;

internal class Checker
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
