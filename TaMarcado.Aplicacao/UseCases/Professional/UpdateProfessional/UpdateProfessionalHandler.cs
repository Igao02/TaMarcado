using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Professionals.UpdateProfessional;

public class UpdateProfessionalHandler(IProfessionalRepository repository)
{
    public async Task<Result<UpdateProfessionalResponse>> Handle(UpdateProfessionalCommand command)
    {
        try
        {
            var professional = await repository.GetByIdAsync(command.Id);
            if (professional is null)
                return Result.Failure<UpdateProfessionalResponse>(
                    Error.NotFound("Professional.NotFound", "Perfil não encontrado."));

            var slugOwner = await repository.GetBySlugAsync(command.Slug);
            if (slugOwner is not null && slugOwner.Id != command.Id)
                return Result.Failure<UpdateProfessionalResponse>(
                    Error.Conflict("Professional.SlugAlreadyExists", "Este slug já está em uso. Escolha outro."));

            professional.CategoryId = command.CategoryId;
            professional.ExibitionName = command.ExibitionName;
            professional.Slug = command.Slug;
            professional.WhatsApp = command.WhatsApp;
            professional.Bio = command.Bio;
            professional.Address = command.Address;
            professional.KeyPix = command.KeyPix;
            professional.KeyPixType = command.KeyPixType;

            if (!string.IsNullOrEmpty(command.PhotoUrl))
                professional.PhotoUrl = command.PhotoUrl;

            await repository.UpdateAsync(professional);

            return Result.Success(new UpdateProfessionalResponse(professional.Id));
        }
        catch (Exception ex)
        {
            return Result.Failure<UpdateProfessionalResponse>(
                Error.Problem("Professional.UpdateError", ex.Message));
        }
    }
}
