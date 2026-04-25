using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaMarcado.Apresentacao.Handlers.AvaliableTime;

public class AvaliableTimeHandler(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<AvaliableTimeResult<List<AvaliableTimeItem>>> GetAvaliableTimes(string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync($"/api/available-times?email={Uri.EscapeDataString(userEmail)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<AvaliableTimeItem>>(content, JsonOptions) ?? [];
                return new AvaliableTimeResult<List<AvaliableTimeItem>> { Success = true, Data = items };
            }

            return new AvaliableTimeResult<List<AvaliableTimeItem>>
            {
                Success = false,
                Error = await ExtractError(response)
            };
        }
        catch (Exception ex)
        {
            return new AvaliableTimeResult<List<AvaliableTimeItem>> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<AvaliableTimeResult> CreateAvaliableTimes(CreateHorarioModel model, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");

            foreach (var weekDay in model.WeekDays)
            {
                var request = new
                {
                    userEmail,
                    weekDay,
                    startTime = model.StartTime!.Value.ToString(@"hh\:mm"),
                    endTime = model.EndTime!.Value.ToString(@"hh\:mm")
                };

                var response = await client.PostAsJsonAsync("/api/available-times", request);

                if (!response.IsSuccessStatusCode)
                    return new AvaliableTimeResult { Success = false, Error = await ExtractError(response) };
            }

            return new AvaliableTimeResult { Success = true };
        }
        catch (Exception ex)
        {
            return new AvaliableTimeResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<AvaliableTimeResult> DeleteAvaliableTime(Guid id, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.DeleteAsync(
                $"/api/available-times/{id}?email={Uri.EscapeDataString(userEmail)}");

            if (response.IsSuccessStatusCode)
                return new AvaliableTimeResult { Success = true };

            return new AvaliableTimeResult { Success = false, Error = await ExtractError(response) };
        }
        catch (Exception ex)
        {
            return new AvaliableTimeResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
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

    public class CreateHorarioModel
    {
        public IEnumerable<int> WeekDays { get; set; } = [];

        [Required(ErrorMessage = "Informe o horário de início")]
        public TimeSpan? StartTime { get; set; }

        [Required(ErrorMessage = "Informe o horário de fim")]
        public TimeSpan? EndTime { get; set; }
    }

    public static readonly List<(int Value, string Label)> WeekDays =
    [
        (0, "Segunda-feira"),
        (1, "Terça-feira"),
        (2, "Quarta-feira"),
        (3, "Quinta-feira"),
        (4, "Sexta-feira"),
        (5, "Sábado"),
        (6, "Domingo")
    ];

    public class AvaliableTimeResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class AvaliableTimeResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
    }

    public class AvaliableTimeItem
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("weekDay")] public int WeekDay { get; set; }
        [JsonPropertyName("startTime")] public string StartTime { get; set; } = string.Empty;
        [JsonPropertyName("endTime")] public string EndTime { get; set; } = string.Empty;
        [JsonPropertyName("active")] public bool Active { get; set; }

        public string WeekDayName => WeekDay switch
        {
            0 => "Segunda-feira",
            1 => "Terça-feira",
            2 => "Quarta-feira",
            3 => "Quinta-feira",
            4 => "Sexta-feira",
            5 => "Sábado",
            6 => "Domingo",
            _ => "?"
        };
    }

    private class ProblemResponse
    {
        [JsonPropertyName("detail")] public string? Detail { get; set; }
    }
}
