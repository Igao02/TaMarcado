using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.AvaliableTimes.DeleteAvaliableTime;

public class DeleteAvaliableTimeHandler(IAvaliableTimeRepository repository)
{
    public async Task<Result<DeleteAvaliableTimeResponse>> Handle(DeleteAvaliableTimeCommand command)
    {
        try
        {
            var avaliableTime = await repository.GetByIdAndProfessionalIdAsync(command.Id, command.ProfessionalId);

            if (avaliableTime is null)
                return Result.Failure<DeleteAvaliableTimeResponse>(
                    Error.NotFound("AvaliableTime.NotFound", "Horário não encontrado."));

            await repository.DeactivateAsync(avaliableTime);

            return Result.Success(new DeleteAvaliableTimeResponse(command.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<DeleteAvaliableTimeResponse>(
                Error.Problem("AvaliableTime.DeleteError", ex.Message));
        }
    }
}
