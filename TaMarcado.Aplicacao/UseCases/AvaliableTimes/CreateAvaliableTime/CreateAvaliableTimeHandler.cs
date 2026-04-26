using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.AvaliableTimes.CreateAvaliableTime;

public class CreateAvaliableTimeHandler(IAvaliableTimeRepository repository)
{
    public async Task<Result<CreateAvaliableTimeResponse>> Handle(CreateAvaliableTimeCommand command)
    {
        try
        {
            if (command.EndTime <= command.StartTime)
                return Result.Failure<CreateAvaliableTimeResponse>(
                    Error.Conflict("AvaliableTime.InvalidRange", "O horário de fim deve ser após o horário de início."));

            var existing = await repository.GetByProfessionalIdAsync(command.ProfessionalId);

            if (existing.Any(at => at.WeekDay == command.WeekDay))
                return Result.Failure<CreateAvaliableTimeResponse>(
                    Error.Conflict("AvaliableTime.Existing", $"Já existe um horário cadastrado para este dia da semana: {command.WeekDay}."));

            var avaliableTime = new AvaliableTime(
                command.ProfessionalId,
                command.WeekDay,
                command.StartTime,
                command.EndTime,
                active: true,
                DateTime.Now
            );

            await repository.AddAsync(avaliableTime);

            return Result.Success(new CreateAvaliableTimeResponse(
                avaliableTime.Id,
                avaliableTime.WeekDay,
                avaliableTime.StartTime,
                avaliableTime.EndTime));
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateAvaliableTimeResponse>(
                Error.Problem("AvaliableTime.CreateError", ex.Message));
        }
    }
}
