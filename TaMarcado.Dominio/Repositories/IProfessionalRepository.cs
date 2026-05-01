using TaMarcado.Dominio.Entities;

namespace TaMarcado.Dominio.Repositories;

public interface IProfessionalRepository
{
    Task<bool> ExistsByUserIdAsync(string userId);
    Task<bool> ExistsBySlugAsync(string slug);
    Task AddAsync(Professional professional);
    Task<Professional?> GetByUserIdWithCategoryAsync(string userId);
    Task<Guid?> GetIdByUserIdAsync(string userId);
    Task<Professional?> GetBySlugAsync(string slug);
    Task<List<Professional>> GetAllActiveWithCategoryAsync();
    Task<Professional?> GetByIdAsync(Guid id);
    Task UpdateAsync(Professional professional);
}
