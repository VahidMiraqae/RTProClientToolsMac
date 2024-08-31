using System.Diagnostics;

namespace RTProClientToolsMac.PackageMaker;

internal class Program
{
    private const string INSTALLER_DIR = "installer";
    private const string MACOS_X64_DIR = "macOS-x64";
    private const string APP_DIR = "application";
    private const string DARWIN_DIR = "darwin";
    private const string PRODUCT_PLACE_HOLDER = "__PRODUCT__";
    private const string VERSION_PLACE_HOLDER = "__VERSION__";
    private const string PACKAGE_BUILDER_BASH = "build-macos-x64.sh";
    private static HashSet<string> _nonTextFileExtensions = [".png"];

    private static void Main(string[] args)
    {
        var productName = nameof(RTProClientToolsMac);
        var version = "1.0.0";

        var projectDir = $@"/Users/imac/Desktop/ClientToolsMac/RTProClientToolsMac";
        if (!Directory.Exists(projectDir)){
            throw new DirectoryNotFoundException();
        }
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;

        // publish
        var applicationDir = Path.Combine(currentDir, INSTALLER_DIR, MACOS_X64_DIR, APP_DIR);
        // var commands = new List<string>()
        // {
        //     $"cd {projectDir}",
        //     $"dotnet publish -o {applicationDir}"
        // };
        // var ars = string.Join(" & ", commands);
        var process = new Process() {
            StartInfo = new ProcessStartInfo(){
                WorkingDirectory = projectDir,
                FileName = "dotnet",
                Arguments = $"publish -o {applicationDir}"
            }
        };
        process.Start();
        process.WaitForExit();
        // Process.Start("bash", "/C " + ars).WaitForExit();

        // replace product and version
        var s = Path.Combine(currentDir, INSTALLER_DIR, MACOS_X64_DIR, DARWIN_DIR);
        foreach (var file in Directory.GetFiles(s, "*", SearchOption.AllDirectories))
        {
            var extension = Path.GetExtension(file);
            if (_nonTextFileExtensions.Contains(extension))
            {
                continue;
            }
            var content = File.ReadAllText(file);
            var content1 = content.Replace(PRODUCT_PLACE_HOLDER, productName)
                .Replace(VERSION_PLACE_HOLDER, version);
            File.WriteAllText(file, content1);
        }

        // build package
        var path = Path.Combine(currentDir, INSTALLER_DIR, MACOS_X64_DIR, PACKAGE_BUILDER_BASH);
        Process.Start("bash", $"{path} {productName} {version}").WaitForExit();

        //Console.WriteLine("package was created in installer/macOS-x64/target");
        //Console.ReadLine();
    }
}