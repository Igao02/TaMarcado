using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Payment;

namespace TaMarcado.Apresentacao.Components.Pages.Schedulings.Components;

public class PaymentDialogBase : ComponentBase
{
    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public Guid SchedulingId { get; set; }
    [Parameter] public string ClientName { get; set; } = string.Empty;
    [Parameter] public EventCallback OnPaymentConfirmed { get; set; }
    [Parameter] public bool IsClientView { get; set; }

    [Inject] protected PaymentHandler PaymentHandler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected PaymentHandler.PaymentItem? _payment;
    protected bool _isLoading;
    protected bool _isConfirming;
    protected string? _error;

    protected readonly DialogOptions _dialogOptions = new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseOnEscapeKey = true
    };

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && _payment is null)
            await LoadPayment();
        else if (!Visible)
        {
            _payment = null;
            _error = null;
        }
    }

    protected async Task HandleVisibleChanged(bool value)
    {
        if (!value)
        {
            _payment = null;
            _error = null;
            await VisibleChanged.InvokeAsync(false);
        }
    }

    protected async Task CloseDialog() => await VisibleChanged.InvokeAsync(false);

    private async Task LoadPayment()
    {
        _isLoading = true;
        _error = null;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var result = IsClientView
                ? await PaymentHandler.GetClientPaymentAsync(SchedulingId, email)
                : await PaymentHandler.GetPaymentAsync(SchedulingId, email);

            if (result.Success)
                _payment = result.Data;
            else
                _error = result.Error;
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            _error = $"Erro ao carregar pagamento: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task HandleConfirmPayment()
    {
        _isConfirming = true;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var result = await PaymentHandler.ConfirmPaymentAsync(SchedulingId, email);

            if (result.Success)
            {
                Snackbar.Add("Pagamento confirmado!", Severity.Success, c =>
                {
                    c.Icon = Icons.Material.Filled.CheckCircle;
                    c.ShowCloseIcon = true;
                });
                await LoadPayment();
                await OnPaymentConfirmed.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.Error ?? "Erro ao confirmar pagamento.", Severity.Error, c =>
                {
                    c.Icon = Icons.Material.Filled.Error;
                    c.ShowCloseIcon = true;
                });
            }
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            Snackbar.Add($"Erro inesperado: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isConfirming = false;
            StateHasChanged();
        }
    }

    protected async Task CopyPayload()
    {
        if (_payment is null) return;
        await JS.InvokeVoidAsync("navigator.clipboard.writeText", _payment.PixPayload);
        Snackbar.Add("Copiado!", Severity.Info, c => c.ShowCloseIcon = true);
    }
}
