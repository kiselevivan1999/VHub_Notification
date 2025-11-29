using Domain.Errors;
using Domain.Exceptions;
using System.Net;

namespace WebApi.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next) 
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) 
    {
        try
        {
            await _next(context);
        }
        catch (AbstractException ex) 
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";
            var apiError = new ApiError(ex.StatusCode, ex.GetApiError().Message, ex.GetApiError().Details);
            await context.Response.WriteAsJsonAsync(apiError);
        }
        catch (Exception ex) 
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
