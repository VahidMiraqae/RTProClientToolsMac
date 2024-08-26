using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RTProClientToolsMac;

public class MacPrint
{
    public static string[] GetPrinters()
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo("lpstat", "-P")
            {
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };
        p.Start();
        var list = new List<string>();
        while (!p.StandardOutput.EndOfStream)
        {
            string line = p.StandardOutput.ReadLine();
            var match = Regex.Match(line, @"printer\s(\w*)");
            list.Add(match.Groups[1].ToString());
        }
        return [.. list];
    }

    internal static void Print(string filePath)
    {
        throw new NotImplementedException();
    }
}