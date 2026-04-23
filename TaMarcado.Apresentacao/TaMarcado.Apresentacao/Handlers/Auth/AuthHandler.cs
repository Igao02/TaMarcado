using Microsoft.AspNetCore.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaMarcado.Apresentacao.Handlers.Auth;

public class AuthHandler
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public class ValidationErrorResponse
    {
        [JsonPropertyName("errors")]
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }

    private string TranslateError(string? error)
    {
        if (string.IsNullOrEmpty(error))
            return "Erro desconhecido.";

        if (error.Contains("already taken"))
            return "Este email já está cadastrado.";

        if (error.Contains("Passwords must"))
            return "A senha não atende aos requisitos de segurança.";

        return error;
    }

    public async Task<AuthResult> Register(RegisterModel model)
    {
        var client = _httpClientFactory.CreateClient("ApiBack");

        var request = new RegisterRequest
        {
            Email = model.Email,
            Password = model.Password
        };

        var response = await client.PostAsJsonAsync("/register", request);

        if (response.IsSuccessStatusCode)
        {
            return new AuthResult { Success = true };
        }

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var errorResponse = JsonSerializer.Deserialize<ValidationErrorResponse>(content);

            var firstError = errorResponse?.Errors
                .SelectMany(e => e.Value)
                .FirstOrDefault();

            return new AuthResult
            {
                Success = false,
                Error = TranslateError(firstError)
            };
        }
        catch
        {
            return new AuthResult
            {
                Success = false,
                Error = "Erro ao registrar usuário."
            };
        }
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Você deve aceitar os termos de uso")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Você deve aceitar os termos de uso")]
        public bool AceitoTermos { get; set; }
    }

}
