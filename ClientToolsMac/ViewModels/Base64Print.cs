namespace RTProClientToolsMac.ViewModels;

public class Base64Print
{
    public string base64String { get; set; }
    public string? printerName { get; set; }
    public string? printFileName { get; set; }
    public string? documentName { get; set; }
    public string? folderName { get; set; }
    public string? paperSource { get; set; }
    public short CopyCount { get; set; } = 1;
}

