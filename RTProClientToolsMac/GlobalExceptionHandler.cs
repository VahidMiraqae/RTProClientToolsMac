using Microsoft.AspNetCore.Diagnostics;
using Serilog;

internal class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Log.Logger.Error(exception.ToString());
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsync("");
        return true;
    }
}