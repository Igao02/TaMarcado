using System.Diagnostics.CodeAnalysis;
using TaMarcado.Dominio.Enum;
using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Scheduling : Entity
{
    public Scheduling()
    {
        //ORM Purpose
    }

    public Guid ProfessionalId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid ClientId { get; set; }
    public DateTime InitDate { get; set; } = DateTime.Now;
    public required DateTime EndDate { get; set; }
    public required StatusEnum Status { get; set; }
    public decimal Price { get; set; }
    public string? Observation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual required Professional Professional { get; set; }
    public virtual required Service Service { get; set; }
    public virtual required Client Client { get; set; }
    public virtual Payment? Payment { get; set; }

    [SetsRequiredMembers]
    public Scheduling(Guid professionalId, Guid serviceId, Guid clientId, DateTime initDate, DateTime endDate, StatusEnum status, decimal price, string? observation, DateTime createdAt)
    {
        ProfessionalId = professionalId;
        ServiceId = serviceId;
        ClientId = clientId;
        InitDate = initDate;
        EndDate = endDate;
        Status = status;
        Price = price;
        Observation = observation;
        CreatedAt = createdAt;
    }
}
