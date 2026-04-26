namespace TaMarcado.Aplicacao.UseCases.Services.DeleteServices;

public record DeleteServicesCommand
(
    Guid Id,
    Guid ProfessionalId
);