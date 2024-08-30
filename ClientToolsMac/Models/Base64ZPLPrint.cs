namespace ClientToolsMac.Models;

public class Base64ZPLPrint
{
    public string Base64String { get; set; }
    public string PrinterName { get; set; }
    public short CopyCount { get; set; } = 1;
}