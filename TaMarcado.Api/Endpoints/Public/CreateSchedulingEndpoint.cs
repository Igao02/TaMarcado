using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Booking.CreateScheduling;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.Public;

public class CreateSchedulingEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/public/scheduling", async (
            CreateSchedulingRequest request,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            CreateSchedulingHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user is null)
                return Results.Unauthorized();

            var professional = await professionalRepository.GetBySlugAsync(request.Slug);
            if (professional is null)
                return Results.NotFound("Profissional não encontrado.");

            if (!TimeSpan.TryParseExact(request.StartTime, @"hh\:mm", null, out var startTime))
                return Results.BadRequest("Formato de hora inválido. Use HH:mm.");

            var result = await handler.Handle(new CreateSchedulingCommand(
                professional.Id,
                request.ServiceId,
                request.Date,
                startTime,
                user.Id,
                request.ClientName,
                request.ClientPhone,
                request.ClientEmail));

            return result.Match(
                onSuccess: r => Results.Created($"/api/public/scheduling/{r.SchedulingId}", r),
                onFailure: r => CustomResults.Problem(r));
        });
    }

    private record CreateSchedulingRequest(
        string UserEmail,
        string Slug,
        Guid ServiceId,
        DateOnly Date,
        string StartTime,
        string ClientName,
        string ClientPhone,
        string? ClientEmail);
}
