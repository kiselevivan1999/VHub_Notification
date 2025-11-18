using Application.Repositories.Abstractions;
using Application.Repositories.Abstractions.Repositories;
using Infrastructure.EntityFramework;
using Infrastructure.Repositories.Repositories;

namespace Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private IEmailNotificationRepository _emailNotificationRepository;
    private NotificationDbContext _context;

    public IEmailNotificationRepository EmailNotificationRepository => _emailNotificationRepository;

    public UnitOfWork(NotificationDbContext context)
    {
        _context = context;
        _emailNotificationRepository = new EmailNotificationRepository(context);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
