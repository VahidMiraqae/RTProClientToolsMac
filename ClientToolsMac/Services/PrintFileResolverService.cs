using RTProClientToolsMac.Controllers;
using RTProClientToolsMac.ViewModels;
using System.Text;

namespace ClientToolsMac.Services;


public class PrintFileResolverService
{
    private Configurations _configurations;
    private PeriodicTimer _periodicTimer;

    public PrintFileResolverService(Configurations configurations)
    {
        _configurations = configurations;
    }

    public async Task StartPeriodicFileDeleter()
    {
#if DEBUG
        _periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
#else
        _periodicTimer = new PeriodicTimer(TimeSpan.FromHours(1));
#endif
        while (await _periodicTimer.WaitForNextTickAsync())
        {
            var nowUtc = DateTime.UtcNow;
            var directories = Directory.GetFiles(_configurations.TempDirectory, "*", SearchOption.AllDirectories)
                .GroupBy(Path.GetDirectoryName)
                .Select(a => new XXX([.. a], a.Key));
            foreach (var directory in directories)
            {
                foreach (var file in directory.FilePaths)
                {
                    var remainingFiles = directory.FilePaths.Length;
                    var creationTime = File.GetCreationTimeUtc(file);
#if DEBUG
                    if ((nowUtc - creationTime).TotalMinutes > 3)
#else
                    if ((nowUtc - creationTime).TotalDays > 1)
#endif
                    {
                        try
                        {
                            if (directory.FilePaths.Length == 1 && !directory.Path.Equals(_configurations.TempDirectory))
                            {
                                Directory.Delete(directory.Path, true);
                            }
                            else
                            {
                                File.Delete(file);
                            }
                        }
                        catch { }
                    }
                }
            }
        }
    }

    record XXX(string[] FilePaths, string Path);

    public async Task<(string absolutePath, string relativePath)> MakeFile(string text, TextContentType type, IPrintFilePath printFilePath)
    {
        var filename = printFilePath.PrintFileName ?? Guid.NewGuid().ToString("N");
        var relativePath = !string.IsNullOrEmpty(printFilePath.FolderName)
               ? Path.Combine(printFilePath.FolderName, filename)
               : filename;
        var absolutePath = Path.Combine(_configurations.TempDirectory, relativePath);
        var bytes = ToBytes(text, type);
        await File.WriteAllBytesAsync(absolutePath, bytes);
        return (absolutePath, relativePath);
    }

    public async Task<(string absolutePath, string relativePath)> MakeFile(string text, TextContentType type)
    {
        var relativePath = Guid.NewGuid().ToString("N");
        var absolutePath = Path.Combine(_configurations.TempDirectory, relativePath);
        var bytes = ToBytes(text, type);
        await File.WriteAllBytesAsync(absolutePath, bytes);
        return (absolutePath, relativePath);
    }

    private static byte[] ToBytes(string text, TextContentType type)
    {
        return type switch
        {
            TextContentType.PlainText => Encoding.UTF8.GetBytes(text),
            TextContentType.Base64 => Convert.FromBase64String(text),
            _ => throw new NotSupportedException($"Text Content Type of '{type}' is not supported.")
        };
    }
}