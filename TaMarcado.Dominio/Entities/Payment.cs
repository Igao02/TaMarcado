using TaMarcado.Dominio.Enum;
using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Payment : Entity
{
    public Payment()
    {
        //ORM Purpose
    }

    public Guid SchedulingId { get; set; }
    public StatusPaymentEnum StatusPayment { get; set; }
    public string? ProofUrl { get; set; }
    public DateTime? DatePayment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual required Scheduling Scheduling { get; set; }

    public Payment(Guid schedulingId, StatusPaymentEnum statusPayment, string? proofUrl, DateTime? datePayment, DateTime createdAt)
    {
        SchedulingId = schedulingId;
        StatusPayment = statusPayment;
        ProofUrl = proofUrl;
        DatePayment = datePayment;
        CreatedAt = createdAt;
    }
}
