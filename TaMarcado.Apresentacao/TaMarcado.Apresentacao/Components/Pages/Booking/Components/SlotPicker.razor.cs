using Microsoft.AspNetCore.Components;
using TaMarcado.Apresentacao.Handlers.Booking;

namespace TaMarcado.Apresentacao.Components.Pages.Booking.Components;

public class SlotPickerBase : ComponentBase
{
    [Parameter] public List<BookingHandler.TimeSlot> Slots { get; set; } = [];
    [Parameter] public string? SelectedSlot { get; set; }
    [Parameter] public EventCallback<string> OnSlotSelected { get; set; }

    protected async Task HandleSelect(string startTime)
    {
        await OnSlotSelected.InvokeAsync(startTime);
    }
}
