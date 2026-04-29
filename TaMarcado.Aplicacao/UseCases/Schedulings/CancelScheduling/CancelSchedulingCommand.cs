namespace TaMarcado.Aplicacao.UseCases.Schedulings.CancelScheduling;

public record CancelSchedulingCommand(Guid SchedulingId, Guid ProfessionalId);
