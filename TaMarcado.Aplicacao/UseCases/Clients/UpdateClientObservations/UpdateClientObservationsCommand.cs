namespace TaMarcado.Aplicacao.UseCases.Clients.UpdateClientObservations;

public record UpdateClientObservationsCommand(Guid Id, Guid ProfessionalId, string? Observations);
