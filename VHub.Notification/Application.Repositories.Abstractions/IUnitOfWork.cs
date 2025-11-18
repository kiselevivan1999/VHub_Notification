using Application.Repositories.Abstractions.Repositories;

namespace Application.Repositories.Abstractions;

public interface IUnitOfWork
{
    IEmailNotificationRepository EmailNotificationRepository { get; }
    Task SaveChangesAsync();
}
