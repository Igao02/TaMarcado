using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Schedulings.CancelSchedulingByClient;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.ClientSchedulings;

public class CancelClientSchedulingEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/client/schedulings/{id:guid}/cancel", async (
            Guid id,
            string email,
            UserManager<ApplicationUser> userManager,
            CancelSchedulingByClientHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.Unauthorized();

            var command = new CancelSchedulingByClientCommand(id, user.Id);
            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
