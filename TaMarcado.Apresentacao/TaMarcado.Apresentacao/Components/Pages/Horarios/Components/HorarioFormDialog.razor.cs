using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.AvaliableTime;

namespace TaMarcado.Apresentacao.Components.Pages.Horarios.Components;

public class HorarioFormDialogBase : ComponentBase
{
    protected AvaliableTimeHandler.CreateHorarioModel _model = new();
    protected bool _isSaving;

    protected readonly DialogOptions _dialogOptions = new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseOnEscapeKey = true
    };

    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    [Inject] protected AvaliableTimeHandler Handler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected async Task HandleSave()
    {
        if (!_model.WeekDays.Any())
        {
            Snackbar.Add("Selecione ao menos um dia da semana.", Severity.Warning);
            return;
        }

        _isSaving = true;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await Handler.CreateAvaliableTimes(_model, email);

            if (result.Success)
            {
                var diasCriados = _model.WeekDays.Count();
                Snackbar.Add(
                    diasCriados == 1 ? "Horário cadastrado com sucesso!" : $"{diasCriados} horários cadastrados com sucesso!",
                    Severity.Success,
                    config =>
                    {
                        config.Icon = Icons.Material.Filled.CheckCircle;
                        config.ShowCloseIcon = true;
                    });
                await CloseDialog();
                await OnSaved.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.Error ?? "Erro ao salvar horário.", Severity.Error, config =>
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
            _isSaving = false;
            StateHasChanged();
        }
    }

    protected async Task HandleCancel()
    {
        await CloseDialog();
    }

    protected async Task HandleVisibleChanged(bool value)
    {
        if (!value)
            await CloseDialog();
    }

    private async Task CloseDialog()
    {
        _model = new();
        await VisibleChanged.InvokeAsync(false);
    }
}
