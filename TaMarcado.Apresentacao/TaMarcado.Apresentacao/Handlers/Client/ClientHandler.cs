using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaMarcado.Apresentacao.Handlers.Client;

public class ClientHandler(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<ClientResult<List<ClientItem>>> GetClients(string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync($"/api/clients?email={Uri.EscapeDataString(userEmail)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<ClientItem>>(content, JsonOptions) ?? [];
                return new ClientResult<List<ClientItem>> { Success = true, Data = items };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var detail = TryExtractDetail(errorContent);
            return new ClientResult<List<ClientItem>>
            {
                Success = false,
                Error = detail ?? $"Erro HTTP {(int)response.StatusCode}: {errorContent}"
            };
        }
        catch (Exception ex)
        {
            return new ClientResult<List<ClientItem>> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<ClientResult> UpdateObservations(Guid id, string? observations, string userEmail)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("ApiBack");
            var request = new { userEmail, observations };
            var response = await httpClient.PutAsJsonAsync($"/api/clients/{id}", request);

            if (response.IsSuccessStatusCode)
                return new ClientResult { Success = true };

            var content = await response.Content.ReadAsStringAsync();
            var detail = TryExtractDetail(content);
            return new ClientResult
            {
                Success = false,
                Error = detail ?? $"Erro HTTP {(int)response.StatusCode}: {content}"
            };
        }
        catch (Exception ex)
        {
            return new ClientResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    private static string? TryExtractDetail(string content)
    {
        try
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(content, JsonOptions);
            return error?.Detail;
        }
        catch
        {
            return null;
        }
    }

    public class ClientResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class ClientResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
    }

    public class ClientItem
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("phone")] public string Phone { get; set; } = string.Empty;
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("observations")] public string? Observations { get; set; }
        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
    }

    private class ErrorResponse
    {
        [JsonPropertyName("detail")] public string? Detail { get; set; }
    }
}
