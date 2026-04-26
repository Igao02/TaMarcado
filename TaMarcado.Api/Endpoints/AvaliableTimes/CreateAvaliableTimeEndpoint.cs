using Microsoft.AspNetCore.Identity;
using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.AvaliableTimes.CreateAvaliableTime;
using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Enum;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Api.Endpoints.AvaliableTimes;

public class CreateAvaliableTimeEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/available-times", async (
            CreateAvaliableTimeRequest request,
            UserManager<ApplicationUser> userManager,
            IProfessionalRepository professionalRepository,
            CreateAvaliableTimeHandler handler) =>
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user is null)
                return Results.Unauthorized();

            var professionalId = await professionalRepository.GetIdByUserIdAsync(user.Id);
            if (professionalId is null)
                return CustomResults.Problem(
                    Result.Failure(Error.NotFound("Professional.NotFound", "Perfil profissional não encontrado.")));

            if (!TimeSpan.TryParse(request.StartTime, out var startTime) ||
                !TimeSpan.TryParse(request.EndTime, out var endTime))
                return CustomResults.Problem(
                    Result.Failure(Error.Conflict("AvaliableTime.InvalidTimeFormat", "Formato de horário inválido. Use HH:mm.")));

            if (!System.Enum.IsDefined(typeof(WeekEnum), request.WeekDay))
                return CustomResults.Problem(
                    Result.Failure(Error.Conflict("AvaliableTime.InvalidWeekDay", "Dia da semana inválido.")));

            var command = new CreateAvaliableTimeCommand(
                professionalId.Value,
                (WeekEnum)request.WeekDay,
                startTime,
                endTime
            );

            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Created($"/api/available-times/{r.Id}", r),
                onFailure: r => CustomResults.Problem(r));
        });
    }
}

public record CreateAvaliableTimeRequest(
    string UserEmail,
    int WeekDay,
    string StartTime,
    string EndTime
);
