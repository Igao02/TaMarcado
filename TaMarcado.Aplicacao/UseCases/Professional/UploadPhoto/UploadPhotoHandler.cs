using TaMarcado.Compartilhado;

namespace TaMarcado.Aplicacao.UseCases.Professionals.UploadPhoto;

public class UploadPhotoHandler
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public async Task<Result<UploadPhotoResponse>> Handle(UploadPhotoCommand command)
    {
        try
        {
            if (command.FileSize == 0)
                return Result.Failure<UploadPhotoResponse>(
                    Error.Failure("Photo.Empty", "Nenhum arquivo enviado."));

            if (command.FileSize > MaxFileSize)
                return Result.Failure<UploadPhotoResponse>(
                    Error.Failure("Photo.TooLarge", "O arquivo excede o tamanho máximo de 5MB."));

            var ext = Path.GetExtension(command.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return Result.Failure<UploadPhotoResponse>(
                    Error.Failure("Photo.InvalidFormat", "Formato inválido. Use JPG, PNG ou WebP."));

            var uploadsPath = Path.GetFullPath(
                Path.Combine(command.ContentRootPath, "..", ".uploads", "photos"));
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using var stream = File.Create(filePath);
            await command.FileStream.CopyToAsync(stream);

            return Result.Success(new UploadPhotoResponse($"{command.BaseUrl}/uploads/photos/{fileName}"));
        }
        catch (Exception ex)
        {
            return Result.Failure<UploadPhotoResponse>(
                Error.Problem("Photo.UploadError", ex.Message));
        }
    }
}
