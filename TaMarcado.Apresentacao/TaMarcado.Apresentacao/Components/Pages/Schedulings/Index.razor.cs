using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Scheduling;

namespace TaMarcado.Apresentacao.Components.Pages.Schedulings;

public class SchedulingsPageBase : ComponentBase
{
    protected List<SchedulingHandler.SchedulingItem> _schedulings = [];
    protected string? _errorMessage;
    protected bool _isLoading = true;
    protected string? _statusFilter;
    protected Guid? _processingId;
    protected bool _paymentDialogOpen;
    protected Guid _selectedSchedulingId;
    protected string _selectedClientName = string.Empty;

    [Inject] protected SchedulingHandler Handler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected IEnumerable<SchedulingHandler.SchedulingItem> FilteredSchedulings =>
        string.IsNullOrEmpty(_statusFilter)
            ? _schedulings
            : _schedulings.Where(s => s.Status == _statusFilter);

    protected override async Task OnInitializedAsync()
    {
        await LoadSchedulings();
    }

    protected async Task LoadSchedulings()
    {
        _isLoading = true;
        _errorMessage = null;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await Handler.GetSchedulingsAsync(email);

            if (result.Success)
                _schedulings = result.Data ?? [];
            else
                _errorMessage = result.Error;
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            _errorMessage = $"Erro ao carregar agendamentos: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task HandleConfirm(Guid id)
    {
        await ExecuteAction(id, async email =>
        {
            var result = await Handler.ConfirmAsync(id, email);
            if (result.Success)
                Snackbar.Add("Agendamento confirmado.", Severity.Success, c => c.Icon = Icons.Material.Filled.CheckCircle);
            else
                Snackbar.Add(result.Error ?? "Erro ao confirmar.", Severity.Error);
            return result.Success;
        });
    }

    protected async Task HandleCancel(Guid id)
    {
        await ExecuteAction(id, async email =>
        {
            var result = await Handler.CancelAsync(id, email);
            if (result.Success)
                Snackbar.Add("Agendamento cancelado.", Severity.Info, c => c.Icon = Icons.Material.Filled.Cancel);
            else
                Snackbar.Add(result.Error ?? "Erro ao cancelar.", Severity.Error);
            return result.Success;
        });
    }

    protected async Task HandleConclude(Guid id)
    {
        await ExecuteAction(id, async email =>
        {
            var result = await Handler.ConcludeAsync(id, email);
            if (result.Success)
                Snackbar.Add("Agendamento concluído.", Severity.Success, c => c.Icon = Icons.Material.Filled.TaskAlt);
            else
                Snackbar.Add(result.Error ?? "Erro ao concluir.", Severity.Error);
            return result.Success;
        });
    }

    private async Task ExecuteAction(Guid id, Func<string, Task<bool>> action)
    {
        _processingId = id;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var success = await action(email);
            if (success) await LoadSchedulings();
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            Snackbar.Add($"Erro inesperado: {ex.Message}", Severity.Error);
        }
        finally
        {
            _processingId = null;
            StateHasChanged();
        }
    }

    protected void OpenPaymentDialog(SchedulingHandler.SchedulingItem item)
    {
        _selectedSchedulingId = item.Id;
        _selectedClientName = item.ClientName;
        _paymentDialogOpen = true;
    }

    protected async Task HandlePaymentDialogVisibleChanged(bool value)
    {
        _paymentDialogOpen = value;
        await Task.CompletedTask;
    }

    protected static Color GetStatusColor(string status) => status switch
    {
        "Pendent" => Color.Warning,
        "Confirmed" => Color.Success,
        "Canceled" => Color.Error,
        _ => Color.Default
    };
}
