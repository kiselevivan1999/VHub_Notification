using Application.Repositories.Abstractions.Repositories;

namespace Application.Repositories.Abstractions;

public interface IUnitOfWork
{
    INotificationRepository NotificationRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
