using Application.Repositories.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.EntityFramework;

namespace Infrastructure.Repositories.Repositories;

internal class EmailNotificationRepository : GenericRepository<EmailNotification, Guid>, IEmailNotificationRepository
{
    public EmailNotificationRepository(NotificationDbContext context) : base(context)
    {
    }
}
