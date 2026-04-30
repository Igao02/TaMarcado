namespace TaMarcado.Aplicacao.UseCases.Payments.ConfirmPayment;

public record ConfirmPaymentCommand(Guid SchedulingId, Guid ProfessionalId);
