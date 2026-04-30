using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Schedulings.GetSchedulingsByClient;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.ClientSchedulings;

public class GetClientSchedulingsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/client/schedulings", async (
            string email,
            UserManager<ApplicationUser> userManager,
            GetSchedulingsByClientHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var command = new GetSchedulingsByClientCommand(user.Id);
            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
