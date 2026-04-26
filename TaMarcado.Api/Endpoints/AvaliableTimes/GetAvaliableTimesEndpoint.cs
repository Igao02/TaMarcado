using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.AvaliableTimes.GetAvaliableTimes;
using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.AvaliableTimes;

public class GetAvaliableTimesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/available-times", async (
            string email,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            GetAvaliableTimesHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return CustomResults.Problem(
                    Result.Failure(Error.NotFound("Professional.NotFound", "Perfil profissional não encontrado.")));

            var result = await handler.Handle(professionalId.Value);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
