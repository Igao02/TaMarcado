using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.AvaliableTimes.DeleteAvaliableTime;

public class DeleteAvaliableTimeHandler(IAvaliableTimeRepository repository)
{
    public async Task<Result> Handle(Guid id, Guid professionalId)
    {
        try
        {
            var avaliableTime = await repository.GetByIdAndProfessionalIdAsync(id, professionalId);

            if (avaliableTime is null)
                return Result.Failure(Error.NotFound("AvaliableTime.NotFound", "Horário não encontrado."));

            await repository.DeactivateAsync(avaliableTime);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("AvaliableTime.DeleteError", ex.Message));
        }
    }
}
