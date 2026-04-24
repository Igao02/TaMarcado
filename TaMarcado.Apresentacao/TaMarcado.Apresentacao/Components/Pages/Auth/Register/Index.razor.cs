using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaMarcado.Apresentacao.Handlers.Auth;

namespace TaMarcado.Apresentacao.Components.Pages.Auth.Register;

public class IndexPageBase : ComponentBase
{
    protected AuthHandler.RegisterModel model = new();
    protected string? errorMessage;
    protected bool isLoading;
    protected string confirmPassword = string.Empty;
    protected bool showPassword;
    protected bool showConfirmPassword;

    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected AuthHandler Auth { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;

    public async Task HandleRegister()
    {
        // Limpa erro anterior
        errorMessage = null;

        // Validação de termos
        if (!model.AceitoTermos)
        {
            errorMessage = "Você precisa aceitar os termos de uso e política de privacidade.";
            return;
        }

        isLoading = true;
        StateHasChanged();

        try
        {
            var resultado = await Auth.Register(model);

            if (resultado.Success)
            {
                Snackbar.Add("Conta criada com sucesso! Verifique seu e-mail para ativação.", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
                Nav.NavigateTo("/login?cadastro=1");
            }
            else
            {
                errorMessage = resultado.Error ?? "Erro ao registrar usuário. Tente novamente.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            Console.WriteLine($"Erro no registro: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}
