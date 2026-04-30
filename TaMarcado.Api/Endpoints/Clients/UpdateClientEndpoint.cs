using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Clients.UpdateClientObservations;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Clients;

public class UpdateClientEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/clients/{id:guid}", async (
            Guid id,
            UpdateClientRequest request,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            UpdateClientObservationsHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return Results.NotFound("Perfil profissional não encontrado.");

            var command = new UpdateClientObservationsCommand(id, professionalId.Value, request.Observations);
            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}

public record UpdateClientRequest(string UserEmail, string? Observations);
