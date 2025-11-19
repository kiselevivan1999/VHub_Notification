using Application.Abstractions.Services;
using Application.Contracts.Commands;
using Application.Contracts.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Implementation.Handlers;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, SendNotificationResponse>
{
    private readonly IDispatcherService _dispatcher;
    private readonly ILogger<SendNotificationCommandHandler> _logger;

    public SendNotificationCommandHandler(
        IDispatcherService dispatcher,
        ILogger<SendNotificationCommandHandler> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task<SendNotificationResponse> Handle(
        SendNotificationCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("выполняем команду для {Recipient}", request.Recipient);
        return await _dispatcher.DispatchAsync(request, cancellationToken);
    }
}
