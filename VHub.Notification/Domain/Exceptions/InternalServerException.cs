using System.Net;

namespace Domain.Exceptions;

public class InternalServerException : AbstractException
{
    public InternalServerException(string titel, string? details = null) 
        : base(HttpStatusCode.InternalServerError, titel, details)
    {}
}
