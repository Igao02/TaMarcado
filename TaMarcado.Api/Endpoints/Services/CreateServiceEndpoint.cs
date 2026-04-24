using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Services.CreateService;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Services;

public class CreateServiceEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/services", async (
            CreateServiceRequest request,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            CreateServiceHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return Results.BadRequest("Perfil profissional não encontrado.");

            var command = new CreateServiceCommand(
                professionalId.Value,
                request.Name,
                request.Description ?? string.Empty,
                request.DurationInMinutes,
                request.Price
            );

            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Created($"/api/services/{r.Id}", r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}

public record CreateServiceRequest(
    string UserEmail,
    string Name,
    string? Description,
    int DurationInMinutes,
    decimal Price
);
