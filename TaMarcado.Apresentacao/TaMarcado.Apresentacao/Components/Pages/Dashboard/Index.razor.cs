using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Professional;

namespace TaMarcado.Apresentacao.Components.Pages.Dashboard;

public class DashboardPageBase : ComponentBase
{
    protected ProfessionalHandler.ProfileData? profile;
    protected bool isLoading = true;

    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected ProfessionalHandler ProfessionalHandler { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await ProfessionalHandler.GetMyProfile(email);

            if (result.IsNotFound)
            {
                Nav.NavigateTo("/onboarding");
                return;
            }

            if (result.Success)
                profile = result.Data;
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            Console.WriteLine($"Erro ao carregar dashboard: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}
