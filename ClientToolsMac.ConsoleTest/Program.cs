using Microsoft.Extensions.Configuration;
using RTProClientToolsMac.Controllers;
using RTProClientToolsMac.ViewModels;




internal class Program
{
    private static IConfiguration Configuration;
    private static Configurations Configs;
    private static PrintService PrintService;

    private async static Task Main(string[] args)
    {
        MakeConfiguration();
        Configs = new Configurations(Configuration);
        PrintService = new PrintService(Configs);

        await PrintFile("sample.pdf");
        //await PrintFile("sample.docx");
        //await PrintFile("sample.png");
        //await PrintFile("sample.rtf");
        //await PrintFile("sample.xlsx");
        await PrintText("this is a sample text to print.");

        Console.ReadLine();
    }

    private static async Task PrintText(string v)
    {
        var printers = PrintService.GetPrinters();
        var base64Print = new TextPrint()
        {
            Text = v,
            CopyCount = 1,
            DocumentName = "",
            FolderName = "",
            PaperSource = "",
            PrinterName = printers[0],
            PrintFileName = Guid.NewGuid().ToString("N"),
        };
        await PrintService.PrintAsync(base64Print);
    }

    static async Task PrintFile(string filename)
    {
        var printers = PrintService.GetPrinters();
        var filePath = Path.Combine("samples", filename);
        var bytes = File.ReadAllBytes(filePath);
        var base64 = Convert.ToBase64String(bytes);
        var base64Print = new Base64Print()
        {
            Base64String = $",{base64}",
            CopyCount = 1,
            DocumentName = "",
            FolderName = "",
            PaperSource = "",
            PrinterName = printers[0],
            PrintFileName = Path.GetFileName(filePath),
        };
        await PrintService.PrintAsync(base64Print);
    }

    private static void MakeConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"CopyPrintPath", ""},
            //{"SectionName:SomeKey", "SectionValue"},
        };

        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
}