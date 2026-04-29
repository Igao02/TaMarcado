using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfile;

public class GetPublicProfileHandler(IProfessionalRepository repository)
{
    public async Task<Result<GetPublicProfileResponse>> Handle(GetPublicProfileCommand command)
    {
        try
        {
            var professional = await repository.GetBySlugAsync(command.Slug);

            if (professional is null)
                return Result.Failure<GetPublicProfileResponse>(
                    Error.NotFound("Professional.NotFound", "Profissional não encontrado."));

            return Result.Success(new GetPublicProfileResponse(
                professional.Id,
                professional.ExibitionName,
                professional.PhotoUrl,
                professional.Bio,
                professional.WhatsApp));
        }
        catch (Exception ex)
        {
            return Result.Failure<GetPublicProfileResponse>(
                Error.Problem("Professional.GetError", ex.Message));
        }
    }
}
