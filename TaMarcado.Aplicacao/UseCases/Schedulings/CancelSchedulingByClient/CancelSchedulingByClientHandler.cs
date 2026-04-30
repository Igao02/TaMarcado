using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Schedulings.CancelSchedulingByClient;

public class CancelSchedulingByClientHandler(ISchedulingRepository repository)
{
    public async Task<Result<CancelSchedulingByClientResponse>> Handle(CancelSchedulingByClientCommand command)
    {
        try
        {
            var scheduling = await repository.GetByIdAndClientUserIdAsync(command.SchedulingId, command.ApplicationUserId);
            if (scheduling is null)
                return Result.Failure<CancelSchedulingByClientResponse>(
                    Error.NotFound("Scheduling.NotFound", "Agendamento não encontrado."));

            if (scheduling.Status is StatusEnum.Canceled or StatusEnum.Concluded)
                return Result.Failure<CancelSchedulingByClientResponse>(
                    Error.Conflict("Scheduling.InvalidStatus", "Este agendamento não pode ser cancelado."));

            scheduling.Status = StatusEnum.Canceled;
            await repository.UpdateAsync(scheduling);

            return Result.Success(new CancelSchedulingByClientResponse(scheduling.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<CancelSchedulingByClientResponse>(
                Error.Problem("Scheduling.CancelClientError", ex.Message));
        }
    }
}
