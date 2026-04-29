using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaMarcado.Apresentacao.Handlers.Booking;

public class BookingHandler(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<BookingResult<ProfessionalProfile>> GetProfileAsync(string slug)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync($"/api/public/profile/{Uri.EscapeDataString(slug)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ProfessionalProfile>(content, JsonOptions);
                return new BookingResult<ProfessionalProfile> { Success = true, Data = data };
            }

            return new BookingResult<ProfessionalProfile> { Success = false, Error = "Profissional não encontrado." };
        }
        catch (Exception ex)
        {
            return new BookingResult<ProfessionalProfile> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<BookingResult<List<PublicServiceItem>>> GetServicesAsync(string slug)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync($"/api/public/services/{Uri.EscapeDataString(slug)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<PublicServiceItem>>(content, JsonOptions) ?? [];
                return new BookingResult<List<PublicServiceItem>> { Success = true, Data = data };
            }

            return new BookingResult<List<PublicServiceItem>> { Success = false, Error = "Erro ao buscar serviços." };
        }
        catch (Exception ex)
        {
            return new BookingResult<List<PublicServiceItem>> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<BookingResult<List<TimeSlot>>> GetAvailableSlotsAsync(string slug, Guid serviceId, DateOnly date)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var dateStr = date.ToString("yyyy-MM-dd");
            var url = $"/api/public/available-slots/{Uri.EscapeDataString(slug)}?serviceId={serviceId}&date={dateStr}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<AvailableSlotsResponse>(content, JsonOptions);
                return new BookingResult<List<TimeSlot>> { Success = true, Data = data?.Slots ?? [] };
            }

            return new BookingResult<List<TimeSlot>> { Success = false, Error = "Erro ao buscar horários disponíveis." };
        }
        catch (Exception ex)
        {
            return new BookingResult<List<TimeSlot>> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<BookingResult<Guid>> CreateBookingAsync(CreateBookingModel model, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");

            var request = new
            {
                userEmail,
                slug = model.Slug,
                serviceId = model.ServiceId,
                date = model.Date.ToString("yyyy-MM-dd"),
                startTime = model.StartTime,
                clientName = model.ClientName,
                clientPhone = model.ClientPhone,
                clientEmail = model.ClientEmail
            };

            var response = await client.PostAsJsonAsync("/api/public/scheduling", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<CreateBookingResponse>(content, JsonOptions);
                return new BookingResult<Guid> { Success = true, Data = data?.SchedulingId ?? Guid.Empty };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(errorContent, JsonOptions);
                return new BookingResult<Guid> { Success = false, Error = error?.Detail ?? "Erro ao criar agendamento." };
            }
            catch
            {
                return new BookingResult<Guid> { Success = false, Error = $"Erro HTTP {(int)response.StatusCode}: {errorContent}" };
            }
        }
        catch (Exception ex)
        {
            return new BookingResult<Guid> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<BookingResult<List<ProfessionalListItem>>> GetProfessionalsAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync("/api/public/professionals");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ProfessionalsApiResponse>(content, JsonOptions);
                return new BookingResult<List<ProfessionalListItem>> { Success = true, Data = data?.Professionals ?? [] };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new BookingResult<List<ProfessionalListItem>> { Success = false, Error = $"Erro HTTP {(int)response.StatusCode}: {errorContent}" };
        }
        catch (Exception ex)
        {
            return new BookingResult<List<ProfessionalListItem>> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    // --- Models ---

    public class CreateBookingModel
    {
        public string Slug { get; set; } = string.Empty;
        public Guid ServiceId { get; set; }
        public DateOnly Date { get; set; }
        public string StartTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string ClientPhone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string? ClientEmail { get; set; }
    }

    public class BookingResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
    }

    public class ProfessionalProfile
    {
        [JsonPropertyName("professionalId")] public Guid ProfessionalId { get; set; }
        [JsonPropertyName("exibitionName")] public string ExibitionName { get; set; } = string.Empty;
        [JsonPropertyName("photoUrl")] public string? PhotoUrl { get; set; }
        [JsonPropertyName("bio")] public string? Bio { get; set; }
        [JsonPropertyName("whatsApp")] public string WhatsApp { get; set; } = string.Empty;
    }

    public class PublicServiceItem
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("durationInMinutes")] public int DurationInMinutes { get; set; }
        [JsonPropertyName("price")] public decimal Price { get; set; }
        [JsonPropertyName("isActive")] public bool IsActive { get; set; }
    }

    public class TimeSlot
    {
        [JsonPropertyName("startTime")] public string StartTime { get; set; } = string.Empty;
        [JsonPropertyName("endTime")] public string EndTime { get; set; } = string.Empty;
    }

    private class AvailableSlotsResponse
    {
        [JsonPropertyName("slots")] public List<TimeSlot> Slots { get; set; } = [];
    }

    private class CreateBookingResponse
    {
        [JsonPropertyName("schedulingId")] public Guid SchedulingId { get; set; }
    }

    private class ErrorResponse
    {
        [JsonPropertyName("detail")] public string? Detail { get; set; }
    }

    private class ProfessionalsApiResponse
    {
        [JsonPropertyName("professionals")] public List<ProfessionalListItem> Professionals { get; set; } = [];
    }

    public class ProfessionalListItem
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("exibitionName")] public string ExibitionName { get; set; } = string.Empty;
        [JsonPropertyName("photoUrl")] public string? PhotoUrl { get; set; }
        [JsonPropertyName("bio")] public string? Bio { get; set; }
        [JsonPropertyName("slug")] public string Slug { get; set; } = string.Empty;
        [JsonPropertyName("categoryName")] public string CategoryName { get; set; } = string.Empty;
    }
}
