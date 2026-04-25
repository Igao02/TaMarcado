using TaMarcado.Dominio.Enum;

namespace TaMarcado.Aplicacao.UseCases.AvaliableTimes.CreateAvaliableTime;

public record CreateAvaliableTimeResponse(
    Guid Id,
    WeekEnum WeekDay,
    TimeSpan StartTime,
    TimeSpan EndTime
);
