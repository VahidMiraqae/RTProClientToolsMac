using Microsoft.AspNetCore.Mvc;
using RTProClientToolsMac.ViewModels;

namespace RTProClientToolsMac.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrintController(
    PrintService printService
    ) : ControllerBase
{
    // post: api/Print/PrintText
    [HttpPost, Route("PrintText")]
    [RequestSizeLimit(209715200)]
    public async Task<IActionResult> PrintText([FromBody] TextPrint model)
    {
        await printService.PrintAsync(model);
        return Ok();
    }

    // post: api/Print/PrintFile
    [HttpPost, Route("PrintFile")]
    [RequestSizeLimit(209715200)]
    public async Task<IActionResult> PrintFile([FromBody] Base64Print model)
    {
        await printService.PrintAsync(model);
        return Ok();
    }

    // post: api/Print/PrintZPLText
    [HttpPost, Route("PrintZPLText")]
    [RequestSizeLimit(209715200)]
    public async Task<IActionResult> PrintZPLText([FromBody] TextZPLPrint model)
    {
        return Ok();
    }

    // post: api/Print/PrintZPLFile
    [HttpPost, Route("PrintZPLFile")]
    [RequestSizeLimit(209715200)]
    public async Task<IActionResult> PrintZPLFile([FromBody] Base64ZPLPrint model)
    {
        return Ok();
    }
}
