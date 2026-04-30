namespace TaMarcado.Aplicacao.UseCases.Payments.GetClientPayment;

public record GetClientPaymentCommand(Guid SchedulingId, string ApplicationUserId);
