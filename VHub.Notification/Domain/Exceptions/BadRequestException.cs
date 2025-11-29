using System.Net;

namespace Domain.Exceptions;

public class BadRequestException : AbstractException
{
    public BadRequestException(string titel, string? details = null) 
        : base(HttpStatusCode.BadRequest, titel, details)
    {}
}
