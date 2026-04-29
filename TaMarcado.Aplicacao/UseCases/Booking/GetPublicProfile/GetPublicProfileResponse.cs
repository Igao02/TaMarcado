namespace TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfile;

public record GetPublicProfileResponse(
    Guid ProfessionalId,
    string ExibitionName,
    string? PhotoUrl,
    string? Bio,
    string WhatsApp);
