namespace TaMarcado.Aplicacao.UseCases.Schedulings.ConfirmScheduling;

public record ConfirmSchedulingCommand(Guid SchedulingId, Guid ProfessionalId);
