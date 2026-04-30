using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Clients.GetClients;

public class GetClientsHandler(IClientRepository repository)
{
    public async Task<Result<List<GetClientsResponse>>> Handle(GetClientsCommand command)
    {
        try
        {
            var clients = await repository.GetByProfessionalIdAsync(command.ProfessionalId);

            var response = clients
                .Select(c => new GetClientsResponse(c.Id, c.Name, c.Phone, c.Email, c.Observations, c.CreatedAt))
                .ToList();

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<GetClientsResponse>>(Error.Problem("Client.GetError", ex.Message));
        }
    }
}
