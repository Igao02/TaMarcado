namespace TaMarcado.Aplicacao.UseCases.Professionals.UploadPhoto;

public record UploadPhotoCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize,
    string ContentRootPath,
    string BaseUrl
);
