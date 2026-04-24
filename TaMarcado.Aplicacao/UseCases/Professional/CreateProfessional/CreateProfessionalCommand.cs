using TaMarcado.Dominio.Enum;

namespace TaMarcado.Aplicacao.UseCases.Professionals.CreateProfessional;

public record CreateProfessionalCommand(
    string ApplicationUserId,
    Guid CategoryId,
    string ExibitionName,
    string Slug,
    string WhatsApp,
    string? Bio,
    string KeyPix,
    KeyPixEnum KeyPixType
);
