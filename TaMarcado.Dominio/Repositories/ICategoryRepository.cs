using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllOrderedByNameAsync();
}
