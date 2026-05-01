using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Professional;
using TaMarcado.Dominio.Enum;

namespace TaMarcado.Apresentacao.Components.Pages.Profile;

public class ProfilePageBase : ComponentBase
{
    protected ProfessionalHandler.CreateProfileModel model = new();
    protected List<ProfessionalHandler.CategoryItem> categories = [];
    protected string? errorMessage;
    protected bool isLoading;
    protected bool isPageLoading = true;
    protected string? photoPreview;
    protected bool isUploadingPhoto;
    protected string? photoUploadError;

    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected ProfessionalHandler ProfessionalHandler { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        var profileResult = await ProfessionalHandler.GetMyProfile(email);
        if (!profileResult.Success || profileResult.Data is null)
        {
            Nav.NavigateTo("/onboarding");
            return;
        }

        categories = await ProfessionalHandler.GetCategories();

        var profile = profileResult.Data;
        model.ExibitionName = profile.ExibitionName;
        model.Slug = profile.Slug;
        model.WhatsApp = profile.WhatsApp;
        model.Bio = profile.Bio;
        model.Address = profile.Address;
        model.PhotoUrl = profile.PhotoUrl;
        model.CategoryId = profile.CategoryId;
        model.KeyPix = profile.KeyPix;
        model.KeyPixType = (KeyPixEnum)profile.KeyPixType;

        if (!string.IsNullOrEmpty(profile.PhotoUrl))
            photoPreview = profile.PhotoUrl;

        isPageLoading = false;
    }

    protected async Task OnPhotoSelected(IBrowserFile file)
    {
        if (file is null) return;

        isUploadingPhoto = true;
        photoUploadError = null;
        StateHasChanged();

        try
        {
            const long maxSize = 5 * 1024 * 1024;
            var buffer = new byte[file.Size];
            using var stream = file.OpenReadStream(maxAllowedSize: maxSize);
            await stream.ReadExactlyAsync(buffer);

            photoPreview = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";

            var result = await ProfessionalHandler.UploadPhotoAsync(buffer, file.Name, file.ContentType);
            if (result.Success)
                model.PhotoUrl = result.Data;
            else
                photoUploadError = result.Error ?? "Erro ao enviar foto.";
        }
        catch
        {
            photoUploadError = "Arquivo inválido ou muito grande (máx. 5MB).";
            photoPreview = model.PhotoUrl;
        }
        finally
        {
            isUploadingPhoto = false;
            StateHasChanged();
        }
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

            var result = await ProfessionalHandler.UpdateProfile(model, email);

            if (result.Success)
            {
                Snackbar.Add("Perfil atualizado com sucesso!", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                    config.ShowCloseIcon = true;
                });
            }
            else
            {
                errorMessage = result.Error ?? "Erro ao atualizar perfil. Tente novamente.";
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
