using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaMarcado.Dominio.Enum;

namespace TaMarcado.Apresentacao.Handlers.Professional;

public class ProfessionalHandler(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<ProfessionalResult> CreateProfile(CreateProfileModel model, string userEmail)
    {
        var client = httpClientFactory.CreateClient("ApiBack");

        var request = new
        {
            userEmail,
            categoryId = model.CategoryId,
            exibitionName = model.ExibitionName,
            slug = model.Slug,
            whatsApp = model.WhatsApp,
            bio = model.Bio,
            address = model.Address,
            photoUrl = model.PhotoUrl,
            keyPix = model.KeyPix,
            keyPixType = (int)model.KeyPixType
        };

        var response = await client.PostAsJsonAsync("/api/professional", request);

        if (response.IsSuccessStatusCode)
            return new ProfessionalResult { Success = true };

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(content, JsonOptions);
            return new ProfessionalResult { Success = false, Error = error?.Detail ?? "Erro ao criar perfil." };
        }
        catch
        {
            return new ProfessionalResult { Success = false, Error = "Erro ao criar perfil." };
        }
    }

    public async Task<ProfessionalResult<ProfileData>> GetMyProfile(string userEmail)
    {
        var client = httpClientFactory.CreateClient("ApiBack");
        var response = await client.GetAsync($"/api/professional/me?email={Uri.EscapeDataString(userEmail)}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<ProfileData>(content, JsonOptions);
            return new ProfessionalResult<ProfileData> { Success = true, Data = profile };
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return new ProfessionalResult<ProfileData> { Success = false, IsNotFound = true };

        return new ProfessionalResult<ProfileData> { Success = false, Error = "Erro ao buscar perfil." };
    }

    public async Task<ProfessionalResult> UpdateProfile(CreateProfileModel model, string userEmail)
    {
        var client = httpClientFactory.CreateClient("ApiBack");

        var request = new
        {
            userEmail,
            categoryId = model.CategoryId,
            exibitionName = model.ExibitionName,
            slug = model.Slug,
            whatsApp = model.WhatsApp,
            bio = model.Bio,
            address = model.Address,
            photoUrl = model.PhotoUrl,
            keyPix = model.KeyPix,
            keyPixType = (int)model.KeyPixType
        };

        var response = await client.PutAsJsonAsync("/api/professional", request);

        if (response.IsSuccessStatusCode)
            return new ProfessionalResult { Success = true };

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(content, JsonOptions);
            return new ProfessionalResult { Success = false, Error = error?.Detail ?? "Erro ao atualizar perfil." };
        }
        catch
        {
            return new ProfessionalResult { Success = false, Error = $"Erro HTTP {(int)response.StatusCode}: {content}" };
        }
    }

    public async Task<ProfessionalResult<string>> UploadPhotoAsync(byte[] fileBytes, string fileName, string contentType)
    {
        var client = httpClientFactory.CreateClient("ApiBack");

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "photo", fileName);

        var response = await client.PostAsync("/api/professional/upload-photo", content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UploadPhotoResponse>(json, JsonOptions);
            return new ProfessionalResult<string> { Success = true, Data = result?.Url };
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        try
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(errorContent, JsonOptions);
            return new ProfessionalResult<string> { Success = false, Error = error?.Detail ?? "Erro ao enviar foto." };
        }
        catch
        {
            return new ProfessionalResult<string> { Success = false, Error = $"Erro HTTP {(int)response.StatusCode}: {errorContent}" };
        }
    }

    public async Task<List<CategoryItem>> GetCategories()
    {
        var client = httpClientFactory.CreateClient("ApiBack");
        var response = await client.GetAsync("/api/categories");

        if (!response.IsSuccessStatusCode)
            return [];

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<CategoryItem>>(content, JsonOptions) ?? [];
    }

    // --- Models ---

    public class CreateProfileModel
    {
        [Required(ErrorMessage = "O nome de exibição é obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string ExibitionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O slug é obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Apenas letras minúsculas, números e hífens")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "O WhatsApp é obrigatório")]
        public string WhatsApp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione uma categoria")]
        public Guid? CategoryId { get; set; }

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string? Bio { get; set; }

        [StringLength(300, ErrorMessage = "Máximo 300 caracteres")]
        public string? Address { get; set; }

        public string? PhotoUrl { get; set; }

        [Required(ErrorMessage = "A chave PIX é obrigatória")]
        public string KeyPix { get; set; } = string.Empty;

        public KeyPixEnum KeyPixType { get; set; } = KeyPixEnum.CPF;
    }

    public class ProfessionalResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class ProfessionalResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public bool IsNotFound { get; set; }
    }

    public record CategoryItem(Guid Id, string Name);

    public class ProfileData
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("exibitionName")] public string ExibitionName { get; set; } = string.Empty;
        [JsonPropertyName("slug")] public string Slug { get; set; } = string.Empty;
        [JsonPropertyName("whatsApp")] public string WhatsApp { get; set; } = string.Empty;
        [JsonPropertyName("bio")] public string? Bio { get; set; }
        [JsonPropertyName("address")] public string? Address { get; set; }
        [JsonPropertyName("photoUrl")] public string? PhotoUrl { get; set; }
        [JsonPropertyName("categoryId")] public Guid CategoryId { get; set; }
        [JsonPropertyName("categoryName")] public string? CategoryName { get; set; }
        [JsonPropertyName("keyPix")] public string KeyPix { get; set; } = string.Empty;
        [JsonPropertyName("keyPixType")] public int KeyPixType { get; set; }
        [JsonPropertyName("active")] public bool Active { get; set; }
    }

    private class ErrorResponse
    {
        [JsonPropertyName("detail")] public string? Detail { get; set; }
    }

    private class UploadPhotoResponse
    {
        [JsonPropertyName("url")] public string? Url { get; set; }
    }
}
