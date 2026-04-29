using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfessionals;

namespace TaMarcado.Api.Endpoints.Public;

public class GetPublicProfessionalsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/professionals", async (GetPublicProfessionalsHandler handler) =>
        {
            var result = await handler.Handle(new GetPublicProfessionalsCommand());
            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
