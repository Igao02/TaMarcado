namespace TaMarcado.Aplicacao.UseCases.AvaliableTimes.GetAvaliableTimes;

public record GetAvaliableTimesResponse(
    Guid Id,
    int WeekDay,
    string StartTime,
    string EndTime,
    bool Active
);
