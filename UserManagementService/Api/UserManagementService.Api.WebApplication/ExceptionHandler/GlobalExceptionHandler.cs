using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace UserManagementService.Api.WebApplication.ExceptionHandler;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, "Could not process request, Trace ID: {traceId}", traceId);

        var (statusCode, title) = MapException(exception);

        await Results.Problem(
            title: title, 
            statusCode: statusCode, 
            extensions: new Dictionary<string, object?>
        {
            { "traceId", traceId }
        }).ExecuteAsync(httpContext);

        return true; //Signifies that the exception has been handled and that the request pipeline can stop here
    }

    private static (int StatusCode, string title) MapException(Exception exception)
    {
        return exception switch
        {
            //ArgumentNullException => 
            _ => (StatusCodes.Status500InternalServerError, "An error has occurred, we are investigating")
        };
    }
}
