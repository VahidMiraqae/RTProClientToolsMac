namespace RTProClientToolsMac.ViewModels;

public class TextPrint
{
    public string Text { get; set; }
    public string? PrinterName { get; set; }
    public string? PrintFileName { get; set; }
    public string? DocumentName { get; set; }
    public string? FolderName { get; set; }
    public string? PaperSource { get; set; }
    public short CopyCount { get; set; } = 1;
}
