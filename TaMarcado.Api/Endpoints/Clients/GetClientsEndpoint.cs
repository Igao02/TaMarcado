using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Clients.GetClients;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Clients;

public class GetClientsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clients", async (
            string email,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            GetClientsHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return Results.NotFound("Perfil profissional não encontrado.");

            var command = new GetClientsCommand(professionalId.Value);
            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
