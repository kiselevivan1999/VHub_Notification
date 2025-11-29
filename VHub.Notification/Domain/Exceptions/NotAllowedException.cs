using System.Net;

namespace Domain.Exceptions;

public class NotAllowedException : AbstractException
{
    public NotAllowedException(string titel, string? details = null) 
        : base(HttpStatusCode.MethodNotAllowed, titel, details)
    {
    }
}
