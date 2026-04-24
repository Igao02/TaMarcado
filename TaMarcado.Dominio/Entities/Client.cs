using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Client : Entity
{
    public Client()
    {
        //ORM Purpose
    }

    public Guid ProfessionalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Observations { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual required Professional Professional { get; set; }

    public Client(Guid professionalId, string name, string phone, string? email, string? observations, DateTime createdAt)
    {
        ProfessionalId = professionalId;
        Name = name;
        Phone = phone;
        Email = email;
        Observations = observations;
        CreatedAt = createdAt;
    }
}
