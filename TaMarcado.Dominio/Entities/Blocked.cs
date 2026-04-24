using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Blocked : Entity
{
    public Blocked()
    {
        //ORM Purpose
    }

    public Guid ProfessionalId  { get; set; }
    public DateTime InitDate { get; set; } = DateTime.Now;
    public required DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Reason { get; set; } = string.Empty;
    public virtual required Professional Professional { get; set; }

    public Blocked(Guid professionalId, DateTime initDate, DateTime endDate, DateTime createdAt, string? reason)
    {
        ProfessionalId = professionalId;
        InitDate = initDate;
        EndDate = endDate;
        CreatedAt = createdAt;
        Reason = reason;
    }
}
