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
                var identity = new ClaimsIdentity(claims, "Identity.Application");
                var principal = new ClaimsPrincipal(identity);

                await context.SignInAsync("Identity.Application", principal);
                context.Response.Redirect("/?logado=1");
            }
            else
            {
                context.Response.Redirect("/login?erro=1");
            }
        }).DisableAntiforgery();

        app.MapPost("/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync("Identity.Application");
            context.Response.Redirect("/login");
        }).DisableAntiforgery();
    }
}
