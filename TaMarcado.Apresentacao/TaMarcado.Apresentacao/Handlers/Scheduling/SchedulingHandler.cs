using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaMarcado.Apresentacao.Handlers.Scheduling;

public class SchedulingHandler(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<SchedulingResult<List<SchedulingItem>>> GetSchedulingsAsync(string email)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync($"/api/schedulings?email={Uri.EscapeDataString(email)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<SchedulingsApiResponse>(content, JsonOptions);
                return new SchedulingResult<List<SchedulingItem>> { Success = true, Data = data?.Schedulings ?? [] };
            }

            return new SchedulingResult<List<SchedulingItem>> { Success = false, Error = await ExtractError(response) };
        }
        catch (Exception ex)
        {
            return new SchedulingResult<List<SchedulingItem>> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<SchedulingResult> ConfirmAsync(Guid id, string email)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.PutAsync(
                $"/api/schedulings/{id}/confirm?email={Uri.EscapeDataString(email)}", null);

            if (response.IsSuccessStatusCode)
                return new SchedulingResult { Success = true };

            return new SchedulingResult { Success = false, Error = await ExtractError(response) };
        }
        catch (Exception ex)
        {
            return new SchedulingResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<SchedulingResult> CancelAsync(Guid id, string email)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.PutAsync(
                $"/api/schedulings/{id}/cancel?email={Uri.EscapeDataString(email)}", null);

            if (response.IsSuccessStatusCode)
                return new SchedulingResult { Success = true };

            return new SchedulingResult { Success = false, Error = await ExtractError(response) };
        }
        catch (Exception ex)
        {
            return new SchedulingResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<SchedulingResult> ConcludeAsync(Guid id, string email)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.PutAsync(
                $"/api/schedulings/{id}/conclude?email={Uri.EscapeDataString(email)}", null);

            if (response.IsSuccessStatusCode)
                return new SchedulingResult { Success = true };

            return new SchedulingResult { Success = false, Error = await ExtractError(response) };
        }
        catch (Exception ex)
        {
            return new SchedulingResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    private async Task<string> ExtractError(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
            return $"Erro HTTP {(int)response.StatusCode}.";

        try
        {
            var problem = JsonSerializer.Deserialize<ProblemResponse>(content, JsonOptions);
            if (!string.IsNullOrWhiteSpace(problem?.Detail))
                return problem.Detail;
        }
        catch { }

        return $"Erro HTTP {(int)response.StatusCode}: {content}";
    }

    // --- Models ---

    public class SchedulingResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class SchedulingResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
    }

    public class SchedulingItem
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("clientName")] public string ClientName { get; set; } = string.Empty;
        [JsonPropertyName("serviceName")] public string ServiceName { get; set; } = string.Empty;
        [JsonPropertyName("initDate")] public DateTime InitDate { get; set; }
        [JsonPropertyName("endDate")] public DateTime EndDate { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
        [JsonPropertyName("price")] public decimal Price { get; set; }

        public string StatusLabel => Status switch
        {
            "Pendent" => "Pendente",
            "Confirmed" => "Confirmado",
            "Canceled" => "Cancelado",
            "Concluded" => "Concluído",
            _ => Status
        };
    }

    private class SchedulingsApiResponse
    {
        [JsonPropertyName("schedulings")] public List<SchedulingItem> Schedulings { get; set; } = [];
    }

    private class ProblemResponse
    {
        [JsonPropertyName("detail")] public string? Detail { get; set; }
    }
}
