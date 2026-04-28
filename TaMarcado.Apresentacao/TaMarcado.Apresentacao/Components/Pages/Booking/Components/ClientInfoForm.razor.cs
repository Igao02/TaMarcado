using Microsoft.AspNetCore.Components;
using TaMarcado.Apresentacao.Handlers.Booking;

namespace TaMarcado.Apresentacao.Components.Pages.Booking.Components;

public class ClientInfoFormBase : ComponentBase
{
    [Parameter] public BookingHandler.CreateBookingModel Model { get; set; } = default!;
    [Parameter] public EventCallback OnSubmit { get; set; }
    [Parameter] public bool IsLoading { get; set; }

    protected async Task HandleSubmit()
    {
        await OnSubmit.InvokeAsync();
    }
}
