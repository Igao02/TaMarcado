using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public Task<List<Category>> GetAllOrderedByNameAsync() =>
        context.Categorie.OrderBy(c => c.Name).ToListAsync();
}
