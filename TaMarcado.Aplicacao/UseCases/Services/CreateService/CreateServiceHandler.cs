using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Services.CreateService;

public class CreateServiceHandler(IServiceRepository repository)
{
    public async Task<Result<CreateServiceResponse>> Handle(CreateServiceCommand command)
    {
        try
        {
            var service = new Service(
                command.ProfessionalId,
                command.Name,
                command.Description,
                command.DurationInMinutes,
                command.Price,
                isActive: true,
                DateTime.Now
            );

            await repository.AddAsync(service);

            return Result.Success(new CreateServiceResponse(
                service.Id,
                service.Name,
                service.Price,
                service.DurationInMinutes));
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateServiceResponse>(
                Error.Problem("Service.CreateError", ex.Message));
        }
    }
}
