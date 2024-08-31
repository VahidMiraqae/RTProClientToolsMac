using RTProClientToolsMac.Enums;
using RTProClientToolsMac.Models.Contracts;
using System.Text;

namespace RTProClientToolsMac.Services;


public class PrintFileResolver
{
    private readonly Configurations _configurations;
    public delegate Task ActionOnCreatedFile(string relativePath, string absolutePath);

    public PrintFileResolver(Configurations configurations)
    {
        _configurations = configurations;
    }

    public async Task MakeTempFile(string? text, TextContentType type, ActionOnCreatedFile action, IPrintFilePath? printFilePath = null)
    {
        if (text is null)
        {
            await Task.CompletedTask;
            return;
        }
        string relativePath;
        if (printFilePath is not null)
        {
            var filename = printFilePath.PrintFileName ?? Guid.NewGuid().ToString("N");
            relativePath = !string.IsNullOrEmpty(printFilePath.FolderName)
                   ? Path.Combine(printFilePath.FolderName, filename)
                   : filename;
        }
        else
        {
            relativePath = Guid.NewGuid().ToString("N");
        }
        var absolutePath = Path.Combine(_configurations.TempDirectory, relativePath);
        try
        {
            var directory = Path.GetDirectoryName(absolutePath);
            if (directory is not null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await File.WriteAllBytesAsync(absolutePath, ToBytes(text, type));
            await action(relativePath, absolutePath);
        }
        finally
        {
            DeleteFile(absolutePath);
        }
    }

    private void DeleteFile(string filePath)
    {
        if (filePath is null || !File.Exists(filePath))
        {
            return;
        }

        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (directory is null)
            {
                return;
            }
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