using Microsoft.AspNetCore.Identity;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Auth;

public class RegisterWithRoleEndpoint : IEndpoint
{
    private static readonly string[] AllowedRoles = ["Profissional", "Cliente"];

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            RegisterWithRoleRequest request,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager) =>
        {
            if (!AllowedRoles.Contains(request.Role))
                return Results.BadRequest("Role inválida. Use 'Profissional' ou 'Cliente'.");

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return Results.BadRequest(new { errors });
            }

            await userManager.AddToRoleAsync(user, request.Role);

            return Results.Ok(new { message = "Usuário criado com sucesso." });
        });
    }

    private record RegisterWithRoleRequest(string Email, string Password, string Role);
}
