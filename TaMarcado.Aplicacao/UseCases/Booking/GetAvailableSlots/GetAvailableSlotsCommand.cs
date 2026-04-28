namespace TaMarcado.Aplicacao.UseCases.Booking.GetAvailableSlots;

public record GetAvailableSlotsCommand(Guid ProfessionalId, Guid ServiceId, DateOnly Date);
