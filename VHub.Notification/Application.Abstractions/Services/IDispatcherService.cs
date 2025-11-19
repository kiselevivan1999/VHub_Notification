using Application.Contracts.Commands;
using Application.Contracts.Responses;

namespace Application.Abstractions.Services;

public interface IDispatcherService
{
    Task<SendNotificationResponse> DispatchAsync(SendNotificationCommand command,
    CancellationToken cancellationToken = default);
}
