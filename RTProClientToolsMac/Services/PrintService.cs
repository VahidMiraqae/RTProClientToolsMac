using RTProClientToolsMac.Enums;
using RTProClientToolsMac.Infos;
using RTProClientToolsMac.MacPrint;
using RTProClientToolsMac.Models;

namespace RTProClientToolsMac.Services;

public class PrintService(
    PrintFileResolver printFileResolver,
    Configurations configurations
    )
{
    public string[] GetPrinters()
    {
        return MacPrintCommand.GetPrinters();
    }

    public async Task PrintAsync(TextPrint textPrint)
    {
        await printFileResolver.MakeTempFile(textPrint.Text, TextContentType.PlainText, async (relativePath, absolutePath) =>
        {
            var printJobData = new PrinterJobDataInfo(absolutePath)
            {
                CopyCount = textPrint.CopyCount,
                PrinterName = textPrint.PrinterName,
                PaperSource = textPrint.PaperSource
            };
            var printJobTask = CommitPrintJob(printJobData);
            var copyFileTask = CopyFile(absolutePath, relativePath);
            await Task.WhenAll(printJobTask, copyFileTask);
        }, textPrint);
    }

    public async Task PrintAsync(Base64Print base64Print)
    {
        await printFileResolver.MakeTempFile(base64Print.Base64String, TextContentType.Base64, async (relativePath, absolutePath) =>
        {
            var printJobData = new PrinterJobDataInfo(absolutePath)
            {
                CopyCount = base64Print.CopyCount,
                PrinterName = base64Print.PrinterName,
                PaperSource = base64Print.PaperSource
            };
            await CommitPrintJob(printJobData);
        }, base64Print);
    }

    public async Task PrintAsync(TextZPLPrint textZplPrint)
    {
        await printFileResolver.MakeTempFile(textZplPrint.TextZPL, TextContentType.PlainText, async (relativePath, absolutePath) =>
        {
            var printJobData = new PrinterJobDataInfo(absolutePath)
            {
                CopyCount = textZplPrint.CopyCount,
                PrinterName = textZplPrint.PrinterName,
            };
            await CommitPrintJob(printJobData);
        });
    }

    public async Task PrintAsync(Base64ZPLPrint base64ZplPrint)
    {
        await printFileResolver.MakeTempFile(base64ZplPrint.Base64String, TextContentType.Base64, async (relativePath, absolutePath) =>
        {
            var printJobData = new PrinterJobDataInfo(absolutePath)
            {
                CopyCount = base64ZplPrint.CopyCount,
                PrinterName = base64ZplPrint.PrinterName,
            };
            await CommitPrintJob(printJobData);
        });
    }

    private async Task CopyFile(string filePath, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(configurations.CopyPrintPath))
        {
            await Task.CompletedTask;
            return;
        }
        var destinationPath = Path.Combine(configurations.CopyPrintPath, relativePath);
        var destinationDirectory = Path.GetDirectoryName(destinationPath);
        await Task.Run(() =>
        {
            if (destinationDirectory is not null && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            File.Copy(filePath, destinationPath);
        });
    }

    private static async Task CommitPrintJob(PrinterJobDataInfo printerJobDataInfo)
    {
        var command = new MacPrintCommand(printerJobDataInfo.FilePath)
            .Printer(printerJobDataInfo.PrinterName)
            .Copies(printerJobDataInfo.CopyCount)
            .MediaSource(printerJobDataInfo.PaperSource);
        Console.WriteLine(command.ToString());
        await command.ExecuteAsync();
    }
}