using Domain.Errors;
using System.Net;

namespace Domain.Exceptions;

public abstract class AbstractException : Exception
{
    private readonly ApiError _error;
    public int StatusCode => _error.StatusCode;

    protected AbstractException(HttpStatusCode httpStatusCode, string titel, 
        string? details = default) 
    {
        _error = new ApiError((int)httpStatusCode, titel, details);
    }

    public ApiError GetApiError()
        => _error;
}
