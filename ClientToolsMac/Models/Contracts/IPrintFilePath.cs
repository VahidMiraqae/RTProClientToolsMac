namespace ClientToolsMac.Models.Contracts
{
    public interface IPrintFilePath
    {
        string? PrintFileName { get; set; }
        string? FolderName { get; set; }
    }
}