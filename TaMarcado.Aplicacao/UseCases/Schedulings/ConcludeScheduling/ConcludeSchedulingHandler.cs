using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Schedulings.ConcludeScheduling;

public class ConcludeSchedulingHandler(ISchedulingRepository schedulingRepository)
{
    public async Task<Result<ConcludeSchedulingResponse>> Handle(ConcludeSchedulingCommand command)
    {
        try
        {
            var scheduling = await schedulingRepository.GetByIdAndProfessionalIdAsync(
                command.SchedulingId, command.ProfessionalId);

            if (scheduling is null)
                return Result.Failure<ConcludeSchedulingResponse>(
                    Error.NotFound("Scheduling.NotFound", "Agendamento não encontrado."));

            if (scheduling.Status != StatusEnum.Confirmed)
                return Result.Failure<ConcludeSchedulingResponse>(
                    Error.Conflict("Scheduling.InvalidStatus", "Apenas agendamentos confirmados podem ser concluídos."));

            scheduling.Status = StatusEnum.Concluded;
            await schedulingRepository.UpdateAsync(scheduling);

            return Result.Success(new ConcludeSchedulingResponse(scheduling.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<ConcludeSchedulingResponse>(
                Error.Problem("Scheduling.ConcludeError", ex.Message));
        }
    }
}
