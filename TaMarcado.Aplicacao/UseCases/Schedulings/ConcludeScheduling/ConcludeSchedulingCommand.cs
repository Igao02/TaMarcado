namespace TaMarcado.Aplicacao.UseCases.Schedulings.ConcludeScheduling;

public record ConcludeSchedulingCommand(Guid SchedulingId, Guid ProfessionalId);
