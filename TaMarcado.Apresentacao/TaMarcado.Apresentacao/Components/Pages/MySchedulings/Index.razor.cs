using Microsoft.AspNetCore.Components;
using TaMarcado.Apresentacao.Handlers.Booking;

namespace TaMarcado.Apresentacao.Components.Pages.MySchedulings;

public class MySchedulingsPageBase : ComponentBase
{
    [Inject] protected BookingHandler BookingHandler { get; set; } = default!;
    [Inject] protected NavigationManager Nav { get; set; } = default!;

    protected bool IsLoading = true;
    protected string SearchText = string.Empty;
    protected string? SelectedCategory;
    protected List<BookingHandler.ProfessionalListItem> Professionals = [];
    protected List<string> Categories = [];

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
}
