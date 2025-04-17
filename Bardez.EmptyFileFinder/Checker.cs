using System.Threading.Tasks;

namespace Bardez.EmptyFileFinder;

internal class Checker
{
    internal static async Task CheckForEmptyFiles(CancellationToken cancel)
    {
        var current = Directory.GetCurrentDirectory();
        var dirInfo = new DirectoryInfo(current);
        using SemaphoreSlim slim = new SemaphoreSlim(20);
        var tasks = ReadDirectory(dirInfo, slim, cancel);

        //wait for it all
        await Task.WhenAll(tasks);
    }

    internal static async Task CheckForEmptyFiles(string path, CancellationToken cancel)
    {
        var dirInfo = new DirectoryInfo(path);
        using SemaphoreSlim slim = new SemaphoreSlim(20);
        var tasks = ReadDirectory(dirInfo, slim, cancel);

        //wait for it all
        await Task.WhenAll(tasks);
    }

    private static List<Task> ReadDirectory(DirectoryInfo directory, SemaphoreSlim slim, CancellationToken cancel)
    {
        var tasks = new List<Task>();

        var files = directory.GetFiles();
        foreach (var file in files)
        {
            if (file.Exists)
            {
                var task = ReadFile(slim, file, cancel);
                tasks.Add(task);
            }
        }

        var directories = directory.GetDirectories();
        foreach (var dir in directories)
        {
            var dirTasks = ReadDirectory(dir, slim, cancel);
            tasks.AddRange(tasks);
        }

        return tasks;
    }

    private static async Task ReadFile(SemaphoreSlim slim, FileInfo file, CancellationToken cancel)
    {
        await slim.WaitAsync();
        using var fileHandle = file.OpenRead();
        if (fileHandle != null && fileHandle.CanRead)
        {
            var empty = true;

            byte[] data = new byte[16_777_216]; //16 MB
            while (fileHandle.Position < file.Length && empty)  //while I can read && all I have found is empty bytes
            {
                var amount = await fileHandle.ReadAsync(data, 0, 16_777_216, cancel);
                for (int i = 0; i < amount && empty; i++)
                {
                    if (data[i] != 0)
                    {
                        empty = false;
                    }
                }
            }

            //if I'm here, I've escaped or the file is empty
            if (empty)
            {
                Console.WriteLine($"NUL Contents: {file}");
            }
        }

        slim.Release();
    }
}
