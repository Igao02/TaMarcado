using TaMarcado.Dominio.Enum;

namespace TaMarcado.Aplicacao.UseCases.Professionals.UpdateProfessional;

public record UpdateProfessionalCommand(
    Guid Id,
    Guid CategoryId,
    string ExibitionName,
    string Slug,
    string WhatsApp,
    string? Bio,
    string? Address,
    string? PhotoUrl,
    string KeyPix,
    KeyPixEnum KeyPixType
);
