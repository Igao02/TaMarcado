using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.AvaliableTimes.GetAvaliableTimes;

public class GetAvaliableTimesHandler(IAvaliableTimeRepository repository)
{
    public async Task<Result<List<GetAvaliableTimesResponse>>> Handle(Guid professionalId)
    {
        try
        {
            var items = await repository.GetByProfessionalIdAsync(professionalId);

            var response = items
                .Select(a => new GetAvaliableTimesResponse(
                    a.Id,
                    (int)a.WeekDay,
                    a.StartTime.ToString(@"hh\:mm"),
                    a.EndTime.ToString(@"hh\:mm"),
                    a.Active))
                .ToList();

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<GetAvaliableTimesResponse>>(
                Error.Problem("AvaliableTime.GetError", ex.Message));
        }
    }
}
