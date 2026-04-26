using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Services.DeleteServices;
using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Services;

public class DeleteServiceEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/services/{id:guid}", async (
            Guid id,
            string email,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            DeleteServicesHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return CustomResults.Problem(
                    Result.Failure(Error.NotFound("Professional.NotFound", "Perfil profissional não encontrado.")));

            var command = new DeleteServicesCommand(id, professionalId.Value);
            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.NoContent(),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
