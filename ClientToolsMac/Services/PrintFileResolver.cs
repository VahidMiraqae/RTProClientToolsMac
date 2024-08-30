using ClientToolsMac.Enums;
using ClientToolsMac.Models.Contracts;
using System.Text;

namespace ClientToolsMac.Services;


public class PrintFileResolver
{
    private Configurations _configurations;
    private PeriodicTimer _periodicTimer;
    public event Action<string> FileCreated;

    public PrintFileResolver(Configurations configurations)
    {
        _configurations = configurations;
    }

    public async Task<(string absolutePath, string relativePath)> MakeFile(string text, TextContentType type, IPrintFilePath printFilePath)
    {
        var filename = printFilePath.PrintFileName ?? Guid.NewGuid().ToString("N");
        var relativePath = !string.IsNullOrEmpty(printFilePath.FolderName)
               ? Path.Combine(printFilePath.FolderName, filename)
               : filename;
        var absolutePath = Path.Combine(_configurations.TempDirectory, relativePath);
        var bytes = ToBytes(text, type);
        var dir = Path.GetDirectoryName(absolutePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        await File.WriteAllBytesAsync(absolutePath, bytes);
        FileCreated?.Invoke(absolutePath);
        return (absolutePath, relativePath);
    }

    public async Task<(string absolutePath, string relativePath)> MakeFile(string text, TextContentType type)
    {
        var relativePath = Guid.NewGuid().ToString("N");
        var absolutePath = Path.Combine(_configurations.TempDirectory, relativePath);
        var bytes = ToBytes(text, type);
        await File.WriteAllBytesAsync(absolutePath, bytes);
        FileCreated?.Invoke(absolutePath);
        return (absolutePath, relativePath);
    }

    private static byte[] ToBytes(string text, TextContentType type)
    {
        return type switch
        {
            TextContentType.PlainText => Encoding.UTF8.GetBytes(text),
            TextContentType.Base64 => Convert.FromBase64String(text.Split(',')[1]),
            _ => throw new NotSupportedException($"Text Content Type of '{type}' is not supported.")
        };
    }
}