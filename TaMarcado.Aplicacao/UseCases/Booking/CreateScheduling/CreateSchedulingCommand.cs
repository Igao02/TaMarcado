namespace TaMarcado.Aplicacao.UseCases.Booking.CreateScheduling;

public record CreateSchedulingCommand(
    Guid ProfessionalId,
    Guid ServiceId,
    DateOnly Date,
    TimeSpan StartTime,
    string ClientUserId,
    string ClientName,
    string ClientPhone,
    string? ClientEmail);
