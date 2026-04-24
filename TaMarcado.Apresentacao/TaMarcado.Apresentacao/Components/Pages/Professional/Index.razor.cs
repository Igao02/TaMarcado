using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Text.RegularExpressions;
using TaMarcado.Apresentacao.Handlers.Professional;

namespace TaMarcado.Apresentacao.Components.Pages.Professional;

public class IndexPageBase : ComponentBase
{
    protected ProfessionalHandler.CreateProfileModel model = new();
    protected List<ProfessionalHandler.CategoryItem> categories = [];
    protected string? errorMessage;
    protected bool isLoading;

    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected ProfessionalHandler ProfessionalHandler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        var existing = await ProfessionalHandler.GetMyProfile(email);
        if (existing.Success)
        {
            Nav.NavigateTo("/dashboard");
            return;
        }

        categories = await ProfessionalHandler.GetCategories();
    }

    protected void GenerateSlug()
    {
        if (string.IsNullOrWhiteSpace(model.ExibitionName)) return;
        if (!string.IsNullOrWhiteSpace(model.Slug)) return;

        model.Slug = Regex.Replace(
            model.ExibitionName.ToLowerInvariant()
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .Aggregate(string.Empty, (acc, c) => acc + c)
                .Replace(" ", "-"),
            @"[^a-z0-9-]", "");
    }

    protected async Task HandleSubmit()
    {
        errorMessage = null;
        isLoading = true;
        StateHasChanged();

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await ProfessionalHandler.CreateProfile(model, email);

            if (result.Success)
            {
                Snackbar.Add("Perfil criado com sucesso!", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
                Nav.NavigateTo("/");
            }
            else
            {
                errorMessage = result.Error ?? "Erro ao criar perfil. Tente novamente.";
            }
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            errorMessage = $"Erro inesperado: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}
