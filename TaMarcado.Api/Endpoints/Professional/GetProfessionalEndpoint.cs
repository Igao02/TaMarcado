using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Api.Extensions;
using TaMarcado.Aplicacao.UseCases.Professionals.GetProfessional;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Professional;

public class GetProfessionalEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/professional/me", async (
            string email,
            UserManager<ApplicationUser> userManager,
            GetProfessionalHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var result = await handler.Handle(user.Id);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
