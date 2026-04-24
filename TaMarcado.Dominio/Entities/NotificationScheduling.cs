using TaMarcado.Dominio.Enum;
using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class NotificationScheduling : Entity
{
    public NotificationScheduling()
    {
        //ORM Purpose
    }

    public Guid SchedulingId { get; set; }
    public required SchedulingTypeEnum SchedulingType { get; set; }
    public required StatusNotificationScheduling StatusNotification { get; set; }
    public DateTime DateScheduling { get; set; }
    public DateTime? DateSend { get; set; }
    public int QuantitySend { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual required Scheduling Scheduling { get; set; }

    public NotificationScheduling(Guid schedulingId, SchedulingTypeEnum schedulingType, StatusNotificationScheduling statusNotification, DateTime dateScheduling, DateTime? dateSend, int quantitySend, DateTime createdAt)
    {
        SchedulingId = schedulingId;
        SchedulingType = schedulingType;
        StatusNotification = statusNotification;
        DateScheduling = dateScheduling;
        DateSend = dateSend;
        QuantitySend = quantitySend;
        CreatedAt = createdAt;
    }
}
