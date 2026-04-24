using TaMarcado.Dominio.Enum;
using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Subscription : Entity
{
    public Subscription()
    {
        //ORM Purpose
    }

    public Guid ProfessionalId { get; set; }
    public Guid PlanId { get; set; }
    public StatusSubscriptionEnum Status { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual required Professional Professional { get; set; }
    public virtual required Plan Plan { get; set; }

    public Subscription(Guid professionalId, Guid planId, StatusSubscriptionEnum status, DateTime startDate, DateTime? endDate, DateTime createdAt)
    {
        ProfessionalId = professionalId;
        PlanId = planId;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = createdAt;
    }
}
