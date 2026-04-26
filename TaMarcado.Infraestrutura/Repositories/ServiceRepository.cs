using Microsoft.EntityFrameworkCore;
using TaMarcado.Dominio.Entities;
using TaMarcado.Dominio.Repositories;
using TaMarcado.Infraestrutura.Data;

namespace TaMarcado.Infraestrutura.Repositories;

public class ServiceRepository(ApplicationDbContext context) : IServiceRepository
{
    public async Task AddAsync(Service service)
    {
        context.Service.Add(service);
        await context.SaveChangesAsync();
    }

    public Task<List<Service>> GetByProfessionalIdAsync(Guid professionalId) =>
        context.Service
            .Where(s => s.ProfessionalId == professionalId)
            .OrderBy(s => s.Name)
            .ToListAsync();

    public Task<Service?> GetByIdAndProfessionalIdAsync(Guid id, Guid professionalId) =>
        context.Service
            .FirstOrDefaultAsync(s => s.Id == id && s.ProfessionalId == professionalId && s.IsActive);

    public async Task UpdateAsync(Service service)
    {
        context.Service.Update(service);
        await context.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Service service)
    {
        service.IsActive = false;
        context.Service.Update(service);
        await context.SaveChangesAsync();
    }
}
