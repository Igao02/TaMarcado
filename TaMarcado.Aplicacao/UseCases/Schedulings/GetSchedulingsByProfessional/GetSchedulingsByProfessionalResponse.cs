namespace TaMarcado.Aplicacao.UseCases.Schedulings.GetSchedulingsByProfessional;

public record GetSchedulingsByProfessionalResponse(List<SchedulingItem> Schedulings);

public record SchedulingItem(
    Guid Id,
    string ClientName,
    string ServiceName,
    DateTime InitDate,
    DateTime EndDate,
    string Status,
    decimal Price,
    string? PaymentStatus,
    Guid? PaymentId);
