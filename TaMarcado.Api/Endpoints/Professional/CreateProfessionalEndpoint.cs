using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Api.Extensions;
using TaMarcado.Aplicacao.UseCases.Professionals.CreateProfessional;
using TaMarcado.Dominio.Enum;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Professional;

public class CreateProfessionalEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/professional", async (
            CreateProfessionalRequest request,
            UserManager<ApplicationUser> userManager,
            CreateProfessionalHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user is null)
                return Results.Unauthorized();

            var command = new CreateProfessionalCommand(
                user.Id,
                request.CategoryId,
                request.ExibitionName,
                request.Slug,
                request.WhatsApp,
                request.Bio,
                request.KeyPix,
                (KeyPixEnum)request.KeyPixType
            );

            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Created($"/api/professional/{r.Id}", r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}

public record CreateProfessionalRequest(
    string UserEmail,
    Guid CategoryId,
    string ExibitionName,
    string Slug,
    string WhatsApp,
    string? Bio,
    string KeyPix,
    int KeyPixType
);
