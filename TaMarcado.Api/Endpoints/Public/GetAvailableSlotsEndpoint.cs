using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Booking.GetAvailableSlots;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Api.Endpoints.Public;

public class GetAvailableSlotsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/available-slots/{slug}", async (
            string slug,
            Guid serviceId,
            DateOnly date,
            IProfessionalRepository professionalRepository,
            GetAvailableSlotsHandler handler) =>
        {
            var professional = await professionalRepository.GetBySlugAsync(slug);
            if (professional is null)
                return Results.NotFound("Profissional não encontrado.");

            var result = await handler.Handle(new GetAvailableSlotsCommand(professional.Id, serviceId, date));

            return result.Match(
                onSuccess: r => Results.Ok(r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}
