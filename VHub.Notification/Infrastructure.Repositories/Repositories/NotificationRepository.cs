using Application.Repositories.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.EntityFramework.Contexts;

namespace Infrastructure.Repositories.Repositories;

internal class NotificationRepository : GenericRepository<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(NotificationDbContext context) : base(context)
    {
    }
}
