
using ClientToolsMac.Utilities;
using RTProClientToolsMac.ViewModels;

namespace RTProClientToolsMac.Controllers;

public class PrintService(
    Configurations configurations
    )
{
    public async Task PrintAsync(Base64Print model)
    {
        var base64String = model.base64String.Split(',')[1];
        var filePath = await CreateFile(base64String, model.printFileName, model.folderName);

        var command = new MacPrintCommand().Filename(filePath)
            .Printer(model.printerName)
            .Copies(model.CopyCount)
            .MediaSource(model.paperSource);
        Console.WriteLine(command.ToString());
        await command.ExecuteAsync();
    }

    private async Task<string> CreateFile(string base64String, string printFileName, string folderName = "")
    {
        var tempExportPath = string.IsNullOrEmpty(folderName)
            ? Path.Combine(configurations.TempDirectory, folderName, printFileName)
            : Path.Combine(configurations.TempDirectory, printFileName);
        var bytes = Convert.FromBase64String(base64String);
        await File.WriteAllBytesAsync(tempExportPath, bytes);
        return tempExportPath;
    }

}