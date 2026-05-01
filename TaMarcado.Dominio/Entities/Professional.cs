using TaMarcado.Dominio.Enum;
using TaMarcado.DominioPrincipal.Entities;

namespace TaMarcado.Dominio.Entities;

public class Professional : Entity
{
    public Professional()
    {
        //ORM Purpose
    }

    public required string ApplicationUserId { get; set; }
    public Guid CategoryId { get; set; }
    public string ExibitionName { get; set; } = string.Empty;
    public required string Slug { get; set; }
    public string WhatsApp { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Address { get; set; }
    public required string PhotoUrl { get; set; }
    public required string KeyPix { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public required KeyPixEnum KeyPixType { get; set; }
    public virtual Category? Category { get; set; }

    public Professional(string applicationUserId, Guid categoryId, string exibitionName, string slug, string whatsApp, string? bio, string photoUrl, string keyPix, bool active, DateTime createdAt, KeyPixEnum keyPixType)
    {
        ApplicationUserId = applicationUserId;
        CategoryId = categoryId;
        ExibitionName = exibitionName;
        Slug = slug;
        WhatsApp = whatsApp;
        Bio = bio;
        PhotoUrl = photoUrl;
        KeyPix = keyPix;
        Active = active;
        CreatedAt = createdAt;
        KeyPixType = keyPixType;
    }
}
