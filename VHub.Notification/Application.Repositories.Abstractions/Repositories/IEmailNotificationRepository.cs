using Domain.Entities;

namespace Application.Repositories.Abstractions.Repositories;

public interface IEmailNotificationRepository : IGenericRepository<EmailNotification, Guid>
{
}
