using TaMarcado.Dominio.Repositories;

namespace TaMarcado.Aplicacao.UseCases.Category.GetCategories;

public class GetCategoriesHandler(ICategoryRepository repository)
{
    public async Task<List<GetCategoriesResponse>> Handle()
    {
        try
        {
            var categories = await repository.GetAllOrderedByNameAsync();
            return categories.Select(c => new GetCategoriesResponse(c.Id, c.Name)).ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao buscar categorias: {ex.Message}", ex);
        }
    }
}
