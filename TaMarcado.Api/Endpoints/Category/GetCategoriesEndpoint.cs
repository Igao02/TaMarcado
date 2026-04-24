using TaMarcado.Aplicacao.UseCases.Category.GetCategories;

namespace TaMarcado.Api.Endpoints.Category;

public class GetCategoriesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", async (GetCategoriesHandler handler) =>
        {
            var result = await handler.Handle();
            return Results.Ok(result);
        });
    }
}
