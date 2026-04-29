using Microsoft.AspNetCore.Components;
using TaMarcado.Apresentacao.Handlers.Booking;

namespace TaMarcado.Apresentacao.Components.Pages.MySchedulings.Components;

public class ProfessionalCardBase : ComponentBase
{
    [Parameter] public BookingHandler.ProfessionalListItem Professional { get; set; } = default!;
    [Parameter] public EventCallback<BookingHandler.ProfessionalListItem> OnSelect { get; set; }
}
