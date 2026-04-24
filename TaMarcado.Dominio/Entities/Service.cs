using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Service : Entity
{
    public Service()
    {
        //ORM Purpose
    }

    public Guid ProfessionalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual Professional? Professional { get; set; }

    public Service(Guid professionalId, string name, string description, int durationInMinutes, decimal price, bool isActive, DateTime createdAt)
    {
        ProfessionalId = professionalId;
        Name = name;
        Description = description;
        DurationInMinutes = durationInMinutes;
        Price = price;
        IsActive = isActive;
        CreatedAt = createdAt;
    }
}
