using ClientToolsMac.MacPrint;
using RTProClientToolsMac.ViewModels;

namespace RTProClientToolsMac.Controllers;

public class PrintService(
    Configurations configurations
    )
{
    public async Task PrintAsync(Base64Print model)
    {
        string filePath = await CreateFile(model);

        var command = new MacPrintCommand()
            .Filename(filePath)
            .Printer(model.PrinterName)
            .Copies(model.CopyCount)
            .MediaSource(model.PaperSource);
        Console.WriteLine(command.ToString());
        await command.ExecuteAsync();
    }

    public async Task PrintAsync(TextPrint model)
    {
        throw new NotImplementedException();
    }

    public string[] GetPrinters()
    {
        return MacPrintCommand.GetPrinters();
    }

    private async Task<string> CreateFile(Base64Print model)
    {
        var base64String = model.Base64String.Split(',')[1];
        var tempExportPath = string.IsNullOrEmpty(model.FolderName)
            ? Path.Combine(configurations.TempDirectory, model.FolderName, model.PrintFileName)
            : Path.Combine(configurations.TempDirectory, model.PrintFileName);
        var bytes = Convert.FromBase64String(base64String);
        await File.WriteAllBytesAsync(tempExportPath, bytes);
        return tempExportPath;
    }
}