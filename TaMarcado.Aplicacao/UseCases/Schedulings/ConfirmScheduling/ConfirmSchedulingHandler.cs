using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Schedulings.ConfirmScheduling;

public class ConfirmSchedulingHandler(ISchedulingRepository schedulingRepository)
{
    public async Task<Result<ConfirmSchedulingResponse>> Handle(ConfirmSchedulingCommand command)
    {
        try
        {
            var scheduling = await schedulingRepository.GetByIdAndProfessionalIdAsync(
                command.SchedulingId, command.ProfessionalId);

            if (scheduling is null)
                return Result.Failure<ConfirmSchedulingResponse>(
                    Error.NotFound("Scheduling.NotFound", "Agendamento não encontrado."));

            if (scheduling.Status != StatusEnum.Pendent)
                return Result.Failure<ConfirmSchedulingResponse>(
                    Error.Conflict("Scheduling.InvalidStatus", "Apenas agendamentos pendentes podem ser confirmados."));

            scheduling.Status = StatusEnum.Confirmed;
            await schedulingRepository.UpdateAsync(scheduling);

            return Result.Success(new ConfirmSchedulingResponse(scheduling.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<ConfirmSchedulingResponse>(
                Error.Problem("Scheduling.ConfirmError", ex.Message));
        }
    }
}
