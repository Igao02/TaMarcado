using Microsoft.AspNetCore.Components;
using TaMarcado.Apresentacao.Handlers.Booking;

namespace TaMarcado.Apresentacao.Components.Pages.Booking.Components;

public class ServiceCardBase : ComponentBase
{
    [Parameter] public BookingHandler.PublicServiceItem Service { get; set; } = default!;
    [Parameter] public bool IsSelected { get; set; }
    [Parameter] public EventCallback<BookingHandler.PublicServiceItem> OnSelect { get; set; }

    protected async Task HandleSelect()
    {
        await OnSelect.InvokeAsync(Service);
    }
}
