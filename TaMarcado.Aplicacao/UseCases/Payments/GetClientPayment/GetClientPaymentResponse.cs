namespace TaMarcado.Aplicacao.UseCases.Payments.GetClientPayment;

public record GetClientPaymentResponse(
    Guid PaymentId,
    string Status,
    string PixPayload,
    DateTime? DatePayment);
