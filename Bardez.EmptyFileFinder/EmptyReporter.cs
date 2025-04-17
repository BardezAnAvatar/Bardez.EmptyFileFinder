using System.Threading.Tasks;

namespace Bardez.EmptyFileFinder;

internal class EmptyReporter : IDisposable
{
    private readonly StreamWriter _stream;
    private readonly SemaphoreSlim _slim = new SemaphoreSlim(20);
    private readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1);

    public EmptyReporter()
    {
        var current = Directory.GetCurrentDirectory();
        var report = $"Report-{DateTime.Now.ToUniversalTime().ToString("yyyyMMdd_hhmmssss")}.log";
        var reportPath = Path.Combine(current, report);
        _stream = File.CreateText(reportPath);
    }

    ~EmptyReporter()
    {
        Dispose();
    }

    public void Dispose()
    {
        _stream.Flush();
        _stream.Close();
        _stream.Dispose();
        _slim.Dispose();
        _fileLock.Dispose();
    }

    internal async Task CheckForEmptyFiles(CancellationToken cancel)
    {
        var current = Directory.GetCurrentDirectory();
        _stream.WriteLine($"Evaluating `{current}`:");

        var dirInfo = new DirectoryInfo(current);
        var tasks = ReadDirectory(dirInfo, cancel);

        //wait for it all
        await Task.WhenAll(tasks);
    }

    internal async Task CheckForEmptyFiles(string path, CancellationToken cancel)
    {
        _stream.WriteLine($"Evaluating `{path}`:");

        var dirInfo = new DirectoryInfo(path);
        var tasks = ReadDirectory(dirInfo, cancel);

        //wait for it all
        await Task.WhenAll(tasks);
    }

    private List<Task> ReadDirectory(DirectoryInfo directory, CancellationToken cancel)
    {
        var tasks = new List<Task>();

        var files = directory.GetFiles();
        foreach (var file in files)
        {
            if (file.Exists)
            {
                var task = ReadFile(file, cancel);
                tasks.Add(task);
            }
        }

        var directories = directory.GetDirectories();
        foreach (var dir in directories)
        {
            var dirTasks = ReadDirectory(dir, cancel);
            tasks.AddRange(tasks);
        }

        return tasks;
    }

    private async Task ReadFile(FileInfo file, CancellationToken cancel)
    {
        await _slim.WaitAsync();
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
                await ReportNulFile(file);
            }
        }

        _slim.Release();
    }

    internal async Task ReportNulFile(FileInfo file)
    { 
        await _fileLock.WaitAsync();
        await _stream.WriteLineAsync($"NUL Contents: {file}");
        _fileLock.Release();
    }
}