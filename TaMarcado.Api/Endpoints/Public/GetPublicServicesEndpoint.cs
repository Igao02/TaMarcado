using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Services.GetServices;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Api.Endpoints.Public;

public class GetPublicServicesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/services/{slug}", async (
            string slug,
            IProfessionalRepository professionalRepository,
            GetServicesHandler handler) =>
        {
            var professional = await professionalRepository.GetBySlugAsync(slug);
            if (professional is null)
                return Results.NotFound("Profissional não encontrado.");

            var result = await handler.Handle(professional.Id);

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
