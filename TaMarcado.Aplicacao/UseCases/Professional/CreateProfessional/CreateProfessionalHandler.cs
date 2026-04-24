using TaMarcado.Compartilhado;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Professionals.CreateProfessional;

public class CreateProfessionalHandler(IProfessionalRepository repository)
{
    public async Task<Result<CreateProfessionalResponse>> Handle(CreateProfessionalCommand command)
    {
        try
        {
            if (await repository.ExistsByUserIdAsync(command.ApplicationUserId))
                return Result.Failure<CreateProfessionalResponse>(
                    Error.Conflict("Professional.UserAlreadyHasProfile", "Este usuário já possui um perfil de profissional."));

            if (await repository.ExistsBySlugAsync(command.Slug))
                return Result.Failure<CreateProfessionalResponse>(
                    Error.Conflict("Professional.SlugAlreadyExists", "Este slug já está em uso. Escolha outro."));

            var professional = new Professional
            {
                ApplicationUserId = command.ApplicationUserId,
                CategoryId = command.CategoryId,
                ExibitionName = command.ExibitionName,
                Slug = command.Slug,
                WhatsApp = command.WhatsApp,
                Bio = command.Bio,
                PhotoUrl = string.Empty,
                KeyPix = command.KeyPix,
                KeyPixType = command.KeyPixType,
                Active = true,
                CreatedAt = DateTime.Now
            };

            await repository.AddAsync(professional);

            return Result.Success(new CreateProfessionalResponse(professional.Id, professional.Slug));
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateProfessionalResponse>(
                Error.Problem("Professional.CreateError", ex.Message));
        }
    }
}
