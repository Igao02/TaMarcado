namespace TaMarcado.Aplicacao.UseCases.Payments.GetPaymentByScheduling;

public record GetPaymentBySchedulingResponse(
    Guid PaymentId,
    string Status,
    string PixPayload,
    DateTime? DatePayment
);
