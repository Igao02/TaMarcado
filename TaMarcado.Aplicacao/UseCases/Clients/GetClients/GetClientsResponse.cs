namespace TaMarcado.Aplicacao.UseCases.Clients.GetClients;

public record GetClientsResponse(
    Guid Id,
    string Name,
    string Phone,
    string? Email,
    string? Observations,
    DateTime CreatedAt
);
