using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Booking.GetPublicProfessionals;

public class GetPublicProfessionalsHandler(IProfessionalRepository repository)
{
    public async Task<Result<GetPublicProfessionalsResponse>> Handle(GetPublicProfessionalsCommand command)
    {
        try
        {
            var professionals = await repository.GetAllActiveWithCategoryAsync();
            var items = professionals.Select(p => new ProfessionalListItem(
                p.Id,
                p.ExibitionName,
                p.PhotoUrl,
                p.Bio,
                p.Slug,
                p.Category?.Name ?? string.Empty)).ToList();

            return Result.Success(new GetPublicProfessionalsResponse(items));
        }
        catch (Exception ex)
        {
            return Result.Failure<GetPublicProfessionalsResponse>(
                Error.Problem("Professional.ListError", ex.Message));
        }
    }
}
