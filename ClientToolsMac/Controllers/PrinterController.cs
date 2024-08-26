using Microsoft.AspNetCore.Mvc;

namespace RTProClientToolsMac.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrinterController : ControllerBase
{ 
    [HttpGet("GetPrinterList")]
    public IActionResult GetPrinterList()
    {
        return Content(string.Join(',', MacPrint.GetPrinters()));
    }
}
