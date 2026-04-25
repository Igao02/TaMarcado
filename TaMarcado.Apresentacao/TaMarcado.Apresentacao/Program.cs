using MudBlazor.Services;
using TaMarcado.Apresentacao.Client.Pages;
using TaMarcado.Apresentacao.Components;
using TaMarcado.Apresentacao.Extensions;
using TaMarcado.Apresentacao.Handlers.Auth;
using TaMarcado.Apresentacao.Handlers.Professional;
using TaMarcado.Apresentacao.Handlers.Service;
using TaMarcado.Apresentacao.Handlers.AvaliableTime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthentication("Identity.Application")
    .AddCookie("Identity.Application", options =>
    {
        options.LoginPath = "/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpClient("ApiBack", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "https://localhost:7034");
});

builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<ProfessionalHandler>();
builder.Services.AddScoped<ServiceHandler>();
builder.Services.AddScoped<AvaliableTimeHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TaMarcado.Apresentacao.Client._Imports).Assembly);

app.Run();
