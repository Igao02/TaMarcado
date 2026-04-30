using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Schedulings.GetSchedulingsByClient;

public class GetSchedulingsByClientHandler(ISchedulingRepository repository)
{
    public async Task<Result<List<GetSchedulingsByClientResponse>>> Handle(GetSchedulingsByClientCommand command)
    {
        try
        {
            var schedulings = await repository.GetByClientUserIdWithDetailsAsync(command.ApplicationUserId);

            var response = schedulings
                .Select(s => new GetSchedulingsByClientResponse(
                    s.Id,
                    s.Professional.ExibitionName,
                    s.Service.Name,
                    s.InitDate,
                    s.EndDate,
                    s.Status.ToString(),
                    s.Price,
                    s.Payment?.StatusPayment.ToString()))
                .ToList();

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<GetSchedulingsByClientResponse>>(
                Error.Problem("Scheduling.GetClientError", ex.Message));
        }
    }
}
