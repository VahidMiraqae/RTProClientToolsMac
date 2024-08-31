using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var projectDir1 = @"..\..\..\..\ClientToolsMac";
        var installerDir = @"..\..\..\..\installer";
        var projectDir = @"C:\Users\Vahid\Desktop\ClientToolsMac\ClientToolsMac\ClientToolsMac.csproj";
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var publishFolder = Path.Combine(currentDir, "publish");

        var targetInstaller = Path.Combine(currentDir, "installer");
        CopyFilesRecursively(installerDir, targetInstaller);

        var applicationDir = Path.Combine(currentDir, "installer", "macOS-x64", "application");
        var commands = new List<string>()
        {
            $"cd {projectDir1}",
            $"dotnet publish -o {applicationDir}"
        };
        var ars = string.Join(" & ", commands);

        Process.Start("cmd.exe", "/C " + ars);



        Console.ReadLine();
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}