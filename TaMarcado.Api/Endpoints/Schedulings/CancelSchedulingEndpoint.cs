using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Schedulings.CancelScheduling;
using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Schedulings;

public class CancelSchedulingEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/schedulings/{id}/cancel", async (
            Guid id,
            string email,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            CancelSchedulingHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return CustomResults.Problem(
                    Result.Failure(Error.NotFound("Professional.NotFound", "Perfil profissional não encontrado.")));

            var result = await handler.Handle(new CancelSchedulingCommand(id, professionalId.Value));

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
