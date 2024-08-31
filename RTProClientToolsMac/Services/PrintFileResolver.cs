using RTProClientToolsMac.Enums;
using RTProClientToolsMac.Models.Contracts;
using System.Text;

namespace RTProClientToolsMac.Services;


public class PrintFileResolver
{
    private Configurations _configurations;
    private PeriodicTimer _periodicTimer;
    public delegate Task ActionOnCreatedFile(string relativePath, string absolutePath);

    public PrintFileResolver(Configurations configurations)
    {
        _configurations = configurations;
    }

    public async Task MakeFile(string text, TextContentType type, IPrintFilePath printFilePath, ActionOnCreatedFile action)
    {
        var filename = printFilePath.PrintFileName ?? Guid.NewGuid().ToString("N");
        var relativePath = !string.IsNullOrEmpty(printFilePath.FolderName)
               ? Path.Combine(printFilePath.FolderName, filename)
               : filename;
        var absolutePath = Path.Combine(_configurations.TempDirectory, relativePath);
        var bytes = ToBytes(text, type);
        try
        {
            var dir = Path.GetDirectoryName(absolutePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            await File.WriteAllBytesAsync(absolutePath, bytes);
            await action(relativePath, absolutePath);
        }
        finally
        {
            DeleteFile(absolutePath);
        }
    }

    public async Task MakeFile(string text, TextContentType type, ActionOnCreatedFile action)
    {
        var relativePath = Guid.NewGuid().ToString("N");
        var absolutePath = Path.Combine(_configurations.TempDirectory, relativePath);
        var bytes = ToBytes(text, type);
        try
        {
            await File.WriteAllBytesAsync(absolutePath, bytes);
            await action(relativePath, absolutePath);
        }
        finally
        {
            DeleteFile(absolutePath);
        }
    }

    private void DeleteFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        try
        {
            var directory = Path.GetDirectoryName(filePath);
            var filesCountInDirectory = Directory.GetFiles(filePath).Length;
            if (filesCountInDirectory == 1 && !directory.Equals(_configurations.TempDirectory))
            {
                Directory.Delete(directory, true);
            }
            else
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            // ignored
        }
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