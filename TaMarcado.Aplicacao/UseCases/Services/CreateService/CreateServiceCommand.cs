namespace TaMarcado.Aplicacao.UseCases.Services.CreateService;

public record CreateServiceCommand(
    Guid ProfessionalId,
    string Name,
    string Description,
    int DurationInMinutes,
    decimal Price
);
