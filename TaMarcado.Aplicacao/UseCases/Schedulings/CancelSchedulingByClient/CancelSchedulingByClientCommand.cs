namespace TaMarcado.Aplicacao.UseCases.Schedulings.CancelSchedulingByClient;

public record CancelSchedulingByClientCommand(Guid SchedulingId, string ApplicationUserId);
