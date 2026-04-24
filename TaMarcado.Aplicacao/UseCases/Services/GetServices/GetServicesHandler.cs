using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Services.GetServices;

public class GetServicesHandler(IServiceRepository repository)
{
    public async Task<Result<List<GetServicesResponse>>> Handle(Guid professionalId)
    {
        try
        {
            var services = await repository.GetByProfessionalIdAsync(professionalId);

            var response = services
                .Select(s => new GetServicesResponse(
                    s.Id, s.Name, s.Description,
                    s.DurationInMinutes, s.Price, s.IsActive))
                .ToList();

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<GetServicesResponse>>(
                Error.Problem("Service.GetError", ex.Message));
        }
    }
}
