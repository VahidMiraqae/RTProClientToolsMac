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

        Console.ReadLine();
    }

    static async Task PrintFile(string filename)
    {
        var filePath = Path.Combine("samples", filename);
        var bytes = File.ReadAllBytes(filePath);
        var base64 = Convert.ToBase64String(bytes);
        var base64Print = new Base64Print()
        {
            base64String = $",{base64}",
            CopyCount = 1,
            documentName = "",
            folderName = "",
            paperSource = "",
            printerName = "Some_Printer_Name",
            printFileName = Path.GetFileName(filePath),
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