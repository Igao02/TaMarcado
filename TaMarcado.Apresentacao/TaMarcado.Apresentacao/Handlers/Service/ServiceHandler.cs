using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaMarcado.Apresentacao.Handlers.Service;

public class ServiceHandler(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<ServiceResult> CreateService(CreateServiceModel model, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");

            var request = new
            {
                userEmail,
                name = model.Name,
                description = model.Description,
                durationInMinutes = model.DurationInMinutes,
                price = model.Price
            };

            var response = await client.PostAsJsonAsync("/api/services", request);

            if (response.IsSuccessStatusCode)
                return new ServiceResult { Success = true };

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(content, JsonOptions);
                return new ServiceResult { Success = false, Error = error?.Detail ?? "Erro ao criar serviço." };
            }
            catch
            {
                return new ServiceResult { Success = false, Error = "Erro ao criar serviço." };
            }
        }
        catch (Exception ex)
        {
            return new ServiceResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<ServiceResult<List<ServiceItem>>> GetServices(string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync($"/api/services?email={Uri.EscapeDataString(userEmail)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<ServiceItem>>(content, JsonOptions) ?? [];
                return new ServiceResult<List<ServiceItem>> { Success = true, Data = items };
            }

            return new ServiceResult<List<ServiceItem>> { Success = false, Error = "Erro ao buscar serviços." };
        }
        catch (Exception ex)
        {
            return new ServiceResult<List<ServiceItem>> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<ServiceResult> DeleteService(Guid id, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.DeleteAsync($"/api/services/{id}?email={Uri.EscapeDataString(userEmail)}");

            if (response.IsSuccessStatusCode)
                return new ServiceResult { Success = true };

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(content, JsonOptions);
                return new ServiceResult { Success = false, Error = error?.Detail ?? "Erro ao deletar serviço." };
            }
            catch
            {
                return new ServiceResult { Success = false, Error = "Erro ao deletar serviço." };
            }
        }
        catch (Exception ex)
        {
            return new ServiceResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    // --- Models ---

    public class CreateServiceModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A duração é obrigatória")]
        [Range(5, 480, ErrorMessage = "Entre 5 e 480 minutos")]
        public int DurationInMinutes { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, 99999.99, ErrorMessage = "Preço inválido")]
        public decimal Price { get; set; }
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
    }

    public class ServiceItem
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
        [JsonPropertyName("durationInMinutes")] public int DurationInMinutes { get; set; }
        [JsonPropertyName("price")] public decimal Price { get; set; }
        [JsonPropertyName("isActive")] public bool IsActive { get; set; }
    }

    private class ErrorResponse
    {
        [JsonPropertyName("detail")] public string? Detail { get; set; }
    }
}
