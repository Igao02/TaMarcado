using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaMarcado.Apresentacao.Handlers.Payment;

public class PaymentHandler(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<PaymentResult<PaymentItem>> GetPaymentAsync(Guid schedulingId, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync(
                $"/api/payments/{schedulingId}?email={Uri.EscapeDataString(userEmail)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var item = JsonSerializer.Deserialize<PaymentItem>(content, JsonOptions);
                return new PaymentResult<PaymentItem> { Success = true, Data = item };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var detail = TryExtractDetail(errorContent);
            return new PaymentResult<PaymentItem>
            {
                Success = false,
                Error = detail ?? $"Erro HTTP {(int)response.StatusCode}: {errorContent}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult<PaymentItem> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<PaymentResult<PaymentItem>> GetClientPaymentAsync(Guid schedulingId, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.GetAsync(
                $"/api/client/payments/{schedulingId}?email={Uri.EscapeDataString(userEmail)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var item = JsonSerializer.Deserialize<PaymentItem>(content, JsonOptions);
                return new PaymentResult<PaymentItem> { Success = true, Data = item };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var detail = TryExtractDetail(errorContent);
            return new PaymentResult<PaymentItem>
            {
                Success = false,
                Error = detail ?? $"Erro HTTP {(int)response.StatusCode}: {errorContent}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult<PaymentItem> { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    public async Task<PaymentResult> ConfirmPaymentAsync(Guid schedulingId, string userEmail)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiBack");
            var response = await client.PutAsync(
                $"/api/payments/{schedulingId}/confirm?email={Uri.EscapeDataString(userEmail)}", null);

            if (response.IsSuccessStatusCode)
                return new PaymentResult { Success = true };

            var content = await response.Content.ReadAsStringAsync();
            var detail = TryExtractDetail(content);
            return new PaymentResult
            {
                Success = false,
                Error = detail ?? $"Erro HTTP {(int)response.StatusCode}: {content}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult { Success = false, Error = $"Erro inesperado: {ex.Message}" };
        }
    }

    private static string? TryExtractDetail(string content)
    {
        try
        {
            var err = JsonSerializer.Deserialize<ErrorResponse>(content, JsonOptions);
            return err?.Detail;
        }
        catch { return null; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class PaymentResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
    }

    public class PaymentItem
    {
        [JsonPropertyName("paymentId")] public Guid PaymentId { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
        [JsonPropertyName("pixPayload")] public string PixPayload { get; set; } = string.Empty;
        [JsonPropertyName("qrCodeBase64")] public string QrCodeBase64 { get; set; } = string.Empty;
        [JsonPropertyName("datePayment")] public DateTime? DatePayment { get; set; }

        public string StatusLabel => Status switch
        {
            "Pending" => "Pendente",
            "Paid" => "Pago",
            _ => Status
        };

        public bool IsPending => Status == "Pending";
    }

    private class ErrorResponse
    {
        [JsonPropertyName("detail")] public string? Detail { get; set; }
    }
}
