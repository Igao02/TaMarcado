using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace TaMarcado.Apresentacao.Extensions;

public static class AuthEndpointsExtension
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (HttpContext context, IHttpClientFactory factory) =>
        {
            var form = await context.Request.ReadFormAsync();
            var email = form["email"].ToString();
            var password = form["password"].ToString();
            var returnUrl = form["returnUrl"].ToString();

            var client = factory.CreateClient("ApiBack");
            var body = JsonSerializer.Serialize(new { email, password });

            var response = await client.PostAsync(
                "/login?useCookies=true",
                new StringContent(body, System.Text.Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                };

                try
                {
                    var rolesResponse = await client.GetAsync($"/api/auth/roles?email={Uri.EscapeDataString(email)}");
                    if (rolesResponse.IsSuccessStatusCode)
                    {
                        var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                        var roles = JsonSerializer.Deserialize<List<string>>(rolesJson) ?? [];
                        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                    }
                }
                catch { }

                var identity = new ClaimsIdentity(claims, "Identity.Application");
                var principal = new ClaimsPrincipal(identity);

                await context.SignInAsync("Identity.Application", principal);

                var redirectTo = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/?logado=1";
                context.Response.Redirect(redirectTo);
            }
            else
            {
                var errorRedirect = !string.IsNullOrEmpty(returnUrl)
                    ? $"/login?erro=1&returnUrl={Uri.EscapeDataString(returnUrl)}"
                    : "/login?erro=1";
                context.Response.Redirect(errorRedirect);
            }
        }).DisableAntiforgery();

        app.MapPost("/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync("Identity.Application");
            context.Response.Redirect("/login");
        }).DisableAntiforgery();
    }
}
