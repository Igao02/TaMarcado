using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Booking;

namespace TaMarcado.Apresentacao.Components.Pages.Booking;

public class BookingPageBase : ComponentBase
{
    [Parameter] public string Slug { get; set; } = string.Empty;

    protected BookingHandler.ProfessionalProfile? Profile { get; set; }
    protected List<BookingHandler.PublicServiceItem> Services { get; set; } = [];
    protected List<BookingHandler.TimeSlot> AvailableSlots { get; set; } = [];

    protected BookingHandler.PublicServiceItem? SelectedService { get; set; }
    protected DateOnly? SelectedDate { get; set; }
    protected string? SelectedSlot { get; set; }

    protected BookingHandler.CreateBookingModel BookingModel { get; set; } = new();

    protected bool IsLoadingProfile { get; set; } = true;
    protected bool IsLoadingSlots { get; set; }
    protected bool IsConfirming { get; set; }
    protected bool BookingSuccess { get; set; }
    protected string? ErrorMessage { get; set; }

    [Inject] protected BookingHandler BookingHandler { get; set; } = default!;
    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(Slug))
            return;

        IsLoadingProfile = true;
        ErrorMessage = null;
        StateHasChanged();

        var profileResult = await BookingHandler.GetProfileAsync(Slug);
        if (!profileResult.Success)
        {
            ErrorMessage = profileResult.Error ?? "Profissional não encontrado.";
            IsLoadingProfile = false;
            StateHasChanged();
            return;
        }

        Profile = profileResult.Data;

        var servicesResult = await BookingHandler.GetServicesAsync(Slug);
        Services = servicesResult.Success ? (servicesResult.Data ?? []) : [];

        IsLoadingProfile = false;
        StateHasChanged();
    }

    protected async Task OnServiceSelected(BookingHandler.PublicServiceItem service)
    {
        SelectedService = service;
        SelectedDate = null;
        SelectedSlot = null;
        AvailableSlots = [];
        StateHasChanged();
    }

    protected async Task OnDateChanged(DateOnly? date)
    {
        SelectedDate = date;
        SelectedSlot = null;
        AvailableSlots = [];

        if (date is null || SelectedService is null)
            return;

        IsLoadingSlots = true;
        StateHasChanged();

        var result = await BookingHandler.GetAvailableSlotsAsync(Slug, SelectedService.Id, date.Value);
        AvailableSlots = result.Success ? (result.Data ?? []) : [];

        if (result.Success && AvailableSlots.Count == 0)
            Snackbar.Add("Nenhum horário disponível para esta data.", Severity.Info);

        IsLoadingSlots = false;
        StateHasChanged();
    }

    protected void OnSlotSelected(string startTime)
    {
        SelectedSlot = startTime;
        StateHasChanged();
    }

    protected async Task HandleConfirm()
    {
        if (SelectedService is null || SelectedDate is null || SelectedSlot is null)
            return;

        var authState = await AuthState;
        if (!authState.User.Identity?.IsAuthenticated ?? true)
        {
            Nav.NavigateTo($"/login?returnUrl={Uri.EscapeDataString($"/agendar/{Slug}")}");
            return;
        }

        var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        IsConfirming = true;
        StateHasChanged();

        try
        {
            BookingModel.Slug = Slug;
            BookingModel.ServiceId = SelectedService.Id;
            BookingModel.Date = SelectedDate.Value;
            BookingModel.StartTime = SelectedSlot;

            var result = await BookingHandler.CreateBookingAsync(BookingModel, email);

            if (result.Success)
            {
                BookingSuccess = true;
                Snackbar.Add("Agendamento realizado com sucesso!", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
            }
            else
            {
                Snackbar.Add(result.Error ?? "Erro ao confirmar agendamento.", Severity.Error, config =>
                {
                    config.Icon = Icons.Material.Filled.Error;
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
            IsConfirming = false;
            StateHasChanged();
        }
    }

    protected void ResetBooking()
    {
        SelectedService = null;
        SelectedDate = null;
        SelectedSlot = null;
        AvailableSlots = [];
        BookingModel = new();
        BookingSuccess = false;
        StateHasChanged();
    }
}
