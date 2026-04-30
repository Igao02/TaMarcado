using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class NotificationSchedulingRepository(ApplicationDbContext context) : INotificationSchedulingRepository
{
    public async Task AddRangeAsync(IEnumerable<NotificationScheduling> notifications)
    {
        context.NotificationScheduling.AddRange(notifications);
        await context.SaveChangesAsync();
    }

    public Task<List<NotificationScheduling>> GetPendingDueAsync(DateTime until) =>
        context.NotificationScheduling
            .Include(n => n.Scheduling).ThenInclude(s => s.Client)
            .Include(n => n.Scheduling).ThenInclude(s => s.Service)
            .Include(n => n.Scheduling).ThenInclude(s => s.Professional)
            .Where(n => n.StatusNotification == StatusNotificationScheduling.Pending
                     && n.DateScheduling <= until
                     && n.Scheduling.Client.Email != null)
            .ToListAsync();

    public async Task UpdateAsync(NotificationScheduling notification)
    {
        context.NotificationScheduling.Update(notification);
        await context.SaveChangesAsync();
    }
}
