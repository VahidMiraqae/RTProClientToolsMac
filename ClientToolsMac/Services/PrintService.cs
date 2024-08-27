using ClientToolsMac.MacPrint;
using ClientToolsMac.Services;
using RTProClientToolsMac.ViewModels;

namespace RTProClientToolsMac.Controllers;

public class PrintService(
    PrintFileResolverService printFileResolverService,
    Configurations configurations
    )
{
    public string[] GetPrinters()
    {
        return MacPrintCommand.GetPrinters();
    }

    public async Task PrintAsync(TextPrint model)
    {
        var (absolutePath, relativePath) = await printFileResolverService.MakeFile(model.Text, TextContentType.Base64, model);
        var printJobData = new PrinterJobDataInfo()
        {
            FilePath = absolutePath,
            CopyCount = model.CopyCount,
            PrinterName = model.PrinterName,
            PaperSource = model.PaperSource
        };
        var printJobTask = CommitPrintJob(printJobData);
        var copyFileTask = CopyFile(absolutePath, relativePath);
        await Task.WhenAll(printJobTask, copyFileTask);
    }

    public async Task PrintAsync(Base64Print model)
    {
        var (absolutePath, _) = await printFileResolverService.MakeFile(model.Base64String, TextContentType.PlainText, model);
        var printJobData = new PrinterJobDataInfo()
        {
            FilePath = absolutePath,
            CopyCount = model.CopyCount,
            PrinterName = model.PrinterName,
            PaperSource = model.PaperSource
        };
        await CommitPrintJob(printJobData);
    }

    public async Task PrintAsync(TextZPLPrint model)
    {
        var (absolutePath, _) = await printFileResolverService.MakeFile(model.TextZPL, TextContentType.PlainText);
        var printJobData = new PrinterJobDataInfo()
        {
            FilePath = absolutePath,
            CopyCount = model.CopyCount,
            PrinterName = model.PrinterName,
        };
        await CommitPrintJob(printJobData);
    }

    public async Task PrintAsync(Base64ZPLPrint model)
    {
        var (absolutePath, _) = await printFileResolverService.MakeFile(model.Base64String, TextContentType.Base64);
        var printJobData = new PrinterJobDataInfo()
        {
            FilePath = absolutePath,
            CopyCount = model.CopyCount,
            PrinterName = model.PrinterName,
        };
        await CommitPrintJob(printJobData);
    }

    private async Task CopyFile(string filePath, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(configurations.CopyPrintPath))
        {
            await Task.CompletedTask;
        }
        var destinationPath = Path.Combine(configurations.CopyPrintPath, relativePath);
        var destinationDirectory = Path.GetDirectoryName(destinationPath);
        await Task.Run(() =>
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            File.Copy(filePath, destinationPath);
        });
    }

    private static async Task CommitPrintJob(PrinterJobDataInfo printerJobDataInfo)
    {
        var command = new MacPrintCommand()
            .Filename(printerJobDataInfo.FilePath)
            .Printer(printerJobDataInfo.PrinterName)
            .Copies(printerJobDataInfo.CopyCount)
            .MediaSource(printerJobDataInfo.PaperSource);
        Console.WriteLine(command.ToString());
        await command.ExecuteAsync();
    }
}