using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Booking;
using TaMarcado.Apresentacao.Handlers.Scheduling;

namespace TaMarcado.Apresentacao.Components.Pages.MySchedulings;

public class MySchedulingsPageBase : ComponentBase
{
    [Inject] protected BookingHandler BookingHandler { get; set; } = default!;
    [Inject] protected SchedulingHandler SchedulingHandler { get; set; } = default!;
    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected bool IsLoading = true;
    protected string SearchText = string.Empty;
    protected string? SelectedCategory;
    protected List<BookingHandler.ProfessionalListItem> Professionals = [];
    protected List<string> Categories = [];

    protected int _activeTab = 0;
    protected bool _schedulingsLoaded = false;
    protected bool _isLoadingSchedulings = false;
    protected string? _schedulingError;
    protected List<SchedulingHandler.ClientSchedulingItem> _clientSchedulings = [];
    protected Guid? _cancelingId;
    protected bool _paymentDialogOpen;
    protected Guid _selectedSchedulingId;
    protected string _selectedProfessionalName = string.Empty;

    protected IEnumerable<BookingHandler.ProfessionalListItem> FilteredProfessionals =>
        Professionals
            .Where(p => string.IsNullOrEmpty(SelectedCategory) || p.CategoryName == SelectedCategory)
            .Where(p => string.IsNullOrEmpty(SearchText) ||
                        p.ExibitionName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        var result = await BookingHandler.GetProfessionalsAsync();
        if (result.Success && result.Data is not null)
        {
            Professionals = result.Data;
            Categories = Professionals
                .Select(p => p.CategoryName)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }
        IsLoading = false;
    }

    protected void NavigateToBooking(BookingHandler.ProfessionalListItem p) =>
        Nav.NavigateTo($"/agendar/{p.Slug}");

    protected async Task OnTabChanged(int index)
    {
        _activeTab = index;
        if (index == 1 && !_schedulingsLoaded)
            await LoadClientSchedulings();
    }

    protected async Task LoadClientSchedulings()
    {
        _isLoadingSchedulings = true;
        _schedulingError = null;

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await SchedulingHandler.GetClientSchedulingsAsync(email);

            if (result.Success)
            {
                _clientSchedulings = result.Data ?? [];
                _schedulingsLoaded = true;
            }
            else
            {
                _schedulingError = result.Error;
            }
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            _schedulingError = $"Erro ao carregar agendamentos: {ex.Message}";
        }
        finally
        {
            _isLoadingSchedulings = false;
            StateHasChanged();
        }
    }

    protected async Task HandleCancelScheduling(Guid id)
    {
        _cancelingId = id;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await SchedulingHandler.CancelClientSchedulingAsync(id, email);

            if (result.Success)
            {
                Snackbar.Add("Agendamento cancelado.", Severity.Success, c =>
                {
                    c.Icon = Icons.Material.Filled.CheckCircle;
                    c.ShowCloseIcon = true;
                });
                await LoadClientSchedulings();
            }
            else
            {
                Snackbar.Add(result.Error ?? "Erro ao cancelar.", Severity.Error, c =>
                {
                    c.Icon = Icons.Material.Filled.Error;
                    c.ShowCloseIcon = true;
                });
            }
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            Snackbar.Add($"Erro inesperado: {ex.Message}", Severity.Error, c =>
            {
                c.Icon = Icons.Material.Filled.Error;
                c.ShowCloseIcon = true;
            });
        }
        finally
        {
            _cancelingId = null;
            StateHasChanged();
        }
    }

    protected void OpenClientPaymentDialog(SchedulingHandler.ClientSchedulingItem item)
    {
        _selectedSchedulingId = item.Id;
        _selectedProfessionalName = item.ProfessionalName;
        _paymentDialogOpen = true;
    }

    protected async Task HandlePaymentDialogVisibleChanged(bool value)
    {
        _paymentDialogOpen = value;
        await Task.CompletedTask;
    }

    protected static Color StatusColor(string status) => status switch
    {
        "Pendent" => Color.Warning,
        "Confirmed" => Color.Info,
        "Concluded" => Color.Success,
        _ => Color.Default
    };
}
