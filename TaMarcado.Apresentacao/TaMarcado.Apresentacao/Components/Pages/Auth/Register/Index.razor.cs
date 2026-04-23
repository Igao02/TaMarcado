using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaMarcado.Apresentacao.Handlers.Auth;

namespace TaMarcado.Apresentacao.Components.Pages.Auth.Register;

public class IndexPageBase : ComponentBase
{
    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected AuthHandler Auth { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;

    protected AuthHandler.RegisterModel model = new();
    protected string? erro;
    protected bool isLoading;

    public async Task Registrar()
    {
        isLoading = true;
        var resultado = await Auth.Register(model);

        try
        {
            if (resultado.Success)
            {
                Snackbar.Add("Conta criada com sucesso! Verifique seu e-mail para ativação.");
                Nav.NavigateTo("/login?cadastro=1");
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(resultado.Error ?? "Erro ao registrar usuário.", Severity.Error);
            Console.Write($"Erro ao registrar usuário: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }
}
