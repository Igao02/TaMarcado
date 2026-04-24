using TaMarcado.Dominio.Enum;

namespace TaMarcado.Aplicacao.UseCases.Professionals.GetProfessional;

public record GetProfessionalResponse(
    Guid Id,
    string ExibitionName,
    string Slug,
    string WhatsApp,
    string? Bio,
    Guid CategoryId,
    string? CategoryName,
    string KeyPix,
    KeyPixEnum KeyPixType,
    bool Active
);
