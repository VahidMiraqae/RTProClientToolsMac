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
        string filePath = await CreateFile(model);

        var command = new MacPrintCommand()
            .Filename(filePath)
            .Printer(model.PrinterName)
            .Copies(model.CopyCount)
            .MediaSource(model.PaperSource);
        Console.WriteLine(command.ToString());
        await command.ExecuteAsync();
    }

    public string[] GetPrinters()
    {
        return MacPrintCommand.GetPrinters();
    }

    private async Task<string> CreateFile(Base64Print model)
    {
        var base64String = model.Base64String.Split(',')[1];
        var bytes = Convert.FromBase64String(base64String);
        var tempExportPath = MakeFilePath(model.FolderName, model.PrintFileName);
        await File.WriteAllBytesAsync(tempExportPath, bytes);
        return tempExportPath;
    }

    private async Task<string> CreateFile(TextPrint model)
    {
        string tempExportPath = MakeFilePath(model.FolderName, model.PrintFileName);
        await File.WriteAllTextAsync(tempExportPath, model.Text);
        return tempExportPath;
    }

    private string MakeFilePath(string folderName, string printFilename)
    {
        return string.IsNullOrEmpty(folderName)
            ? Path.Combine(configurations.TempDirectory, folderName, printFilename)
            : Path.Combine(configurations.TempDirectory, printFilename);
    }
}