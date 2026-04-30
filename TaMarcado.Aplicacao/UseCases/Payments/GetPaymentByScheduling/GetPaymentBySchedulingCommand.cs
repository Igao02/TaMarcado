namespace TaMarcado.Aplicacao.UseCases.Payments.GetPaymentByScheduling;

public record GetPaymentBySchedulingCommand(Guid SchedulingId, Guid ProfessionalId);
