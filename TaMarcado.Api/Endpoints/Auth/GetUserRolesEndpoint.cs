using Microsoft.AspNetCore.Identity;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Auth;

public class GetUserRolesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/roles", async (
            string email,
            UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.NotFound();

            var roles = await userManager.GetRolesAsync(user);
            return Results.Ok(roles);
        });
    }
}
