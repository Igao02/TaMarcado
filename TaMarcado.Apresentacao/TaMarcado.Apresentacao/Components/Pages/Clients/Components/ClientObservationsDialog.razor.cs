using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Client;

namespace TaMarcado.Apresentacao.Components.Pages.Clients.Components;

public class ClientObservationsDialogBase : ComponentBase
{
    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public ClientHandler.ClientItem Client { get; set; } = default!;
    [Parameter] public EventCallback OnSaved { get; set; }

    [Inject] protected ClientHandler ClientHandler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected ObservationsModel _model = new();
    protected bool _isSaving;

    protected readonly DialogOptions _dialogOptions = new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseOnEscapeKey = true
    };

    protected override void OnParametersSet()
    {
        if (Visible)
            _model = new ObservationsModel { Observations = Client.Observations };
    }

    protected async Task HandleVisibleChanged(bool value)
    {
        if (!value) await CloseDialog();
    }

    protected async Task CloseDialog()
    {
        await VisibleChanged.InvokeAsync(false);
    }

    protected async Task HandleSave()
    {
        _isSaving = true;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await ClientHandler.UpdateObservations(Client.Id, _model.Observations, email);

            if (result.Success)
            {
                Snackbar.Add("Observações salvas!", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
                Client.Observations = _model.Observations;
                await CloseDialog();
                await OnSaved.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.Error ?? "Erro ao salvar observações.", Severity.Error, config =>
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
            _isSaving = false;
            StateHasChanged();
        }
    }

    public class ObservationsModel
    {
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string? Observations { get; set; }
    }
}
