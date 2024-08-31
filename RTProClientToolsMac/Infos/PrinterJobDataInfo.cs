namespace RTProClientToolsMac.Infos
{
    public class PrinterJobDataInfo
    {
        public string FilePath { get; set; }
        public string? PrinterName { get; set; }
        public short CopyCount { get; set; } = 1;
        public string? PaperSource { get; set; }

        public PrinterJobDataInfo(string filePath)
        {
            FilePath = filePath;
        }

    }
}