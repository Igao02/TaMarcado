using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Clients.UpdateClientObservations;

public class UpdateClientObservationsHandler(IClientRepository repository)
{
    public async Task<Result<UpdateClientObservationsResponse>> Handle(UpdateClientObservationsCommand command)
    {
        try
        {
            var client = await repository.GetByIdAndProfessionalIdAsync(command.Id, command.ProfessionalId);
            if (client is null)
                return Result.Failure<UpdateClientObservationsResponse>(
                    Error.NotFound("Client.NotFound", "Cliente não encontrado."));

            client.Observations = command.Observations;
            await repository.UpdateAsync(client);

            return Result.Success(new UpdateClientObservationsResponse(client.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<UpdateClientObservationsResponse>(Error.Problem("Client.UpdateError", ex.Message));
        }
    }
}
