using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class ProfessionalRepository(ApplicationDbContext context) : IProfessionalRepository
{
    public Task<bool> ExistsByUserIdAsync(string userId) =>
        context.Professional.AnyAsync(p => p.ApplicationUserId == userId);

    public Task<bool> ExistsBySlugAsync(string slug) =>
        context.Professional.AnyAsync(p => p.Slug == slug);

    public async Task AddAsync(Professional professional)
    {
        context.Professional.Add(professional);
        await context.SaveChangesAsync();
    }

    public Task<Professional?> GetByUserIdWithCategoryAsync(string userId) =>
        context.Professional
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

    public Task<Guid?> GetIdByUserIdAsync(string userId) =>
        context.Professional
            .Where(p => p.ApplicationUserId == userId)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync();
}
