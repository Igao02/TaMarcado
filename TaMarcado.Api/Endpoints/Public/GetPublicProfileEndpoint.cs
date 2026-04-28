using TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfile;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;

namespace TaMarcado.Api.Endpoints.Public;

public class GetPublicProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/profile/{slug}", async (
            string slug,
            GetPublicProfileHandler handler) =>
        {
            var result = await handler.Handle(new GetPublicProfileCommand(slug));

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
