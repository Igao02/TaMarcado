using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Professionals.GetProfessional;

public class GetProfessionalHandler(IProfessionalRepository repository)
{
    public async Task<Result<GetProfessionalResponse>> Handle(string applicationUserId)
    {
        try
        {
            var professional = await repository.GetByUserIdWithCategoryAsync(applicationUserId);

            if (professional is null)
                return Result.Failure<GetProfessionalResponse>(
                    Error.NotFound("Professional.NotFound", "Perfil profissional não encontrado."));

            return Result.Success(new GetProfessionalResponse(
                professional.Id,
                professional.ExibitionName,
                professional.Slug,
                professional.WhatsApp,
                professional.Bio,
                professional.CategoryId,
                professional.Category?.Name,
                professional.KeyPix,
                professional.KeyPixType,
                professional.Active
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<GetProfessionalResponse>(
                Error.Problem("Professional.GetError", ex.Message));
        }
    }
}
