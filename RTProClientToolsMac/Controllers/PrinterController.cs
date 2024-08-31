using RTProClientToolsMac.Services;
using Microsoft.AspNetCore.Mvc;

namespace RTProClientToolsMac.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrinterController(
    PrintService printService
    ) : ControllerBase
{
    // Get: api/Printer/GetPrinterList
    [HttpGet("GetPrinterList")]
    public IActionResult GetPrinterList()
    {
        return Content(string.Join(',', printService.GetPrinters()));
    }
}
