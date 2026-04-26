using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Service;

namespace TaMarcado.Apresentacao.Components.Pages.Services;

public class ServicesPageBase : ComponentBase
{
    protected List<ServiceHandler.ServiceItem> services = [];
    protected ServiceHandler.CreateServiceModel model = new();
    protected string? errorMessage;
    protected bool isLoading = true;
    protected bool isSaving;
    protected bool dialogOpen;
    protected Guid? _deletingId;


    protected readonly DialogOptions dialogOptions = new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseOnEscapeKey = true
    };

    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected ServiceHandler ServiceHandler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadServices();
    }

    private async Task LoadServices()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await ServiceHandler.GetServices(email);

            if (result.Success)
                services = result.Data ?? [];
            else
                errorMessage = result.Error;
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            errorMessage = $"Erro ao carregar serviços: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected void OpenCreateDialog()
    {
        model = new();
        errorMessage = null;
        dialogOpen = true;
    }

    protected void CloseDialog()
    {
        dialogOpen = false;
    }

    protected async Task HandleCreate()
    {
        isSaving = true;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await ServiceHandler.CreateService(model, email);

            if (result.Success)
            {
                Snackbar.Add("Serviço criado com sucesso!", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
                dialogOpen = false;
                await LoadServices();
            }
            else
            {
                errorMessage = result.Error ?? "Erro ao criar serviço.";
                dialogOpen = false;
            }
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            errorMessage = $"Erro inesperado: {ex.Message}";
            dialogOpen = false;
        }
        finally
        {
            isSaving = false;
            StateHasChanged();
        }
    }

    protected async Task HandleDelete(Guid id)
    {
        _deletingId = id;
        StateHasChanged();
        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            if(string.IsNullOrEmpty(email))
            {
                Snackbar.Add("Usuário não autenticado.", Severity.Warning, config =>
                {
                    config.Icon = Icons.Material.Filled.Warning;
                    config.ShowCloseIcon = true;
                });
                return;
            }

            var result = await ServiceHandler.DeleteService(id, email);
            if (result.Success)
            {
                Snackbar.Add("Serviço deletado com sucesso!", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
                await LoadServices();
            }
            else
            {
                Snackbar.Add(result.Error ?? "Erro ao deletar serviço.", Severity.Error, config =>
                {
                    config.Icon = Icons.Material.Filled.Error;
                    config.ShowCloseIcon = true;
                });
            }
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            Snackbar.Add($"Erro inesperado: {ex.Message}", Severity.Error, config =>
            {
                config.Icon = Icons.Material.Filled.Error;
                config.ShowCloseIcon = true;
            });
        }
        finally
        {
            _deletingId = null;
            StateHasChanged();
        }
    }
}
