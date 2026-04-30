namespace TaMarcado.Aplicacao.UseCases.Schedulings.GetSchedulingsByClient;

public record GetSchedulingsByClientResponse(
    Guid Id,
    string ProfessionalName,
    string ServiceName,
    DateTime InitDate,
    DateTime EndDate,
    string Status,
    decimal Price,
    string? PaymentStatus
);
