using TaMarcado.Dominio.Enum;

namespace TaMarcado.Aplicacao.UseCases.AvaliableTimes.CreateAvaliableTime;

public record CreateAvaliableTimeCommand(
    Guid ProfessionalId,
    WeekEnum WeekDay,
    TimeSpan StartTime,
    TimeSpan EndTime
);
