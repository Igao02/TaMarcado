using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Professionals.UpdateProfessional;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Professional;

public class UpdateProfessionalEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/professional", async (
            UpdateProfessionalRequest request,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            UpdateProfessionalHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return Results.NotFound();

            var command = new UpdateProfessionalCommand(
                professionalId.Value,
                request.CategoryId,
                request.ExibitionName,
                request.Slug,
                request.WhatsApp,
                request.Bio,
                request.Address,
                request.PhotoUrl,
                request.KeyPix,
                (KeyPixEnum)request.KeyPixType
            );

            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        })
        .RequireAuthorization();
    }
}

public record UpdateProfessionalRequest(
    string UserEmail,
    Guid CategoryId,
    string ExibitionName,
    string Slug,
    string WhatsApp,
    string? Bio,
    string? Address,
    string? PhotoUrl,
    string KeyPix,
    int KeyPixType
);
