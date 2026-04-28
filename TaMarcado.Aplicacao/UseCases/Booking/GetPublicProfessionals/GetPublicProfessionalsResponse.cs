namespace TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfessionals;

public record GetPublicProfessionalsResponse(List<ProfessionalListItem> Professionals);

public record ProfessionalListItem(Guid Id, string ExibitionName, string? PhotoUrl, string? Bio, string Slug, string CategoryName);
