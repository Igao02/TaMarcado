namespace TaMarcado.Aplicacao.UseCases.Services.GetServices;

public record GetServicesResponse(
    Guid Id,
    string Name,
    string Description,
    int DurationInMinutes,
    decimal Price,
    bool IsActive
);
