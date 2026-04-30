using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface INotificationSchedulingRepository
{
    Task AddRangeAsync(IEnumerable<NotificationScheduling> notifications);
    Task<List<NotificationScheduling>> GetPendingDueAsync(DateTime until);
    Task UpdateAsync(NotificationScheduling notification);
}
