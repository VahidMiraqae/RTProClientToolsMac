namespace ClientToolsMac.Models;

public class TextZPLPrint
{
    public string TextZPL { get; set; }
    public string PrinterName { get; set; }
    public short CopyCount { get; set; } = 1;
}
