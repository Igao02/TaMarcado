using TaMarcado.Api.Extensions;
using TaMarcado.Api.Infrastructure;
using TaMarcado.Aplicacao.UseCases.Professionals.UploadPhoto;

namespace TaMarcado.Api.Endpoints.Professional;

public class UploadPhotoEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/professional/upload-photo", async (
            IFormFile photo,
            IWebHostEnvironment env,
            HttpContext context,
            UploadPhotoHandler handler) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

            var command = new UploadPhotoCommand(
                photo?.OpenReadStream() ?? Stream.Null,
                photo?.FileName ?? string.Empty,
                photo?.ContentType ?? string.Empty,
                photo?.Length ?? 0,
                env.ContentRootPath,
                baseUrl
            );

            var result = await handler.Handle(command);

            return result.Match(
                onSuccess: r => Results.Ok(new { url = r.Url }),
                onFailure: r => CustomResults.Problem(r));
        })
        .RequireAuthorization()
        .DisableAntiforgery();
    }
}
