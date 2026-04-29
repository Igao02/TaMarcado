using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Schedulings.CancelScheduling;

public class CancelSchedulingHandler(ISchedulingRepository schedulingRepository)
{
    public async Task<Result<CancelSchedulingResponse>> Handle(CancelSchedulingCommand command)
    {
        try
        {
            var scheduling = await schedulingRepository.GetByIdAndProfessionalIdAsync(
                command.SchedulingId, command.ProfessionalId);

            if (scheduling is null)
                return Result.Failure<CancelSchedulingResponse>(
                    Error.NotFound("Scheduling.NotFound", "Agendamento não encontrado."));

            if (scheduling.Status is StatusEnum.Canceled or StatusEnum.Concluded)
                return Result.Failure<CancelSchedulingResponse>(
                    Error.Conflict("Scheduling.InvalidStatus", "Este agendamento não pode ser cancelado."));

            scheduling.Status = StatusEnum.Canceled;
            await schedulingRepository.UpdateAsync(scheduling);

            return Result.Success(new CancelSchedulingResponse(scheduling.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<CancelSchedulingResponse>(
                Error.Problem("Scheduling.CancelError", ex.Message));
        }
    }
}
