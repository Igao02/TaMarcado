using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Professional;

namespace TaMarcado.Apresentacao.Components.Pages.Home;

public class HomePageBase : ComponentBase
{
    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected ProfessionalHandler ProfessionalHandler { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;

        if (authState.User.Identity?.IsAuthenticated != true)
        {
            Nav.NavigateTo("/login");
            return;
        }

        var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        try
        {
            var result = await ProfessionalHandler.GetMyProfile(email);

            if (result.Success)
            {
                Nav.NavigateTo("/dashboard");
            }
            else if (result.IsNotFound)
                Nav.NavigateTo("/onboarding");
            else
                Nav.NavigateTo("/login");
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            Nav.NavigateTo("/login");
        }
    }
}
