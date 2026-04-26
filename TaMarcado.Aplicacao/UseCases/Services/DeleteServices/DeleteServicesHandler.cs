using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Services.DeleteServices;

public class DeleteServicesHandler(IServiceRepository repository)
{
    public async Task<Result<DeleteServicesResponse>> Handle(DeleteServicesCommand command)
    {
        try
        {
            var service = await repository.GetByIdAndProfessionalIdAsync(command.Id, command.ProfessionalId);
            if (service is null)
                return Result.Failure<DeleteServicesResponse>(
                    Error.NotFound("Service.NotFound", "Serviço não encontrado."));

            await repository.DeactivateAsync(service);
            return Result.Success(new DeleteServicesResponse(command.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<DeleteServicesResponse>(
                Error.Problem("Service.DeleteError", ex.Message));
        }
    }
}
