using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TaMarcado.Apresentacao.Handlers.Client;

namespace TaMarcado.Apresentacao.Components.Pages.Clients;

public class ClientsPageBase : ComponentBase
{
    protected List<ClientHandler.ClientItem> clients = [];
    protected ClientHandler.ClientItem? selectedClient;
    protected string? errorMessage;
    protected bool isLoading = true;
    protected bool dialogOpen;

    [Inject] protected ClientHandler ClientHandler { get; set; } = default!;
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadClients();
    }

    protected async Task LoadClients()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var authState = await AuthState;
            var email = authState.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var result = await ClientHandler.GetClients(email);

            if (result.Success)
                clients = result.Data ?? [];
            else
                errorMessage = result.Error;
        }
        catch (Exception ex) when (ex is not NavigationException)
        {
            errorMessage = $"Erro ao carregar clientes: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected void OpenEditDialog(ClientHandler.ClientItem client)
    {
        selectedClient = client;
        dialogOpen = true;
    }

    protected async Task HandleDialogVisibleChanged(bool value)
    {
        dialogOpen = value;
        await Task.CompletedTask;
    }
}
