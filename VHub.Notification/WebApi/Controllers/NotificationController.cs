using Application.Contracts.Commands;
using Application.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotificationController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Send(SendNotificationRequest request) 
    {
        await _mediator.Send(new SendNotificationCommand(request));
        return Ok();
    }
}
