using ClientToolsMac.Models;
using ClientToolsMac.Services;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static IConfiguration Configuration;
    private static Configurations Configs;
    private static PrintFileResolver PrintFileResolver;
    private static PrintService PrintService;

    private async static Task Main(string[] args)
    {
        MakeConfiguration(); 
        Configs = new Configurations(Configuration);
        PrintFileResolver = new PrintFileResolver(Configs);
        PrintService = new PrintService(PrintFileResolver, Configs);

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
        var printer = "";
        try
        {
            printer = PrintService.GetPrinters()[1];
        }
        catch { }
        var base64Print = new TextPrint()
        {
            Text = v,
            CopyCount = 1,
            DocumentName = "",
            FolderName = "",
            PaperSource = "",
            PrinterName = printer,
            PrintFileName = Guid.NewGuid().ToString("N"),
        };
        await PrintService.PrintAsync(base64Print);
    }

    static async Task PrintFile(string filename)
    {
        var printer = "";
        try
        {
            printer = PrintService.GetPrinters()[1];
        }
        catch { }
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
            PrinterName = printer,
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