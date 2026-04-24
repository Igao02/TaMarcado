using TaMarcado.Dominio.Enum;
using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class AvaliableTime : Entity
{
    public AvaliableTime()
    {
        //ORM Purpose
    }

    public Guid ProfessionalId { get; set; }
    public WeekEnum WeekDay { get; set; }
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
    public TimeSpan EndTime { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual required Professional Professional { get; set; }

    public AvaliableTime(Guid professionalId, WeekEnum weekDay, TimeSpan startTime, TimeSpan endTime, bool active, DateTime createdAt)
    {
        ProfessionalId = professionalId;
        WeekDay = weekDay;
        StartTime = startTime;
        EndTime = endTime;
        Active = active;
        CreatedAt = createdAt;
    }
}
