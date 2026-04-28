namespace TaMarcado.Aplicacao.UseCases.Booking.GetAvailableSlots;

public record GetAvailableSlotsResponse(List<TimeSlotItem> Slots);

public record TimeSlotItem(string StartTime, string EndTime);
