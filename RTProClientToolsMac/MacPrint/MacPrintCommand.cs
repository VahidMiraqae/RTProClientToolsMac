using RTProClientToolsMac.Exceptions;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RTProClientToolsMac.MacPrint;

public class MacPrintCommand : IPrintCommand
{
    private string? _printerName;
    private string _filename;
    private Orientation _orientation = MacPrint.Orientation.Portrait;
    private bool _fitToPage;
    private MediaSize? _mediaSize;
    private int? _numberOfCopies;
    private bool _collate;
    private bool _customMediaSize;
    private double _width;
    private double _height;
    private LengthUnit _unit;
    private bool _reverseOutputOrder;
    private bool _mirror;
    private PrintSides _printSides;
    private MediaType? _mediaType;
    private MediaSource? _mediaSource;
    private string? _mediaSizeName;
    private string[]? _ranges;
    private string? _mediaSourceName;

    public MacPrintCommand(string filename)
    {
        _filename = filename;
    }

    public MacPrintCommand Printer(string? printerName)
    {
        _printerName = printerName;
        return this;
    }

    public MacPrintCommand Orientation(Orientation orientation)
    {
        _orientation = orientation;
        return this;
    }

    public MacPrintCommand FitToPage()
    {
        _fitToPage = true;
        return this;
    }

    public MacPrintCommand PageSize(MediaSize mediaSize)
    {
        _mediaSize = mediaSize;
        return this;
    }

    public MacPrintCommand PageSize(string mediaSize)
    {
        _mediaSizeName = mediaSize;
        return this;
    }

    public MacPrintCommand Copies(int numberOfCopies)
    {
        _numberOfCopies = numberOfCopies;
        return this;
    }

    public MacPrintCommand Collate()
    {
        _collate = true;
        return this;
    }

    public MacPrintCommand CustomPageSize(double width, double height, LengthUnit unit)
    {
        _customMediaSize = true;
        _width = width;
        _height = height;
        _unit = unit;
        return this;
    }

    public MacPrintCommand ReverseOutputOrder()
    {
        _reverseOutputOrder = true;
        return this;
    }

    public MacPrintCommand Mirror()
    {
        _mirror = true;
        return this;
    }

    public MacPrintCommand Sides(PrintSides sides)
    {
        _printSides = sides;
        return this;
    }

    public MacPrintCommand MediaType(MediaType type)
    {
        _mediaType = type;
        return this;
    }

    public MacPrintCommand MediaSource(MediaSource sourse)
    {
        _mediaSource = sourse;
        return this;
    }

    public MacPrintCommand MediaSource(string? sourse)
    {
        _mediaSourceName = sourse;
        return this;
    }

    /// <summary>
    /// pages to print
    /// </summary>
    /// <param name="ranges">example: "1-4" or "7"</param>
    /// <returns></returns>
    public MacPrintCommand Range(params string[] ranges)
    {
        _ranges = ranges;
        return this;
    }

    public Task SetDefaultPrinter(string filename)
    {
        throw new NotImplementedException();
    }

    public async Task ExecuteAsync()
    {
        var arguements = MakeArgument();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo("lpr", arguements)
            {
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };

        try
        {
            process.Start();
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            throw new TerminalCommandFailedException(ex);
        }
    }

    private string MakeArgument()
    {
        ArgumentNullException.ThrowIfNull(_filename);
        var argsList = new List<string>();

        if (_printerName is not null)
        {
            argsList.Add($"-P {_printerName}");
        }

        if (_orientation is not MacPrint.Orientation.Portrait)
        {
            var orientationArg = _orientation switch
            {
                MacPrint.Orientation.Landscape => "-o landscape", // -o orientation-requested=4
                MacPrint.Orientation.ReversePortrait => "-o orientation-requested=6",
                MacPrint.Orientation.ReverseLandscape => "-o orientation-requested=5",
                _ => throw new NotSupportedException(nameof(_orientation))
            };
            argsList.Add(orientationArg);
        }

        if (_fitToPage)
        {
            argsList.Add("-o fit-to-page");
        }

        MakeMediaOptions(argsList);

        argsList.Add($"-#{_numberOfCopies ?? 1}");

        if (_collate)
        {
            argsList.Add("-o collate=true");
        }

        if (_reverseOutputOrder)
        {
            argsList.Add("-o outputorder=reverse"); // -o outputorder=normal
        }

        if (_mirror)
        {
            argsList.Add("-o mirror");
        }

        if (_printSides is not PrintSides.OneSide)
        {
            var sides = _printSides switch
            {
                PrintSides.TwoSidePortrait => "two-sided-long-edge",
                PrintSides.TwoSideLandscape => "two-sided-short-edge",
                _ => "one-sided"
            };
            argsList.Add($"-o sides={sides}");
        }

        if (_ranges is not null && _ranges.Length > 0)
        {
            argsList.Add($"-o page-ranges={string.Join(',', _ranges)}");
        }

        var options = string.Join(' ', argsList);
        return $"{options} \"{_filename}\"";
    }

    private void MakeMediaOptions(List<string> argsList)
    {
        if (_mediaSize is not null || _mediaSizeName is not null || _customMediaSize || _mediaType is not null || _mediaSource is not null)
        {
            var values = new List<string>();
            if (_mediaSize is not null)
            {
                var mediaSize = _mediaSize.Value switch
                {
                    MediaSize.Letter => "Letter",
                    MediaSize.Legal => "Legal",
                    MediaSize.A4 => "A4",
                    MediaSize.COM10 => "COM10",
                    MediaSize.DL => "DL",
                    _ => throw new NotSupportedException(nameof(_mediaSize)),
                };
                values.Add(mediaSize);
            }
            else if (!string.IsNullOrWhiteSpace(_mediaSizeName))
            {
                values.Add(_mediaSizeName);
            }
            else if (_customMediaSize)
            {
                var unit = _unit switch
                {
                    LengthUnit.Point => "",
                    LengthUnit.Inch => "in",
                    LengthUnit.Centimeter => "cm",
                    LengthUnit.Millimeter => "mm",
                    _ => throw new NotSupportedException(nameof(_unit))
                };
                values.Add($"Custom.{_width:N3}{_height:N3}{unit}");
            }

            if (_mediaType is not null)
            {
                values.Add(nameof(_mediaType.Value));
            }

            if (_mediaSource is not null)
            {
                values.Add(nameof(_mediaSource.Value));
            }
            else if (!string.IsNullOrWhiteSpace(_mediaSourceName))
            {
                values.Add(_mediaSourceName);
            }

            argsList.Add($"-o media={string.Join(',', values)}");
        }
    }

    public static string[] GetPrinters()
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo("lpstat", "-p")
            {
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };

        try
        {
            p.Start();
            var list = new List<string>();
            while (!p.StandardOutput.EndOfStream)
            {
                var line = p.StandardOutput.ReadLine();
                if (line is null)
                {
                    continue;
                }
                var match = Regex.Match(line, @"printer\s(\w*)");
                list.Add(match.Groups[1].ToString());
            }
            return [.. list];
        }
        catch (Exception ex)
        {
            throw new TerminalCommandFailedException(ex);
        }
    }

    public override string ToString()
    {
        return $"lpr {MakeArgument()}";
    }
}

// lpr -l filename