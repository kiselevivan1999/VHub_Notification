using Application.Repositories.Abstractions;
using Application.Repositories.Abstractions.Repositories;
using Infrastructure.EntityFramework;
using Infrastructure.Repositories.Repositories;

namespace Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private INotificationRepository _notificationRepository;
    private NotificationDbContext _context;

    public INotificationRepository NotificationRepository => _notificationRepository;

    public UnitOfWork(NotificationDbContext context)
    {
        _context = context;
        _notificationRepository = new NotificationRepository(context);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
