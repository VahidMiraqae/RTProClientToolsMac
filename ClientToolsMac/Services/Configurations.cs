namespace ClientToolsMac.Services;

public class Configurations(IConfiguration configuration)
{
    private const string FILES_TEMP_FOLDER_NAME = "Temp";

    public string? CopyPrintPath { get; } = configuration.GetSection("CopyPrintPath").Value;
    public string TempDirectory
    {
        get
        {
            var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILES_TEMP_FOLDER_NAME);
            Directory.CreateDirectory(tempPath);
            return tempPath;
        }
    }
    public TimeSpan KeepTempFilesFor { get; } = TimeSpan.FromDays(1);

}