namespace RTProClientToolsMac.Controllers;public class Configurations{    public Configurations(IConfiguration configuration)    {        CopyPrintPath = configuration.GetSection("CopyPrintPath").Value;    }    public string CopyPrintPath { get; set; }    public string TempDirectory    {        get        {            var tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClientToolsTemp");            Directory.CreateDirectory(tempPath);            return tempPath;        }    }}