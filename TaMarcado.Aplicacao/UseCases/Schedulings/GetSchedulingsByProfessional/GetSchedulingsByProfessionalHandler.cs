using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Schedulings.GetSchedulingsByProfessional;

public class GetSchedulingsByProfessionalHandler(ISchedulingRepository schedulingRepository)
{
    public async Task<Result<GetSchedulingsByProfessionalResponse>> Handle(GetSchedulingsByProfessionalCommand command)
    {
        try
        {
            var schedulings = await schedulingRepository.GetByProfessionalIdWithDetailsAsync(command.ProfessionalId);

            var items = schedulings.Select(s => new SchedulingItem(
                s.Id,
                s.Client.Name,
                s.Service.Name,
                s.InitDate,
                s.EndDate,
                s.Status.ToString(),
                s.Price)).ToList();

            return Result.Success(new GetSchedulingsByProfessionalResponse(items));
        }
        catch (Exception ex)
        {
            return Result.Failure<GetSchedulingsByProfessionalResponse>(
                Error.Problem("Scheduling.GetError", ex.Message));
        }
    }
}
