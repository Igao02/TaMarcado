using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.AvaliableTime;

namespace TaMarcado.Apresentacao.Components.Pages.Horarios;

public class HorariosPageBase : ComponentBase
{
    protected List<AvaliableTimeHandler.AvaliableTimeItem> _horarios = [];
    protected string? _errorMessage;
    protected bool _isLoading = true;
    protected bool _dialogOpen;
    protected Guid? _deletingId;

    [Inject] protected AvaliableTimeHandler Handler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadHorarios();
    }

    protected async Task LoadHorarios()
    {
        _isLoading = true;
        _errorMessage = null;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await Handler.GetAvaliableTimes(email);

            if (result.Success)
                _horarios = result.Data ?? [];
            else
                _errorMessage = result.Error;
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            _errorMessage = $"Erro ao carregar horários: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    protected void OpenDialog()
    {
        _dialogOpen = true;
    }

    protected async Task HandleDelete(Guid id)
    {
        _deletingId = id;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await Handler.DeleteAvaliableTime(id, email);

            if (result.Success)
            {
                Snackbar.Add("Horário excluído.", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
                await LoadHorarios();
            }
            else
            {
                Snackbar.Add(result.Error ?? "Erro ao excluir horário.", Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                });
            }
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            Snackbar.Add($"Erro inesperado: {ex.Message}", Severity.Error);
        }
        finally
        {
            _deletingId = null;
            StateHasChanged();
        }
    }
}
